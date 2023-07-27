using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using Api.SteamAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace Api.TokenSystem;

internal static class UserTokenValidator
{
    public static ValidatedSteamLoginData ValidateToken(HttpRequest request)
    {
        var token = ExtractTokenFromHttpRequest(request);
        var handler = new JwtSecurityTokenHandler();

        if (handler.ReadToken(token) is not JwtSecurityToken jwtToken || jwtToken.ValidTo <= DateTime.UtcNow)
        {
            return new ValidatedSteamLoginData(string.Empty, false);
        }

        var publicKeyPem = TokenSystemConstants.RsaPublicKey;
        var publicKey = RsaHelper.ConvertFromPemToRsa(publicKeyPem);

        var validatedTokenObject = ValidateToken(token, publicKey);
        if (validatedTokenObject == null)
        {
            return new ValidatedSteamLoginData(string.Empty, false);
        }

        var steamId = validatedTokenObject.Claims.FirstOrDefault(claim => claim.Type == "steamId")?.Value;
        return new ValidatedSteamLoginData(steamId, !string.IsNullOrWhiteSpace(steamId));
    }

    private static ClaimsPrincipal ValidateToken(string token, RSA publicKey)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(publicKey),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        try
        {
            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            return null;
        }
    }

    private static string ExtractTokenFromHttpRequest(HttpRequest req)
    {
        if (!req.Headers.TryGetValue(TokenSystemConstants.AuthorizationHeaderName, out var headerValues))
        {
            return null;
        }

        var bearerToken = headerValues.FirstOrDefault();
        if (bearerToken != null && bearerToken.StartsWith(TokenSystemConstants.AuthorizationSchemeName))
        {
            return bearerToken[TokenSystemConstants.AuthorizationSchemeName.Length..];
        }

        return null;
    }
}