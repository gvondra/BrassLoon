using AutoMapper;
using BrassLoon.CommonAPI;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Log;
using BrassLoon.Interface.WorkTask.Models;
using BrassLoon.WorkTask.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkTaskAPI.Controllers
{
    [Route("api/[controller]/{domainId}")]
    [ApiController]
    public class WorkTaskController : WorkTaskControllerBase
    {
        private readonly IWorkTaskFactory _workTaskFactory;
        private readonly IWorkTaskSaver _workTaskSaver;
        private readonly IWorkTaskCommentFactory _workTaskCommentFactory;
        private readonly ICommentSaver _commentSaver;
        private readonly IWorkTaskTypeFactory _workTaskTypeFactory;
        private readonly IWorkTaskStatusFactory _workTaskStatusFactory;

        public WorkTaskController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            MapperFactory mapperFactory,
            IDomainService domainService,
            IWorkTaskFactory workTaskFactory,
            IWorkTaskSaver workTaskSaver,
            IWorkTaskCommentFactory workTaskCommentFactory,
            ICommentSaver commentSaver,
            IWorkTaskTypeFactory workTaskTypeFactory,
            IWorkTaskStatusFactory workTaskStatusFactory)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        {
            _workTaskFactory = workTaskFactory;
            _workTaskSaver = workTaskSaver;
            _workTaskCommentFactory = workTaskCommentFactory;
            _commentSaver = commentSaver;
            _workTaskTypeFactory = workTaskTypeFactory;
            _workTaskStatusFactory = workTaskStatusFactory;
        }

        [HttpGet("{id}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(WorkTask), 200)]
        public async Task<IActionResult> Get([FromRoute] Guid? domainId, [FromRoute] Guid? id)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing id parameter value");
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                {
                    CoreSettings settings = CreateCoreSettings();
                    IWorkTask innerWorkTask = await _workTaskFactory.Get(settings, id.Value);
                    if (innerWorkTask == null)
                        result = NotFound();
                    if (result == null)
                    {
                        IMapper mapper = CreateMapper();
                        result = Ok(
                            mapper.Map<WorkTask>(innerWorkTask)
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpGet("/api/WorkGroup/{domainId}/{workGroupId}/WorkTask")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(List<WorkTask>), 200)]
        public async Task<IActionResult> GetByWorkGroupId([FromRoute] Guid? domainId, [FromRoute] Guid? workGroupId, bool? includeClosed = null)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!workGroupId.HasValue || workGroupId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing id parameter value");
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                {
                    CoreSettings settings = CreateCoreSettings();
                    IEnumerable<IWorkTask> innerWorkTasks = await _workTaskFactory.GetByWorkGroupId(settings, workGroupId.Value, includeClosed ?? false);
                    IMapper mapper = CreateMapper();
                    result = Ok(
                        innerWorkTasks.Select<IWorkTask, WorkTask>(t => mapper.Map<WorkTask>(t))                        
                        );
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPost()]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(WorkTask), 200)]
        public async Task<IActionResult> Create([FromRoute] Guid? domainId, [FromBody] WorkTask workTask)
        {
            IActionResult result = null;
            try
            {
                CoreSettings settings = CreateCoreSettings();
                IWorkTaskType innerWorkTaskType = null;
                IWorkTaskStatus innerWorkTaskStatus = null;
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null && workTask.WorkTaskType == null)
                    result = BadRequest("Missing work task type");
                if (result == null)
                    result = ValidateWorkTask(workTask);
                if (result == null)
                {
                    if (workTask.WorkTaskType.WorkTaskTypeId.HasValue)
                    {
                        innerWorkTaskType = await _workTaskTypeFactory.Get(settings, workTask.WorkTaskType.WorkTaskTypeId.Value);
                    }
                    if (innerWorkTaskType == null)
                        result = BadRequest($"Work task type not found ({workTask.WorkTaskType?.WorkTaskTypeId})");
                }
                if (result == null && innerWorkTaskType != null)
                {
                    innerWorkTaskStatus = (await _workTaskStatusFactory.GetByWorkTaskTypeId(settings, innerWorkTaskType.WorkTaskTypeId))
                        .FirstOrDefault(s => s.WorkTaskStatusId.Equals(workTask.WorkTaskStatus.WorkTaskStatusId.Value));
                    if (innerWorkTaskStatus == null)
                        result = BadRequest("Invalid work task status. The status doesn't exist or is not valid for the task type");
                }
                if (result == null)
                {
                    IWorkTask innerWorkTask = _workTaskFactory.Create(domainId.Value, innerWorkTaskType, innerWorkTaskStatus);
                    IMapper mapper = CreateMapper();
                    mapper.Map(workTask, innerWorkTask);
                    ApplyContexts(workTask, innerWorkTask);
                    await _workTaskSaver.Create(settings, innerWorkTask);
                    result = Ok(
                        Map(mapper, innerWorkTask)
                        );
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [NonAction]
        private IActionResult ValidateWorkTask(WorkTask workTask)
        {
            IActionResult result = null;
            if (result == null && workTask == null)
                result = BadRequest("Missing work task data");
            if (result == null && string.IsNullOrEmpty(workTask?.Title))
                result = BadRequest("Missing work task title");
            if (result == null && workTask.WorkTaskStatus == null)
                result = BadRequest("Missing work task status");
            return result;
        }

        [NonAction] 
        private void ApplyContexts(WorkTask workTask, IWorkTask innerWorkTask)
        {
            if (workTask.WorkTaskContexts != null)
            {
                foreach (WorkTaskContext workTaskContext in workTask.WorkTaskContexts.Where(c => c.ReferenceType.HasValue && !string.IsNullOrEmpty(c.ReferenceValue)))
                {
                    // the IWorkTask will filter out duplicates
                    innerWorkTask.AddContext(workTaskContext.ReferenceType.Value, workTaskContext.ReferenceValue);
                }
            }
        }

        [NonAction]
        private WorkTask Map(IMapper mapper, IWorkTask innerWorkTask)
        {
            WorkTask workTask = mapper.Map<WorkTask>(innerWorkTask);
            return workTask;
        }

        [HttpPut("{id}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(WorkTask), 200)]
        public async Task<IActionResult> Update([FromRoute] Guid? domainId, [FromRoute] Guid? id, [FromBody] WorkTask workTask)
        {
            IActionResult result = null;
            try
            {
                CoreSettings settings = CreateCoreSettings();
                IWorkTask innerWorkTask = null;
                IWorkTaskStatus innerWorkTaskStatus = null;
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing id parameter value");
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                    result = ValidateWorkTask(workTask);
                if (result == null)
                {
                    innerWorkTask = await _workTaskFactory.Get(settings, id.Value);
                    if (innerWorkTask == null)
                    {
                        result = NotFound();
                    }
                }
                if (result == null && innerWorkTask != null)
                {                    
                    innerWorkTaskStatus = (await _workTaskStatusFactory.GetByWorkTaskTypeId(settings, innerWorkTask.WorkTaskType.WorkTaskTypeId))
                        .FirstOrDefault(s => s.WorkTaskStatusId.Equals(workTask.WorkTaskStatus.WorkTaskStatusId.Value));
                    if (innerWorkTaskStatus == null)
                        result = BadRequest("Invalid work task status. The status doesn't exist or is not valid for the task type");
                }
                if (result == null && innerWorkTask != null && innerWorkTaskStatus != null)
                {
                    IMapper mapper = CreateMapper();
                    mapper.Map(workTask, innerWorkTask);
                    innerWorkTask.WorkTaskStatus = innerWorkTaskStatus;
                    ApplyContexts(workTask, innerWorkTask);
                    await _workTaskSaver.Update(settings, innerWorkTask);
                    result = Ok(
                        Map(mapper, innerWorkTask)
                        );
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPut("{id}/AssignTo")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(ClaimWorkTaskResponse), 200)]
        public async Task<IActionResult> Claim([FromRoute] Guid? domainId, [FromRoute] Guid? id, [FromQuery] string assignToUserId)
        {
            IActionResult result = null;
            try
            {
                CoreSettings settings = CreateCoreSettings();
                IWorkTask innerWorkTask = null;
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing id parameter value");
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                {
                    innerWorkTask = await _workTaskFactory.Get(settings, id.Value);
                    if (innerWorkTask == null)
                    {
                        result = NotFound();
                    }
                }
                if (result == null && innerWorkTask != null)
                {
                    if (!string.IsNullOrEmpty(assignToUserId) 
                        && !string.IsNullOrEmpty(innerWorkTask.AssignedToUserId)
                        && !string.Equals(assignToUserId, innerWorkTask.AssignedToUserId, StringComparison.OrdinalIgnoreCase))
                    {
                        result = Ok(
                            new ClaimWorkTaskResponse
                            {
                                IsAssigned = false,
                                Message = "Work task already assigned"
                            });
                    }
                }
                if (result == null && innerWorkTask != null)
                {
                    ClaimWorkTaskResponse respone = new ClaimWorkTaskResponse();
                    respone.IsAssigned = await _workTaskSaver.Claim(settings, domainId.Value, id.Value, assignToUserId);
                    if (respone.IsAssigned)
                    {
                        respone.Message = "Work task assigned";
                        respone.AssignedToUserId = assignToUserId;
                    }
                    else
                    {
                        respone.Message = "Failed to assign work task";
                    }
                    result = Ok(respone);
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }
    }
}
