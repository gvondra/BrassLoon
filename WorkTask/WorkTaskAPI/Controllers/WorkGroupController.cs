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
    [Route("api/[controller]")]
    [ApiController]
    public class WorkGroupController : WorkTaskControllerBase
    {
        private readonly ILogger<WorkGroupController> _logger;
        private readonly IWorkGroupFactory _workGroupFactory;
        private readonly IWorkGroupSaver _workGroupSaver;

        public WorkGroupController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            ILogger<WorkGroupController> logger,
            IExceptionService exceptionService,
            MapperFactory mapperFactory,
            IDomainService domainService,
            IWorkGroupFactory workGroupFactory,
            IWorkGroupSaver workGroupSaver)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        {
            _logger = logger;
            _workGroupFactory = workGroupFactory;
            _workGroupSaver = workGroupSaver;
        }

        [HttpGet("{domainId}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(List<WorkGroup>), 200)]
        public async Task<IActionResult> GetAll([FromRoute] Guid? domainId, [FromQuery] string userId = null)
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
                    IEnumerable<IWorkGroup> innerWorkGroups;
                    CoreSettings settings = CreateCoreSettings();
                    IMapper mapper = CreateMapper();
                    if (!string.IsNullOrEmpty(userId))
                    {
                        innerWorkGroups = await _workGroupFactory.GetByMemberUserId(settings, domainId.Value, userId);
                    }
                    else
                    {
                        innerWorkGroups = await _workGroupFactory.GetByDomainId(settings, domainId.Value);
                    }
                    result = Ok(
                        innerWorkGroups.Select<IWorkGroup, WorkGroup>(t => mapper.Map<WorkGroup>(t))
                        );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpGet("{domainId}/{id}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(WorkGroup), 200)]
        public async Task<IActionResult> Get([FromRoute] Guid? domainId, [FromRoute] Guid? id)
        {
            IActionResult result;
            try
            {
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain id parameter value");
                }
                else if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing id parameter value");
                }
                else if (!await VerifyDomainAccount(domainId.Value))
                {
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                else
                {
                    CoreSettings settings = CreateCoreSettings();
                    IWorkGroup innerWorkGroup = await _workGroupFactory.Get(settings, domainId.Value, id.Value);
                    if (innerWorkGroup == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        IMapper mapper = CreateMapper();
                        result = Ok(
                            mapper.Map<WorkGroup>(innerWorkGroup)
                            );
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

        [NonAction]
        private IActionResult ValidateRequest(WorkGroup workGroup)
        {
            IActionResult result = null;
            if (result == null && workGroup == null)
                result = BadRequest("Missing work group body");
            if (result == null && string.IsNullOrEmpty(workGroup?.Title))
                result = BadRequest("Missing work group title value");
            return result;
        }

        [HttpPost("{domainId}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(WorkGroup), 200)]
        public async Task<IActionResult> Create([FromRoute] Guid? domainId, [FromBody] WorkGroup workGroup)
        {
            IActionResult result;
            try
            {
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing domain id parameter value");
                else if (!await VerifyDomainAccount(domainId.Value))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                else
                    result = ValidateRequest(workGroup);
                if (result == null)
                {
                    CoreSettings settings = CreateCoreSettings();
                    IWorkGroup innerWorkGroup = _workGroupFactory.Create(domainId.Value);
                    IMapper mapper = CreateMapper();
                    mapper.Map(workGroup, innerWorkGroup);
                    ApplyMemberChanges(innerWorkGroup, workGroup.MemberUserIds);
                    await _workGroupSaver.Create(settings, innerWorkGroup);
                    result = Ok(mapper.Map<WorkGroup>(innerWorkGroup));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPut("{domainId}/{id}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(WorkGroup), 200)]
        public async Task<IActionResult> Update([FromRoute] Guid? domainId, [FromRoute] Guid? id, [FromBody] WorkGroup workGroup)
        {
            IActionResult result;
            try
            {
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing domain id parameter value");
                else if (!id.HasValue || id.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing id parameter value");
                else if (!await VerifyDomainAccount(domainId.Value))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                else
                    result = ValidateRequest(workGroup);
                if (result == null)
                {
                    CoreSettings settings = CreateCoreSettings();
                    IWorkGroup innerWorkGroup = await _workGroupFactory.Get(settings, domainId.Value, id.Value);
                    if (innerWorkGroup == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        IMapper mapper = CreateMapper();
                        mapper.Map(workGroup, innerWorkGroup);
                        ApplyMemberChanges(innerWorkGroup, workGroup.MemberUserIds);
                        await _workGroupSaver.Update(settings, innerWorkGroup);
                        result = Ok(
                            mapper.Map<WorkGroup>(innerWorkGroup)
                            );
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

        [NonAction]
        private static void ApplyMemberChanges(IWorkGroup innerWorkGroup, List<string> memberUserIds)
        {
            if (memberUserIds != null)
            {
                foreach (string userId in memberUserIds)
                {
                    // iwork group will ellimate duplicates
                    innerWorkGroup.AddMember(userId);
                }
                foreach (string userId in innerWorkGroup.MemberUserIds)
                {
                    if (!memberUserIds.Any(id => string.Equals(id, userId, StringComparison.OrdinalIgnoreCase)))
                    {
                        innerWorkGroup.RemoveMember(userId);
                    }
                }
            }
        }

        [HttpPost("{domainId}/{id}/WorkTaskType")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        public async Task<IActionResult> AddWorkTaskTypeLink([FromRoute] Guid? domainId, [FromRoute] Guid? id, [FromQuery] Guid? workTaskTypeId)
        {
            IActionResult result;
            try
            {
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain id parameter value");
                }
                else if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing id parameter value");
                }
                else if (!workTaskTypeId.HasValue || workTaskTypeId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing work task type id parameter value");
                }
                else if (!await VerifyDomainAccount(domainId.Value))
                {
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                else
                {
                    CoreSettings settings = CreateCoreSettings();
                    await _workGroupSaver.CreateWorkTaskTypeGroup(settings, domainId.Value, workTaskTypeId.Value, id.Value);
                    result = Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpDelete("{domainId}/{id}/WorkTaskType")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        public async Task<IActionResult> DeleteWorkTaskTypeLink([FromRoute] Guid? domainId, [FromRoute] Guid? id, [FromQuery] Guid? workTaskTypeId)
        {
            IActionResult result;
            try
            {
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain id parameter value");
                }
                else if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing id parameter value");
                }
                else if (!workTaskTypeId.HasValue || workTaskTypeId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing work task type id parameter value");
                }
                else if (!await VerifyDomainAccount(domainId.Value))
                {
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                else
                {
                    CoreSettings settings = CreateCoreSettings();
                    await _workGroupSaver.DeleteWorkTaskTypeGroup(settings, domainId.Value, workTaskTypeId.Value, id.Value);
                    result = Ok();
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
