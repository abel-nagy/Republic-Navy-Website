using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Api.SteamAuth;
using Microsoft.IdentityModel.Tokens;

namespace Api.TokenSystem;

internal static class UserTokenGenerator
{
    public static string GenerateUserToken(string steamId)
    {
        var privateKeyPem = TokenSystemConstants.RsaPrivateKey;
        var privateKey = RsaHelper.ConvertFromPemToRsa(privateKeyPem);
        
        var token = GenerateToken(steamId, privateKey);
        return token;
    }

    public static string GenerateToken(string steamId, RSA privateKey)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var rsaSecurityKey = new RsaSecurityKey(privateKey) { KeyId = TokenSystemConstants.TokenIssuer };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(SteamAuthConstants.SteamIdTokenKey, steamId) }),
            Expires = DateTime.UtcNow.Add(TokenSystemConstants.GetTokenExpirationTimeSpan()),
            IssuedAt = DateTime.UtcNow,
            Issuer = TokenSystemConstants.TokenIssuer,
            SigningCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256)
        };

        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(securityToken);

        return token;
    }
}