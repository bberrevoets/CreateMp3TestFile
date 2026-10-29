// =====================================================================
// Author: Bert Berrevoets
// License: GNU GPL v3
// =====================================================================

#nullable enable

namespace CreateMp3TestFile.Models;

/// <summary>
/// Represents metadata for an MP3 file, compatible with both ID3v1 and ID3v2.4 tags.
/// </summary>
public class Mp3Metadata
{
    /// <summary>
    /// Full path to the MP3 file.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Song title. ID3v1 limit: 30 characters.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Artist name. ID3v1 limit: 30 characters.
    /// </summary>
    public string Artist { get; set; } = string.Empty;

    /// <summary>
    /// Album name. ID3v1 limit: 30 characters.
    /// </summary>
    public string Album { get; set; } = string.Empty;

    /// <summary>
    /// Recording year (e.g., "2020"). ID3v1 limit: 4 characters.
    /// </summary>
    public string Year { get; set; } = string.Empty;

    /// <summary>
    /// Comment text. ID3v1 limit: 28 characters.
    /// </summary>
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// Track number (1-255 for ID3v1).
    /// </summary>
    public byte TrackNumber { get; set; }

    /// <summary>
    /// Total number of tracks on the album (ID3v2 only, for "3/12" format).
    /// </summary>
    public byte? TotalTracks { get; set; }

    /// <summary>
    /// Genre ID (0-255). See ID3v1 genre list for standard values.
    /// </summary>
    public byte GenreId { get; set; }

    /// <summary>
    /// Genre as text (ID3v2 only). If null, GenreId will be used.
    /// </summary>
    public string? GenreText { get; set; }

    /// <summary>
    /// Gets the track number formatted for ID3v2 (e.g., "3" or "3/12").
    /// </summary>
    public string GetFormattedTrackNumber()
    {
        if (TotalTracks.HasValue && TotalTracks.Value > 0)
        {
            return $"{TrackNumber}/{TotalTracks.Value}";
        }
        return TrackNumber.ToString();
    }

    /// <summary>
    /// Gets the genre string for ID3v2 tags.
    /// Returns GenreText if set, otherwise the numeric genre ID.
    /// </summary>
    public string GetGenreString()
    {
        return !string.IsNullOrEmpty(GenreText) ? GenreText : GenreId.ToString();
    }
}
