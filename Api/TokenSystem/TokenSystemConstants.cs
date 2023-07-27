using System;

namespace Api.TokenSystem;

internal class TokenSystemConstants
{
    public const string AuthorizationHeaderName = "Authorization";
    public const string AuthorizationSchemeName = "Bearer ";
    public const string TokenIssuer = "The Republic Navy";
    public static string RsaPrivateKey = Environment.GetEnvironmentVariable("RSA_PRIVATE_KEY")?.Replace("\\n", "\n");
    public static string RsaPublicKey = Environment.GetEnvironmentVariable("RSA_PUBLIC_KEY")?.Replace("\\n", "\n");

    public static TimeSpan GetTokenExpirationTimeSpan()
    {
        return TimeSpan.FromMinutes(
            int.TryParse(Environment.GetEnvironmentVariable("TOKEN_VALIDITY_IN_MINUTES"), out var minutes)
                ? minutes
                : 1);
    }
}