using BuildingBlocks.Constants;
using BuildingBlocks.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Jwt
{
    public static class JwtExtensions
    {
        public static IServiceCollection AddJwt(this IServiceCollection services)
        {
            // Bind Jwt settings from configuration
            var jwtOptions = services.GetOptions<JwtBearerOptions>("Jwt");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = jwtOptions.Authority;
                options.Audience = jwtOptions.Audience;
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuers = [jwtOptions.Authority],
                    ValidateAudience = true,
                    ValidAudiences = [jwtOptions.Audience],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(2), // Reduce default clock skew
                    // For IdentityServer4/Duende, we should also validate the signing key
                    ValidateIssuerSigningKey = true,
                    NameClaimType = "name", // Map "name" claim to User.Identity.Name
                    RoleClaimType = "role", // Map "role" claim to User.IsInRole()
                };

                // Preserve ALL claims from the token (including "sub")
                options.MapInboundClaims = false;
            });


            services.AddAuthorization(
                options =>
                {
                    options.AddPolicy(
                        nameof(ApiScope),
                        policy =>
                        {
                            policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                            policy.RequireAuthenticatedUser();
                            policy.RequireClaim("scope", jwtOptions.Audience);
                        });

                    // Role-based policies
                    options.AddPolicy(
                        IdentityConstant.Role.Admin,
                        x =>
                        {
                            x.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                            x.RequireRole(IdentityConstant.Role.Admin);
                        }
                    );
                    options.AddPolicy(
                        IdentityConstant.Role.User,
                        x =>
                        {
                            x.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                            x.RequireRole(IdentityConstant.Role.User);
                        }
                    );
                });

            return services;
        }
    }
}