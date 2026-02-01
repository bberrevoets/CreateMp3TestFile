// =====================================================================
// Author: Bert Berrevoets
// License: GNU GPL v3
// =====================================================================

using System;
using System.Text;

namespace CreateMp3TestFile.Id3.Id3v2.Frames;

/// <summary>
/// Represents an ID3v2.4 comment frame (COMM).
/// </summary>
/// <remarks>
/// Frame structure:
/// Bytes 0-3:   Frame ID "COMM"
/// Bytes 4-7:   Frame size (syncsafe)
/// Bytes 8-9:   Flags
/// Byte 10:     Text encoding (0x03 = UTF-8)
/// Bytes 11-13: Language code (e.g., "eng")
/// Bytes 14+:   Short description (null-terminated) + actual comment
/// </remarks>
public static class CommentFrame
{
    public const string CommentFrameId = "COMM";
    public const int FrameHeaderSize = 10;
    public const byte Utf8Encoding = 0x03;

    /// <summary>
    /// Builds a comment frame.
    /// </summary>
    /// <param name="comment">The comment text.</param>
    /// <param name="language">3-character ISO-639-2 language code (default: "eng").</param>
    /// <param name="description">Short content description (default: empty).</param>
    public static byte[] Build(string comment, string language = "eng", string description = "")
    {
        if (string.IsNullOrEmpty(comment))
            return [];

        // Ensure language is exactly 3 characters
        language = (language ?? "eng").PadRight(3)[..3];
        description ??= string.Empty;

        // Encode strings as UTF-8
        var languageBytes = Encoding.ASCII.GetBytes(language);
        var descriptionBytes = Encoding.UTF8.GetBytes(description);
        var commentBytes = Encoding.UTF8.GetBytes(comment);

        // Frame data: encoding (1) + language (3) + description + null (1) + comment
        var frameDataSize = 1 + 3 + descriptionBytes.Length + 1 + commentBytes.Length;

        // Total frame size
        var frame = new byte[FrameHeaderSize + frameDataSize];

        // Frame ID
        var idBytes = Encoding.ASCII.GetBytes(CommentFrameId);
        Array.Copy(idBytes, 0, frame, 0, 4);

        // Frame size (syncsafe)
        var sizeBytes = SyncsafeInteger.Encode(frameDataSize);
        Array.Copy(sizeBytes, 0, frame, 4, 4);

        // Flags (none)
        frame[8] = 0x00;
        frame[9] = 0x00;

        // Encoding
        frame[10] = Utf8Encoding;

        // Language
        Array.Copy(languageBytes, 0, frame, 11, 3);

        // Description + null terminator
        var offset = 14;
        Array.Copy(descriptionBytes, 0, frame, offset, descriptionBytes.Length);
        offset += descriptionBytes.Length;
        frame[offset] = 0x00; // null terminator for description
        offset++;

        // Comment text
        Array.Copy(commentBytes, 0, frame, offset, commentBytes.Length);

        return frame;
    }
}
