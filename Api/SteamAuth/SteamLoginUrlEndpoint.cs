using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Api.SteamAuth;

public static class SteamLoginUrlEndpoint
{
    [FunctionName("SteamLoginUrl")]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "auth/steam/get-login-url")]
        HttpRequest request)
    {
        var loginUrlParams = new Dictionary<string, string>
        {
            { "openid.ns", "http://specs.openid.net/auth/2.0" },
            { "openid.mode", "checkid_setup" },
            { "openid.return_to", $"{SteamAuthConstants.ApiBaseUrl}/api/auth/steam/callback" },
            { "openid.realm", $"{SteamAuthConstants.ApiBaseUrl}/" },
            { "openid.identity", "http://specs.openid.net/auth/2.0/identifier_select" },
            { "openid.claimed_id", "http://specs.openid.net/auth/2.0/identifier_select" }
        };

        var queryString =
            string.Join("&", loginUrlParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        var steamLoginUrl = "https://steamcommunity.com/openid/login?" + queryString;

        return new OkObjectResult(steamLoginUrl);
    }
}