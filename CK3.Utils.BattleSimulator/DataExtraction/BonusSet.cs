using System;
using System.Collections.Generic;
using System.Text;
using CK3.Utils.BattleSimulator.Data;
using Newtonsoft.Json;

namespace CK3.Utils.BattleSimulator.DataExtraction
{
    public class BonusSet
    {
        [JsonProperty("heavy_infantry")]
        public RegimentBonuses HeavyInfantry { get; set; }

        [JsonProperty("pikemen")]
        public RegimentBonuses Pikeman { get; set; }
        
        [JsonProperty("archers")]
        public RegimentBonuses Archers { get; set; }
        
        [JsonProperty("skirmishers")]
        public RegimentBonuses Skirmishers { get; set; }
        
        [JsonProperty("heavy_cavalry")]
        public RegimentBonuses HeavyCavalry { get; set; }

        [JsonProperty("light_cavalry")]
        public RegimentBonuses LightCavalry { get; set; }
        
        [JsonProperty("archer_cavalry")]
        public RegimentBonuses ArcherCavalry { get; set; }
        
        [JsonProperty("elephant_cavalry")]
        public RegimentBonuses ElephantCavalry { get; set; }
        
        [JsonProperty("camel_cavalry")]
        public RegimentBonuses CamelCavalry { get; set; }

        public RegimentBonuses GetBonusesForType(string type)
        {
            switch (type)
            {
                case "heavy_infantry":
                    return HeavyInfantry;

                case "pikemen":
                    return Pikeman;
                case "archers":
                    return Archers;
                case "skirmishers":
                    return Skirmishers;
                case "heavy_cavalry":
                    return HeavyCavalry;
                case "light_cavalry":
                    return LightCavalry;
                case "archer_cavalry":
                    return ArcherCavalry;
                case "elephant_cavalry":
                    return ElephantCavalry;
                case "camel_cavalry":
                    return CamelCavalry;
                default:
                    throw new ArgumentOutOfRangeException(type);
            }
        }
    }
}
