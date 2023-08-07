using System;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace Api.TokenSystem;

internal static class UserTokenGenerator
{
    public static string GenerateSignedToken(string steamId)
    {
        var date = new DateTimeOffset(DateTime.UtcNow);
        var inSeconds = date.ToUnixTimeSeconds();

        var headerText = JsonSerializer.Serialize(new { alg = "RS256", typ = "JWT" });
        var payloadText = JsonSerializer.Serialize(new
        {
            nbf = inSeconds,
            exp = inSeconds + TokenSystemConstants.GetTokenExpirationTimeSeconds(),
            iat = inSeconds,
            steamId = $"{steamId}"
        });

        var convertedHeader = Base64UrlEncoder.Encode(headerText);
        var convertedPayload = Base64UrlEncoder.Encode(payloadText);

        var manualJwt = $"{convertedHeader}.{convertedPayload}";

        var data = Encoding.UTF8.GetBytes(manualJwt);
        var signature = RsaHelper.SignData(data);
        var base64Signature = Base64UrlEncoder.Encode(signature);

        var jwt = $"{convertedHeader}.{convertedPayload}.{base64Signature}";

        var verified = RsaHelper.VerifyData(data, signature);

        return verified ? jwt : string.Empty;
    }
}