using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace Client.Authentication;

public class AuthenticatedHttpClient : HttpClient
{
    private readonly ILocalStorageService _localStorage;

    public AuthenticatedHttpClient(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<HttpResponseMessage> SendAuthorizedRequestAsync(HttpMethod method, string url, bool requireAuthorization = true)
    {
        var token = await _localStorage.GetItemAsStringAsync(AuthenticationConstants.TokenKey);
        var request = CreateAuthorizedRequest(method, url, token);
        return await SendAsync(request, requireAuthorization, token);
    }

    private HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string url, string token)
    {
        var message = new HttpRequestMessage(method, url);
        
        if (!string.IsNullOrEmpty(token))
        {
            message.Headers.Authorization = new AuthenticationHeaderValue(AuthenticationConstants.SchemeName, token);
        }

        return message;
    }

    private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, bool requireAuthorization, string token)
    {
        switch (requireAuthorization)
        {
            case true when string.IsNullOrEmpty(token):
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            case false when string.IsNullOrEmpty(token):
                return await SendAsync(request);
        }

        var handler = new JwtSecurityTokenHandler();
        try
        {
            var jwt = handler.ReadJwtToken(token);

            //if (jwt == null || jwt.ValidTo <= DateTime.UtcNow)
            //{
            //    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            //}

            return await SendAsync(request);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }
    }
}