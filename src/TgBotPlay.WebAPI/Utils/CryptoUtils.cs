using System.Security.Cryptography;

namespace TgBotPlay.WebAPI.Utils;

public static class CryptoUtils
{
    public static byte[] Sha1Encrypt(this byte[] data) =>
        SHA1.HashData(data);

    public static string BytesToString(this byte[] data) =>
        BitConverter.ToString(data).Replace("-", string.Empty);

    public static byte[] StringToBytes(this string data) =>
        Array.ConvertAll(data.ToArray(), d => (byte)d);
}
