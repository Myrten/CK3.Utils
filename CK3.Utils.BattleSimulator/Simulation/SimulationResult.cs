using CK3.Utils.BattleSimulator.Data;

namespace CK3.Utils.BattleSimulator.Simulation
{
    public class SimulationResult
    {
        public Regiment Regiment { get; set; }
        public int Count { get; set; }

        public int Lost { get; set; }

        public int Killed { get; set; }

        public bool Won { get; set; }
        public override string ToString()
        {
            
            return $"{Regiment,-18}[{Regiment.Damage,-4}D {Regiment.Toughness,-3}T {Regiment.Pursuit,-3}P {Regiment.Screen,-3}S] - {Count,-6} stacks of {Regiment.Stack,-3} - {Count*Regiment.Stack,-6} soldiers";
        }
    }
}
