using System;
using System.Collections.Generic;
using System.Text;

namespace CK3.Utils.BattleSimulator.DataExtraction
{
    public record EraBonus
    {
        public string Era { get; set; }
        public int Damage { get; set; }
        public int Toughness { get; set; }
        public int Screen { get; set; }
        public int Pursuit { get; set; }
    }
}
