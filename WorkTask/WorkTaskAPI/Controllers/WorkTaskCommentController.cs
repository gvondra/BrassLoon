using AutoMapper;
using BrassLoon.CommonAPI;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Log;
using BrassLoon.Interface.WorkTask.Models;
using BrassLoon.WorkTask.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkTaskAPI.Controllers
{
    [Route("api/WorkTask/{domainId}/{workTaskId}/[controller]")]
    [ApiController]
    public class WorkTaskCommentController : WorkTaskControllerBase
    {
        private readonly ILogger<WorkTaskCommentController> _logger;
        private readonly IWorkTaskCommentFactory _workTaskCommentFactory;
        private readonly ICommentSaver _commentSaver;

        public WorkTaskCommentController(
            IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            ILogger<WorkTaskCommentController> logger,
            IExceptionService exceptionService,
            MapperFactory mapperFactory,
            IDomainService domainService,
            IWorkTaskCommentFactory workTaskCommentFactory,
            ICommentSaver commentSaver)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        {
            _logger = logger;
            _workTaskCommentFactory = workTaskCommentFactory;
            _commentSaver = commentSaver;
        }

        [HttpGet]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(List<Comment>), 200)]
        public async Task<IActionResult> GetAll([FromRoute] Guid? domainId, [FromRoute] Guid? workTaskId)
        {
            IActionResult result = null;
            try
            {
                if (!workTaskId.HasValue || workTaskId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing work task id parameter value");
                }
                else if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain id parameter value");
                }
                else if (!await VerifyDomainAccount(domainId.Value))
                {
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                else
                {
                    CoreSettings settings = CreateCoreSettings();
                    IEnumerable<IComment> comments = await _workTaskCommentFactory.GetByWorkTaskId(settings, domainId.Value, workTaskId.Value);
                    IMapper mapper = CreateMapper();
                    result = Ok(
                        comments.Select(mapper.Map<Comment>));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPost]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(Comment), 200)]
        public async Task<IActionResult> Create([FromRoute] Guid? domainId, [FromRoute] Guid? workTaskId, [FromBody] List<Comment> comments)
        {
            IActionResult result = null;
            try
            {
                if (!workTaskId.HasValue || workTaskId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing work task id parameter value");
                }
                else if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain id parameter value");
                }
                else if (comments == null || comments.Count == 0)
                {
                    result = Ok(); // if no comments are submitted then just call it ok. No comments received no comments created
                }
                else if (!await VerifyDomainAccount(domainId.Value))
                {
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                else
                {
                    CoreSettings settings = CreateCoreSettings();
                    List<IComment> innerComments = new List<IComment>();
                    foreach (Comment comment in comments.Where(c => !string.IsNullOrEmpty(c.Text)))
                    {
                        innerComments.Add(_workTaskCommentFactory.Create(domainId.Value, workTaskId.Value, comment.Text));
                    }
                    await _commentSaver.Create(settings, innerComments.ToArray());
                    IMapper mapper = CreateMapper();
                    result = Ok(
                        innerComments.Select(mapper.Map<Comment>));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }
    }
}
