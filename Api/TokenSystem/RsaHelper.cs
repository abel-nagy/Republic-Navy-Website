using System.Security.Cryptography;

namespace Api.TokenSystem;

internal static class RsaHelper
{
    public static RSA ConvertFromPemToRsa(string pemKey)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(pemKey);
        return rsa;
    }
}