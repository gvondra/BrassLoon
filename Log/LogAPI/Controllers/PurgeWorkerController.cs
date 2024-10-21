using AutoMapper;
using BrassLoon.CommonAPI;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Log.Models;
using BrassLoon.Log.Framework;
using BrassLoon.Log.Framework.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Log = BrassLoon.Interface.Log;

namespace LogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Constants.POLICY_SYS_ADMIN)]
    public class PurgeWorkerController : LogControllerBase
    {
        private readonly IPurgeWorkerFactory _purgeWorkerFactory;
        private readonly IPurgeWorkerSaver _purgeWorkerSaver;

        public PurgeWorkerController(
            IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            Log.IExceptionService exceptionService,
            MapperFactory mapperFactory,
            IDomainService domainService,
            IPurgeWorkerFactory purgeWorkerFactory,
            IPurgeWorkerSaver purgeWorkerSaver)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        {
            _purgeWorkerFactory = purgeWorkerFactory;
            _purgeWorkerSaver = purgeWorkerSaver;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PurgeWorker[]), 200)]
        public async Task<IActionResult> Search()
        {
            IActionResult result = null;
            try
            {
                CoreSettings settings = CreateCoreSettings();
                IMapper mapper = CreateMapper();
                result = Ok(
                    (await _purgeWorkerFactory.GetAll(settings))
                    .Select(mapper.Map<PurgeWorker>));
            }
            catch (System.Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PurgeWorker), 200)]
        public async Task<IActionResult> Get([FromRoute] Guid? id)
        {
            IActionResult result;
            try
            {
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing id parameter value");
                }
                else
                {
                    CoreSettings settings = CreateCoreSettings();
                    IPurgeWorker innerPurgeWorker = await _purgeWorkerFactory.Get(settings, id.Value);
                    if (innerPurgeWorker == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        IMapper mapper = CreateMapper();
                        result = Ok(mapper.Map<PurgeWorker>(innerPurgeWorker));
                    }
                }
            }
            catch (System.Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPatch("{id}/Status")]
        [ProducesResponseType(typeof(PurgeWorker), 200)]
        public async Task<IActionResult> UpdateStatus([FromRoute] Guid? id, [FromBody] Dictionary<string, object> patch)
        {
            IActionResult result;
            try
            {
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing id parameter value");
                }
                else if (patch == null)
                {
                    result = BadRequest("Missing patch data");
                }
                else
                {
                    CoreSettings settings = CreateCoreSettings();
                    IPurgeWorker innerPurgeWorker = await _purgeWorkerFactory.Get(settings, id.Value);
                    if (innerPurgeWorker == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        if (patch.ContainsKey("Status"))
                            innerPurgeWorker.Status = (PurgeWorkerStatus)Convert.ChangeType(patch["Status"], typeof(short), CultureInfo.InvariantCulture);
                        await _purgeWorkerSaver.Update(settings, innerPurgeWorker);
                        IMapper mapper = CreateMapper();
                        result = Ok(mapper.Map<PurgeWorker>(innerPurgeWorker));
                    }
                }
            }
            catch (System.Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }
    }
}
