// =====================================================================
// Author: Bert Berrevoets
// License: GNU GPL v3
// =====================================================================

using System.IO;

namespace CreateMp3TestFile.Utilities;

/// <summary>
/// Utility methods for file operations.
/// </summary>
public static class FileUtilities
{
    private const int BufferSize = 81920; // 80KB buffer for file operations

    /// <summary>
    /// Prepends data to the beginning of a file.
    /// Uses a temporary file for safe operation.
    /// </summary>
    /// <param name="filePath">Path to the file.</param>
    /// <param name="data">Data to prepend.</param>
    public static void PrependToFile(string filePath, byte[] data)
    {
        if (data == null || data.Length == 0)
            return;

        var tempPath = filePath + ".tmp";

        try
        {
            // Write new data + existing content to temp file
            using (var tempStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                // Write the new data first
                tempStream.Write(data, 0, data.Length);

                // Copy existing file content
                using var sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                sourceStream.CopyTo(tempStream, BufferSize);
            }

            // Replace original with temp file
            File.Delete(filePath);
            File.Move(tempPath, filePath);
        }
        finally
        {
            // Clean up temp file if it still exists
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

    /// <summary>
    /// Removes data from the beginning of a file.
    /// </summary>
    /// <param name="filePath">Path to the file.</param>
    /// <param name="bytesToRemove">Number of bytes to remove from the beginning.</param>
    public static void RemoveFromBeginning(string filePath, int bytesToRemove)
    {
        if (bytesToRemove <= 0)
            return;

        var tempPath = filePath + ".tmp";

        try
        {
            using (var tempStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            using (var sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // Skip the bytes to remove
                if (sourceStream.Length <= bytesToRemove)
                {
                    // File would be empty, just create empty temp file
                    return;
                }

                sourceStream.Seek(bytesToRemove, SeekOrigin.Begin);
                sourceStream.CopyTo(tempStream, BufferSize);
            }

            // Replace original with temp file
            File.Delete(filePath);
            File.Move(tempPath, filePath);
        }
        finally
        {
            // Clean up temp file if it still exists
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

    /// <summary>
    /// Appends data to the end of a file.
    /// </summary>
    /// <param name="filePath">Path to the file.</param>
    /// <param name="data">Data to append.</param>
    public static void AppendToFile(string filePath, byte[] data)
    {
        if (data == null || data.Length == 0)
            return;

        using var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write);
        stream.Write(data, 0, data.Length);
    }
}
