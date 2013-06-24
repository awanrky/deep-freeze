using CommandLine;

namespace GlacierBackup
{
    public class Options
    {
        [Option('d', "directory")]
        public string Directory { get; set; }

        [Option('t', "temp-directory", DefaultValue = "C:/Temp")]
        public string TempDirectory { get; set; }
    }
}