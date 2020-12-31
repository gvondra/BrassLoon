using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountAPI
{
    public class Startup
    {
        internal const string POLICY_EDIT_USER = "EDIT:USER";
        internal const string POLICY_READ_ACCOUNT = "READ:ACCOUNT";
        internal const string POLICY_EDIT_ACCOUNT = "EDIT:ACCOUNT";
        internal const string POLICY_ADMIN_ACCOUNT = "ADMIN:ACCOUNT";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Settings>(Configuration);
            services.AddSingleton<IContainer>(servcieProvider =>
            {
                ContainerBuilder builder = new ContainerBuilder();
                builder.RegisterModule(new AccountApiModule());
                return builder.Build();
            });
            services.AddControllers()
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.PropertyNamingPolicy = null;
                })
                ;
            AddCors(services);
            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Brass Loon Account API"
                    });
                o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                o.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                    },
                    new string[] { }
                }
                });
            });            
            services.AddSingleton<IAuthorizationHandler, AuthorizationHandler>();
            AddAuthentication(services);
            AddAuthorization(services);
        }

        private void AddCors(IServiceCollection services)
        {
            IConfigurationSection section = Configuration.GetSection("CorsOrigins");
            string[] corsOrigins = section.GetChildren().Select<IConfigurationSection, string>(child => child.Value).ToArray();
            if (corsOrigins != null && corsOrigins.Length > 0)
            {
                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(builder =>
                    {
                        builder
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                        builder.WithOrigins(corsOrigins);
                    });
                });
            }            
        }

        private void AddAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("External", o =>
            {
                o.Authority = Configuration["IdIssuer"];
                o.Audience = Configuration["IdAudience"];
            })
            .AddJwtBearer("BrassLoon", o =>
            {
                //o.Authority = "urn:brassloon";
                //o.Audience = "urn:brassloon";
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateActor = false,
                    ValidateTokenReplay = false,
                    RequireAudience = false,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ValidAudience = Configuration["Issuer"],
                    ValidIssuer = Configuration["Issuer"],
                    IssuerSigningKey = Controllers.JwksController.GetSecurityKey(Configuration["TknCsp"])
                };
                o.IncludeErrorDetails = true;
            })
            ;
        }

        private void AddAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(o =>
            {
                o.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes("External", "BrassLoon")
                .Build();
                o.AddPolicy(POLICY_EDIT_USER,
                    configure =>
                    {
                        configure.AddRequirements(new AuthorizationRequirement(POLICY_EDIT_USER, Configuration["IdIssuer"]))
                        .AddAuthenticationSchemes("External")
                        .Build();
                    });
                o.AddPolicy(POLICY_READ_ACCOUNT,
                    configure =>
                    {
                        configure.AddRequirements(new AuthorizationRequirement(POLICY_READ_ACCOUNT, Configuration["Issuer"]))
                        .AddAuthenticationSchemes("BrassLoon")
                        .Build();
                    });
                o.AddPolicy(POLICY_EDIT_ACCOUNT,
                    configure =>
                    {
                        configure.AddRequirements(new AuthorizationRequirement(POLICY_EDIT_ACCOUNT, Configuration["Issuer"]))
                        .AddAuthenticationSchemes("BrassLoon")
                        .Build();
                    });
                o.AddPolicy(POLICY_ADMIN_ACCOUNT,
                    configure =>
                    {
                        configure.AddRequirements(new AuthorizationRequirement(POLICY_ADMIN_ACCOUNT, Configuration["Issuer"], "actadmin"))
                        .AddAuthenticationSchemes("BrassLoon")
                        .Build();
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Brass Loon Account API");
            });

            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
