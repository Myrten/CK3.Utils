using CK3.Utils.BattleSimulator.Data;

namespace CK3.Utils.BattleSimulator.Simulation
{
    public class SimulationResult
    {
        public Regiment Regiment { get; set; }
        public int Value { get; set; }

        public int Lost { get; set; }

        public int Killed { get; set; }

        public override string ToString()
        {
            
            return $"{Regiment,-18}[{Regiment.Damage,-4}D {Regiment.Toughness,-3}T] - {Value,-6} stacks of {Regiment.Stack,-3} - {Value*Regiment.Stack,-6} soldiers";
        }
    }
}
