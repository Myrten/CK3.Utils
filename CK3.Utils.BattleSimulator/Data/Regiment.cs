using System;
using CK3.Utils.BattleSimulator.DataExtraction;

namespace CK3.Utils.BattleSimulator.Data
{
    public class Regiment
    {
        public static Regiment Levies { get; } = new Regiment()
        {
            Damage = 10,
            Toughness = 10,
            Name = "levies",
            LocalizedName = "Levies"
        };

        public static Regiment UnarmedLevies { get; } = new Regiment()
        {
            Damage = 0,
            Toughness = 10,
            Name = "levies",
            LocalizedName = "Levies",
            Type = "levies"
        };

        public static Regiment Knights15Prowess { get; } = new Regiment()
        {
            Damage = 100 * 15,
            Toughness = 10 * 15,
            Name = "knights_15_prowess",
            Type = "knights",
            LocalizedName = "Knights 15 Prowess"
        };

        public Regiment GetCloneWithBonuses(BonusSet bonuses)
        {
            var bonusesForType = bonuses.GetBonusesForType(Type);
            var clone = (Regiment) MemberwiseClone();

            clone.Damage += bonusesForType.Damage;
            clone.Damage = (int) (clone.Damage * bonusesForType.DamageMultiplier);

            clone.Toughness += bonusesForType.Toughness;
            clone.Toughness = (int)(clone.Toughness * bonusesForType.ToughnessMultiplier);
            
            clone.Pursuit += bonusesForType.Pursuit;
            clone.Pursuit = (int)(clone.Pursuit * bonusesForType.PursuitMultiplier);

            clone.Screen += bonusesForType.Screen;
            clone.Screen = (int)(clone.Screen * bonusesForType.ScreenMultiplier);

            return clone;
        }

        public string Name { get; set; }
        public string LocalizedName { get; set; }
        public string Type { get; set; }
        public int Damage { get; set; }
        public int Toughness { get; set; }
        public int Pursuit { get; set; }
        public int Screen { get; set; }
        public int Stack { get; set; } = 1;

        //not yet implemented
        public double BuyCost { get; set; }
        public double LowMaintenanceCost { get; set; }
        public double HighMaintenanceCost { get; set; }
        public override string ToString()
        {
            return LocalizedName ?? Name;
        }

        public string GetRegimentTypeCode()
        {
            switch (Type)
            {
                case "heavy_infantry":
                    return "HI";
                case "pikemen":
                    return "SP";
                case "archers":
                    return "AR";
                case "skirmishers":
                    return "SK";
                case "heavy_cavalry":
                    return "HC";
                case "light_cavalry":
                    return "LC";
                case "elephant_cavalry":
                    return "EC";
                case "camel_cavalry":
                    return "CC";
                case "archer_cavalry":
                    return "AC";
                case "levies":
                    return "LV";
                case "knights":
                    return "KN";
                default:
                    return "??";
            }
        }
    }
}
