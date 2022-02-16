using CK3.Utils.BattleSimulator.Data;

namespace CK3.Utils.BattleSimulator.Ranking
{
    public class RegimentRanking
    {
        public Regiment Regiment { get; set; }

        public int WinRank { get; set; }
        public int WinRating { get; set; }

        public int DamageRating { get; set; }
        public int DamageRank { get; set; }

        public override string ToString()
        {
            return $"[{Regiment.GetRegimentTypeCode()}] {Regiment,-18}[D:{Regiment.Damage,-4} T:{Regiment.Toughness,-3} P:{Regiment.Pursuit,-3} S:{Regiment.Screen,-3}] Win: {WinRating} [#{WinRank}]  Damage: {DamageRating} [#{DamageRank}]";
        }

        public string ToMaintenanceString()
        {
            return $"[{Regiment.GetRegimentTypeCode()}] {Regiment,-18}[D:{Regiment.Damage,-4} T:{Regiment.Toughness,-3} P:{Regiment.Pursuit,-3} S:{Regiment.Screen,-3}] [Maint: {Regiment.HighMaintenanceCost:N1}g] Total High Maintenance to win: {WinRating} [#{WinRank}]";
        }
    }
}