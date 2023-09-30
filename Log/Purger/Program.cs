using Autofac;
using BrassLoon.Interface.Log;
using BrassLoon.Log.Framework;
using BrassLoon.Log.Framework.Enumerations;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrassLoon.Log.Purger
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                AppSettings appSettings = LoadSettings(args);
                DependencyInjection.ContainerFactory.Initialize(appSettings);
                await StartPurge();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        private static async Task StartPurge()
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
            AppSettings appSettings = scope.Resolve<AppSettings>();
            SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
            ITraceService traceService = scope.Resolve<ITraceService>();
            await WriteTrace(traceService, appSettings, settingsFactory, "Start Purge");
            await InitializeWorkers();
            CoreSettings settings = settingsFactory.CreateCore();
            IPurgeWorkerFactory workerFactory = scope.Resolve<IPurgeWorkerFactory>();
            IPurgeWorkerSaver workerSaver = scope.Resolve<IPurgeWorkerSaver>();
            Guid? workerId = await workerFactory.Claim(settings);
            while (workerId.HasValue && !workerId.Value.Equals(Guid.Empty))
            {
                await WriteTrace(traceService, appSettings, settingsFactory, $"Claimed worker {workerId.Value}");
                IPurgeWorker purgeWorker = await workerFactory.Get(settings, workerId.Value);
                try
                {
                    await Purge(purgeWorker);
                }
                catch (System.Exception ex)
                {
                    purgeWorker.Status = PurgeWorkerStatus.Error;
                    try
                    {
                        await scope.Resolve<IExceptionService>().Create(settingsFactory.CreateLog(), appSettings.ExceptionLoggingDomainId, ex);
                    }
                    catch { }
                    throw;
                }
                finally
                {
                    await workerSaver.Update(settings, purgeWorker);
                    await WriteTrace(traceService, appSettings, settingsFactory, $"Updated Worker {workerId.Value} Status {purgeWorker.Status}");
                }
                workerId = await workerFactory.Claim(settings);
            }
            try
            {
                await WriteTrace(traceService, appSettings, settingsFactory, "Purging meta data");
                await PurgMetaData(appSettings, settingsFactory, scope.Resolve<IPurgeSaver>());
            }
            catch (System.Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                try
                {
                    await scope.Resolve<IExceptionService>().Create(settingsFactory.CreateLog(), appSettings.ExceptionLoggingDomainId, ex);
                }
                catch { }
            }
        }

        private static async Task Purge(IPurgeWorker purgeWorker)
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
            AppSettings appSettings = scope.Resolve<AppSettings>();
            SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
            if (purgeWorker.Status == PurgeWorkerStatus.InProgress)
            {
                await Task.WhenAll(
                    Task.Run(() => UpdateExceptionMetaData(
                        appSettings,
                        settingsFactory,
                        scope.Resolve<ITraceService>(),
                        scope.Resolve<IPurgeSaver>(),
                        purgeWorker.DomainId)),
                    Task.Run(() => UpdateMetricMetaData(
                        appSettings,
                        settingsFactory,
                        scope.Resolve<ITraceService>(),
                        scope.Resolve<IPurgeSaver>(),
                        purgeWorker.DomainId)),
                    Task.Run(() => UpdateTraceMetaData(
                        appSettings,
                        settingsFactory,
                        scope.Resolve<ITraceService>(),
                        scope.Resolve<IPurgeSaver>(),
                        purgeWorker.DomainId)));
                await Task.WhenAll(
                    Task.Run(() => PurgeException(
                        appSettings,
                        settingsFactory,
                        scope.Resolve<ITraceService>(),
                        scope.Resolve<IPurgeSaver>(),
                        purgeWorker.DomainId)),
                    Task.Run(() => PurgeMetric(
                        appSettings,
                        settingsFactory,
                        scope.Resolve<ITraceService>(),
                        scope.Resolve<IPurgeSaver>(),
                        purgeWorker.DomainId)),
                    Task.Run(() => PurgeTrace(
                        appSettings,
                        settingsFactory,
                        scope.Resolve<ITraceService>(),
                        scope.Resolve<IPurgeSaver>(),
                        purgeWorker.DomainId)));
                purgeWorker.Status = PurgeWorkerStatus.Complete;
            }
        }

        private static async Task PurgeException(
            AppSettings appSettings,
            SettingsFactory settingsFactory,
            ITraceService traceService,
            IPurgeSaver purgeSaver,
            Guid domainId)
        {
            DateTime maxTimestamp = DateTime.UtcNow;
            CoreSettings settings = settingsFactory.CreateCore();
            await WriteTrace(traceService, appSettings, settingsFactory, $"Purging exceptions");
            await purgeSaver.PurgeException(settings, domainId, maxTimestamp);
        }

        private static async Task PurgeMetric(
            AppSettings appSettings,
            SettingsFactory settingsFactory,
            ITraceService traceService,
            IPurgeSaver purgeSaver,
            Guid domainId)
        {
            DateTime maxTimestamp = DateTime.UtcNow;
            CoreSettings settings = settingsFactory.CreateCore();
            await WriteTrace(traceService, appSettings, settingsFactory, $"Purging metrics");
            await purgeSaver.PurgeMetric(settings, domainId, maxTimestamp);
        }

        private static async Task PurgeTrace(
            AppSettings appSettings,
            SettingsFactory settingsFactory,
            ITraceService traceService,
            IPurgeSaver purgeSaver,
            Guid domainId)
        {
            DateTime maxTimestamp = DateTime.UtcNow;
            CoreSettings settings = settingsFactory.CreateCore();
            await WriteTrace(traceService, appSettings, settingsFactory, $"Purging traces");
            await purgeSaver.PurgeTrace(settings, domainId, maxTimestamp);
        }

        private static async Task UpdateExceptionMetaData(
            AppSettings appSettings,
            SettingsFactory settingsFactory, 
            ITraceService traceService,
            IPurgeSaver purgeSaver,
            Guid domainId)
        {
            DateTime minTimestamp = SubtractSettingsTimespan(DateTime.UtcNow.Date, appSettings.RetensionPeriod).ToUniversalTime();
            CoreSettings settings = settingsFactory.CreateCore();
            DateTime expiration = DateTime.UtcNow;
            expiration = new DateTime(expiration.Year, expiration.Month, 1).AddMonths(1);
            await WriteTrace(traceService, appSettings, settingsFactory, $"Updating meta data for exceptions created up to {minTimestamp}");
            await purgeSaver.InitializeException(settings, domainId, expiration, minTimestamp);
        }

        private static async Task UpdateMetricMetaData(
            AppSettings appSettings,
            SettingsFactory settingsFactory,
            ITraceService traceService,
            IPurgeSaver purgeSaver,
            Guid domainId)
        {
            DateTime minTimestamp = SubtractSettingsTimespan(DateTime.UtcNow.Date, appSettings.RetensionPeriod).ToUniversalTime();
            CoreSettings settings = settingsFactory.CreateCore();
            DateTime expiration = DateTime.UtcNow;
            expiration = new DateTime(expiration.Year, expiration.Month, 1).AddMonths(1);
            await WriteTrace(traceService, appSettings, settingsFactory, $"Updating meta data for metrics created up to {minTimestamp}");
            await purgeSaver.InitializeMetric(settings, domainId, expiration, minTimestamp);
        }

        private static async Task UpdateTraceMetaData(
            AppSettings appSettings,
            SettingsFactory settingsFactory,
            ITraceService traceService,
            IPurgeSaver purgeSaver,
            Guid domainId)
        {
            DateTime minTimestamp = SubtractSettingsTimespan(DateTime.UtcNow.Date, appSettings.RetensionPeriod).ToUniversalTime();
            CoreSettings settings = settingsFactory.CreateCore();
            DateTime expiration = DateTime.UtcNow;
            expiration = new DateTime(expiration.Year, expiration.Month, 1).AddMonths(1);
            await WriteTrace(traceService, appSettings, settingsFactory, $"Updating meta data for traces created up to {minTimestamp}");
            await purgeSaver.InitializeTrace(settings, domainId, expiration, minTimestamp);
        }

        /// <summary>
        /// Calling stored procedure to 
        /// 1) delete old completed workers
        /// 2) reset hung workers
        /// 3) add domain workers
        /// </summary>
        private static async Task InitializeWorkers()
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope())
            {
                AppSettings appSettings = scope.Resolve<AppSettings>();
                SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                IPurgeWorkerSaver saver = scope.Resolve<IPurgeWorkerSaver>();
                await saver.InitializePurgeWorker(settingsFactory.CreateCore());
                await WriteTrace(scope.Resolve<ITraceService>(), appSettings, settingsFactory, "Workers Initialized");
            }
        }

        /// <summary>
        /// Calling stored procs to delete old purge meta data records
        /// </summary>
        private static async Task PurgMetaData(
            AppSettings appSettings,
            SettingsFactory settingsFactory,
            IPurgeSaver saver)
        {
            DateTime minTimestamp = SubtractSettingsTimespan(DateTime.UtcNow.Date, appSettings.PurgeMetaDataTimespan).ToUniversalTime();
            CoreSettings settings = settingsFactory.CreateCore();
            await Task.WhenAll(
                saver.DeleteExceptionByMinTimestamp(settings, minTimestamp),
                saver.DeleteMetricByMinTimestamp(settings, minTimestamp),
                saver.DeleteTraceByMinTimestamp(settings, minTimestamp)
                );
        }

        private static DateTime SubtractSettingsTimespan(DateTime timestamp, string value)
        {
            return AddSettingsTimespan(timestamp, value, -1);
        }

        private static DateTime AddSettingsTimespan(DateTime timestamp, string value, int multiplier)
        {
            DateTime result;
            Match match = Regex.Match(value, @"^\s*(-?[0-9]{1,5})\s*(minute|hour|day|month)s?\s*$", RegexOptions.IgnoreCase);
            if (!match.Success)
                throw new ApplicationException($"Invalid timespan {value}");
            switch (match.Groups[2].Value.ToLower())
            {
                case "minute":
                    result = timestamp.AddMinutes(int.Parse(match.Groups[1].Value) * multiplier);
                    break;
                case "hour":
                    result = timestamp.AddHours(int.Parse(match.Groups[1].Value) * multiplier);
                    break;
                case "day":
                    result = timestamp.AddDays(int.Parse(match.Groups[1].Value) * multiplier);
                    break;
                case "month":
                    result = timestamp.AddMonths(int.Parse(match.Groups[1].Value) * multiplier);
                    break;
                default:
                    result = timestamp;
                    break;
            }
            return result;
        }

        private static AppSettings LoadSettings(string[] args)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder
            .AddJsonFile("appSettings.json", false)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            ;
            IConfiguration configuration = builder.Build();
            AppSettings settings = new AppSettings();
            ConfigurationBinder.Bind(configuration, settings);
            return settings;
        }

        public static async Task WriteTrace(
            ITraceService traceService,
            AppSettings appSettings,
            SettingsFactory settingsFactory,
            string value)
        {
            Console.WriteLine(value);
            await traceService.Create(settingsFactory.CreateLog(), appSettings.TraceLoggingDomainId, "log-purger", value);
        }
    }
}
