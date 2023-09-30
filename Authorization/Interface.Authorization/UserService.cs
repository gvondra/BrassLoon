using BrassLoon.Interface.Authorization.Models;
using Google.Protobuf.WellKnownTypes;
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
                return Map(response);
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
                        Map(stream.ResponseStream.Current));
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
            return await _userNameCache.ExecuteAsync(async context =>
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
                        Map(stream.ResponseStream.Current));
                }
            }
            return results;
        }

        public async Task<User> Update(ISettings settings, Guid domainId, Guid userId, User user)
        {
            Protos.User request = Map(user);
            request.UserId = userId.ToString("D");
            request.DomainId = domainId.ToString("D");
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.UserService.UserServiceClient userService = new Protos.UserService.UserServiceClient(channel);
                Protos.User response = await userService.UpdateAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return Map(response);
            }
        }

        public Task<User> Update(ISettings settings, User user)
        {
            if (!user.DomainId.HasValue || user.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(user.DomainId));
            if (!user.UserId.HasValue || user.UserId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(user.UserId));
            return Update(settings, user.DomainId.Value, user.UserId.Value, user);
        }

        private static Protos.User Map(User user)
        {
            Protos.User result = new Protos.User
            {
                CreateTimestamp = user.CreateTimestamp.HasValue ? Timestamp.FromDateTime(user.CreateTimestamp.Value) : null,
                DomainId = user.DomainId?.ToString("D"),
                UserId = user.UserId?.ToString("D"),
                EmailAddress = user.EmailAddress,
                Name = user.Name,
                ReferenceId = user.ReferenceId,
                UpdateTimestamp = user.UpdateTimestamp.HasValue ? Timestamp.FromDateTime(user.UpdateTimestamp.Value) : null
            };
            foreach (AppliedRole role in user.Roles ?? new List<AppliedRole>())
            {
                result.Roles.Add(Map(role));
            }
            return result;
        }

        private static User Map(Protos.User user)
        {
            User result = new User
            {
                CreateTimestamp = user.CreateTimestamp?.ToDateTime(),
                DomainId = !string.IsNullOrEmpty(user.DomainId) ? Guid.Parse(user.DomainId) : default(Guid?),
                EmailAddress = user.EmailAddress,
                Name = user.Name,
                ReferenceId = user.ReferenceId,
                UpdateTimestamp = user.UpdateTimestamp?.ToDateTime(),
                UserId = !string.IsNullOrEmpty(user.UserId) ? Guid.Parse(user.UserId) : default(Guid?),
                Roles = new List<AppliedRole>()
            };
            foreach (Protos.AppliedRole role in user.Roles)
            {
                result.Roles.Add(Map(role));
            }
            return result;
        }

        private static Protos.AppliedRole Map(AppliedRole role)
        {
            return new Protos.AppliedRole
            {
                Name = role.Name,
                PolicyName = role.PolicyName
            };
        }

        private static AppliedRole Map(Protos.AppliedRole role)
        {
            return new AppliedRole
            {
                Name = role.Name,
                PolicyName = role.PolicyName
            };
        }
    }
}
