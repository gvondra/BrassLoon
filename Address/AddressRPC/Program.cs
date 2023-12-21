using AddressRPC.Services;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BrassLoon.CommonAPI;
using BrassLoon.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AddressRPC
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            _ = builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            _ = builder.Host.ConfigureContainer((ContainerBuilder builder) => builder.RegisterModule(new AddressRPCModule()));
            _ = builder.Services.Configure<Settings>(builder.Configuration);

            _ = builder.Services.AddLogging(b =>
            {
                _ = b.ClearProviders();
                _ = b.AddConsole();
                Settings settings = new Settings();
                builder.Configuration.Bind(settings);
                if (settings.LoggingDomainId.HasValue && !string.IsNullOrEmpty(settings.LogApiBaseAddress) && settings.LoggingClientId.HasValue)
                {
                    _ = b.AddBrassLoonLogger(c =>
                    {
                        c.LogApiBaseAddress = settings.LogApiBaseAddress;
                        c.LogDomainId = settings.LoggingDomainId.Value;
                        c.LogClientId = settings.LoggingClientId.Value;
                        c.LogClientSecret = settings.LoggingClientSecret;
                    });
                }
            });

            _ = builder.Services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddBrassLoonAuthentication(builder.Configuration)
            ;
            _ = builder.Services.AddSingleton<IAuthorizationHandler, AuthorizationHandler>();
            _ = builder.Services.AddAuthorization(builder.Configuration);

            // Add services to the container.
            _ = builder.Services.AddGrpc();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            _ = app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            _ = app.UseAuthentication();
            _ = app.UseAuthorization();

            _ = app.MapGrpcService<AddressService>();
            _ = app.MapGrpcService<EmailAddressService>();
            _ = app.MapGrpcService<PhoneService>();

            app.Run();
        }
    }
}