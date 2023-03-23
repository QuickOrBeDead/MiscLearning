namespace IdentityServer;

using IdentityServer4.Models;

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
}