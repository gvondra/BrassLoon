using BrassLoon.Account.TestClient.Settings;
using BrassLoon.Interface.Account;
using Serilog;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace BrassLoon.Account.TestClient
{
    public class CreateClientTokenTest
    {
        private readonly AppSettings _appSettings;
        private readonly ISettingsFactory _settingsFactory;
        private readonly ITokenService _tokenService;
        private readonly ILogger _logger;
        private readonly object _lock = new { };
        private int _tokenCount;
        private double _tokenDuration;

        public CreateClientTokenTest(
            AppSettings appSettings,
            ISettingsFactory settingsFactory,
            ITokenService tokenService,
            ILogger logger)
        {
            _appSettings = appSettings;
            _settingsFactory = settingsFactory;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.Information("Starting  test execution");
            AccountSettings settings = _settingsFactory.CreateAccountSettings();
            Queue<Task> tasks = new Queue<Task>();
            foreach (int i in Enumerable.Range(0, 1500))
            {
                while (tasks.Count >= 64)
                {
                    try
                    {
                        await tasks.Dequeue();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, ex.Message);
                    }
                }
                if (i > 0 && i % 100 == 0)
                    _logger.Information($"Created {i:###,##0} tokens");
                tasks.Enqueue(Task.Run(() => GenerateToken(settings)));
            }
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
            _logger.Information($"{_tokenCount} tokens created");
            _logger.Information($"Average create duration {Math.Round(_tokenDuration / _tokenCount, 3):###,#00.000} seconds");
            _logger.Information("Completed test execution");
        }

        private async Task GenerateToken(AccountSettings settings)
        {
            DateTime start = DateTime.UtcNow;
            await _tokenService.CreateClientCredentialToken(settings, _appSettings.ClientId.Value, _appSettings.ClientSecret);
            double duration = DateTime.UtcNow.Subtract(start).TotalSeconds;
            Monitor.Enter(_lock);
            try
            {
                _tokenCount += 1;
                _tokenDuration += duration;
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }
    }
}
