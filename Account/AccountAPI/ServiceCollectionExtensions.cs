using BrassLoon.CommonAPI;
using BrassLoon.JwtUtility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace AccountAPI
{
    public static class ServiceCollectionExtensions
    {
        internal const string POLICY_EDIT_USER = "EDIT:USER";
        internal const string POLICY_READ_ACCOUNT = "READ:ACCOUNT";
        internal const string POLICY_EDIT_ACCOUNT = "EDIT:ACCOUNT";
        internal const string POLICY_ADMIN_ACCOUNT = "ADMIN:ACCOUNT";
        internal const string POLICY_ADMIN_SYS = "ADMIN:SYS";

        public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection section = configuration.GetSection("CorsOrigins");
            string[] corsOrigins = section.GetChildren().Select(child => child.Value).ToArray();
            if (corsOrigins.Length > 0)
            {
                _ = services.AddCors(options =>
                {
                    options.AddDefaultPolicy(builder =>
                    {
                        _ = builder
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                        _ = builder.WithOrigins(corsOrigins);
                    });
                });
            }
            return services;
        }

        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            _ = services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddGoogleAuthentication(configuration)
            .AddJwtBearer("BrassLoon", o =>
            {
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
                    ValidAudience = configuration["Issuer"],
                    ValidIssuer = configuration["Issuer"],
                    IssuerSigningKey = RsaSecurityKeySerializer.GetSecurityKey(configuration["TknCsp"])
                };
                o.IncludeErrorDetails = true;
            })
            ;
            return services;
        }

        public static IServiceCollection AddAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            _ = services.AddAuthorization(o =>
            {
                o.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(Constants.AUTH_SCHEME_GOOGLE, "BrassLoon")
                .Build();
                o.AddPolicy(POLICY_EDIT_USER,
                    configure =>
                    {
                        _ = configure.AddRequirements(new AuthorizationRequirement(POLICY_EDIT_USER, configuration["GoogleIdIssuer"]))
                        .AddAuthenticationSchemes(Constants.AUTH_SCHEME_GOOGLE)
                        .Build();
                    });
                o.AddPolicy(POLICY_READ_ACCOUNT,
                    configure =>
                    {
                        _ = configure.AddRequirements(new AuthorizationRequirement(POLICY_READ_ACCOUNT, configuration["Issuer"]))
                        .AddAuthenticationSchemes("BrassLoon")
                        .Build();
                    });
                o.AddPolicy(POLICY_EDIT_ACCOUNT,
                    configure =>
                    {
                        _ = configure.AddRequirements(new AuthorizationRequirement(POLICY_EDIT_ACCOUNT, configuration["Issuer"]))
                        .AddAuthenticationSchemes("BrassLoon")
                        .Build();
                    });
                o.AddPolicy(POLICY_ADMIN_ACCOUNT,
                    configure =>
                    {
                        _ = configure.AddRequirements(new AuthorizationRequirement(POLICY_ADMIN_ACCOUNT, configuration["Issuer"], "actadmin"))
                        .AddAuthenticationSchemes("BrassLoon")
                        .Build();
                    });
                o.AddPolicy(POLICY_ADMIN_SYS,
                    configure =>
                    {
                        _ = configure.AddRequirements(new AuthorizationRequirement(POLICY_ADMIN_SYS, configuration["Issuer"], "sysadmin"))
                        .AddAuthenticationSchemes("BrassLoon")
                        .Build();
                    });
            });
            return services;
        }
    }
}
