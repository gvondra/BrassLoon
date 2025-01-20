using BrassLoon.Interface.WorkTask.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask
{
    public class WorkTaskCommentService : IWorkTaskCommentService
    {
        public async Task<List<Comment>> Create(ISettings settings, Guid workTaskId, params Comment[] comments)
        {
            if (workTaskId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workTaskId));
            if (comments == null)
                throw new ArgumentNullException(nameof(comments));
            if (Array.Exists(comments, c => !c.DomainId.HasValue))
                throw new ArgumentException("At least one comment is missing a domain id");
            List<Comment> result = new List<Comment>();
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskCommentService.WorkTaskCommentServiceClient service = new Protos.WorkTaskCommentService.WorkTaskCommentServiceClient(channel);
                AsyncDuplexStreamingCall<Protos.CreateWorkTaskCommentRequest, Protos.Comment> streamingCall = service.Create(await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                for (int i = 0; i < comments.Length; i += 1)
                {
                    await streamingCall.RequestStream.WriteAsync(
                        new Protos.CreateWorkTaskCommentRequest
                        {
                            Comment = Map(comments[i]),
                            WorkTaskId = workTaskId.ToString("D")
                        });
                }
                await streamingCall.RequestStream.CompleteAsync();
                while (await streamingCall.ResponseStream.MoveNext())
                {
                    result.Add(
                        Comment.Create(streamingCall.ResponseStream.Current));
                }
            }
            return result;
        }

        public Task<List<Comment>> Create(ISettings settings, Guid domainId, Guid workTaskId, params string[] comments)
        => Create(settings, workTaskId, comments.Select<string, Comment>(c => new Comment { DomainId = domainId, Text = c }).ToArray());

        public async Task<List<Comment>> GetAll(ISettings settings, Guid domainId, Guid workTaskId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workTaskId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workTaskId));
            Protos.GetAllWorkTaskCommentsRequest request = new Protos.GetAllWorkTaskCommentsRequest
            {
                DomainId = domainId.ToString("D"),
                WorkTaskIdId = workTaskId.ToString("D")
            };
            List<Comment> result = new List<Comment>();
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskCommentService.WorkTaskCommentServiceClient service = new Protos.WorkTaskCommentService.WorkTaskCommentServiceClient(channel);
                AsyncServerStreamingCall<Protos.Comment> streamingCall = service.GetAll(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                while (await streamingCall.ResponseStream.MoveNext())
                {
                    result.Add(
                        Comment.Create(streamingCall.ResponseStream.Current));
                }
            }
            return result;
        }

        private static Protos.Comment Map(Comment comment)
        {
            return new Protos.Comment
            {
                CommentId = comment.CommentId?.ToString("D") ?? string.Empty,
                DomainId = comment.DomainId?.ToString("D") ?? string.Empty,
                CreateTimestamp = comment.CreateTimestamp.HasValue ? Timestamp.FromDateTime(comment.CreateTimestamp.Value) : default,
                Text = comment.Text
            };
        }
    }
}
