using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK3.Utils.BattleSimulator.Simulation
{
    internal class MultiRegimentSimulationResult : SimulationResult
    {
        public Army Army { get; set; }
        public override string ToString()
        {
            var typeCode = string.Join('+',Army.ArmyRegiments.Select(r => r.Regiment.GetRegimentTypeCode()));
            var name = string.Join('/', Army.ArmyRegiments.Select(r => r.Regiment.ToString()));
            var stacks = Army.ArmyRegiments.Sum(r => r.Regiment.Stack);

            return $"[{typeCode}] {name,-36} - {RegimentCount,-6} stacks of {stacks,-3} - {RegimentCount * stacks,-6} soldiers";

        }
    }
}
