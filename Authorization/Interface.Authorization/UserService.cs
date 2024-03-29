﻿using BrassLoon.Interface.Authorization.Models;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Authorization
{
    public class UserService : IUserService
    {
        private static readonly AsyncPolicy _userNameCache = Policy.CacheAsync(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromMinutes(6));

        public async Task<User> Get(ISettings settings, Guid domainId, Guid userId)
        {
            Protos.GetUserRequest request = new Protos.GetUserRequest
            {
                DomainId = domainId.ToString("D"),
                UserId = userId.ToString("D")
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.UserService.UserServiceClient userService = new Protos.UserService.UserServiceClient(channel);
                Protos.User response = await userService.GetAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return User.Create(response);
            }
        }

        public async Task<User> Get(ISettings settings, Guid domainId)
        {
            Protos.SearchUserRequest request = new Protos.SearchUserRequest
            {
                DomainId = domainId.ToString("D"),
            };
            List<User> results = new List<User>();
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.UserService.UserServiceClient userService = new Protos.UserService.UserServiceClient(channel);
                AsyncServerStreamingCall<Protos.User> stream = userService.Search(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                while (await stream.ResponseStream.MoveNext())
                {
                    results.Add(
                        User.Create(stream.ResponseStream.Current));
                }
            }
            return results.FirstOrDefault();
        }

        public async Task<string> GetName(ISettings settings, Guid domainId, Guid userId)
        {
            Protos.GetUserRequest request = new Protos.GetUserRequest
            {
                DomainId = domainId.ToString("D"),
                UserId = userId.ToString("D")
            };
            return await _userNameCache.ExecuteAsync(
                async context =>
                {
                    using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
                    {
                        Protos.UserService.UserServiceClient userService = new Protos.UserService.UserServiceClient(channel);
                        Protos.GetUserNameResponse response = await userService.GetNameAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                        return response.Name;
                    }
                },
                new Context(string.Concat(domainId.ToString("N"), "|", userId.ToString("N"))));
        }

        public Task<IAsyncEnumerable<User>> GetByDomainId(ISettings settings, Guid domainId)
        {
            Protos.GetByDomainRequest request = new Protos.GetByDomainRequest
            {
                DomainId = domainId.ToString("D")
            };
            return Task.FromResult<IAsyncEnumerable<User>>(
                new StreamEnumerable<Protos.User, User>(
                    settings,
                    async channel =>
                    {
                        Protos.UserService.UserServiceClient userService = new Protos.UserService.UserServiceClient(channel);
                        return userService.GetByDomain(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                    },
                    User.Create));
        }

        public async Task<List<User>> Search(ISettings settings, Guid domainId, string emailAddress = null, string referenceId = null)
        {
            Protos.SearchUserRequest request = new Protos.SearchUserRequest
            {
                DomainId = domainId.ToString("D"),
                EmailAddress = emailAddress,
                ReferenceId = referenceId
            };
            List<User> results = new List<User>();
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.UserService.UserServiceClient userService = new Protos.UserService.UserServiceClient(channel);
                AsyncServerStreamingCall<Protos.User> stream = userService.Search(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                while (await stream.ResponseStream.MoveNext())
                {
                    results.Add(
                        User.Create(stream.ResponseStream.Current));
                }
            }
            return results;
        }

        public async Task<User> Update(ISettings settings, Guid domainId, Guid userId, User user)
        {
            Protos.User request = user.ToProto();
            request.UserId = userId.ToString("D");
            request.DomainId = domainId.ToString("D");
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.UserService.UserServiceClient userService = new Protos.UserService.UserServiceClient(channel);
                Protos.User response = await userService.UpdateAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return User.Create(response);
            }
        }

        public Task<User> Update(ISettings settings, User user)
        {
            if (!user.DomainId.HasValue || user.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(user.DomainId)} property of Role is not set");
            if (!user.UserId.HasValue || user.UserId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(user.UserId)} property of Role is not set");
            return Update(settings, user.DomainId.Value, user.UserId.Value, user);
        }
    }
}
