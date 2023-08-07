using System.Security.Cryptography;
using System.Text;

namespace Api.TokenSystem;

internal static class RsaHelper
{
    public static RSA ConvertFromPemToRsa(string pemKey)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(pemKey);
        return rsa;
    }

    public static byte[] SignData(byte[] data)
    {
        var privateKey = TokenSystemConstants.GetRsaPrivateKey();
        var signedData = privateKey.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return signedData;
    }
    
    public static bool VerifyData(byte[] data, byte[] signature)
    {
        var publicKey = TokenSystemConstants.GetRsaPublicKey();
        var verified = publicKey.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return verified;
    }
}