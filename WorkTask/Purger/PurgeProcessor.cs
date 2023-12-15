using BrassLoon.WorkTask.Framework;
using BrassLoon.WorkTask.Framework.Enumerations;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Purger
{
    public class PurgeProcessor
    {
        private readonly AppSettings _appSettings;
        private readonly ISettingsFactory _settingsFactory;
        private readonly ILogger<PurgeProcessor> _logger;
        private readonly IPurgeWorkerFactory _purgeWorkerFactory;
        private readonly IPurgeWorkerSaver _purgeWorkerSaver;
        private readonly IPurgeSaver _purgeSaver;

        public PurgeProcessor(
            AppSettings appSettings,
            ISettingsFactory settingsFactory,
            ILogger<PurgeProcessor> logger,
            IPurgeWorkerFactory purgeWorkerFactory,
            IPurgeWorkerSaver purgeWorkerSaver,
            IPurgeSaver purgeSaver)
        {
            _appSettings = appSettings;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _purgeWorkerFactory = purgeWorkerFactory;
            _purgeWorkerSaver = purgeWorkerSaver;
            _purgeSaver = purgeSaver;
        }

        public async Task StartPurge()
        {
            CoreSettings settings = _settingsFactory.CreateCore();
            Guid? workerId = await _purgeWorkerFactory.Claim(settings);
            while (workerId.HasValue && !workerId.Value.Equals(Guid.Empty))
            {
                _logger.LogInformation($"Claimed worker {workerId.Value}");
                IPurgeWorker purgeWorker = await _purgeWorkerFactory.Get(settings, workerId.Value);
                try
                {
                    await Purge(purgeWorker);
                }
                catch (Exception ex)
                {
                    purgeWorker.Status = PurgeWorkerStatus.Error;
                    _logger.LogError(ex, ex.Message);
                    throw;
                }
                finally
                {
                    await _purgeWorkerSaver.Update(settings, purgeWorker);
                    _logger.LogInformation($"Updated Worker {workerId.Value} Status {purgeWorker.Status}");
                }
                workerId = await _purgeWorkerFactory.Claim(settings);
            }
        }

        private async Task Purge(IPurgeWorker purgeWorker)
        {
            if (purgeWorker.Status == PurgeWorkerStatus.InProgress)
            {
                await UpdateWorkTaskMetaData(purgeWorker.DomainId);
                await PurgeWorkTask(purgeWorker.DomainId);
                purgeWorker.Status = PurgeWorkerStatus.Complete;
            }
        }

        private async Task UpdateWorkTaskMetaData(Guid domainId)
        {
            CoreSettings settings = _settingsFactory.CreateCore();
            DateTime expiration = DateTime.UtcNow;
            expiration = new DateTime(expiration.Year, expiration.Month, 1, 0, 0, 0, DateTimeKind.Unspecified).AddMonths(1);
            _logger.LogInformation("Updating purge meta data for work tasks");
            await _purgeSaver.InitializeWorkTask(settings, domainId, expiration, _appSettings.DefaultPurgePeriod);
        }

        private async Task PurgeWorkTask(Guid domainId)
        {
            CoreSettings settings = _settingsFactory.CreateCore();
            _logger.LogInformation("Purging work tasks");
            await _purgeSaver.PurgeWorkTask(settings, domainId, DateTime.UtcNow);
        }

        public async Task PugeMetaData()
        {
            DateTime minTimstamp = DateTime.UtcNow.AddDays(_appSettings.PurgeMetaDataDays * -1).Date.ToUniversalTime();
            _logger.LogInformation($"Purging Meta Data using min timestamp {minTimstamp:O}");
            CoreSettings settings = _settingsFactory.CreateCore();
            await _purgeSaver.DeleteWorkTaskByMinTimestamp(settings, minTimstamp);
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
