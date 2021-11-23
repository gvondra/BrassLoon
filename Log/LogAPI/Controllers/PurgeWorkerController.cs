using AutoMapper;
using BrassLoon.Interface.Log;
using BrassLoon.Interface.Log.Models;
using BrassLoon.Log.Framework;
using BrassLoon.Log.Framework.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("SysAdmin")]
    public class PurgeWorkerController : LogControllerBase
    {
        private readonly IOptions<Settings> _settings;
        private readonly SettingsFactory _settingsFactory;
        private readonly Lazy<IExceptionService> _exceptionService;
        private readonly IPurgeWorkerFactory _purgeWorkerFactory;
        private readonly IPurgeWorkerSaver _purgeWorkerSaver;

        public PurgeWorkerController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            Lazy<IExceptionService> exceptionService,
            IPurgeWorkerFactory purgeWorkerFactory,
            IPurgeWorkerSaver purgeWorkerSaver)
        {
            _settings = settings;
            _settingsFactory = settingsFactory;
            _exceptionService = exceptionService; 
            _purgeWorkerFactory = purgeWorkerFactory;
            _purgeWorkerSaver = purgeWorkerSaver;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(PurgeWorker[]), 200)]
        public async Task<IActionResult> Search()
        {
            IActionResult result = null;
            try
            {
                CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                IMapper mapper = MapperConfigurationFactory.CreateMapper();
                result = Ok(
                    (await _purgeWorkerFactory.GetAll(settings))
                    .Select<IPurgeWorker, PurgeWorker>(innerPurgeWorker => mapper.Map<PurgeWorker>(innerPurgeWorker))
                    );
            }
            catch (System.Exception ex)
            {
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PurgeWorker), 200)]
        public async Task<IActionResult> Get([FromRoute] Guid? id)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing id parameter value");
                if (result == null)
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IPurgeWorker innerPurgeWorker = await _purgeWorkerFactory.Get(settings, id.Value);
                    if (innerPurgeWorker == null)
                        result = NotFound();
                    else
                    {
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        result = Ok(mapper.Map<PurgeWorker>(innerPurgeWorker));
                    }                    
                }
            }
            catch (System.Exception ex)
            {
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPatch("{id}/Status")]
        [ProducesResponseType(typeof(PurgeWorker), 200)]
        public async Task<IActionResult> UpdateStatus([FromRoute] Guid? id, [FromBody] Dictionary<string, object> patch)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing id parameter value");
                if (result == null && patch == null)
                    result = BadRequest("Missing patch data");
                if (result == null)
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IPurgeWorker innerPurgeWorker = await _purgeWorkerFactory.Get(settings, id.Value);
                    if (innerPurgeWorker == null)
                        result = NotFound();
                    else
                    {
                        if (patch.ContainsKey("Status"))
                            innerPurgeWorker.Status = (PurgeWorkerStatus)Convert.ChangeType(patch["Status"], typeof(short));
                        await _purgeWorkerSaver.Update(settings, innerPurgeWorker);
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        result = Ok(mapper.Map<PurgeWorker>(innerPurgeWorker));
                    }
                }
            }
            catch (System.Exception ex)
            {
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }
    }
}
