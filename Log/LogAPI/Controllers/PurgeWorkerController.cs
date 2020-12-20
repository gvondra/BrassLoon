using Autofac;
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
        private readonly IContainer _container;

        public PurgeWorkerController(IOptions<Settings> settings,
            IContainer container)
        {
            _settings = settings;
            _container = container;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(PurgeWorker[]), 200)]
        public async Task<IActionResult> Search()
        {
            IActionResult result = null;
            try
            {
                using ILifetimeScope scope = _container.BeginLifetimeScope();
                SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                CoreSettings settings = settingsFactory.CreateCore(_settings.Value);
                IPurgeWorkerFactory factory = scope.Resolve<IPurgeWorkerFactory>();
                IMapper mapper = MapperConfigurationFactory.CreateMapper();
                result = Ok(
                    (await factory.GetAll(settings))
                    .Select<IPurgeWorker, PurgeWorker>(innerPurgeWorker => mapper.Map<PurgeWorker>(innerPurgeWorker))
                    );
            }
            catch (System.Exception ex)
            {
                using (ILifetimeScope scope = _container.BeginLifetimeScope())
                {
                    await LogException(ex, scope.Resolve<IExceptionService>(), scope.Resolve<SettingsFactory>(), _settings.Value);
                }
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
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    CoreSettings settings = settingsFactory.CreateCore(_settings.Value);
                    IPurgeWorkerFactory factory = scope.Resolve<IPurgeWorkerFactory>();
                    IPurgeWorker innerPurgeWorker = await factory.Get(settings, id.Value);
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
                using (ILifetimeScope scope = _container.BeginLifetimeScope())
                {
                    await LogException(ex, scope.Resolve<IExceptionService>(), scope.Resolve<SettingsFactory>(), _settings.Value);
                }
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
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    CoreSettings settings = settingsFactory.CreateCore(_settings.Value);
                    IPurgeWorkerFactory factory = scope.Resolve<IPurgeWorkerFactory>();
                    IPurgeWorker innerPurgeWorker = await factory.Get(settings, id.Value);
                    if (innerPurgeWorker == null)
                        result = NotFound();
                    else
                    {
                        if (patch.ContainsKey("Status"))
                            innerPurgeWorker.Status = (PurgeWorkerStatus)Convert.ChangeType(patch["Status"], typeof(short));
                        IPurgeWorkerSaver saver = scope.Resolve<IPurgeWorkerSaver>();
                        await saver.Update(settings, innerPurgeWorker);
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        result = Ok(mapper.Map<PurgeWorker>(innerPurgeWorker));
                    }
                }
            }
            catch (System.Exception ex)
            {
                using (ILifetimeScope scope = _container.BeginLifetimeScope())
                {
                    await LogException(ex, scope.Resolve<IExceptionService>(), scope.Resolve<SettingsFactory>(), _settings.Value);
                }
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }
    }
}
