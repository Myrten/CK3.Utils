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

            foreach (var regiment in regiments.Where(regiment => regiment.Damage != 0))
            {
                //start from single regiment 
                for (int i = 1; ; i++)
                {
                    var levyArmy = new Army("Levy army") { { Regiment.Levies, 150000 } }; //compare with 150k levies

                    var regimentArmy = new Army(regiment.LocalizedName + " army") {{regiment, i*regiment.Stack}};
                    var victor =  simulator.SimulateBattle(regimentArmy, levyArmy);
                    
                    //increase number of regiments for as long as levies win
                    if (victor != regimentArmy) continue;
                    
                    //save resu;t
                    var result = new SimulationResult() {Regiment = regiment, Value = i};
                    results.Add(result);
                    break;
                }
            }


            //order results from best to worst and print on consoe
            results = results.OrderBy(r => r.Value).ToList();

            foreach (var simulationResult in results)
            {
                Console.WriteLine(simulationResult);
            }
        }
    }
}
