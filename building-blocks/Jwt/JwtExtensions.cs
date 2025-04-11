using BuildingBlocks.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Jwt;

using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.IdentityModel.Tokens;

public static class JwtExtensions
{
    public static IServiceCollection AddJwt(this IServiceCollection services)
    {
        var jwtOptions = services.GetOptions<JwtBearerOptions>("Jwt");

        services.AddAuthentication(
                o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
            .AddCookie(cfg => cfg.SlidingExpiration = true)
            .AddJwtBearer(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.Authority = jwtOptions.Authority;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.FromSeconds(2), // For prevent add default value (5min) to life time token!
                        ValidateLifetime = true,       // Enforce token expiry
                    };

                    options.RequireHttpsMetadata = jwtOptions.RequireHttpsMetadata;
                    options.MetadataAddress = jwtOptions.MetadataAddress;
                });

        if (!string.IsNullOrEmpty(jwtOptions.Audience))
        {
            services.AddAuthorization(
                options =>
                {
                    // Set JWT as the default scheme for all [Authorize] attributes
                    options.DefaultPolicy =
                        new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                            .RequireAuthenticatedUser()
                            .Build();

                    options.AddPolicy(
                        nameof(ApiScope),
                        policy =>
                        {
                            policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                            policy.RequireAuthenticatedUser();
                            policy.RequireClaim("scope", jwtOptions.Audience);
                        });
                });
        }

        return services;
    }
}
