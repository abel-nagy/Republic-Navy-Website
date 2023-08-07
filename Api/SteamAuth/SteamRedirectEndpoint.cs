using System.Threading.Tasks;
using Api.TokenSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Api.SteamAuth;

public static class SteamRedirectEndpoint
{
    [FunctionName("SteamCallback")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "auth/steam/callback")]
        HttpRequest req)
    {
        var responseParams = req.Query;
        var steamLoginValidity = await SteamHelper.ValidateSteamLogin(responseParams);

        if (!steamLoginValidity.IsLoginValid)
        {
            return new RedirectResult($"{SteamAuthConstants.SiteBaseUrl}/steam-login-redirect");
        }

        var jwt = UserTokenGenerator.GenerateSignedToken(steamLoginValidity.SteamId);
        return new RedirectResult($"{SteamAuthConstants.SiteBaseUrl}/steam-login-redirect?authToken={jwt}");
    }
}