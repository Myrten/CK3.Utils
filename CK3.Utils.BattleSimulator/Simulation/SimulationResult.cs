using System;
using CK3.Utils.BattleSimulator.Data;

namespace CK3.Utils.BattleSimulator.Simulation
{
    public class SimulationResult
    {
        public Regiment Regiment { get; set; }
        public int RegimentCount { get; set; }

        public int Lost { get; set; }

        public int Killed { get; set; }

        public int Days { get; set; }

        public bool Won { get; set; }
        public override string ToString()
        {
            
            return $"[{Regiment.GetRegimentTypeCode()}] {Regiment,-18}[D:{Regiment.Damage,-4} T:{Regiment.Toughness,-3} P:{Regiment.Pursuit,-3} S:{Regiment.Screen,-3}] - {RegimentCount,-6} stacks of {Regiment.Stack,-3} - {RegimentCount*Regiment.Stack,-6} soldiers";
        }
       
    }
}
