using System;
using System.Security.Cryptography;

namespace Api.TokenSystem;

internal class TokenSystemConstants
{
    public const string AuthorizationHeaderName = "Authorization";
    public const string AuthorizationSchemeName = "Bearer ";
    public const string TokenIssuer = "The Republic Navy";
    public static string RsaPrivateKey = Environment.GetEnvironmentVariable("RSA_PRIVATE_KEY")?.Replace("\\n", "\n");
    public static string RsaPublicKey = Environment.GetEnvironmentVariable("RSA_PUBLIC_KEY")?.Replace("\\n", "\n");

    public static RSA GetRsaPrivateKey()
    {
        var privateKeyPem = RsaPrivateKey;
        var privateKeyRsa = RsaHelper.ConvertFromPemToRsa(privateKeyPem);
        return privateKeyRsa;
    }

    public static RSA GetRsaPublicKey()
    {
        var publicKeyPem = RsaPublicKey;
        var publicKeyRsa = RsaHelper.ConvertFromPemToRsa(publicKeyPem);
        return publicKeyRsa;
    }

    public static long GetTokenExpirationTimeSeconds()
    {
        var asd = Environment.GetEnvironmentVariable("TOKEN_VALIDITY_IN_MINUTES");
        if (asd != null)
        {
            return long.TryParse(asd, out var seconds) ? seconds * 60 : 60;
        }

        return 60;
    }
}