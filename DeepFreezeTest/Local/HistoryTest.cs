using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepFreeze.Local;
using NUnit.Framework;

namespace DeepFreezeTest.Local
{
    [TestFixture]
    class HistoryTest
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

        [Test]
        public void ShouldCeateANewHistoryFile()
        {
            ClearHistoryFilesFromTestDirectory();
            var historyFiles = GetHistoryFilesFromTestDirecory();
            Assert.AreEqual(0, historyFiles.Count());
            var history = new History(TestDirectory);

            history.Read();
            history.Write();

            historyFiles = GetHistoryFilesFromTestDirecory();
            Assert.AreEqual(1, historyFiles.Count());
        }

        private IEnumerable<FileInfo> GetHistoryFilesFromTestDirecory()
        {
            return TestDirectory.GetFiles("*.json");
        }

        private void ClearHistoryFilesFromTestDirectory()
        {
            var files = GetHistoryFilesFromTestDirecory();

            foreach (var file in files)
            {
                file.Delete();
            }
        }
    }
}
