using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CK3.Utils.BattleSimulator.Data;

namespace CK3.Utils.BattleSimulator.Simulation
{
    public class Army :IEnumerable<KeyValuePair<Regiment,double>>
    {
        public string Name { get;  }
        public Army(string name)
        {
            Name = name;
        }

        public void Add(Regiment regiment, int value)
        {
            InitialStrength += value;
            ArmyStrength += value;
            Regiments.Add(regiment, value);
        }

        public Dictionary<Regiment, double> Regiments { get; set; } = new Dictionary<Regiment, double>();

        public double InitialStrength
        {
            get;
            private set;
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

            var damage = 0.0;
            foreach (var regiment in Regiments)
            {
                damage += regiment.Value * regiment.Key.Damage * BattleSimulationConstants.DamageMultiplier;
            }

            
            if (ArmyStrength > combatWidth)
            {
                damage *= (combatWidth / ArmyStrength);
            }

            return damage;
        }


        public double ApplyDamageAndGetLosses(double damage)
        {
            var initialArmyStrength = ArmyStrength;
            
            foreach (var unit in Regiments.ToArray())
            {
                var unitShare = unit.Value / initialArmyStrength;
                var unitDamage =  damage * unitShare;
                var unitLosses = unitDamage / unit.Key.Toughness;
                var newUnitStrength = Math.Max(0, unit.Value - unitLosses);
                ArmyStrength = Math.Max(0, ArmyStrength - (unit.Value-newUnitStrength));
                Regiments[unit.Key] = newUnitStrength;
            }

            return initialArmyStrength - ArmyStrength;
        }

        public override string ToString()
        {
            return $"{Name} : {ArmyStrength:N0}";
        }

        public IEnumerator<KeyValuePair<Regiment, double>> GetEnumerator()
        {
            return Regiments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
