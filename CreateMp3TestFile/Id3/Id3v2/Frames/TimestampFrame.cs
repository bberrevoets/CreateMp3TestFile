// =====================================================================
// Author: Bert Berrevoets
// License: GNU GPL v3
// =====================================================================

using System;

namespace CreateMp3TestFile.Id3.Id3v2.Frames;

/// <summary>
/// Represents an ID3v2.4 timestamp frame (TDRC - Recording Time).
/// Replaces TYER, TDAT, TIME from ID3v2.3.
/// </summary>
/// <remarks>
/// TDRC supports various timestamp formats:
/// - "2020" (year only)
/// - "2020-05" (year-month)
/// - "2020-05-12" (year-month-day)
/// - "2020-05-12T13:35" (with time)
/// - "2020-05-12T13:35:00" (with seconds)
/// </remarks>
public static class TimestampFrame
{
    public const string RecordingTimeFrameId = "TDRC";

    /// <summary>
    /// Builds a recording time frame (TDRC) with a year value.
    /// </summary>
    /// <param name="year">The recording year (e.g., "2020").</param>
    public static byte[] BuildRecordingTime(string year)
    {
        if (string.IsNullOrEmpty(year))
            return [];

        // TDRC is just a text frame, so we can reuse TextFrame
        return TextFrame.Build(RecordingTimeFrameId, year.Trim());
    }

    /// <summary>
    /// Builds a recording time frame with full date information.
    /// </summary>
    /// <param name="dateTime">The recording date/time.</param>
    /// <param name="includeTime">Whether to include time in the timestamp.</param>
    public static byte[] BuildRecordingTime(DateTime dateTime, bool includeTime = false)
    {
        var format = includeTime ? "yyyy-MM-ddTHH:mm:ss" : "yyyy-MM-dd";
        var timestamp = dateTime.ToString(format);
        return TextFrame.Build(RecordingTimeFrameId, timestamp);
    }

    /// <summary>
    /// Builds a recording time frame with just the year.
    /// </summary>
    /// <param name="year">The recording year.</param>
    public static byte[] BuildRecordingTime(int year)
    {
        return TextFrame.Build(RecordingTimeFrameId, year.ToString());
    }
}
