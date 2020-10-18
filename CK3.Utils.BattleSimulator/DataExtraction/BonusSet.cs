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
    }
}
