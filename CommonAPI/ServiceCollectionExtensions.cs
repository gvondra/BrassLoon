﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace BrassLoon.CommonAPI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection section = configuration.GetSection("CorsOrigins");
            if (section != null)
            {
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
            }
            return services;
        }

        public static AuthenticationBuilder AddBrassLoonAuthentication(this AuthenticationBuilder builder, IConfiguration configuration)
        {
            HttpDocumentRetriever documentRetriever = new HttpDocumentRetriever() { RequireHttps = false };
            JsonWebKeySet keySet = JsonWebKeySet.Create(
                documentRetriever.GetDocumentAsync(configuration["JwkAddress"], System.Threading.CancellationToken.None).Result);
            _ = builder.AddJwtBearer(Constants.AUTH_SCHEME_BRASSLOON, o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
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
                    IssuerSigningKeys = keySet.GetSigningKeys(),
                    TryAllIssuerSigningKeys = true
                };
                o.IncludeErrorDetails = true;
            })
            ;
            return builder;
        }

        public static AuthenticationBuilder AddGoogleAuthentication(this AuthenticationBuilder builder, IConfiguration configuration)
        {
            HttpDocumentRetriever documentRetriever = new HttpDocumentRetriever() { RequireHttps = false };
            JsonWebKeySet keySet = JsonWebKeySet.Create(
                documentRetriever.GetDocumentAsync(configuration["GoogleJwksUrl"], System.Threading.CancellationToken.None).Result);
            List<string> audiences = GetGoogleAudiences(configuration);
            _ = builder.AddJwtBearer(Constants.AUTH_SCHEME_GOOGLE, o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
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
                    ValidAudiences = audiences,
                    ValidIssuer = configuration["GoogleIdIssuer"],
                    IssuerSigningKeys = keySet.GetSigningKeys(),
                    TryAllIssuerSigningKeys = true
                };
                o.IncludeErrorDetails = true;
            })
            ;
            return builder;
        }

        private static List<string> GetGoogleAudiences(IConfiguration configuration)
        {
            string googleIdAudience = configuration["GoogleIdAudience"];
            List<string> googleIdAudiences = null;
            IConfigurationSection section = configuration.GetSection("GoogleIdAudiences");
            if (section != null)
            {
                googleIdAudiences = section.GetChildren().Select(child => child.Value).ToList();
            }
            if (googleIdAudiences == null)
            {
                googleIdAudiences = new List<string>();
            }
            if (!string.IsNullOrEmpty(googleIdAudience) && !googleIdAudiences.Contains(googleIdAudience))
            {
                googleIdAudiences.Add(googleIdAudience);
            }

            return googleIdAudiences;
        }

        public static IServiceCollection AddAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            string googleIdIssuer = configuration["GoogleIdIssuer"];
            string brassLoonIdIssuer = configuration["Issuer"];
            List<string> authenticationSchemes = new List<string>()
            {
                Constants.AUTH_SCHEME_BRASSLOON
            };
            if (!string.IsNullOrEmpty(googleIdIssuer))
                authenticationSchemes.Add(Constants.AUTH_SCHEME_GOOGLE);

            _ = services.AddAuthorization(o =>
            {
                o.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(authenticationSchemes.ToArray())
                .Build();
                if (!string.IsNullOrEmpty(googleIdIssuer))
                {
                    _ = o.AddPolicyWithoutRoles(Constants.POLICY_CREATE_TOKEN, Constants.AUTH_SCHEME_GOOGLE, googleIdIssuer);
                }
                if (!string.IsNullOrEmpty(brassLoonIdIssuer))
                {
                    _ = o.AddPolicyWithoutRoles(Constants.POLICY_BL_AUTH, Constants.AUTH_SCHEME_BRASSLOON, brassLoonIdIssuer);
                    _ = o.AddPolicy(Constants.POLICY_SYS_ADMIN, Constants.AUTH_SCHEME_BRASSLOON, brassLoonIdIssuer);
                }
            });
            return services;
        }

        public static AuthorizationOptions AddPolicy(this AuthorizationOptions authorizationOptions, string policyName, string schema, string issuer, IEnumerable<string> additinalPolicies = null)
        {
            if (additinalPolicies == null)
            {
                additinalPolicies = new List<string> { policyName };
            }
            else if (!additinalPolicies.Contains(policyName))
            {
                additinalPolicies = additinalPolicies.Concat(new List<string> { policyName });
            }
            authorizationOptions.AddPolicy(
                policyName,
                configure =>
                {
                    _ = configure.AddRequirements(new AuthorizationRequirement(policyName, issuer, additinalPolicies.ToArray()))
                    .AddAuthenticationSchemes(schema)
                    .Build();
                });
            return authorizationOptions;
        }

        public static AuthorizationOptions AddPolicyWithoutRoles(this AuthorizationOptions authorizationOptions, string policyName, string schema, string issuer)
        {
            authorizationOptions.AddPolicy(
                policyName,
                configure =>
                {
                    _ = configure.AddRequirements(new AuthorizationRequirement(policyName, issuer))
                    .AddAuthenticationSchemes(schema)
                    .Build();
                });
            return authorizationOptions;
        }
    }
}
