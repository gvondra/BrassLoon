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
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;

namespace ConfigAPI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            string useMongoDb = builder.Configuration["UseMongoDb"] ?? "false";
            _ = builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            _ = builder.Host.ConfigureContainer((ContainerBuilder builder) => builder.RegisterModule(new ConfigAPIModule(bool.Parse(useMongoDb))));
            // Add services to the container.
            _ = builder.Services.Configure<Settings>(builder.Configuration);

            _ = builder.Services.AddLogging(b =>
            {
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

            _ = builder.Services.AddControllers()
            .AddNewtonsoftJson(o =>
            {
                o.SerializerSettings.ContractResolver = new DefaultContractResolver();
            })
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
            })
            ;
            _ = builder.Services.AddCors(builder.Configuration);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            _ = builder.Services.AddEndpointsApiExplorer();
            _ = builder.Services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Brass Loon Configuration API"
                    });
                o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                o.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            _ = builder.Services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddBrassLoonAuthentication(builder.Configuration);
            _ = builder.Services.AddSingleton<IAuthorizationHandler, AuthorizationHandler>();
            _ = builder.Services.AddAuthorization(builder.Configuration);

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                _ = app.UseDeveloperExceptionPage();
                _ = app.UseSwagger();
                _ = app.UseSwaggerUI();
            }

            _ = app.UseRouting();
            _ = app.UseCors();
            _ = app.UseAuthentication();
            _ = app.UseAuthorization();

            _ = app.UseEndpoints(endpoints => endpoints.MapControllers());

            _ = app.MapGet("api/HealthCheck", () => "Success");

            app.Run();
        }
    }
}