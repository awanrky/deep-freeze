using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DeepFreeze.Local;
using NUnit.Framework;

namespace DeepFreezeTest.Local
{
    [TestFixture]
    public class LocationTest
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
        public void ShouldBeAbleToGetAllFiles()
        {
            var location = new Location(TestDirectory, "desc");

            Assert.AreEqual(39, location.GetFiles().Count());
        }

        [TestCase]
        public void ShouldBeAbleToFilterFilesByAddingARegularExpressionShowingFilesToInclude()
        {
            var location = new Location(TestDirectory, "desc")
                               {
                                   IncludedExpressions = new []
                                                             {
                                                                 ".*e3.*"
                                                             }
                               };

            Assert.AreEqual(13, location.GetFiles().Count());
        }
    }
}
