using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CK3.Utils.BattleSimulator.FileReading
{
    public class RegimentLocalization
    {
        public Dictionary<string,string> Localizations { get; set; }
        static readonly Regex LineRegex = new Regex("(\\w+):\\d\\s+\\\"(?:#F )?([^#]+)(?:#\\!)?\\\"");

        public static RegimentLocalization Parse(string path)
        {
            var lines = File.ReadAllLines(path);
            var dict = new RegimentLocalization();
            dict.Localizations = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                var parsed = LineRegex.Match(line);
                if (parsed.Success)
                {
                    dict.Localizations[parsed.Groups[1].Value] = parsed.Groups[2].Value;
                }
            }

            return dict;
        }
    }
}
