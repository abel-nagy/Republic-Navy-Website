using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Jose;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace Api.TokenSystem;

internal static class UserTokenGenerator
{
    public static string GenerateSignedToken(string steamId)
    {
        var date = new DateTimeOffset(DateTime.UtcNow);
        var inSeconds = date.ToUnixTimeSeconds();

        var claims = new List<Claim>
        {
            new Claim("steamId", steamId),
            new Claim("nbf", $"{inSeconds}"),
            new Claim("exp", $"{inSeconds + 30}"),
            new Claim("iat", $"{inSeconds}")
        };

        var privateKey = TokenSystemConstants.RsaPrivateKey;
        var publicKey = TokenSystemConstants.RsaPublicKey;
        var token = CreateToken(claims, privateKey);

        return token;
        //var date = new DateTimeOffset(DateTime.UtcNow);
        //var inSeconds = date.ToUnixTimeSeconds();

        //var headerText = JsonSerializer.Serialize(new { alg = "RS256", typ = "JWT" });
        //var payloadText = JsonSerializer.Serialize(new
        //{
        //    nbf = inSeconds,
        //    exp = inSeconds + TokenSystemConstants.GetTokenExpirationTimeSeconds(),
        //    iat = inSeconds,
        //    steamId = $"{steamId}"
        //});

        //var convertedHeader = Base64UrlEncoder.Encode(headerText);
        //var convertedPayload = Base64UrlEncoder.Encode(payloadText);

        //var manualJwt = $"{convertedHeader}.{convertedPayload}";

        //var data = Encoding.UTF8.GetBytes(manualJwt);
        //var signature = RsaHelper.SignData(data);
        //var base64Signature = Base64UrlEncoder.Encode(signature);

        //var jwt = $"{convertedHeader}.{convertedPayload}.{base64Signature}";

        //var verified = RsaHelper.VerifyData(data, signature);

        //return verified ? jwt : string.Empty;
    }

    public static string CreateToken(List<Claim> claims, string privateRsaKey)
    {
        //RSAParameters rsaParams = new RSAParameters();
        //using (var tr = new StringReader(privateRsaKey))
        //{
        //    var pemReader = new PemReader(tr);
        //    var keyPair = pemReader.ReadObject() as AsymmetricCipherKeyPair;
        //    if (keyPair == null)
        //    {
        //        throw new Exception("Could not read RSA private key");
        //    }

        //    var privateRsaParams = keyPair.Private as RsaPrivateCrtKeyParameters;
        //    rsaParams = DotNetUtilities.ToRSAParameters(privateRsaParams);
        //}

        //using var rsa = new RSACryptoServiceProvider();
        //rsa.ImportParameters(rsaParams);
        var payload = claims.ToDictionary(k => k.Type, v => (object)v.Value);
        return JWT.Encode(payload, TokenSystemConstants.GetRsaPrivateKey(), JwsAlgorithm.RS256);
    }
}