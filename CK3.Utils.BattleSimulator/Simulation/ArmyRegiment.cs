using System;
using CK3.Utils.BattleSimulator.Data;

namespace CK3.Utils.BattleSimulator.Simulation
{
    public class ArmyRegiment
    {
        public Regiment Regiment { get; }
        public int InitialStrength { get; }

        public double Strength { get; set; }

        public double RoutedCasualties { get; set; }

        public double FatalCasualties { get; set; }

        public ArmyRegiment(Regiment regiment, int initialStrength)
        {
            Regiment = regiment;
            Strength = InitialStrength = initialStrength;
        }

        public override string ToString()
        {
            return Regiment.ToString();
        }

        public double GetDamage()
        { 
            return Strength * Regiment.Damage * BattleSimulationConstants.DamageMultiplier;
        }

        public double GetPursuitDamage()
        {
            return Strength * Regiment.Pursuit;
        }

        public void ApplyDamage(double armyDamage, double armyStrength)
        {
            var regimentDamage =  armyDamage * (Strength / armyStrength);
            var regimentCasualties = Math.Min(regimentDamage / Regiment.Toughness, Strength);
            Strength -= regimentCasualties;
            var fatal = regimentCasualties * BattleSimulationConstants.FatalCasualtiesRatio;
            FatalCasualties += fatal;
            RoutedCasualties += (regimentCasualties - fatal);
        }

        public void Wipe()
        {
            FatalCasualties += Strength;
            FatalCasualties += RoutedCasualties;
            RoutedCasualties = 0;
            Strength = 0;
        }

        public void RoutAll()
        {
            RoutedCasualties += Strength;
            Strength = 0;
        }

        public void ApplyPursuitDamage(double damage)
        {
            var kills = Math.Min(Math.Max(damage / Regiment.Toughness, RoutedCasualties * BattleSimulationConstants.MinimunDailyPursuitLosses),RoutedCasualties);
            RoutedCasualties -= kills;
            FatalCasualties += kills;
        }

        public void ApplyPostBattleRounding()
        {
            Strength = Math.Round(Strength);
            RoutedCasualties = Math.Round(RoutedCasualties);
            FatalCasualties = Math.Round(FatalCasualties);
        }
    }
}
