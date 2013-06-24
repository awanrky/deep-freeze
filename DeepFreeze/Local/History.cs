using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeepFreeze.Local
{
    public class History
    {
        private const string Filename = "glacier-backup-history.json";

        private DirectoryInfo Location { get; set; }

        private FileInfo FileInfo { get; set; }

        private HistoryInformation HistoryInformation { get; set; }

        public History(DirectoryInfo location)
        {
            Location = location;

            FileInfo = new FileInfo(Path.Combine(Location.FullName, Filename));
            Read();
        }

        public void Read()
        {
            if (!FileInfo.Exists)
            {
                HistoryInformation = GetNewHistoryInformation();
                return;
            }
            var sr = FileInfo.OpenText();
            var jsonString = sr.ReadToEnd();
            sr.Close();
            HistoryInformation = JsonConvert.DeserializeObject<HistoryInformation>(jsonString);
        }

        public void Write()
        {
            var jsonString = JsonConvert.SerializeObject(HistoryInformation, Formatting.Indented);
            var sw = FileInfo.Open(FileMode.Create);
            sw.Write(Encoding.UTF8.GetBytes(jsonString), 0, Encoding.UTF8.GetByteCount(jsonString));
            sw.Close();
        }

        public HistoryInformation GetNewHistoryInformation()
        {
            return new HistoryInformation
                              {
                                  createdon = DateTime.Now,
                                  file = Location.FullName,
                                  location = Location.FullName,
                                  previousarchives = new List<ArchiveInformation>
                                                        {
                                                            new ArchiveInformation
                                                                {
                                                                    date = new DateTime(),
                                                                    files = new List<string>()
                                                                }
                                                        }
                              };
        }

//        public DateTime GetMostRecentArchiveDate()
//        {
//            var dates = (Json["archive-dates"]).Select(d => (DateTime) d).ToArray();
//            Array.Sort(dates);
//            return dates[0];
//        }
    }
}

public class ArchiveInformation
{
    public DateTime date { get; set; }
    public IList<string> files { get; set; } 
}

public class HistoryInformation
{
    public string file { get; set; }
    public string location { get; set; }
    public DateTime createdon { get; set; }
    public IList<ArchiveInformation> previousarchives { get; set; }
}