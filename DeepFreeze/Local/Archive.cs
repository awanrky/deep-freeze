using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace DeepFreeze.Local
{
    public class Archive
    {
        private int _compressionLevel;
        public int CompressionLevel
        {
            get { return _compressionLevel; }
            set
            {
                _compressionLevel = value;
                if (_compressionLevel > 9)
                {
                    _compressionLevel = 9;
                }
                if (_compressionLevel < 0)
                {
                    _compressionLevel = 0;
                }
            }
        }

        public Archive()
        {
            CompressionLevel = 9;
        }

        public static string GetArchiveName(string baseFilename, DateTime dateTime)
        {
            return string.Format("{0}{1:-yyyy-MM-dd.hh.mm.ss}.zip", baseFilename, dateTime);
        }

        private ZipOutputStream GetZipOutputStream(string filename)
        {
            var filestream = File.Create(filename);
            var zipstream = new ZipOutputStream(filestream);
            zipstream.SetLevel(CompressionLevel);
            zipstream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            return zipstream;
        }


        public void Compress(string filename, IEnumerable<FileInfo> files)
        {
            using (var zipstream = GetZipOutputStream(filename))
            {
                foreach (var file in files)
                {
                    var folderOffset = 0;
                    var entryName = file.FullName.Substring(folderOffset); // Makes the name in zip based on the folder
                    entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                    var newEntry = new ZipEntry(entryName)
                                       {
                                           DateTime = file.LastWriteTime,
                                           Size = file.Length
                                       };

                    zipstream.PutNextEntry(newEntry);

                    // Zip the file in buffered chunks
                    // the "using" will close the stream even if an exception occurs
                    var buffer = new byte[4096];
                    using (var streamReader = File.OpenRead(file.FullName))
                    {
                        StreamUtils.Copy(streamReader, zipstream, buffer);
                    }
                    zipstream.CloseEntry();
                }
            }
        }
    }
}
