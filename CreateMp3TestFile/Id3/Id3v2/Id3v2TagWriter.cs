// =====================================================================
// Author: Bert Berrevoets
// License: GNU GPL v3
// =====================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CreateMp3TestFile.Id3.Id3v2.Frames;
using CreateMp3TestFile.Models;
using CreateMp3TestFile.Utilities;

namespace CreateMp3TestFile.Id3.Id3v2;

/// <summary>
/// Writes ID3v2.4 tags to MP3 files.
/// The tag is placed at the beginning of the file.
/// </summary>
public static class Id3v2TagWriter
{
    private const int DefaultPaddingSize = 1024; // 1KB padding for future edits

    /// <summary>
    /// Writes an ID3v2.4 tag to the beginning of the specified MP3 file.
    /// </summary>
    /// <param name="metadata">The metadata to write.</param>
    /// <param name="paddingSize">Padding size in bytes (default: 1KB).</param>
    public static void WriteTag(Mp3Metadata metadata, int paddingSize = DefaultPaddingSize)
    {
        // Build all frames
        var tagData = BuildTagData(metadata, paddingSize);

        // Remove existing ID3v2 tag if present
        RemoveExistingTag(metadata.FilePath);

        // Prepend the new tag to the file
        FileUtilities.PrependToFile(metadata.FilePath, tagData);
    }

    /// <summary>
    /// Builds the complete ID3v2.4 tag (header + frames + padding).
    /// </summary>
    public static byte[] BuildTagData(Mp3Metadata metadata, int paddingSize = DefaultPaddingSize)
    {
        // Build all frames
        var frames = new List<byte[]>();

        // Title (TIT2)
        if (!string.IsNullOrEmpty(metadata.Title))
            frames.Add(TextFrame.BuildTitle(metadata.Title));

        // Artist (TPE1)
        if (!string.IsNullOrEmpty(metadata.Artist))
            frames.Add(TextFrame.BuildArtist(metadata.Artist));

        // Album (TALB)
        if (!string.IsNullOrEmpty(metadata.Album))
            frames.Add(TextFrame.BuildAlbum(metadata.Album));

        // Year/Recording Time (TDRC)
        if (!string.IsNullOrEmpty(metadata.Year))
            frames.Add(TimestampFrame.BuildRecordingTime(metadata.Year));

        // Track Number (TRCK)
        if (metadata.TrackNumber > 0)
            frames.Add(TextFrame.BuildTrack(metadata.TrackNumber, metadata.TotalTracks));

        // Genre (TCON)
        var genreText = metadata.GetGenreString();
        if (!string.IsNullOrEmpty(genreText))
            frames.Add(TextFrame.BuildGenre(genreText));

        // Comment (COMM)
        if (!string.IsNullOrEmpty(metadata.Comment))
            frames.Add(CommentFrame.Build(metadata.Comment));

        // Calculate total frames size
        var framesSize = frames.Sum(f => f.Length);
        var totalDataSize = framesSize + paddingSize;

        // Build header
        var header = Id3v2Header.Build(totalDataSize);

        // Assemble complete tag
        var tag = new byte[Id3v2Header.HeaderSize + totalDataSize];

        // Copy header
        Array.Copy(header, 0, tag, 0, Id3v2Header.HeaderSize);

        // Copy frames
        var offset = Id3v2Header.HeaderSize;
        foreach (var frame in frames)
        {
            Array.Copy(frame, 0, tag, offset, frame.Length);
            offset += frame.Length;
        }

        // Padding is already zeros (default byte[] initialization)

        return tag;
    }

    /// <summary>
    /// Checks if a file has an existing ID3v2 tag.
    /// </summary>
    public static bool HasId3v2Tag(string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        if (stream.Length < Id3v2Header.HeaderSize)
            return false;

        var header = new byte[Id3v2Header.HeaderSize];
        stream.ReadExactly(header, 0, Id3v2Header.HeaderSize);

        return Id3v2Header.IsValidHeader(header);
    }

    /// <summary>
    /// Gets the size of an existing ID3v2 tag (including header).
    /// </summary>
    public static int GetExistingTagSize(string filePath)
    {
        if (!HasId3v2Tag(filePath))
            return 0;

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var header = new byte[Id3v2Header.HeaderSize];
        stream.ReadExactly(header, 0, Id3v2Header.HeaderSize);

        var dataSize = Id3v2Header.GetTagSize(header);
        return Id3v2Header.HeaderSize + dataSize;
    }

    /// <summary>
    /// Removes an existing ID3v2 tag from a file.
    /// </summary>
    public static void RemoveExistingTag(string filePath)
    {
        var tagSize = GetExistingTagSize(filePath);
        if (tagSize == 0)
            return;

        FileUtilities.RemoveFromBeginning(filePath, tagSize);
    }
}
