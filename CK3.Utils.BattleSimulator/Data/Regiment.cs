namespace CK3.Utils.BattleSimulator.Data
{
    public class Regiment
    {
        public static Regiment Levies { get; } = new Regiment()
        {
            Damage = 10,
            Toughness = 10,
            Name = "levies",
            LocalizedName = "Levies"
        };

        public static Regiment Knights15Prowess { get; } = new Regiment()
        {
            Damage = 100 * 15,
            Toughness = 10 * 15,
            Name = "knights_15_prowess",
            LocalizedName = "Knights 15 Prowess"
        };
        public string Name { get; set; }
        public string LocalizedName { get; set; }
        public string Type { get; set; }
        public int Damage { get; set; }
        public int Toughness { get; set; }
        public int Pursuit { get; set; }
        public int Screen { get; set; }
        public int Stack { get; set; } = 1;

        //not yet implemented
        public double BuyCost { get; set; }
        public double LowMaintenanceCost { get; set; }
        public double HighMaintenanceCost { get; set; }
        public override string ToString()
        {
            return LocalizedName ?? Name;
        }
    }
}
