using System;
using System.IO;
using System.Text;

namespace CreateMp3TestFile
{
    public static class Program
    {
        private static void Main()
        {
            if (File.Exists("TestFile.mp3")) File.Delete("TestFile.mp3");

            var fileStream = new FileStream("Test.mp3", FileMode.CreateNew);
            var mp3 = new byte[10000];
            fileStream.Write(mp3);
            fileStream.Dispose();

            var mp3Tag = new Mp3("Test.mp3")
            {
                Id3Album = "The Best!",
                Id3Artist = "Bert Berrevoets",
                Id3Comment = "This is the best album ever.",
                Id3Title = "Test song.",
                Id3Year = "2020",
                Id3TrackNumber = 8,
                Id3Genre = 12
            };


            UpdateMp3Tag(mp3Tag);
        }

        private static void UpdateMp3Tag(Mp3 paramMp3)
        {
            // Trim any whitespace
            paramMp3.Id3Title = paramMp3.Id3Title.Trim();
            paramMp3.Id3Artist = paramMp3.Id3Artist.Trim();
            paramMp3.Id3Album = paramMp3.Id3Album.Trim();
            paramMp3.Id3Year = paramMp3.Id3Year.Trim();
            paramMp3.Id3Comment = paramMp3.Id3Comment.Trim();

            // Ensure all properties are correct size
            if (paramMp3.Id3Title.Length > 30) paramMp3.Id3Title = paramMp3.Id3Title.Substring(0, 30);
            if (paramMp3.Id3Artist.Length > 30) paramMp3.Id3Artist = paramMp3.Id3Artist.Substring(0, 30);
            if (paramMp3.Id3Album.Length > 30) paramMp3.Id3Album = paramMp3.Id3Album.Substring(0, 30);
            if (paramMp3.Id3Year.Length > 4) paramMp3.Id3Year = paramMp3.Id3Year.Substring(0, 4);
            if (paramMp3.Id3Comment.Length > 28) paramMp3.Id3Comment = paramMp3.Id3Comment.Substring(0, 28);

            // Build a new ID3 Tag (128 Bytes)
            var tagByteArray = new byte[128];
            for (var i = 0; i < tagByteArray.Length; i++) tagByteArray[i] = 0; // Initialise array to nulls

            // Convert the Byte Array to a String
            Encoding instEncoding = new ASCIIEncoding();   // NB: Encoding is an Abstract class // ************ To DO: Make a shared instance of ASCIIEncoding so we don't keep creating/destroying it
                                                           // Copy "TAG" to Array
            var workingByteArray = instEncoding.GetBytes("TAG");
            Array.Copy(workingByteArray, 0, tagByteArray, 0, workingByteArray.Length);
            // Copy Title to Array
            workingByteArray = instEncoding.GetBytes(paramMp3.Id3Title);
            Array.Copy(workingByteArray, 0, tagByteArray, 3, workingByteArray.Length);
            // Copy Artist to Array
            workingByteArray = instEncoding.GetBytes(paramMp3.Id3Artist);
            Array.Copy(workingByteArray, 0, tagByteArray, 33, workingByteArray.Length);
            // Copy Album to Array
            workingByteArray = instEncoding.GetBytes(paramMp3.Id3Album);
            Array.Copy(workingByteArray, 0, tagByteArray, 63, workingByteArray.Length);
            // Copy Year to Array
            workingByteArray = instEncoding.GetBytes(paramMp3.Id3Year);
            Array.Copy(workingByteArray, 0, tagByteArray, 93, workingByteArray.Length);
            // Copy Comment to Array
            workingByteArray = instEncoding.GetBytes(paramMp3.Id3Comment);
            Array.Copy(workingByteArray, 0, tagByteArray, 97, workingByteArray.Length);
            // Copy Track and Genre to Array
            tagByteArray[126] = paramMp3.Id3TrackNumber;
            tagByteArray[127] = paramMp3.Id3Genre;

            // SAVE TO DISK: Replace the final 128 Bytes with our new ID3 tag
            var oFileStream = new FileStream(paramMp3.FileComplete, FileMode.Open);
            oFileStream.Seek(0, SeekOrigin.End);
            oFileStream.Write(tagByteArray, 0, 128);
            oFileStream.Close();
        }

    }

    public struct Mp3
    {
        public readonly string FileComplete;
        public string Id3Title;
        public string Id3Artist;
        public string Id3Album;
        public string Id3Year;
        public string Id3Comment;
        public byte Id3TrackNumber;
        public byte Id3Genre;

        // Required struct constructor
        public Mp3(string name)
        {
            this.FileComplete = name;
            this.Id3Title = null;
            this.Id3Artist = null;
            this.Id3Album = null;
            this.Id3Year = null;
            this.Id3Comment = null;
            this.Id3TrackNumber = 0;
            this.Id3Genre = 0;
        }
    }

}

