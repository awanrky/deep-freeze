﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace DeepFreeze.Local
{
    public class Location
    {
        public DirectoryInfo BaseDirectoryDirectoryInfo { get; private set; }
        public string Description { get; private set; }

        public IEnumerable<string> IncludedExpressions { get; set; }

        public IEnumerable<Regex> IncludeRegularExpressions
        {
            get
            {
                return
                    (IncludedExpressions == null)
                    ? new List<Regex>() { new Regex(".*") }
                    : IncludedExpressions.Select(e => new Regex(e));
            }
        }

        public IEnumerable<string> ExcludedExpressions { get; set; }

        public IEnumerable<Regex> ExcludedRegularExpressions
        {
            get
            {
                return (ExcludedExpressions == null)
                           ? new List<Regex>()
                           : ExcludedExpressions.Select(e => new Regex(e));
            }
        }

        public DateTime BeginningDate { get; set; }

        public DateTime EndingDate { get; set; }

        public Location(DirectoryInfo baseDirectoryDirectoryInfo, string description)
        {
            BaseDirectoryDirectoryInfo = baseDirectoryDirectoryInfo;
            Description = description;
        }

        public Location(string baseDirectory, string description) : this(new DirectoryInfo(baseDirectory), description)
        {
        }

        
        public void CreateArchive(IEnumerable<string> excludedFiles, IEnumerable<string> includedFiles)
        {
        }

        public IEnumerable<FileInfo> GetFiles()
        {
            return BaseDirectoryDirectoryInfo
                .GetFiles("*", SearchOption.AllDirectories)
                .Where(f => IncludeRegularExpressions.Any(e => e.IsMatch(f.Name)))
                .Where(f => !ExcludedRegularExpressions.Any(e => e.IsMatch(f.Name)));
//                .Where(f => f.LastWriteTimeUtc >= BeginningDate.ToUniversalTime())
//                .Where(f => f.LastWriteTimeUtc <= EndingDate.ToUniversalTime());
        }
    }
}
