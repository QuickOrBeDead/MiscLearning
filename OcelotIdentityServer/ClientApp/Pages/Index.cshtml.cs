namespace ClientApp.Pages;

using Constants;

using IdentityModel.Client;

using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    private IEnumerable<string> _books;
    private static readonly HttpClient _httpClient = new();

    public IEnumerable<string> Books
    {
        get => _books ??= new List<string>();
        set => _books = value;
    }

    public async Task OnGet()
    {
        var disco = await _httpClient.GetDiscoveryDocumentAsync(Constants.Endpoints.IdentityServer);
        if (disco.IsError)
        {
            throw new InvalidOperationException(disco.Error);
        }

        var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                                                                           {
                                                                               Address = disco.TokenEndpoint,
                                                                               ClientId = "bookClient",
                                                                               ClientSecret = "Password1",
                                                                           });
        if (tokenResponse.IsError)
        {
            throw new InvalidOperationException(tokenResponse.Error);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, new Uri(new Uri(Constants.Endpoints.ApiGateway), "/books"));
        request.SetBearerToken(tokenResponse.AccessToken);
        var booksResponse = await _httpClient.SendAsync(request);
        booksResponse.EnsureSuccessStatusCode();

        Books = await booksResponse.Content.ReadFromJsonAsync<IEnumerable<string>>() ?? new List<string>(0);
    }
}