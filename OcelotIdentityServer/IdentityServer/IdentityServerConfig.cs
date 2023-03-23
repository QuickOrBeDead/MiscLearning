namespace IdentityServer;

using IdentityServer4.Models;
using IdentityServer4.Test;

public static class IdentityServerConfig
{
    public static IEnumerable<Client> Clients =>
        new[]
            {
                new Client
                    {
                        ClientId = "bookClient",
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        ClientSecrets =
                            {
                                new Secret("Password1".Sha256())
                            },
                        AllowedScopes = { "bookAPI" }
                    }
            };

    public static IEnumerable<ApiScope> ApiScopes =>
        new[]
            {
                new ApiScope("bookAPI", "Book API")
            };

    public static IEnumerable<ApiResource> ApiResources =>
        new[]
            {
                new ApiResource("bookAPI", "Book API")
            };

    public static IEnumerable<IdentityResource> IdentityResources =>
        new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResources.Email(),
                new IdentityResource(
                    "roles",
                    "Your role(s)",
                    new List<string> { "role" })
            };

    public static List<TestUser> TestUsers => new();
}