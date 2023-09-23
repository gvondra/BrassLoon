using BrassLoon.WorkTask.Framework;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Purger
{
    public class PurgeProcessor
    {
        private readonly AppSettings _appSettings;
        private readonly ISettingsFactory _settingsFactory;
        private readonly ILogger<PurgeProcessor> _logger;
        private readonly IPurgeWorkerSaver _purgeWorkerSaver;

        public PurgeProcessor(
            AppSettings appSettings,
            ISettingsFactory settingsFactory,
            ILogger<PurgeProcessor> logger,
            IPurgeWorkerSaver purgeWorkerSaver)
        {
            _appSettings = appSettings;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _purgeWorkerSaver = purgeWorkerSaver;
        }

        /// <summary>
        /// Calling stored procedure to 
        /// 1) delete old completed workers
        /// 2) reset hung workers
        /// 3) add domain workers
        /// </summary>
        public async Task InitializeWorkers()
        {
            await _purgeWorkerSaver.InitializePurgeWorker(_settingsFactory.CreateCore());
            _logger.LogInformation("Workers Initialized");
        }
    }
}
