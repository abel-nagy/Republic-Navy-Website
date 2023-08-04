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
        var token = ExtractTokenFromHttpRequest(request, out var message);
        if (token == null)
        {
            return new ValidatedSteamLoginData(message, false);
        }
        var handler = new JwtSecurityTokenHandler();

        if (handler.ReadToken(token) is not JwtSecurityToken jwtToken)
        {
            return new ValidatedSteamLoginData("token is not JwtSecurityToken", false);
        } 
        else if (jwtToken.ValidTo <= DateTime.UtcNow)
        {
            return new ValidatedSteamLoginData($"token({token}) is expired (token: {jwtToken.ValidTo}, now: {DateTime.UtcNow})", false);
        }

        var publicKeyPem = TokenSystemConstants.RsaPublicKey;
        var publicKey = RsaHelper.ConvertFromPemToRsa(publicKeyPem);
        if (publicKey == null)
        {
            return new ValidatedSteamLoginData("public key is null", false);
        }

        var validatedTokenObject = ValidateToken(token, publicKey, out message);
        if (validatedTokenObject == null)
        {
            return new ValidatedSteamLoginData($"{message}", false);
        }

        var steamId = validatedTokenObject.Claims.FirstOrDefault(claim => claim.Type == "steamId")?.Value;
        return new ValidatedSteamLoginData(steamId, !string.IsNullOrWhiteSpace(steamId));
    }

    private static ClaimsPrincipal ValidateToken(string token, RSA publicKey, out string message)
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
            message = "nothing so far";
            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
        catch(Exception ex)
        {
            message = ex.Message;
            return null;
        }
    }

    private static string ExtractTokenFromHttpRequest(HttpRequest req, out string message)
    {
        try
        {
            if (!req.Headers.TryGetValue(TokenSystemConstants.AuthorizationHeaderName, out var headerValues))
            {
                message = $"couldn't get {TokenSystemConstants.AuthorizationHeaderName} value from header";
                return null;
            }

            var bearerToken = headerValues.FirstOrDefault();
            if (bearerToken == null)
            {
                message = $"bearerToken was null. headerValues didn't have any items";
                return null;
            }

            if (bearerToken.StartsWith(TokenSystemConstants.AuthorizationSchemeName))
            {
                message = "nothing so far";
                return bearerToken[TokenSystemConstants.AuthorizationSchemeName.Length..];
            }

            message =
                $"bearerToken didn't start with '{TokenSystemConstants.AuthorizationSchemeName}'. value: {bearerToken}";
            return null;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return null;
        }
    }
}