using System.Diagnostics.Tracing;

namespace CK3.Utils.BattleSimulator.Simulation
{
    public class BattleSimulator
    {
        const double AliveThreshold = 0.01;

        /// <summary>
        /// Simulates battle between two armies
        /// </summary>
        /// <param name="a">First army</param>
        /// <param name="b">Second army</param>
        /// <returns>Army that won</returns>
        public BattleResult SimulateBattle(Army a, Army b)
        {
            var combatWidth = (a.ArmyStrength + b.ArmyStrength) / 2;
            var days = 1;
            while (a.ArmyStrength > AliveThreshold && b.ArmyStrength > AliveThreshold)
            {
                
                var aDamage = a.GetDamage(combatWidth);
                var bDamage = b.GetDamage(combatWidth);

                a.ApplyDamage(bDamage);
                b.ApplyDamage(aDamage);
                days++;
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

            if (days < BattleSimulationConstants.EarlyPhaseDays)
                loser.Wipe();
            else
                loser.ApplyPursuitDamage(winner.GetPursuitDamage());


            a.ApplyPostBattleRounding();
            b.ApplyPostBattleRounding();

            return new BattleResult()
            {
                Days = days,
                Winner = winner,
                Loser = loser
            };
        }
    }
}