namespace CK3.Utils.BattleSimulator.Simulation
{
    public class BattleSimulator
    {
        const double DamageMultiplier = 0.03;

        /// <summary>
        /// Simulates battle between two armies
        /// </summary>
        /// <param name="a">First army</param>
        /// <param name="b">Second army</param>
        /// <returns>Army that won</returns>
        public Army SimulateBattle(Army a, Army b)
        {

            //Console.WriteLine($"{a} | {b}");
            while (a.ArmyStrength > 0 && b.ArmyStrength > 0)
            {
                var combatWidth = (a.ArmyStrength + b.ArmyStrength) / 2;
                var aDamage = a.GetDamage(combatWidth);
                var bDamage = b.GetDamage(combatWidth);

                a.ApplyDamageAndGetlosses(bDamage);
                b.ApplyDamageAndGetlosses(aDamage);

                //Console.WriteLine($"{a} | {b}");
            }

            if (a.ArmyStrength > 0)
                return a;
            return b;
        }
    }
}
