using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CK3.Utils.BattleSimulator.Data;
using Pdoxcl2Sharp;

namespace CK3.Utils.BattleSimulator.DataExtraction
{
    public class RegimentDataExtractor
    {
        readonly string gameDirectory;

        readonly string[] RegimentLocPaths;

        public RegimentDataExtractor(string gameDirectory)
        {
            this.gameDirectory = gameDirectory;
            RegimentLocPaths = new[]
            {
                Path.Combine(gameDirectory,@"game\localization\english\regiment_l_english.yml"),
                Path.Combine(gameDirectory,@"game\localization\english\dlc\fp1\dlc_fp1_regiment_l_english.yml"),
            };
        }
        public List<Regiment> ExtractRegimentsFromFiles()
        {
            var locDict = RegimentLocalization.Parse(RegimentLocPaths);

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

            var errorRegiment = regiments.FirstOrDefault(r => r.Damage < 0 || r.Toughness < 0 || r.Pursuit < 0 || r.Screen < 0);

            if (errorRegiment != null)
                throw new InvalidOperationException($"Invalid regiment: {errorRegiment} {errorRegiment.Damage} {errorRegiment.Toughness}T {errorRegiment.Pursuit} {errorRegiment.Screen}S ");

            return regiments;
        }
    }
}
