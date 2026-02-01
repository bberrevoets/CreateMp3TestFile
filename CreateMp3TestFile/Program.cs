// =====================================================================
// Author: Bert Berrevoets
//
// Created  : 12-05-2020    13:35
// Updated  : 01-02-2026
//
// Solution: CreateMp3TestFile
// Project:  CreateMp3TestFile
// Filename: Program.cs
// =====================================================================

using System;
using System.IO;
using CreateMp3TestFile.Audio;
using CreateMp3TestFile.Id3;
using CreateMp3TestFile.Models;

namespace CreateMp3TestFile;

public static class Program
{
    private static void Main()
    {
        const string fileName = "Test.mp3";

        // Clean up any existing test file
        if (File.Exists(fileName))
            File.Delete(fileName);

        // Create a valid silent MP3 file (~3 seconds of silence)
        using (var fileStream = new FileStream(fileName, FileMode.CreateNew))
        {
            var audioData = SilentMp3Generator.Generate(3.0);
            fileStream.Write(audioData);
        }

        // Create metadata for the MP3 file
        var metadata = new Mp3Metadata
        {
            FilePath = fileName,
            Title = "Test song.",
            Artist = "Bert Berrevoets",
            Album = "The Best!",
            Year = "2020",
            Comment = "This is the best album ever.",
            TrackNumber = 8,
            TotalTracks = 12, // New: ID3v2 supports "8/12" format
            GenreId = 12,
            GenreText = "Other" // New: ID3v2 can use text genres
        };

        // Write both ID3v1 and ID3v2.4 tags
        Id3TagWriter.WriteTags(metadata);

        Console.WriteLine($"Created {fileName} with ID3v1 and ID3v2.4 tags.");
        Console.WriteLine();
        Console.WriteLine("Metadata written:");
        Console.WriteLine($"  Title:  {metadata.Title}");
        Console.WriteLine($"  Artist: {metadata.Artist}");
        Console.WriteLine($"  Album:  {metadata.Album}");
        Console.WriteLine($"  Year:   {metadata.Year}");
        Console.WriteLine($"  Track:  {metadata.GetFormattedTrackNumber()}");
        Console.WriteLine($"  Genre:  {metadata.GetGenreString()} (ID: {metadata.GenreId})");
        Console.WriteLine($"  Comment: {metadata.Comment}");
    }
}
