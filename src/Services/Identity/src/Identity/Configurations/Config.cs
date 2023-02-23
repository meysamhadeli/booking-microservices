namespace Identity.Configurations;

using System.Collections.Generic;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Identity.Constants;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResources.Phone(),
            new IdentityResources.Address(),
            new(Constants.StandardScopes.Roles, new List<string> {"role"})
        };


    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new(Constants.StandardScopes.FlightApi),
            new(Constants.StandardScopes.PassengerApi),
            new(Constants.StandardScopes.BookingApi),
            new(Constants.StandardScopes.IdentityApi)
        };


    public static IList<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new(Constants.StandardScopes.FlightApi),
            new(Constants.StandardScopes.PassengerApi),
            new(Constants.StandardScopes.BookingApi),
            new(Constants.StandardScopes.IdentityApi)
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new()
            {
                ClientId = "client",

                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    Constants.StandardScopes.FlightApi,
                    Constants.StandardScopes.PassengerApi,
                    Constants.StandardScopes.BookingApi,
                    new(Constants.StandardScopes.IdentityApi)
                }
            }
        };
}
