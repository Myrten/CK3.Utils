using System;
using System.Collections.Generic;
using System.Linq;
using CK3.Utils.BattleSimulator.Data;
using CK3.Utils.BattleSimulator.DataExtraction;
using CK3.Utils.BattleSimulator.Simulation;

namespace CK3.Utils.BattleSimulator
{
    class Program
    {
        const int TestLevySize = 100000;
        static void Main(string[] args)
        {
            //change it to your game path
            const string gamePath = @"D:\Steam\steamapps\common\Crusader Kings III\";

            //data extraction
            var dataExtractor = new RegimentDataExtractor(gamePath);
            var regiments = dataExtractor.ExtractRegimentsFromFiles();
            
            //levies and 15 prowess knights for comparison
            regiments.Add(Regiment.Knights15Prowess);
            regiments.Add(Regiment.Levies);


            var simulator = new Simulation.BattleSimulator();
            var results = new List<SimulationResult>();

            var regimentsToTest = regiments.Where(regiment => regiment.Damage != 0).ToArray();

            for (var i = 0; i < regimentsToTest.Length; i++)
            {
                var regiment = regimentsToTest[i];
                //Console.Write("\r" + new string(' ', Console.WindowWidth));
                Console.Write($"\rSimulating {i}/{regimentsToTest.Length}");
                //start from single regiment 
                for (int j = 1;; j++)
                {
                    var levyArmy = new Army("Levy army") {{Regiment.Levies, TestLevySize } }; //compare with TestLevySize levies

                    var regimentArmy = new Army(regiment.LocalizedName + " army") {{regiment, j * regiment.Stack}};
                    var victor = simulator.SimulateBattle(regimentArmy, levyArmy);

                    //increase number of regiments for as long as levies win
                    if (victor != regimentArmy) continue;

                    //save result
                    var result = new SimulationResult() {Regiment = regiment, Value = j};
                    results.Add(result);
                    break;
                }
            }
            Console.Clear();

            //order results from best to worst and print on console
            results = results.OrderBy(r => r.Value).ToList();

            foreach (var simulationResult in results)
            {
                Console.WriteLine($"{simulationResult} | 1 regiment = {Math.Round((double)TestLevySize/simulationResult.Value,0):N0} levies");
            }
        }
    }
}
