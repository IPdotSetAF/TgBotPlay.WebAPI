using System.Security.Cryptography;

namespace TgBotPlay.WebAPI.Utils;

/// <summary>
/// Utility class for cryptographic operations used in TgBotPlay WebAPI.
/// Provides methods for SHA1 hashing and string/byte array conversions.
/// </summary>
public static class CryptoUtils
{
    /// <summary>
    /// Computes the SHA1 hash of the specified byte array.
    /// </summary>
    /// <param name="data">The input data to hash.</param>
    /// <returns>The SHA1 hash as a byte array.</returns>
    public static byte[] Sha1Encrypt(this byte[] data) =>
        SHA1.HashData(data);

    /// <summary>
    /// Converts a byte array to a hexadecimal string representation.
    /// </summary>
    /// <param name="data">The byte array to convert.</param>
    /// <returns>A hexadecimal string without separators.</returns>
    public static string BytesToString(this byte[] data) =>
        BitConverter.ToString(data).Replace("-", string.Empty);

    /// <summary>
    /// Converts a string to a byte array using ASCII encoding.
    /// </summary>
    /// <param name="data">The string to convert.</param>
    /// <returns>A byte array representation of the string.</returns>
    public static byte[] StringToBytes(this string data) =>
        Array.ConvertAll(data.ToArray(), d => (byte)d);
}
