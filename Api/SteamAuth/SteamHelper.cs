using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Api.SteamAuth;

internal static class SteamHelper
{
    public static async Task<ValidatedSteamLoginData> ValidateSteamLogin(IQueryCollection responseParams)
    {
        var validationParams = PrepareValidationParameters(responseParams);

        var validationResponseContent = await GetValidationResponse(validationParams);

        var isSteamLoginValid = IsValidLoginRequest(validationResponseContent);
        var steamId = GetSteamId(validationParams);

        return new ValidatedSteamLoginData(steamId, isSteamLoginValid);
    }

    private static string GetSteamId(Dictionary<string, string> validationParams)
    {
        var stringValue = validationParams["openid.claimed_id"].Split("/").LastOrDefault();
        var success = long.TryParse(stringValue, out var longValue);
        return success ? CommunityIdToSteamId(longValue) : null;
    }

    private static Dictionary<string, string> PrepareValidationParameters(IQueryCollection responseParams)
    {
        // Extract the signed fields
        var signedParams = responseParams["openid.signed"].ToString().Split(',');

        var validationParams = new Dictionary<string, string>
        {
            { "openid.ns", "http://specs.openid.net/auth/2.0" },
            { "openid.mode", "check_authentication" },
            { "openid.sig", responseParams["openid.sig"] } // Include openid.sig
        };

        // Add all signed fields to the validation parameters
        foreach (var param in signedParams) validationParams[$"openid.{param}"] = responseParams[$"openid.{param}"];

        return validationParams;
    }

    private static async Task<string> GetValidationResponse(Dictionary<string, string> validationParams)
    {
        var client = new HttpClient();
        var content = new FormUrlEncodedContent(validationParams);
        var validationResponse = await client.PostAsync("https://steamcommunity.com/openid/login", content);
        var validationResponseContent = await validationResponse.Content.ReadAsStringAsync();
        return validationResponseContent;
    }

    private static bool IsValidLoginRequest(string validationResponse)
    {
        return validationResponse.Contains("is_valid:true");
    }

    private static string CommunityIdToSteamId(long commId)
    {
        var steamId = new StringBuilder
        {
            Capacity = 0,
            Length = 0
        };
        steamId.Append(SteamAuthConstants.SteamIdPrefix);

        var steamIdAcct = commId - SteamAuthConstants.SteamId64Ident;

        steamId.Append(steamIdAcct % 2 == 0 ? "0:" : "1:");

        steamId.Append(steamIdAcct / 2);

        return steamId.ToString();
    }
}