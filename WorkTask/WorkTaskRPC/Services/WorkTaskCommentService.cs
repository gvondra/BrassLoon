using BrassLoon.CommonAPI;
using BrassLoon.Interface.WorkTask.Protos;
using BrassLoon.WorkTask.Framework;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Protos = BrassLoon.Interface.WorkTask.Protos;

namespace WorkTaskRPC.Services
{
    public class WorkTaskCommentService : Protos.WorkTaskCommentService.WorkTaskCommentServiceBase
    {
        private readonly IDomainAcountAccessVerifier _domainAcountAccessVerifier;
        private readonly IMetaDataProcessor _metaDataProcessor;
        private readonly SettingsFactory _settingsFactory;
        private readonly ILogger<WorkTaskCommentService> _logger;
        private readonly IWorkTaskCommentFactory _workTaskCommentFactory;
        private readonly ICommentSaver _commentSaver;

        public WorkTaskCommentService(
            IDomainAcountAccessVerifier domainAcountAccessVerifier,
            IMetaDataProcessor metaDataProcessor,
            SettingsFactory settingsFactory,
            ILogger<WorkTaskCommentService> logger,
            IWorkTaskCommentFactory workTaskCommentFactory,
            ICommentSaver commentSaver)
        {
            _domainAcountAccessVerifier = domainAcountAccessVerifier;
            _metaDataProcessor = metaDataProcessor;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _workTaskCommentFactory = workTaskCommentFactory;
            _commentSaver = commentSaver;
        }

#pragma warning disable S2589 // Boolean expressions should not be gratuitous
        public override async Task Create(IAsyncStreamReader<CreateWorkTaskCommentRequest> requestStream, IServerStreamWriter<Comment> responseStream, ServerCallContext context)
        {
            try
            {
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                Guid? domainId = null;
                Guid? workTaskId = null;
                List<IComment> comments = null;
                List<Task> createTasks = new List<Task>();
                while (await requestStream.MoveNext())
                {
                    CreateWorkTaskCommentRequest request = requestStream.Current;
                    if (!string.IsNullOrEmpty(request.WorkTaskId)
                        && !string.IsNullOrEmpty(request.Comment.DomainId)
                        && !string.IsNullOrEmpty(request.Comment.Text))
                    {
                        if (workTaskId.HasValue
                            && domainId.HasValue
                            && !Guid.Parse(request.Comment.DomainId).Equals(domainId.Value)
                            && !Guid.Parse(request.WorkTaskId).Equals(workTaskId.Value))
                        {
                            createTasks.Add(CreateComments(comments, responseStream));
                            domainId = null;
                            workTaskId = null;
                            comments = null;
                        }
                        if (!domainId.HasValue)
                        {
                            domainId = Guid.Parse(request.Comment.DomainId);
                            if (!await _domainAcountAccessVerifier.HasAccess(
                                _settingsFactory.CreateAccount(accessToken),
                                domainId.Value,
                                accessToken))
                            {
                                throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                            }
                        }
                        if (!workTaskId.HasValue)
                            workTaskId = Guid.Parse(request.WorkTaskId);
                        if (comments == null)
                            comments = new List<IComment>();
                        comments.Add(_workTaskCommentFactory.Create(domainId.Value, workTaskId.Value, request.Comment.Text));
                    }
                }
                if (workTaskId.HasValue
                    && domainId.HasValue
                    && comments != null)
                {
                    createTasks.Add(CreateComments(comments, responseStream));
                }
                await Task.WhenAll(createTasks);
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Internal System Error"), ex.Message);
            }
        }
#pragma warning restore S2589 // Boolean expressions should not be gratuitous

        private async Task CreateComments(List<IComment> comments, IServerStreamWriter<Comment> responseStream)
        {
            if (comments != null && comments.Count > 0)
            {
                CoreSettings settings = _settingsFactory.CreateCore();
                await _commentSaver.Create(settings, comments.ToArray());
                foreach (IComment comment in comments)
                {
                    await responseStream.WriteAsync(Map(comment));
                }
            }
        }

        public override async Task GetAll(GetAllWorkTaskCommentsRequest request, IServerStreamWriter<Comment> responseStream, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid workTaskId;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request.WorkTaskIdId) || !Guid.TryParse(request.WorkTaskIdId, out workTaskId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid work task id \"{request.WorkTaskIdId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IEnumerable<IComment> innerComments = await _workTaskCommentFactory.GetByWorkTaskId(settings, domainId, workTaskId);
                foreach (Comment comment in innerComments.Select(Map))
                {
                    await responseStream.WriteAsync(comment);
                }
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Internal System Error"), ex.Message);
            }
        }

        private static Comment Map(IComment innerComment)
        {
            return new Comment
            {
                CommentId = innerComment.CommentId.ToString("D"),
                CreateTimestamp = Timestamp.FromDateTime(innerComment.CreateTimestamp),
                DomainId = innerComment.DomainId.ToString("D"),
                Text = innerComment.Text
            };
        }
    }
}
