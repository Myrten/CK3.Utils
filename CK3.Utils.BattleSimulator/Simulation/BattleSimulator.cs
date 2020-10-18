namespace CK3.Utils.BattleSimulator.Simulation
{
    public class BattleSimulator
    {
        const double AliveThreshold = 0.1;

        /// <summary>
        /// Simulates battle between two armies
        /// </summary>
        /// <param name="a">First army</param>
        /// <param name="b">Second army</param>
        /// <returns>Army that won</returns>
        public Army SimulateBattle(Army a, Army b)
        {
            var combatWidth = (a.ArmyStrength + b.ArmyStrength) / 2;
            while (a.ArmyStrength > AliveThreshold && b.ArmyStrength > AliveThreshold)
            {
                
                var aDamage = a.GetDamage(combatWidth);
                var bDamage = b.GetDamage(combatWidth);

                a.ApplyDamage(bDamage);
                b.ApplyDamage(aDamage);
            }

            Army winner, loser;

            if (a.ArmyStrength > AliveThreshold)
            {
                winner = a;
                loser = b;
            }
            else
            {
                winner = b;
                loser = a;
            }
            
            loser.ApplyPursuitDamage(winner.GetPursuitDamage());

            a.ApplyPostBattleRounding();
            b.ApplyPostBattleRounding();

            return a.ArmyStrength > AliveThreshold ? a : b;
        }
    }
}