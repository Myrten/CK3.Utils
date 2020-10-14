using CK3.Utils.BattleSimulator.Data;

namespace CK3.Utils.BattleSimulator.Simulation
{
    public class SimulationResult
    {
        public Regiment Regiment { get; set; }
        public int Value { get; set; }

        public override string ToString()
        {
            return $"{Regiment} - {Value} stacks of {Regiment.Stack} - {Value*Regiment.Stack} Total";
        }
    }
}
