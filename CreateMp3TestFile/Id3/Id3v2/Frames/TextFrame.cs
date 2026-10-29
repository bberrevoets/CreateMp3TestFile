// =====================================================================
// Author: Bert Berrevoets
// License: GNU GPL v3
// =====================================================================

using System;
using System.Text;

namespace CreateMp3TestFile.Id3.Id3v2.Frames;

/// <summary>
/// Represents an ID3v2.4 text frame (T*** frames).
/// Used for TIT2, TPE1, TALB, TRCK, TCON, etc.
/// </summary>
/// <remarks>
/// Frame structure:
/// Bytes 0-3:   Frame ID (e.g., "TIT2")
/// Bytes 4-7:   Frame size (syncsafe in v2.4)
/// Bytes 8-9:   Flags
/// Byte 10:     Text encoding (0x03 = UTF-8)
/// Bytes 11+:   Text content
/// </remarks>
public static class TextFrame
{
    public const int FrameHeaderSize = 10;
    public const byte Utf8Encoding = 0x03;

    // Standard text frame IDs
    public const string TitleFrameId = "TIT2";
    public const string ArtistFrameId = "TPE1";
    public const string AlbumFrameId = "TALB";
    public const string TrackFrameId = "TRCK";
    public const string GenreFrameId = "TCON";

    /// <summary>
    /// Builds a text frame with the specified ID and content.
    /// </summary>
    /// <param name="frameId">4-character frame ID (e.g., "TIT2").</param>
    /// <param name="text">The text content.</param>
    /// <returns>Complete frame bytes including header and data.</returns>
    public static byte[] Build(string frameId, string text)
    {
        if (string.IsNullOrEmpty(frameId) || frameId.Length != 4)
            throw new ArgumentException("Frame ID must be exactly 4 characters.", nameof(frameId));

        if (string.IsNullOrEmpty(text))
            return [];

        // Encode text as UTF-8
        var textBytes = Encoding.UTF8.GetBytes(text);

        // Frame data = encoding byte + text
        var frameDataSize = 1 + textBytes.Length;

        // Total frame = header (10 bytes) + data
        var frame = new byte[FrameHeaderSize + frameDataSize];

        // Frame ID (4 bytes)
        var idBytes = Encoding.ASCII.GetBytes(frameId);
        Array.Copy(idBytes, 0, frame, 0, 4);

        // Frame size (syncsafe, 4 bytes) - size of data only, not header
        var sizeBytes = SyncsafeInteger.Encode(frameDataSize);
        Array.Copy(sizeBytes, 0, frame, 4, 4);

        // Flags (2 bytes) - no flags set
        frame[8] = 0x00;
        frame[9] = 0x00;

        // Encoding byte
        frame[10] = Utf8Encoding;

        // Text content
        Array.Copy(textBytes, 0, frame, 11, textBytes.Length);

        return frame;
    }

    /// <summary>
    /// Builds a title frame (TIT2).
    /// </summary>
    public static byte[] BuildTitle(string title) => Build(TitleFrameId, title);

    /// <summary>
    /// Builds an artist frame (TPE1).
    /// </summary>
    public static byte[] BuildArtist(string artist) => Build(ArtistFrameId, artist);

    /// <summary>
    /// Builds an album frame (TALB).
    /// </summary>
    public static byte[] BuildAlbum(string album) => Build(AlbumFrameId, album);

    /// <summary>
    /// Builds a track number frame (TRCK).
    /// </summary>
    /// <param name="trackNumber">Track number.</param>
    /// <param name="totalTracks">Optional total tracks (for "3/12" format).</param>
    public static byte[] BuildTrack(byte trackNumber, byte? totalTracks = null)
    {
        var trackText = totalTracks.HasValue && totalTracks.Value > 0
            ? $"{trackNumber}/{totalTracks.Value}"
            : trackNumber.ToString();
        return Build(TrackFrameId, trackText);
    }

    /// <summary>
    /// Builds a genre frame (TCON).
    /// </summary>
    /// <param name="genre">Genre text or numeric ID.</param>
    public static byte[] BuildGenre(string genre) => Build(GenreFrameId, genre);
}
