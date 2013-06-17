using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace DeepFreeze.Local
{
    public class Archive
    {
        private DirectoryInfo DirectoryInfo { get; set; }

        public Archive(DirectoryInfo directoryInfo)
        {
            DirectoryInfo = directoryInfo;
        }

        public Archive(string directoryName) : this (new DirectoryInfo(directoryName))
        {
            
        }

        public string GetArchiveName(DateTime dateTime)
        {
            return string.Format("{0}{1:-yyyy-MM-dd.hh.mm.ss}.zip", DirectoryInfo.Name, dateTime);
        }

        // Compresses the files in the nominated folder, and creates a zip file on disk named as outPathname.
        //
        public void CreateSample(string outPathname, string password, string folderName)
        {
            var fsOut = File.Create(outPathname);
            var zipStream = new ZipOutputStream(fsOut);

            zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

            zipStream.Password = password;  // optional. Null is the same as not setting. Required if using AES.

            // This setting will strip the leading part of the folder path in the entries, to
            // make the entries relative to the starting folder.
            // To include the full path for each entry up to the drive root, assign folderOffset = 0.
            var folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

            CompressFolder(folderName, zipStream, folderOffset);

            zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            zipStream.Close();
        }

        // Recurses down the folder structure
        //
        private void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {

            var files = Directory.GetFiles(path);

            foreach (var filename in files)
            {

                var fi = new FileInfo(filename);

                var entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                var newEntry = new ZipEntry(entryName) { DateTime = fi.LastWriteTime, Size = fi.Length };

                // Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
                // A password on the ZipOutputStream is required if using AES.
                //   newEntry.AESKeySize = 256;

                // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
                // you need to do one of the following: Specify UseZip64.Off, or set the Size.
                // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
                // but the zip will be in Zip64 format which not all utilities can understand.
                //   zipStream.UseZip64 = UseZip64.Off;

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                var buffer = new byte[4096];
                using (var streamReader = File.OpenRead(filename))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }
            var folders = Directory.GetDirectories(path);
            foreach (var folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset);
            }
        }
    }
}
