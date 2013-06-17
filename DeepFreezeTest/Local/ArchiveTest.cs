using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DeepFreezeTest.Local
{
    [TestFixture]
    public class ArchiveTest
    {
        private DirectoryInfo _currentDirectory;
        private DirectoryInfo CurrentDirectory
        {
            get { return _currentDirectory ?? (_currentDirectory = new DirectoryInfo(System.Environment.CurrentDirectory)); }
        }

        [TestCase]
        public void ShouldReturnAnArchiveName()
        {
            var dateTime = new DateTime(2013, 6, 4, 12, 33, 22);
            var archive = new DeepFreeze.Local.Archive(CurrentDirectory);

            var expected = string.Format("{0}-2013-06-04.12.33.22.zip", CurrentDirectory.Name);
            Assert.AreEqual(expected, archive.GetArchiveName(dateTime));
        }
    }
}
