using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CK3.Utils.BattleSimulator.DataExtraction
{
    public class Bonuses
    {
        [JsonProperty("end_game")]
        public BonusSet EndGame { get; set; }
    }
}
