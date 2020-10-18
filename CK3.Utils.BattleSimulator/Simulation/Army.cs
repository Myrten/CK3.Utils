using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CK3.Utils.BattleSimulator.Data;

namespace CK3.Utils.BattleSimulator.Simulation
{
    public class Army :IEnumerable<ArmyRegiment>
    {
        public string Name { get;  }
        public Army(string name)
        {
            Name = name;
        }

        public List<ArmyRegiment> ArmyRegiments { get; } = new List<ArmyRegiment>();

        public void Add(ArmyRegiment armyRegiment)
        {
            ArmyRegiments.Add(armyRegiment);
            ArmyStrength = ArmyRegiments.Sum(ar => ar.Strength);
        }
        public void Add(Regiment regiment, int strength)
        {
            Add(new ArmyRegiment(regiment, strength));
        }

        public double ArmyStrength
        {
            get;
            private set;
        }

        public double GetDamage(double combatWidth)
        {
            if (ArmyStrength == 0)
                return 0.0;

            var damage = ArmyRegiments.Sum(ar=>ar.GetDamage());
            
            if (ArmyStrength > combatWidth)
            {
                damage *= (combatWidth / ArmyStrength);
            }

            return damage;
        }

        public double GetPursuitDamage()
        {
            return ArmyRegiments.Sum(ar => ar.GetPursuitDamage());
        }

        public int GetFatalCasualties()
        {
            return (int) ArmyRegiments.Sum(ar => ar.FatalCasualties);
        }

        public void ApplyDamage(double damage)
        {
            var initialArmyStrength = ArmyStrength;
            var newArmyStrength = 0.0;

            foreach (var armyRegiment in ArmyRegiments)
            {
                armyRegiment.ApplyDamage(damage,initialArmyStrength);
                newArmyStrength += armyRegiment.Strength;
            }

            ArmyStrength = newArmyStrength;
        }

        public void ApplyPursuitDamage(double totalEnemyPursuit)
        {
            ArmyStrength = 0;
            var initialRouted = new Dictionary<ArmyRegiment, double>();
            foreach (var armyRegiment in ArmyRegiments)
            {
                armyRegiment.RoutAll();
                initialRouted[armyRegiment] = armyRegiment.RoutedCasualties;
            }

            var dailyDamageFromEnemyPursuit = (totalEnemyPursuit * BattleSimulationConstants.PursuitEfficiency) / BattleSimulationConstants.PursuitDays;
            
            for (int i = 0; i < BattleSimulationConstants.PursuitDays; i++)
            {
                var totalScreen = 0.0;
                var totalRouted = 0.0;
                var totalToughness = 0.0;

                foreach (var armyRegiment in ArmyRegiments)
                {
                    totalScreen += armyRegiment.RoutedCasualties * armyRegiment.Regiment.Screen;
                    totalToughness += armyRegiment.RoutedCasualties * armyRegiment.Regiment.Toughness;
                    totalRouted += armyRegiment.RoutedCasualties;
                }

                totalScreen /= BattleSimulationConstants.PursuitDays;

                if(totalRouted == 0)
                    return;

                var totalPassiveDamage = (BattleSimulationConstants.LeftBehindRatio * totalToughness) / BattleSimulationConstants.PursuitDays;

                var totalDamage = dailyDamageFromEnemyPursuit + totalPassiveDamage;

                foreach (var armyRegiment in ArmyRegiments)
                {
                    var share = initialRouted[armyRegiment] / totalRouted;
                    var damage = Math.Max(totalDamage * share - totalScreen * share,0);
                    
                    armyRegiment.ApplyPursuitDamage(damage);
                }
            }
        }

        public void ApplyPostBattleRounding()
        {
            ArmyStrength = 0;
            foreach (var armyRegiment in ArmyRegiments)
            {
                armyRegiment.ApplyPostBattleRounding();
                ArmyStrength += armyRegiment.Strength;
            }

            ArmyStrength = Math.Round(ArmyStrength);
        }

        public override string ToString()
        {
            return $"{Name} : {ArmyStrength:N0}";
        }

        public IEnumerator<ArmyRegiment> GetEnumerator()
        {
            return ArmyRegiments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
