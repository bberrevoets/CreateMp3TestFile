// =====================================================================
// Author: Bert Berrevoets
// License: GNU GPL v3
// =====================================================================

using System;

namespace CreateMp3TestFile.Id3.Id3v2;

/// <summary>
/// Represents and builds the 10-byte ID3v2 header.
/// </summary>
/// <remarks>
/// Header structure:
/// Bytes 0-2:  "ID3" identifier
/// Byte 3:     Version major (0x04 for ID3v2.4)
/// Byte 4:     Version minor (0x00)
/// Byte 5:     Flags
/// Bytes 6-9:  Tag size (syncsafe integer, excludes header)
/// </remarks>
public static class Id3v2Header
{
    public const int HeaderSize = 10;
    public const byte VersionMajor = 0x04; // ID3v2.4
    public const byte VersionMinor = 0x00;

    /// <summary>
    /// Flag bits for the ID3v2 header.
    /// </summary>
    [Flags]
    public enum HeaderFlags : byte
    {
        None = 0,
        Unsynchronisation = 0x80,
        ExtendedHeader = 0x40,
        ExperimentalIndicator = 0x20,
        FooterPresent = 0x10
    }

    /// <summary>
    /// Builds a 10-byte ID3v2.4 header.
    /// </summary>
    /// <param name="tagSize">Size of the tag data (excluding header and footer).</param>
    /// <param name="flags">Header flags.</param>
    /// <returns>10-byte header array.</returns>
    public static byte[] Build(int tagSize, HeaderFlags flags = HeaderFlags.None)
    {
        var header = new byte[HeaderSize];

        // "ID3" identifier
        header[0] = (byte)'I';
        header[1] = (byte)'D';
        header[2] = (byte)'3';

        // Version
        header[3] = VersionMajor;
        header[4] = VersionMinor;

        // Flags
        header[5] = (byte)flags;

        // Tag size (syncsafe)
        var sizeBytes = SyncsafeInteger.Encode(tagSize);
        Array.Copy(sizeBytes, 0, header, 6, 4);

        return header;
    }

    /// <summary>
    /// Checks if a byte array starts with a valid ID3v2 header.
    /// </summary>
    public static bool IsValidHeader(byte[] data)
    {
        if (data == null || data.Length < HeaderSize)
            return false;

        return data[0] == 'I' && data[1] == 'D' && data[2] == '3'
            && data[3] < 0xFF && data[4] < 0xFF  // Version bytes
            && (data[5] & 0x0F) == 0;            // Reserved flag bits must be 0
    }

    /// <summary>
    /// Reads the tag size from an existing ID3v2 header.
    /// </summary>
    /// <param name="header">The 10-byte header.</param>
    /// <returns>The tag size (excluding header).</returns>
    public static int GetTagSize(byte[] header)
    {
        if (header == null || header.Length < HeaderSize)
            throw new ArgumentException("Invalid header data.", nameof(header));

        return SyncsafeInteger.Decode(header.AsSpan(6, 4));
    }

    /// <summary>
    /// Gets the ID3v2 version from a header.
    /// </summary>
    public static (int Major, int Minor) GetVersion(byte[] header)
    {
        if (header == null || header.Length < HeaderSize)
            throw new ArgumentException("Invalid header data.", nameof(header));

        return (header[3], header[4]);
    }
}
