// =====================================================================
// Author: Bert Berrevoets
// License: GNU GPL v3
// =====================================================================

using System;

namespace CreateMp3TestFile.Id3.Id3v2;

/// <summary>
/// Utility class for encoding and decoding syncsafe integers used in ID3v2 tags.
/// Syncsafe integers use only 7 bits per byte (MSB is always 0) to avoid
/// false sync signals in the MP3 stream.
/// </summary>
public static class SyncsafeInteger
{
    /// <summary>
    /// Encodes a 32-bit integer as a 4-byte syncsafe integer.
    /// Maximum value: 0x0FFFFFFF (268,435,455)
    /// </summary>
    /// <param name="value">The value to encode (must be less than 2^28).</param>
    /// <returns>4-byte array with syncsafe encoding.</returns>
    public static byte[] Encode(int value)
    {
        if (value < 0 || value > 0x0FFFFFFF)
            throw new ArgumentOutOfRangeException(nameof(value),
                "Value must be between 0 and 268,435,455 for syncsafe encoding.");

        var result = new byte[4];
        result[0] = (byte)((value >> 21) & 0x7F);
        result[1] = (byte)((value >> 14) & 0x7F);
        result[2] = (byte)((value >> 7) & 0x7F);
        result[3] = (byte)(value & 0x7F);

        return result;
    }

    /// <summary>
    /// Decodes a 4-byte syncsafe integer to a regular 32-bit integer.
    /// </summary>
    /// <param name="bytes">4-byte syncsafe encoded value.</param>
    /// <returns>The decoded integer value.</returns>
    public static int Decode(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 4)
            throw new ArgumentException("Syncsafe integer must be exactly 4 bytes.", nameof(bytes));

        return (bytes[0] << 21) | (bytes[1] << 14) | (bytes[2] << 7) | bytes[3];
    }

    /// <summary>
    /// Decodes a syncsafe integer from a span of bytes.
    /// </summary>
    public static int Decode(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 4)
            throw new ArgumentException("Syncsafe integer must be exactly 4 bytes.", nameof(bytes));

        return (bytes[0] << 21) | (bytes[1] << 14) | (bytes[2] << 7) | bytes[3];
    }
}
