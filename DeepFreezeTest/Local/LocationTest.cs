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

        private DateTime? _baseDateTime;
        private DateTime? BaseDateTime
        {
            get { return _baseDateTime ?? (_baseDateTime = new DateTime(2000, 01, 01)); }
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

        [TestCase]
        public void ShouldBeAbleToFilterFilesByAddingARegularExpressionShowingFilesToExclude()
        {
            var location = new Location(TestDirectory, "desc")
            {
                ExcludedExpressions = new[]
                                                             {
                                                                 ".*e3.*"
                                                             }
            };

            Assert.AreEqual(26, location.GetFiles().Count());
        }

        [TestCase]
        public void ShouldBeAbleToFilterFilesByUsingABeginningDate()
        {
            ResetTestFiles();
            foreach (var file in TestDirectory.GetFiles("*.*").Take(3))
            {
                file.LastWriteTimeUtc = new DateTime(2012, 01, 01);
            }

            var location = new Location(TestDirectory, "desc") {BeginningDate = new DateTime(2011, 01, 01)};

            Assert.AreEqual(3, location.GetFiles().Count());
        }

        [TestCase]
        public void ShouldBeAbleToFilterFilesByUsingAEndingDate()
        {
            ResetTestFiles();
            foreach (var file in TestDirectory.GetFiles("*.*").Take(3))
            {
                file.LastWriteTimeUtc = new DateTime(2012, 01, 01);
            }

            var location = new Location(TestDirectory, "desc") { EndingDate = new DateTime(2011, 01, 01) };

            Assert.AreEqual(36, location.GetFiles().Count());
        }
        
       

        private void ResetTestFiles()
        {
            foreach (var file in TestDirectory.GetFiles("*", SearchOption.AllDirectories))
            {
                file.LastWriteTimeUtc = BaseDateTime.Value.ToUniversalTime();
            }
        }
    }
}
