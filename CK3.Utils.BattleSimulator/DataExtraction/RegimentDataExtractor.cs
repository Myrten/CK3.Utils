using System.Collections.Generic;
using System.IO;
using CK3.Utils.BattleSimulator.Data;
using Pdoxcl2Sharp;

namespace CK3.Utils.BattleSimulator.DataExtraction
{
    public class RegimentDataExtractor
    {
        readonly string gameDirectory;

        public RegimentDataExtractor(string gameDirectory)
        {
            this.gameDirectory = gameDirectory;
        }
        public List<Regiment> ExtractRegimentsFromFiles()
        {
            var locPath = Path.Combine(gameDirectory, @"game\localization\english\regiment_l_english.yml");

            var locDict = RegimentLocalization.Parse(locPath);

            var regimentsPath = Path.Combine(gameDirectory, @"game\common\men_at_arms_types\");

            var regiments = new List<Regiment>();
            foreach (var filePath in Directory.GetFiles(regimentsPath, "*.txt"))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    var file = ParadoxParser.Parse(fs, new RegimentFile(locDict));
                    regiments.AddRange(file.Regiments);
                }
            }

            return regiments;
        }
    }
}
