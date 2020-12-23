using Autofac;
using BrassLoon.Log.Framework;
using BrassLoon.Log.Framework.Enumerations;
using BrassLoon.Interface.Log;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Binder;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrassLoon.Log.Purger
{
    public class Program
    {
        private static Settings _settings;
        private static IContainer _container;

        public static async Task Main(string[] args)
        {
            try
            {
                _settings = LoadSettings(args);
                _container = LoadDiContainer();
                await StartPurge();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
            finally
            {
                if (_container != null) _container.Dispose();
            }
        }

        private static async Task StartPurge()
        {
            using ILifetimeScope scope = _container.BeginLifetimeScope();
            SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
            ITraceService traceService = scope.Resolve<ITraceService>();
            await WriteTrace(traceService, settingsFactory, "Start Purge");
            await InitializeWorkers();
            CoreSettings settings = settingsFactory.CreateCore(_settings);
            IPurgeWorkerFactory workerFactory = scope.Resolve<IPurgeWorkerFactory>();
            IPurgeWorkerSaver workerSaver = scope.Resolve<IPurgeWorkerSaver>();
            Guid? workerId = await workerFactory.Claim(settings);
            while (workerId.HasValue && !workerId.Value.Equals(Guid.Empty))
            {
                await WriteTrace(traceService, settingsFactory, $"Claimed worker {workerId.Value}");
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
                        await scope.Resolve<IExceptionService>().Create(settingsFactory.CreateLog(_settings), _settings.ExceptionLoggingDomainId, ex);
                    }
                    catch { }
                    throw;
                }
                finally
                {
                    await workerSaver.Update(settings, purgeWorker);
                    await WriteTrace(traceService, settingsFactory, $"Updated Worker {workerId.Value} Status {purgeWorker.Status}");
                }
                workerId = await workerFactory.Claim(settings);
            }
            await PurgMetaData(settingsFactory, scope.Resolve<IPurgeSaver>());
        }

        private static async Task Purge(IPurgeWorker purgeWorker)
        {
            using ILifetimeScope scope = _container.BeginLifetimeScope();
            SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
            if (purgeWorker.Status == PurgeWorkerStatus.InProgress)
            {
                await Task.WhenAll(
                Task.Run(() => UpdateExceptionMetaData(
                    settingsFactory,
                    scope.Resolve<ITraceService>(),
                    scope.Resolve<IPurgeSaver>(),
                    purgeWorker.DomainId
                    )
                ),
                Task.Run(() => UpdateMetricMetaData(
                    settingsFactory,
                    scope.Resolve<ITraceService>(),
                    scope.Resolve<IPurgeSaver>(),
                    purgeWorker.DomainId
                    )
                ),
                Task.Run(() => UpdateTraceMetaData(
                    settingsFactory,
                    scope.Resolve<ITraceService>(),
                    scope.Resolve<IPurgeSaver>(),
                    purgeWorker.DomainId
                    )
                )
                );
                await Task.WhenAll(
                Task.Run(() => PurgeException(
                    settingsFactory,
                    scope.Resolve<ITraceService>(),
                    scope.Resolve<IPurgeSaver>(),
                    purgeWorker.DomainId
                    )
                ),
                Task.Run(() => PurgeMetric(
                    settingsFactory,
                    scope.Resolve<ITraceService>(),
                    scope.Resolve<IPurgeSaver>(),
                    purgeWorker.DomainId
                    )
                ),
                Task.Run(() => PurgeTrace(
                    settingsFactory,
                    scope.Resolve<ITraceService>(),
                    scope.Resolve<IPurgeSaver>(),
                    purgeWorker.DomainId
                    )
                )
                );
                purgeWorker.Status = PurgeWorkerStatus.Complete;
            }
        }

        private static async Task PurgeException(
            SettingsFactory settingsFactory,
            ITraceService traceService,
            IPurgeSaver purgeSaver,
            Guid domainId)
        {
            DateTime maxTimestamp = DateTime.UtcNow;
            CoreSettings settings = settingsFactory.CreateCore(_settings);
            await WriteTrace(traceService, settingsFactory, $"Purging exceptions");
            await purgeSaver.PurgeException(settings, domainId, maxTimestamp);
        }

        private static async Task PurgeMetric(
            SettingsFactory settingsFactory,
            ITraceService traceService,
            IPurgeSaver purgeSaver,
            Guid domainId)
        {
            DateTime maxTimestamp = DateTime.UtcNow;
            CoreSettings settings = settingsFactory.CreateCore(_settings);
            await WriteTrace(traceService, settingsFactory, $"Purging metrics");
            await purgeSaver.PurgeMetric(settings, domainId, maxTimestamp);
        }

        private static async Task PurgeTrace(
            SettingsFactory settingsFactory,
            ITraceService traceService,
            IPurgeSaver purgeSaver,
            Guid domainId)
        {
            DateTime maxTimestamp = DateTime.UtcNow;
            CoreSettings settings = settingsFactory.CreateCore(_settings);
            await WriteTrace(traceService, settingsFactory, $"Purging traces");
            await purgeSaver.PurgeTrace(settings, domainId, maxTimestamp);
        }

        private static async Task UpdateExceptionMetaData(
            SettingsFactory settingsFactory, 
            ITraceService traceService,
            IPurgeSaver purgeSaver,
            Guid domainId)
        {
            DateTime minTimestamp = SubtractSettingsTimespan(DateTime.UtcNow.Date, _settings.RetensionPeriod).ToUniversalTime();
            CoreSettings settings = settingsFactory.CreateCore(_settings);
            DateTime expiration = DateTime.UtcNow;
            expiration = new DateTime(expiration.Year, expiration.Month, 1).AddMonths(1);
            await WriteTrace(traceService, settingsFactory, $"Updating meta data for exceptions created up to {minTimestamp}");
            await purgeSaver.InitializeException(settings, domainId, expiration, minTimestamp);
        }

        private static async Task UpdateMetricMetaData(
            SettingsFactory settingsFactory,
            ITraceService traceService,
            IPurgeSaver purgeSaver,
            Guid domainId)
        {
            DateTime minTimestamp = SubtractSettingsTimespan(DateTime.UtcNow.Date, _settings.RetensionPeriod).ToUniversalTime();
            CoreSettings settings = settingsFactory.CreateCore(_settings);
            DateTime expiration = DateTime.UtcNow;
            expiration = new DateTime(expiration.Year, expiration.Month, 1).AddMonths(1);
            await WriteTrace(traceService, settingsFactory, $"Updating meta data for metrics created up to {minTimestamp}");
            await purgeSaver.InitializeMetric(settings, domainId, expiration, minTimestamp);
        }

        private static async Task UpdateTraceMetaData(
            SettingsFactory settingsFactory,
            ITraceService traceService,
            IPurgeSaver purgeSaver,
            Guid domainId)
        {
            DateTime minTimestamp = SubtractSettingsTimespan(DateTime.UtcNow.Date, _settings.RetensionPeriod).ToUniversalTime();
            CoreSettings settings = settingsFactory.CreateCore(_settings);
            DateTime expiration = DateTime.UtcNow;
            expiration = new DateTime(expiration.Year, expiration.Month, 1).AddMonths(1);
            await WriteTrace(traceService, settingsFactory, $"Updating meta data for traces created up to {minTimestamp}");
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
            using (ILifetimeScope scope = _container.BeginLifetimeScope())
            {
                SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                IPurgeWorkerSaver saver = scope.Resolve<IPurgeWorkerSaver>();
                await saver.InitializePurgeWorker(settingsFactory.CreateCore(_settings));
                await WriteTrace(scope.Resolve<ITraceService>(), settingsFactory, "Workers Initialized");
            }
        }

        /// <summary>
        /// Calling stored procs to delete old purge meta data records
        /// </summary>
        private static async Task PurgMetaData(SettingsFactory settingsFactory, IPurgeSaver saver)
        {
            DateTime minTimestamp = SubtractSettingsTimespan(DateTime.UtcNow.Date, _settings.PurgeMetaDataTimespan).ToUniversalTime();
            CoreSettings settings = settingsFactory.CreateCore(_settings);
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

        private static Settings LoadSettings(string[] args)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder
            .AddJsonFile("appSettings.json", false)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            ;
            IConfiguration configuration = builder.Build();
            Settings settings = new Settings();
            ConfigurationBinder.Bind(configuration, settings);
            return settings;
        }

        private static IContainer LoadDiContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule(new BrassLoon.Log.Core.LogModule());
            builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
            builder.RegisterType<SettingsFactory>();
            return builder.Build();
        }

        public static async Task WriteTrace(ITraceService traceService, SettingsFactory settingsFactory, string value)
        {
            Console.WriteLine(value);
            await traceService.Create(settingsFactory.CreateLog(_settings), _settings.TraceLoggingDomainId, "log-purger", value);
        }
    }
}
