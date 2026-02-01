// =====================================================================
// Author: Bert Berrevoets
// License: GNU GPL v3
// =====================================================================

using System;
using System.IO;
using CreateMp3TestFile.Id3.Id3v1;
using CreateMp3TestFile.Id3.Id3v2;
using CreateMp3TestFile.Models;

namespace CreateMp3TestFile.Id3;

/// <summary>
/// Configuration options for ID3 tag writing.
/// </summary>
public class Id3TagWriterOptions
{
    /// <summary>
    /// Write ID3v1 tag at end of file. Default: true.
    /// </summary>
    public bool WriteId3v1 { get; set; } = true;

    /// <summary>
    /// Write ID3v2.4 tag at beginning of file. Default: true.
    /// </summary>
    public bool WriteId3v2 { get; set; } = true;

    /// <summary>
    /// Padding size for ID3v2 tag in bytes. Default: 1024 (1KB).
    /// </summary>
    public int Id3v2PaddingSize { get; set; } = 1024;
}

/// <summary>
/// Coordinates writing of ID3 tags (v1 and/or v2.4) to MP3 files.
/// </summary>
public static class Id3TagWriter
{
    /// <summary>
    /// Writes ID3 tags to an MP3 file using default options (both v1 and v2.4).
    /// </summary>
    /// <param name="metadata">The metadata to write.</param>
    public static void WriteTags(Mp3Metadata metadata)
    {
        WriteTags(metadata, new Id3TagWriterOptions());
    }

    /// <summary>
    /// Writes ID3 tags to an MP3 file with specified options.
    /// </summary>
    /// <param name="metadata">The metadata to write.</param>
    /// <param name="options">Configuration options.</param>
    public static void WriteTags(Mp3Metadata metadata, Id3TagWriterOptions options)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrEmpty(metadata.FilePath))
            throw new ArgumentException("File path must be specified.", nameof(metadata));

        if (!File.Exists(metadata.FilePath))
            throw new FileNotFoundException("MP3 file not found.", metadata.FilePath);

        // Write ID3v2.4 tag first (at beginning of file)
        // This must be done before ID3v1 to avoid affecting file positions
        if (options.WriteId3v2)
        {
            Id3v2TagWriter.WriteTag(metadata, options.Id3v2PaddingSize);
        }

        // Write ID3v1 tag (at end of file)
        if (options.WriteId3v1)
        {
            // Remove existing v1 tag if present to avoid duplicates
            Id3v1TagWriter.RemoveTag(metadata.FilePath);
            Id3v1TagWriter.WriteTag(metadata);
        }
    }

    /// <summary>
    /// Writes only an ID3v1 tag to an MP3 file.
    /// </summary>
    public static void WriteId3v1Only(Mp3Metadata metadata)
    {
        WriteTags(metadata, new Id3TagWriterOptions
        {
            WriteId3v1 = true,
            WriteId3v2 = false
        });
    }

    /// <summary>
    /// Writes only an ID3v2.4 tag to an MP3 file.
    /// </summary>
    public static void WriteId3v2Only(Mp3Metadata metadata, int paddingSize = 1024)
    {
        WriteTags(metadata, new Id3TagWriterOptions
        {
            WriteId3v1 = false,
            WriteId3v2 = true,
            Id3v2PaddingSize = paddingSize
        });
    }

    /// <summary>
    /// Removes all ID3 tags from a file.
    /// </summary>
    public static void RemoveAllTags(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("MP3 file not found.", filePath);

        // Remove v2 first (changes file beginning)
        Id3v2TagWriter.RemoveExistingTag(filePath);

        // Remove v1 (at file end)
        Id3v1TagWriter.RemoveTag(filePath);
    }
}
