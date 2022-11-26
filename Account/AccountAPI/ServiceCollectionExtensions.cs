using BrassLoon.JwtUtility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace AccountAPI
{
    public static class ServiceCollectionExtensions
    {
        internal const string POLICY_EDIT_USER = "EDIT:USER";
        internal const string POLICY_READ_ACCOUNT = "READ:ACCOUNT";
        internal const string POLICY_EDIT_ACCOUNT = "EDIT:ACCOUNT";
        internal const string POLICY_ADMIN_ACCOUNT = "ADMIN:ACCOUNT";

        public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection section = configuration.GetSection("CorsOrigins");
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
            return services;
        }

        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            HttpDocumentRetriever documentRetriever = new HttpDocumentRetriever() { RequireHttps = false };
            JsonWebKeySet keySet = JsonWebKeySet.Create(
                documentRetriever.GetDocumentAsync(configuration["ExternalIssuerJwksUrl"], new System.Threading.CancellationToken()).Result
                );
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("External", o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = false,
                    ValidateLifetime = false,
                    ValidateActor = false,
                    ValidateTokenReplay = false,
                    RequireAudience = false,
                    RequireExpirationTime = false,
                    RequireSignedTokens = false,
                    ValidAudience = configuration["ExternalIdAudience"],
                    ValidIssuer = configuration["ExternalIdIssuer"],
                    IssuerSigningKeys = keySet.GetSigningKeys(),
                    TryAllIssuerSigningKeys = true                   
                };
            })
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
            services.AddAuthorization(o =>
            {
                o.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes("External", "BrassLoon")
                .Build();
                o.AddPolicy(POLICY_EDIT_USER,
                    configure =>
                    {
                        configure.AddRequirements(new AuthorizationRequirement(POLICY_EDIT_USER, configuration["ExternalIdIssuer"]))
                        .AddAuthenticationSchemes("External")
                        .Build();
                    });
                o.AddPolicy(POLICY_READ_ACCOUNT,
                    configure =>
                    {
                        configure.AddRequirements(new AuthorizationRequirement(POLICY_READ_ACCOUNT, configuration["Issuer"]))
                        .AddAuthenticationSchemes("BrassLoon")
                        .Build();
                    });
                o.AddPolicy(POLICY_EDIT_ACCOUNT,
                    configure =>
                    {
                        configure.AddRequirements(new AuthorizationRequirement(POLICY_EDIT_ACCOUNT, configuration["Issuer"]))
                        .AddAuthenticationSchemes("BrassLoon")
                        .Build();
                    });
                o.AddPolicy(POLICY_ADMIN_ACCOUNT,
                    configure =>
                    {
                        configure.AddRequirements(new AuthorizationRequirement(POLICY_ADMIN_ACCOUNT, configuration["Issuer"], "actadmin"))
                        .AddAuthenticationSchemes("BrassLoon")
                        .Build();
                    });
            });
            return services;
        }
    }
}
