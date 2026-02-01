// =====================================================================
// Author: Bert Berrevoets
// License: GNU GPL v3
// =====================================================================

using System;
using System.IO;
using System.Text;
using CreateMp3TestFile.Models;

namespace CreateMp3TestFile.Id3.Id3v1;

/// <summary>
/// Writes ID3 v1.0/v1.1 tags to MP3 files.
/// The 128-byte tag is appended to the end of the file.
/// </summary>
public static class Id3v1TagWriter
{
    private const int TagSize = 128;
    private const int TitleMaxLength = 30;
    private const int ArtistMaxLength = 30;
    private const int AlbumMaxLength = 30;
    private const int YearMaxLength = 4;
    private const int CommentMaxLength = 28; // 28 for v1.1 (allows track number)

    private static readonly Encoding AsciiEncoding = Encoding.ASCII;

    /// <summary>
    /// Writes an ID3v1 tag to the end of the specified MP3 file.
    /// </summary>
    /// <param name="metadata">The metadata to write.</param>
    public static void WriteTag(Mp3Metadata metadata)
    {
        var tagBytes = BuildTagBytes(metadata);
        AppendTagToFile(metadata.FilePath, tagBytes);
    }

    /// <summary>
    /// Builds the 128-byte ID3v1 tag from metadata.
    /// </summary>
    public static byte[] BuildTagBytes(Mp3Metadata metadata)
    {
        var tagBytes = new byte[TagSize];

        // Copy "TAG" marker (bytes 0-2)
        var tagMarker = AsciiEncoding.GetBytes("TAG");
        Array.Copy(tagMarker, 0, tagBytes, 0, tagMarker.Length);

        // Copy Title (bytes 3-32)
        WriteField(tagBytes, 3, TruncateAndTrim(metadata.Title, TitleMaxLength));

        // Copy Artist (bytes 33-62)
        WriteField(tagBytes, 33, TruncateAndTrim(metadata.Artist, ArtistMaxLength));

        // Copy Album (bytes 63-92)
        WriteField(tagBytes, 63, TruncateAndTrim(metadata.Album, AlbumMaxLength));

        // Copy Year (bytes 93-96)
        WriteField(tagBytes, 93, TruncateAndTrim(metadata.Year, YearMaxLength));

        // Copy Comment (bytes 97-124)
        WriteField(tagBytes, 97, TruncateAndTrim(metadata.Comment, CommentMaxLength));

        // ID3v1.1: byte 125 must be 0, byte 126 is track number
        tagBytes[125] = 0;
        tagBytes[126] = metadata.TrackNumber;

        // Genre (byte 127)
        tagBytes[127] = metadata.GenreId;

        return tagBytes;
    }

    private static string TruncateAndTrim(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var trimmed = value.Trim();
        return trimmed.Length > maxLength ? trimmed[..maxLength] : trimmed;
    }

    private static void WriteField(byte[] tagBytes, int offset, string value)
    {
        if (string.IsNullOrEmpty(value))
            return;

        var bytes = AsciiEncoding.GetBytes(value);
        Array.Copy(bytes, 0, tagBytes, offset, bytes.Length);
    }

    private static void AppendTagToFile(string filePath, byte[] tagBytes)
    {
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Write);
        fileStream.Seek(0, SeekOrigin.End);
        fileStream.Write(tagBytes, 0, TagSize);
    }

    /// <summary>
    /// Checks if a file already has an ID3v1 tag.
    /// </summary>
    public static bool HasId3v1Tag(string filePath)
    {
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        if (fileStream.Length < TagSize)
            return false;

        fileStream.Seek(-TagSize, SeekOrigin.End);
        var marker = new byte[3];
        fileStream.ReadExactly(marker, 0, 3);

        return marker[0] == 'T' && marker[1] == 'A' && marker[2] == 'G';
    }

    /// <summary>
    /// Removes an existing ID3v1 tag from a file by truncating the last 128 bytes.
    /// </summary>
    public static void RemoveTag(string filePath)
    {
        if (!HasId3v1Tag(filePath))
            return;

        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Write);
        fileStream.SetLength(fileStream.Length - TagSize);
    }
}
