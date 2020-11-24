﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CK3.Utils.BattleSimulator.Data;
using CK3.Utils.BattleSimulator.DataExtraction;
using CK3.Utils.BattleSimulator.Simulation;
using Newtonsoft.Json;

namespace CK3.Utils.BattleSimulator
{
    class Program
    {
        const int TestLevySize = 100000;
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            //change it to your game path
            const string gamePath = @"D:\Steam\steamapps\common\Crusader Kings III\";

            //data extraction
            var dataExtractor = new RegimentDataExtractor(gamePath);
            var regiments = dataExtractor.ExtractRegimentsFromFiles();
            
            //levies and 15 prowess knights for comparison
            //regiments.Add(Regiment.Knights15Prowess);
            //regiments.Add(Regiment.Levies);

            var regimentsToTest = regiments.Where(regiment => regiment.Damage != 0).ToArray();

            CreateWinningBattlesRanking(regimentsToTest);
            Console.WriteLine();
            CreateDamageRanking(regimentsToTest);

            ToCsv(regimentsToTest);
        }


        static void CreateWinningBattlesRanking(Regiment[] regimentsToTest)
        {
            var simulator = new Simulation.BattleSimulator();

            var results = new List<SimulationResult>();

            for (var i = 0; i < regimentsToTest.Length; i++)
            {
                var regiment = regimentsToTest[i];
                //Console.Write("\r" + new string(' ', Console.WindowWidth));
                Console.Write($"\rSimulating {i}/{regimentsToTest.Length}");
                //start from single regiment 
                for (int j = 1;; j++)
                {
                    var levyArmy = new Army("Levy army") {{Regiment.Levies, TestLevySize}}; //compare with TestLevySize levies

                    var regimentArmy = new Army(regiment.LocalizedName + " army") {{regiment, j * regiment.Stack}};
                    var battleResult = simulator.SimulateBattle(regimentArmy, levyArmy);

                    //increase number of regiments for as long as levies win
                    if (battleResult.Winner != regimentArmy) continue;

                    //save result
                    var result = new SimulationResult()
                    {
                        Regiment = regiment, Count = j, Killed = levyArmy.GetFatalCasualties(),
                        Lost = regimentArmy.GetFatalCasualties(),
                        Won = true,
                        Days = battleResult.Days
                    };
                    results.Add(result);
                    break;
                }
            }

            Console.Clear();

            //order results from best to worst and print on console
            results = results.OrderBy(r => r.Count).ToList();

            foreach (var simulationResult in results)
            {
                Console.WriteLine(
                    $"{simulationResult} | 1 regiment = {Math.Round((double) TestLevySize / simulationResult.Count, 0),-3:N0} levies " +
                    $"| Lost {simulationResult.Lost / ((double) simulationResult.Count * simulationResult.Regiment.Stack):P2} soldiers Killed {simulationResult.Killed,-6} levies" +
                    //$"Metric = {simulationResult.Regiment.Stack* Math.Sqrt(simulationResult.Regiment.Damage*simulationResult.Regiment.Toughness):F0}" +
                    $" | {simulationResult.Days} days");
            }
        }


        static void CreateDamageRanking(Regiment[] regimentsToTest)
        {
            var simulator = new Simulation.BattleSimulator();

            var results = new List<SimulationResult>();

            for (var i = 0; i < regimentsToTest.Length; i++)
            {
                var regiment = regimentsToTest[i];
                //Console.Write("\r" + new string(' ', Console.WindowWidth));
            //    Console.Write($"\rSimulating {i}/{regimentsToTest.Length}");
                //start from single regiment 
                
                    var levyArmy = new Army("Levy army") { { Regiment.Levies, TestLevySize } }; //compare with TestLevySize levies

                    var regiments = 500;

                    var regimentArmy = new Army(regiment.LocalizedName + " army") { { regiment, regiments*regiment.Stack } };
                    var battleResult = simulator.SimulateBattle(regimentArmy, levyArmy);
                    
                    //save result
                    var result = new SimulationResult()
                    {
                        Regiment = regiment,
                        Count = regiments,
                        Killed = levyArmy.GetFatalCasualties(),
                        Lost = regimentArmy.GetFatalCasualties(),
                        Won = battleResult.Winner == regimentArmy,
                        Days = battleResult.Days
                    };
                    results.Add(result);
            }

//            Console.Clear();

            //order results from best to worst and print on console
            results = results.OrderByDescending(r => r.Killed).ToList();

            foreach (var simulationResult in results)
            {
                Console.WriteLine(
                    $"{simulationResult} | 1 regiment vs {Math.Round((double)TestLevySize / simulationResult.Count, 0),-3:N0} levies " +
                    $"| Lost {simulationResult.Lost / ((double)simulationResult.Count * simulationResult.Regiment.Stack):P2} soldiers Killed {simulationResult.Killed,-6} levies" +
                    //$"Metric = {simulationResult.Regiment.Stack* Math.Sqrt(simulationResult.Regiment.Damage*simulationResult.Regiment.Toughness):F0}" +
                    $"{(simulationResult.Won?" WON":" LOST")} | {simulationResult.Days} days");
            }
        }

        static void ToCsv(Regiment[] regimentsToTest)
        {
            var simulator = new Simulation.BattleSimulator();

            var results = new List<SimulationResult>();

            const int Count = 1000;

            for (var i = 0; i < regimentsToTest.Length; i++)
            {
                var regiment = regimentsToTest[i];
                for (int j = 1; j< Count+1; j++)
                {
                    var levyArmy = new Army("Levy army") { { Regiment.Levies, TestLevySize } }; //compare with TestLevySize levies

                    var regimentArmy = new Army(regiment.LocalizedName + " army") { { regiment, j * regiment.Stack } };
                    var battleResult = simulator.SimulateBattle(regimentArmy, levyArmy);

                    //increase number of regiments for as long as levies win
                    
                    //save result
                    var result = new SimulationResult()
                    {
                        Regiment = regiment,
                        Count = j,
                        Killed = levyArmy.GetFatalCasualties(),
                        Lost = regimentArmy.GetFatalCasualties(),
                        Won = true,
                        Days = battleResult.Days
                    };
                    results.Add(result);
                }
            }

            var group = results.GroupBy(r => r.Count).OrderBy(g => g.Key).ToArray();

            using (var writer = File.CreateText("results.csv"))
            {
                writer.Write("Regiments;");

                foreach (var r in regimentsToTest.OrderBy(r=>r.Name))
                {
                    writer.Write(r.ToString()+";");
                }
                writer.WriteLine();

                var pastResults = regimentsToTest.ToDictionary(r => r, r=> -1);

                foreach (var grouping in group)
                {
                    writer.Write(grouping.Key+";");
                    foreach (var simulationResult in grouping.OrderBy(g=>g.Regiment.Name))
                    {
                        if (pastResults[simulationResult.Regiment] == simulationResult.Killed)
                            writer.Write(";");
                        else
                        {
                            writer.Write(simulationResult.Killed + ";");
                            pastResults[simulationResult.Regiment] = simulationResult.Killed;
                        }
                    }
                    writer.WriteLine();
                }
            }

        }
    }
}
