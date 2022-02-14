using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CK3.Utils.BattleSimulator.DataExtraction
{
    public class RegimentLocalization
    {
        public Dictionary<string,string> Localizations { get; set; }
        static readonly Regex LineRegex = new Regex("(\\w+):\\d\\s+\\\"(?:#F )?([^#]+)(?:#\\!)?\\\"");

        public static RegimentLocalization Parse(params string[] paths)
        {
            var dict = new RegimentLocalization();
            dict.Localizations = new Dictionary<string, string>();

            foreach (var path in paths)
            {
                var lines = File.ReadAllLines(path);
                foreach (var line in lines)
                {
                    var parsed = LineRegex.Match(line);
                    if (parsed.Success)
                    {
                        dict.Localizations[parsed.Groups[1].Value] = parsed.Groups[2].Value;
                    }
                }
            }
            return dict;
        }
    }
}
