using System;
using System.IO;
using System.Linq;
using DeepFreeze.Local;
using NUnit.Framework;

namespace VaultTest.Local
{
    [TestFixture]
    public class ArchiveTest
    {
        private DirectoryInfo _currentDirectory;
        private DirectoryInfo CurrentDirectory
        {
            get { return _currentDirectory ?? (_currentDirectory = new DirectoryInfo(System.Environment.CurrentDirectory)); }
        }

        private DirectoryInfo _testDirectory;
        private DirectoryInfo TestDirectory
        {
            get
            {
                return _testDirectory ??
                       (_testDirectory = new DirectoryInfo(Path.Combine(CurrentDirectory.FullName, "Local/TestDirectory")));
            }
        }

        [TestCase]
        public void ShouldReturnAnArchiveName()
        {
            var dateTime = new DateTime(2013, 6, 4, 12, 33, 22);
            var archive = new Archive();

            const string expected = "base-filename-2013-06-04.12.33.22.zip";
            Assert.AreEqual(expected, Archive.GetArchiveName("base-filename", dateTime));
        }

        [TestCase]
        public void ShouldCreateAnArchive()
        {
            var location = new Location(TestDirectory, "desc");

            var archive = new Archive();

            var archiveFilename = Archive.GetArchiveName(
                String.Format("{0}-should-create-an-archive-test", CurrentDirectory.FullName),
                DateTime.Now);

            archive.Compress(archiveFilename, location.GetFiles());

            var archiveFile = new FileInfo(archiveFilename);

            Assert.IsTrue(archiveFile.Exists, String.Format("{0} does not exist.", archiveFile.FullName));

        }
    }
}
