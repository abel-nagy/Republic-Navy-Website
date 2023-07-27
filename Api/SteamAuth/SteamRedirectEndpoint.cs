using System.Threading.Tasks;
using Api.TokenSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Api.SteamAuth;

public static class SteamRedirectEndpoint
{
    [FunctionName("SteamCallback")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "auth/steam/callback")]
        HttpRequest req, ILogger log)
    {
        var responseParams = req.Query;
        var steamLoginValidity = await SteamHelper.ValidateSteamLogin(responseParams);

        if (steamLoginValidity.IsLoginValid)
        {
            var jwt = UserTokenGenerator.GenerateUserToken(steamLoginValidity.SteamId);
            return new RedirectResult($"https://localhost:7030/steam-login-redirect?authToken={jwt}");
        }

        log.Log(LogLevel.Error, $"Couldn't validate {steamLoginValidity.SteamId}");
        return new RedirectResult("https://localhost:7030/steam-login-redirect");
    }
}