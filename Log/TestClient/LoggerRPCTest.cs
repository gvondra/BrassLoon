﻿using BrassLoon.Extensions.Logging;
using BrassLoon.Log.TestClient.Settings;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Log.TestClient
{
    public class LoggerRPCTest
    {
        private readonly AppSettings _appSettings;

        public LoggerRPCTest(AppSettings settings)
        {
            _appSettings = settings;
        }
        public Task Generate()
        {
            DateTime start = DateTime.Now;
            Console.WriteLine($"start    {start:hh:mm:ss tt}");
            using (ILoggerFactory loggerFactory = LoadLogger(_appSettings))
            {
                ILogger logger = loggerFactory.CreateLogger("LoggingTest");
                logger.Log(LogLevel.Information, new EventId(1, "test client"), "test message");
                _ = logger.LogMetric(
                    new EventId(1, "test client"),
                    new Metric
                    {
                        CreateTimestamp = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        EventCode = "test code",
                        Magnitude = 1.23,
                        Requestor = "test requestor",
                        Status = "500",
                        Data = new Dictionary<string, string>
                        {
                            { "data", "value" }
                        }
                    });
                try
                {
                    ThrowException();
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, new EventId(1, "test client"), ex, ex.Message);
                }
            }
            DateTime finish = DateTime.Now;
            TimeSpan duration = finish.Subtract(start);
            Console.WriteLine($"finish   {finish:hh:mm:ss tt}");
            Console.WriteLine($"duration {Math.Round(duration.TotalMinutes, 3)} minute");
            return Task.CompletedTask;
        }

        private static ILoggerFactory LoadLogger(AppSettings settings)
        {
            return LoggerFactory.Create(builder =>
            {
                _ = builder.AddBrassLoonLogger((config) =>
                {
                    config.LogApiBaseAddress = settings.LogAPIBaseAddress;
                    config.LogDomainId = settings.DomainId;
                    config.LogClientId = settings.ClientId;
                    config.LogClientSecret = settings.Secret;
                });
            });
        }

        private static void ThrowException()
        {
            try
            {
                Exception exception = new ArgumentException("test argument exception");
                exception.Data.Add("test guid", Guid.NewGuid());
                throw exception;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("outer exception", ex);
            }
        }
    }
}
