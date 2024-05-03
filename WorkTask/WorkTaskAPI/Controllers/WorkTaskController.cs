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
    [Route("api/[controller]/{domainId}")]
    [ApiController]
    public class WorkTaskController : WorkTaskControllerBase
    {
        private readonly ILogger<WorkTaskController> _logger;
        private readonly IWorkTaskFactory _workTaskFactory;
        private readonly IWorkTaskSaver _workTaskSaver;
        private readonly IWorkTaskTypeFactory _workTaskTypeFactory;
        private readonly IWorkTaskStatusFactory _workTaskStatusFactory;
        private readonly IWorkTaskPatcher _workTaskPatcher;

        public WorkTaskController(
            IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            ILogger<WorkTaskController> logger,
            MapperFactory mapperFactory,
            IDomainService domainService,
            IWorkTaskFactory workTaskFactory,
            IWorkTaskSaver workTaskSaver,
            IWorkTaskTypeFactory workTaskTypeFactory,
            IWorkTaskStatusFactory workTaskStatusFactory,
            IWorkTaskPatcher workTaskPatcher)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        {
            _logger = logger;
            _workTaskFactory = workTaskFactory;
            _workTaskSaver = workTaskSaver;
            _workTaskTypeFactory = workTaskTypeFactory;
            _workTaskStatusFactory = workTaskStatusFactory;
            _workTaskPatcher = workTaskPatcher;

        }

        [HttpGet]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(WorkTask[]), 200)]
        public async Task<IActionResult> Search(
            [FromRoute] Guid? domainId,
            [FromQuery] short? referenceType,
            [FromQuery] string referenceValue,
            [FromQuery] bool? includeClosed = null)
        {
            IActionResult result = null;
            try
            {
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain id parameter value");
                }
                else if (!await VerifyDomainAccount(domainId.Value))
                {
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                else
                {
                    IEnumerable<IWorkTask> innerWorkTasks = null;
                    CoreSettings settings = CreateCoreSettings();
                    if (referenceType.HasValue && !string.IsNullOrEmpty(referenceValue))
                    {
                        innerWorkTasks = await _workTaskFactory.GetByContextReference(settings, domainId.Value, referenceType.Value, referenceValue, includeClosed ?? false);
                    }
                    if (innerWorkTasks == null)
                        innerWorkTasks = new List<IWorkTask>();
                    IMapper mapper = CreateMapper();
                    result = Ok(
                        innerWorkTasks
                        .Select(mapper.Map<WorkTask>)
                        .ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpGet("{id}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(WorkTask), 200)]
        public async Task<IActionResult> Get([FromRoute] Guid? domainId, [FromRoute] Guid? id)
        {
            IActionResult result = null;
            try
            {
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing id parameter value");
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
                    IWorkTask innerWorkTask = await _workTaskFactory.Get(settings, domainId.Value, id.Value);
                    if (!innerWorkTask.DomainId.Equals(domainId.Value))
                        innerWorkTask = null;
                    if (innerWorkTask == null)
                        result = NotFound();
                    if (result == null)
                    {
                        IMapper mapper = CreateMapper();
                        result = Ok(
                            mapper.Map<WorkTask>(innerWorkTask));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpGet("/api/WorkGroup/{domainId}/{workGroupId}/WorkTask")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(List<WorkTask>), 200)]
        public async Task<IActionResult> GetByWorkGroupId([FromRoute] Guid? domainId, [FromRoute] Guid? workGroupId, [FromQuery] bool? includeClosed = null)
        {
            IActionResult result = null;
            try
            {
                if (!workGroupId.HasValue || workGroupId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing id parameter value");
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
                    IEnumerable<IWorkTask> innerWorkTasks = await _workTaskFactory
                        .GetByWorkGroupId(settings, domainId.Value, workGroupId.Value, includeClosed ?? false);
                    IMapper mapper = CreateMapper();
                    result = Ok(
                        innerWorkTasks
                        .Where(wt => wt.DomainId.Equals(domainId.Value))
                        .Select(mapper.Map<WorkTask>));
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
        [ProducesResponseType(typeof(WorkTask), 200)]
        public async Task<IActionResult> Create([FromRoute] Guid? domainId, [FromBody] WorkTask workTask)
        {
            IActionResult result = null;
            try
            {
                CoreSettings settings = CreateCoreSettings();
                IWorkTaskType innerWorkTaskType = null;
                IWorkTaskStatus innerWorkTaskStatus = null;
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing domain id parameter value");
                else if (!await VerifyDomainAccount(domainId.Value))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                else if (workTask.WorkTaskType == null)
                    result = BadRequest("Missing work task type");
                else
                    result = ValidateWorkTask(workTask);
                if (result == null && domainId.HasValue)
                {
                    if (workTask.WorkTaskType.WorkTaskTypeId.HasValue)
                    {
                        innerWorkTaskType = await _workTaskTypeFactory.Get(settings, domainId.Value, workTask.WorkTaskType.WorkTaskTypeId.Value);
                    }
                    if (innerWorkTaskType == null)
                        result = BadRequest($"Work task type not found ({workTask.WorkTaskType?.WorkTaskTypeId})");
                }
                if (result == null && innerWorkTaskType != null)
                {
                    innerWorkTaskStatus = (await _workTaskStatusFactory.GetByWorkTaskTypeId(settings, domainId.Value, innerWorkTaskType.WorkTaskTypeId))
                        .FirstOrDefault(s => s.WorkTaskStatusId.Equals(workTask.WorkTaskStatus.WorkTaskStatusId.Value));
                    if (innerWorkTaskStatus == null)
                        result = BadRequest("Invalid work task status. The status doesn't exist or is not valid for the task type");
                }
                if (result == null && domainId.HasValue)
                {
                    IWorkTask innerWorkTask = _workTaskFactory.Create(domainId.Value, innerWorkTaskType, innerWorkTaskStatus);
                    IMapper mapper = CreateMapper();
                    _ = mapper.Map(workTask, innerWorkTask);
                    ApplyContexts(workTask, innerWorkTask);
                    await _workTaskSaver.Create(settings, innerWorkTask);
                    result = Ok(
                        Map(mapper, innerWorkTask));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [NonAction]
        private IActionResult ValidateWorkTask(WorkTask workTask)
        {
            IActionResult result = null;
            if (workTask == null)
                result = BadRequest("Missing work task data");
            else if (string.IsNullOrEmpty(workTask.Title))
                result = BadRequest("Missing work task title");
            else if (workTask.WorkTaskStatus == null)
                result = BadRequest("Missing work task status");
            return result;
        }

        [NonAction]
        private static void ApplyContexts(WorkTask workTask, IWorkTask innerWorkTask)
        {
            if (workTask.WorkTaskContexts != null)
            {
                foreach (WorkTaskContext workTaskContext in workTask.WorkTaskContexts.Where(c => c.ReferenceType.HasValue && !string.IsNullOrEmpty(c.ReferenceValue)))
                {
                    // the IWorkTask will filter out duplicates
                    _ = innerWorkTask.AddContext(workTaskContext.ReferenceType.Value, workTaskContext.ReferenceValue);
                }
            }
        }

        [NonAction]
        private static WorkTask Map(IMapper mapper, IWorkTask innerWorkTask)
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
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing id parameter value");
                else if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing domain id parameter value");
                else if (!await VerifyDomainAccount(domainId.Value))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                else
                    result = ValidateWorkTask(workTask);
                if (result == null && id.HasValue && domainId.HasValue)
                {
                    innerWorkTask = await _workTaskFactory.Get(settings, domainId.Value, id.Value);
                    if (innerWorkTask == null)
                    {
                        result = NotFound();
                    }
                }
                if (result == null && innerWorkTask != null)
                {
                    innerWorkTaskStatus = (await _workTaskStatusFactory.GetByWorkTaskTypeId(settings, domainId.Value, innerWorkTask.WorkTaskType.WorkTaskTypeId))
                        .FirstOrDefault(s => s.WorkTaskStatusId.Equals(workTask.WorkTaskStatus.WorkTaskStatusId.Value));
                    if (innerWorkTaskStatus == null)
                        result = BadRequest("Invalid work task status. The status doesn't exist or is not valid for the task type");
                }
                if (result == null && innerWorkTask != null && innerWorkTaskStatus != null)
                {
                    IMapper mapper = CreateMapper();
                    _ = mapper.Map(workTask, innerWorkTask);
                    innerWorkTask.WorkTaskStatus = innerWorkTaskStatus;
                    ApplyContexts(workTask, innerWorkTask);
                    await _workTaskSaver.Update(settings, innerWorkTask);
                    result = Ok(
                        Map(mapper, innerWorkTask));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPut("{id}/AssignTo")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(ClaimWorkTaskResponse), 200)]
        public async Task<IActionResult> Claim([FromRoute] Guid? domainId, [FromRoute] Guid? id, [FromQuery] string assignToUserId, [FromQuery] DateTime? assignedDate = null)
        {
            IActionResult result = null;
            try
            {
                CoreSettings settings = CreateCoreSettings();
                IWorkTask innerWorkTask = null;
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing id parameter value");
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
                    innerWorkTask = await _workTaskFactory.Get(settings, domainId.Value, id.Value);
                    if (innerWorkTask == null)
                    {
                        result = NotFound();
                    }
                }
                if (result == null
                    && innerWorkTask != null
                    && !string.IsNullOrEmpty(assignToUserId)
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
                if (result == null && string.IsNullOrEmpty(assignToUserId) && assignedDate.HasValue)
                {
                    assignedDate = null;
                }
                if (result == null && !string.IsNullOrEmpty(assignToUserId) && !assignedDate.HasValue)
                {
                    assignedDate = DateTime.Today;
                }
                if (result == null && innerWorkTask != null)
                {
                    ClaimWorkTaskResponse respone = new ClaimWorkTaskResponse();
                    respone.IsAssigned = await _workTaskSaver.Claim(settings, domainId.Value, id.Value, assignToUserId, assignedDate);
                    if (respone.IsAssigned)
                    {
                        respone.Message = "Work task assigned";
                        respone.AssignedToUserId = assignToUserId;
                        respone.AssignedDate = assignedDate;
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
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [NonAction]
        private IActionResult ValidatePatchData(List<Dictionary<string, object>> patchData)
        {
            int i;
            string[] required = new string[] { "WorkTaskId" };
            string[] validFields = new string[] { "WorkTaskStatusId" };
            IActionResult result = null;
            string fields = string.Join(
                ", ",
                patchData.SelectMany(dct => dct.Keys)
                .Where(fld => !required.Concat(validFields).Contains(fld))
                .Distinct()
                .OrderBy(fld => fld));
            if (!string.IsNullOrEmpty(fields))
            {
                result = BadRequest("Found invalid fields: " + fields);
            }
            else
            {
                i = 0;
                while (result == null && i < required.Length)
                {
                    if (patchData.Exists(dct => !dct.ContainsKey(required[i])))
                    {
                        result = BadRequest($"Found patch missing required field \"{required[i]}\"");
                    }
                    i += 1;
                }
            }
            return result;
        }

        [HttpPatch]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(WorkTask[]), 200)]
        public async Task<IActionResult> Patch([FromRoute] Guid? domainId, [FromBody] List<Dictionary<string, object>> patchData)
        {
            IActionResult result;
            try
            {
                CoreSettings settings = CreateCoreSettings();
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing domain id parameter value");
                else if (!await VerifyDomainAccount(domainId.Value))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                else
                    result = ValidatePatchData(patchData);
                if (result == null && domainId.HasValue)
                {
                    IEnumerable<IWorkTask> innerWorkTasks = await _workTaskPatcher.Apply(settings, domainId.Value, patchData);
                    await _workTaskSaver.Update(settings, innerWorkTasks.ToArray());
                    IMapper mapper = CreateMapper();
                    result = Ok(
                        innerWorkTasks
                        .Select(wt => Map(mapper, wt))
                        .ToList());
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
