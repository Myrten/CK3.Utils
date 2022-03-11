using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CK3.Utils.BattleSimulator.Data;
using CK3.Utils.BattleSimulator.DataExtraction;
using CK3.Utils.BattleSimulator.Ranking;
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
            dataExtractor.ExtractRegimentCostsFromFiles();
            var regiments = dataExtractor.ExtractRegimentsFromFiles();

            //levies and 15 prowess knights for comparison
            //regiments.Add(Regiment.Knights15Prowess);
            //regiments.Add(Regiment.Levies);


            var regimentsToTest = regiments.Where(regiment => regiment.Damage != 0).ToArray();

            var bonuses = JsonConvert.DeserializeObject<Bonuses>(File.ReadAllText("bonuses.json"));
            var regimentsWithBonuses = regimentsToTest.Select(r => r.GetCloneWithBonuses(bonuses.EndGame)).ToArray();

           // PrintWinRatingDual(regimentsToTest);

            PrintSingleRegimentRatings(regimentsToTest, regimentsWithBonuses);

            //CreateDamageProfileCsv(regimentsWithBonuses);
        }




        static void PrintSingleRegimentRatings(Regiment[] regimentsToTest, Regiment[] regimentsWithBonuses)
        {
            Console.WriteLine("");
            Console.WriteLine("[B]Base values without any modifiers[B]");
            Console.WriteLine();


            Console.WriteLine("[Code]");
            PrintWinRating(regimentsToTest);
            Console.WriteLine("[/Code]");
            Console.WriteLine();
            Console.WriteLine("[Code]");
            PrintDamageRanking(regimentsToTest, 500);

            Console.WriteLine("[/Code]");
            Console.WriteLine();
            Console.WriteLine("[Code]");

            CreateMaintenanceRanking(regimentsToTest);

            Console.WriteLine("[/Code]");
            Console.WriteLine();

            Console.WriteLine(
                "[B]Values with 8x county and 3x duchy highest level buildings giving bonus to given unit type[/B]");
            Console.WriteLine("[Code]");

            PrintWinRating(regimentsWithBonuses);
            Console.WriteLine("[/Code]");
            Console.WriteLine();
            Console.WriteLine("[Code]");
            PrintDamageRanking(regimentsWithBonuses, 200);


            Console.WriteLine("[/Code]");
            Console.WriteLine();
            Console.WriteLine("[Code]");
            CreateRanking(regimentsWithBonuses);

            Console.WriteLine("[/Code]");
        }


        static void PrintWinRating(Regiment[] regimentsToTest)
        {
            Console.WriteLine($"Minimum number of regiments required to win against {TestLevySize} levies");
            var simulator = new Simulation.BattleSimulator();

            var results = new List<SimulationResult>();

            for (var i = 0; i < regimentsToTest.Length; i++)
            {
                var regiment = regimentsToTest[i];
                //Console.Write("\r" + new string(' ', Console.WindowWidth));
                //Console.Write($"\rSimulating {i}/{regimentsToTest.Length}");
                //start from single regiment 
                for (int j = 1; ; j++)
                {
                    var levyArmy = new Army("Levy army") { { Regiment.Levies, TestLevySize } }; //compare with TestLevySize levies

                    var regimentArmy = new Army(regiment.LocalizedName + " army") { { regiment, j * regiment.Stack } };
                    var battleResult = simulator.SimulateBattle(regimentArmy, levyArmy);

                    //increase number of regiments for as long as levies win
                    if (battleResult.Winner != regimentArmy) continue;

                    //save result
                    var result = new SimulationResult()
                    {
                        Regiment = regiment,
                        RegimentCount = j,
                        Killed = levyArmy.GetFatalCasualties(),
                        Lost = regimentArmy.GetFatalCasualties(),
                        Won = true,
                        Days = battleResult.Days
                    };
                    results.Add(result);
                    break;
                }
            }

            //Console.Clear();

            //order results from best to worst and print on console
            results = results.OrderBy(r => r.RegimentCount).ToList();

            foreach (var simulationResult in results)
            {
                Console.WriteLine(
                    $"{simulationResult} | 1 regiment = {Math.Round((double)TestLevySize / simulationResult.RegimentCount, 0),-3:N0} levies " +
                    $"| Lost {simulationResult.Lost / ((double)simulationResult.RegimentCount * simulationResult.Regiment.Stack):P2} soldiers Killed {simulationResult.Killed,-6} levies" +
                    //$"Metric = {simulationResult.Regiment.Stack* Math.Sqrt(simulationResult.Regiment.Damage*simulationResult.Regiment.Toughness):F0}" +
                    $" | {simulationResult.Days} days");
            }
        }


        static void PrintDamageRanking(Regiment[] regimentsToTest, int regimentCount = 250)
        {
            Console.WriteLine($"Damage done by {regimentCount} regiments vs {TestLevySize} levies");
            var simulator = new Simulation.BattleSimulator();

            var results = new List<SimulationResult>();

            for (var i = 0; i < regimentsToTest.Length; i++)
            {
                var regiment = regimentsToTest[i];
                //Console.Write("\r" + new string(' ', Console.WindowWidth));
                //    Console.Write($"\rSimulating {i}/{regimentsToTest.Length}");
                //start from single regiment 

                var levyArmy = new Army("Levy army") { { Regiment.Levies, TestLevySize } }; //compare with TestLevySize levies

                var regimentArmy = new Army(regiment.LocalizedName + " army") { { regiment, regimentCount * regiment.Stack } };
                var battleResult = simulator.SimulateBattle(regimentArmy, levyArmy);

                //save result
                var result = new SimulationResult()
                {
                    Regiment = regiment,
                    RegimentCount = regimentCount,
                    Killed = levyArmy.GetFatalCasualties(),
                    Lost = regimentArmy.GetFatalCasualties(),
                    Won = battleResult.Winner == regimentArmy,
                    Days = battleResult.Days
                };
                results.Add(result);
            }

            //            Console.Clear();

            //order results from best to worst and print on console
            results = results.OrderByDescending(r => r.Killed).ThenBy(r=>r.Days).ToList();

            foreach (var simulationResult in results)
            {
                Console.WriteLine(
                    $"{simulationResult} | 1 regiment vs {Math.Round((double)TestLevySize / simulationResult.RegimentCount, 0),-3:N0} levies " +
                    $"| Lost {simulationResult.Lost / ((double)simulationResult.RegimentCount * simulationResult.Regiment.Stack):P2} soldiers Killed {simulationResult.Killed,-6} levies" +
                    //$"Metric = {simulationResult.Regiment.Stack* Math.Sqrt(simulationResult.Regiment.Damage*simulationResult.Regiment.Toughness):F0}" +
                    $"{(simulationResult.Won ? " WON" : " LOST")} | {simulationResult.Days} days");
            }
        }

        static void CreateMaintenanceRanking(Regiment[] regimentsToTest)
        {
            Console.WriteLine();
            Console.WriteLine($"Maintenance Ranking vs {TestLevySize} levies");
            Console.WriteLine("Minimum total high maintenance required to win");
            Console.WriteLine();
            var simulator = new Simulation.BattleSimulator();

            var results = new List<SimulationResult>();
            var resDict = new Dictionary<RegimentRanking, List<SimulationResult>>();
            const int count = 10000;

            foreach (var regiment in regimentsToTest)
            {
                var ranking = new RegimentRanking() { Regiment = regiment };
                resDict[ranking] = new List<SimulationResult>();
                for (int j = 1; j < count + 1; j++)
                {
                    var levyArmy = new Army("Levy army") { { Regiment.Levies, TestLevySize } }; //compare with TestLevySize levies

                    var regimentArmy = new Army(regiment.LocalizedName + " army") { { regiment, j * regiment.Stack } };
                    var battleResult = simulator.SimulateBattle(regimentArmy, levyArmy);

                    //save result
                    var result = new SimulationResult()
                    {
                        Regiment = regiment,
                        RegimentCount = j,
                        Killed = levyArmy.GetFatalCasualties(),
                        Lost = regimentArmy.GetFatalCasualties(),
                        Won = battleResult.Winner == regimentArmy,
                        Days = battleResult.Days
                    };
                    resDict[ranking].Add(result);
                    results.Add(result);

                    if (ranking.WinRating == 0 && result.Won)
                    {
                        ranking.WinRating = (int) Math.Round(result.RegimentCount*result.Regiment.HighMaintenanceCost);

                    }
                    if (ranking.DamageRating == 0 && result.Killed >= TestLevySize)
                    {
                        ranking.DamageRating = result.RegimentCount;
                        if (result.Won)
                            break;
                    }
                }
            }


            //var leviesKilledOnWinWithoutPursuitOrWipe = TestLevySize * (BattleSimulationConstants.FatalCasualtiesRatio)+(TestLevySize*(1-BattleSimulationConstants.FatalCasualtiesRatio))*BattleSimulationConstants.LeftBehindRatio;

            //foreach (var pair in resDict)
            //{
            //    var rating = pair.Key;
            //    foreach (var result in pair.Value)
            //    {
            //        if (rating.WinRating == 0 && result.Won)
            //        {
            //            rating.WinRating = result.RegimentCount;
            //        }
            //        if (rating.DamageRating == 0 && result.Killed >= TestLevySize)
            //        {
            //            rating.DamageRating = result.RegimentCount;
            //        }
            //    }
            //}

            var winRanking = resDict.Select(rd => rd.Key).OrderBy(r => r.WinRating).ToArray();
            for (int i = 0; i < winRanking.Length; i++)
            {
                winRanking[i].WinRank = i + 1;
            }
            

            foreach (var regimentRanking in winRanking)
            {
                Console.WriteLine(regimentRanking.ToMaintenanceString());
            }
            
        }
        

        static void CreateRanking(Regiment[] regimentsToTest)
        {
            Console.WriteLine();
            Console.WriteLine($"Ranking vs {TestLevySize} levies");
            Console.WriteLine("Win - minimum number of regiments to win. Damage - minimum number of regiments to wipe");
            Console.WriteLine();
            var simulator = new Simulation.BattleSimulator();

            var results = new List<SimulationResult>();
            var resDict = new Dictionary<RegimentRanking, List<SimulationResult>>();
            const int count = 10000;

            foreach (var regiment in regimentsToTest)
            {
                var ranking = new RegimentRanking() { Regiment = regiment };
                resDict[ranking] = new List<SimulationResult>();
                for (int j = 1; j < count + 1; j++)
                {
                    var levyArmy = new Army("Levy army") { { Regiment.Levies, TestLevySize } }; //compare with TestLevySize levies

                    var regimentArmy = new Army(regiment.LocalizedName + " army") { { regiment, j * regiment.Stack } };
                    var battleResult = simulator.SimulateBattle(regimentArmy, levyArmy);

                    //save result
                    var result = new SimulationResult()
                    {
                        Regiment = regiment,
                        RegimentCount = j,
                        Killed = levyArmy.GetFatalCasualties(),
                        Lost = regimentArmy.GetFatalCasualties(),
                        Won = battleResult.Winner == regimentArmy,
                        Days = battleResult.Days
                    };
                    resDict[ranking].Add(result);
                    results.Add(result);

                    if (ranking.WinRating == 0 && result.Won)
                    {
                        ranking.WinRating = result.RegimentCount;

                    }
                    if (ranking.DamageRating == 0 && result.Killed >= TestLevySize)
                    {
                        ranking.DamageRating = result.RegimentCount;
                        if (result.Won)
                            break;
                    }
                }
            }


            //var leviesKilledOnWinWithoutPursuitOrWipe = TestLevySize * (BattleSimulationConstants.FatalCasualtiesRatio)+(TestLevySize*(1-BattleSimulationConstants.FatalCasualtiesRatio))*BattleSimulationConstants.LeftBehindRatio;

            //foreach (var pair in resDict)
            //{
            //    var rating = pair.Key;
            //    foreach (var result in pair.Value)
            //    {
            //        if (rating.WinRating == 0 && result.Won)
            //        {
            //            rating.WinRating = result.RegimentCount;
            //        }
            //        if (rating.DamageRating == 0 && result.Killed >= TestLevySize)
            //        {
            //            rating.DamageRating = result.RegimentCount;
            //        }
            //    }
            //}

            var winRanking = resDict.Select(rd => rd.Key).OrderBy(r => r.WinRating).ToArray();
            for (int i = 0; i < winRanking.Length; i++)
            {
                winRanking[i].WinRank = i + 1;
            }

            var damageRanking = resDict.Select(rd => rd.Key).OrderBy(r => r.DamageRating).ToArray();
            for (int i = 0; i < winRanking.Length; i++)
            {
                damageRanking[i].DamageRank = i + 1;
            }

            foreach (var regimentRanking in winRanking)
            {
                Console.WriteLine(regimentRanking);
            }


            using var writer = File.CreateText("ratings.csv");
            writer.WriteLine("Regiment;Win Rating;Damage Rating");

            foreach (var reg in winRanking)
            {
                writer.WriteLine($"[{reg.Regiment.GetRegimentTypeCode()}] {reg.Regiment};{reg.WinRating};{reg.DamageRating}");
            }
        }

        static void CreateDamageProfileCsv(Regiment[] regimentsToTest)
        {
            var simulator = new Simulation.BattleSimulator();

            var results = new List<SimulationResult>();

            const int Count = 1000;

            for (var i = 0; i < regimentsToTest.Length; i++)
            {
                var regiment = regimentsToTest[i];
                for (int j = 1; j < Count + 1; j++)
                {
                    var levyArmy = new Army("Levy army") { { Regiment.Levies, TestLevySize } }; //compare with TestLevySize levies

                    var regimentArmy = new Army(regiment.LocalizedName + " army") { { regiment, j * regiment.Stack } };
                    var battleResult = simulator.SimulateBattle(regimentArmy, levyArmy);

                    //increase number of regiments for as long as levies win

                    //save result
                    var result = new SimulationResult()
                    {
                        Regiment = regiment,
                        RegimentCount = j,
                        Killed = levyArmy.GetFatalCasualties(),
                        Lost = regimentArmy.GetFatalCasualties(),
                        Won = true,
                        Days = battleResult.Days
                    };
                    results.Add(result);
                }
            }

            var group = results.GroupBy(r => r.RegimentCount).OrderBy(g => g.Key).ToArray();

            using (var writer = File.CreateText("damage profile.csv"))
            {
                writer.Write("Regiments;");

                foreach (var r in regimentsToTest.OrderBy(r => r.Name))
                {
                    writer.Write(r.ToString() + ";");
                }
                writer.WriteLine();

                var pastResults = regimentsToTest.ToDictionary(r => r, r => -1);

                foreach (var grouping in group)
                {
                    writer.Write(grouping.Key + ";");
                    foreach (var simulationResult in grouping.OrderBy(g => g.Regiment.Name))
                    {
                        //if (pastResults[simulationResult.Regiment] == simulationResult.Killed)
                        //    writer.Write(";");
                        //else
                        {
                            writer.Write(simulationResult.Killed + ";");
                            pastResults[simulationResult.Regiment] = simulationResult.Killed;
                        }
                    }
                    writer.WriteLine();
                }
            }

        }


        static void PrintWinRatingDual(Regiment[] regimentsToTest)
        {
            Console.WriteLine($"Minimum number of regiments required to win against {TestLevySize} levies");
            var simulator = new Simulation.BattleSimulator();

            var results = new List<MultiRegimentSimulationResult>();

            for (var i = 0; i < regimentsToTest.Length; i++)
            {
                var firstRegiment = regimentsToTest[i];
                for (int j = i ; j < regimentsToTest.Length; j++)
                {
                    var secondRegiment = regimentsToTest[j];

                    //Console.Write("\r" + new string(' ', Console.WindowWidth));
                    //Console.Write($"\rSimulating {i}/{regimentsToTest.Length}");
                    //start from single regiment 
                    for (int round = 1;; round++)
                    {
                        var levyArmy = new Army("Levy army")
                            { { Regiment.Levies, TestLevySize } }; //compare with TestLevySize levies

                        var regimentArmy = new Army($"{firstRegiment.LocalizedName}+{secondRegiment.LocalizedName} army")
                        {
                            { firstRegiment, round * firstRegiment.Stack }, 
                            { secondRegiment, round * secondRegiment.Stack }
                        };
                        var battleResult = simulator.SimulateBattle(regimentArmy, levyArmy);

                        //increase number of regiments for as long as levies win
                        if (battleResult.Winner != regimentArmy) continue;

                        //save result
                        var result = new MultiRegimentSimulationResult()
                        {
                            Army = regimentArmy,
                            Regiment = firstRegiment,
                            RegimentCount = round,
                            Killed = levyArmy.GetFatalCasualties(),
                            Lost = regimentArmy.GetFatalCasualties(),
                            Won = true,
                            Days = battleResult.Days
                        };
                        results.Add(result);
                        break;
                    }
                }
            }

            //Console.Clear();

            //order results from best to worst and print on console
            results = results.OrderBy(r => r.RegimentCount).ToList();
            
            //results.RemoveAll(r => !r.ToString().Contains("Elephants", StringComparison.InvariantCulture));

            foreach (var simulationResult in results)
            {
                Console.WriteLine(
                    $"{simulationResult} | 2 regiments = {Math.Round((double)TestLevySize / simulationResult.RegimentCount, 0),-3:N0} levies " +
                    $"| Lost {simulationResult.Lost / ((double)simulationResult.RegimentCount * (simulationResult.Army.ArmyRegiments.Sum(r=>r.Regiment.Stack))):P2} soldiers Killed {simulationResult.Killed,-6} levies" +
                    //$"Metric = {simulationResult.Regiment.Stack* Math.Sqrt(simulationResult.Regiment.Damage*simulationResult.Regiment.Toughness):F0}" +
                    $" | {simulationResult.Days} days");
            }
        }
    }
}
