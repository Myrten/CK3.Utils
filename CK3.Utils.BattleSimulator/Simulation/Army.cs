using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CK3.Utils.BattleSimulator.Data;

namespace CK3.Utils.BattleSimulator.Simulation
{
    public class Army :IEnumerable<KeyValuePair<Regiment,int>>
    {
        public string Name { get;  }
        public Army(string name)
        {
            Name = name;
        }

        public void Add(Regiment regiment, int value)
        {
            ArmyStrength += value;
            Troops.Add(regiment, value);
        }

        public Dictionary<Regiment, int> Troops { get; set; } = new Dictionary<Regiment, int>();

        public int ArmyStrength
        {
            get;
            private set;
        }

        public double GetDamage(double combatWidth)
        {
            if (ArmyStrength == 0)
                return 0.0;

            var damage = 0.0;
            foreach (var regiment in Troops)
            {
                damage += regiment.Value * regiment.Key.Damage * BattleSimulationConstants.DamageMultiplier;
            }

            
            if (ArmyStrength > combatWidth)
            {
                damage = damage * (combatWidth / ArmyStrength);
            }

            return damage;
        }


        public double ApplyDamageAndGetlosses(double damage)
        {
            var initialArmyStrength = ArmyStrength;
            
            foreach (var unit in Troops.ToArray())
            {
                var unitShare = unit.Value / initialArmyStrength;
                var unitDamage =  damage * unitShare;
                var losses = unitDamage / unit.Key.Toughness;
                var newStrength = Math.Max(0, unit.Value - losses);
                var newArmyStrength = Math.Max(0, ArmyStrength - losses);

                Troops[unit.Key] = (int) newStrength;
                ArmyStrength = (int) newArmyStrength;
            }

            return initialArmyStrength - ArmyStrength;
        }

        public override string ToString()
        {
            return $"{Name} : {ArmyStrength}";
        }

        public IEnumerator<KeyValuePair<Regiment, int>> GetEnumerator()
        {
            return Troops.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
