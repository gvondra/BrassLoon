using AccountInterface = BrassLoon.Interface.Account;
using AccountInterfaceModels = BrassLoon.Interface.Account.Models;
using Autofac;
using BrassLoon.Interface.Log;
using BrassLoon.Interface.Log.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Binder;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestClient
{
    public sealed class Program
    {
        private static Settings _settings = null;
        private static IContainer _diContainer = null;
        public static async Task Main(string[] args)
        {        
            try
            {
                _settings = LoadSettings(args);
                if (ValidateArgs())
                {
                    _diContainer = LoadDiContainer();
                    AccountInterfaceModels.Domain domain = await GetDomain(_settings.DomainId);
                    Console.WriteLine($"Loaded domain {domain.Name}");
                    await GenerateEntries(domain);
                }                    
            } 
            catch (System.Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        private static async Task GenerateEntries(AccountInterfaceModels.Domain domain) 
        {
            DateTime start = DateTime.UtcNow;
            Console.WriteLine($"Entry generation started at {start:HH:mm:ss}");
            using ILifetimeScope scope = _diContainer.BeginLifetimeScope();
            IMetricService metricService = scope.Resolve<IMetricService>();
            SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
            LogSettings logSettings = settingsFactory.CreateLog(_settings);
            Queue<Task<Metric>> queue = new Queue<Task<Metric>>();
            for (int i = 0; i < _settings.EntryCount; i += 1)
            {
                if (queue.Count >= _settings.ConcurentTaskCount) 
                {
                    await queue.Dequeue();
                }
                queue.Enqueue(metricService.Create(logSettings, domain.DomainId.Value, DateTime.UtcNow, "bl-t-client-gen", DateTime.UtcNow.Subtract(new DateTime(2000, 1, 1)).TotalSeconds));
            }
            await Task.WhenAll(queue);
            DateTime end = DateTime.UtcNow;
            Console.WriteLine($"Entry generation ended at {end:HH:mm:ss} and took {end.Subtract(start).TotalMinutes:0.0##} minutes");
            double rate = Math.Round((double)_settings.EntryCount / end.Subtract(start).TotalSeconds, 3, MidpointRounding.ToEven);
            Console.WriteLine($"at {rate} records per second");
        }

        private static async Task<AccountInterfaceModels.Domain> GetDomain(Guid domainId)
        {
            using ILifetimeScope scope = _diContainer.BeginLifetimeScope();
            AccountInterface.IDomainService domainService = scope.Resolve<AccountInterface.IDomainService>();
            SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
            return await domainService.Get(settingsFactory.CreateAccount(_settings), domainId);
        }

        private static IContainer LoadDiContainer() 
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
            builder.Register<SettingsFactory>(context => new SettingsFactory(context.Resolve<BrassLoon.Interface.Account.ITokenService>()));
            return builder.Build();
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

        private static bool ValidateArgs()
        {
            bool result = true;
            if (_settings.DomainId.Equals(Guid.Empty)) 
            {
                result = false;
                Console.Error.WriteLine("Missing or invalid domain id");
            }
            if (_settings.ClientId.Equals(Guid.Empty))
            {
                result = false;
                Console.Error.WriteLine("Missing or invalid client id");
            }
            if (string.IsNullOrEmpty(_settings.Secret)) {
                result = false;
                Console.Error.WriteLine("Mssing client secret");
            }
            return result;
        }

    }
}
