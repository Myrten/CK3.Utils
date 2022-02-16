using System.Collections.Generic;
using System.Security.Cryptography;
using CK3.Utils.BattleSimulator.Data;
using Pdoxcl2Sharp;

namespace CK3.Utils.BattleSimulator.DataExtraction
{
    public class RegimentFileParser :  IParadoxRead
    {
        public Dictionary<string, double> Variables { get; }
        readonly RegimentLocalization regimentLocalization;

        public RegimentFileParser(Dictionary<string,double> variables,  RegimentLocalization regimentLocalization)
        {
            Variables = variables;
            this.regimentLocalization = regimentLocalization;
        }
        public List<Regiment> Regiments { get; set; } = new List<Regiment>();
        public void TokenCallback(ParadoxParser parser, string token)
        {
            token = token.Replace(@"ï»¿", "");

            if (string.IsNullOrWhiteSpace(token))
                return;

            if (token.StartsWith("@"))
            {
                var variableName = token.Substring(1);
                var value = parser.ReadDouble();
                Variables[variableName] = value;
            }
            else if (token.StartsWith("#"))
            {

            }
            else
            {
                var regiment = new Regiment()
                {
                    Name = token
                };
                Regiments.Add(regiment);
                parser.ReadInsideBrackets(p =>
                {
                    switch (parser.ReadString())
                    {
                        case "type":
                            regiment.Type = parser.ReadString();
                            break;
                        case "damage":
                            regiment.Damage = parser.ReadInt32();
                            break;
                        case "toughness":
                            regiment.Toughness = parser.ReadInt32();
                            break;
                        case "pursuit":
                            regiment.Pursuit = parser.ReadInt32();
                            break;
                        case "screen":
                            regiment.Screen = parser.ReadInt32();
                            break;
                        case "terrain_bonus":
                            parser.ReadInsideBrackets(p2=>{});
                            break;
                        case "winter_bonus":
                            parser.ReadInsideBrackets(p2 => { });
                            break;
                        case "era_bonus":
                            regiment.EraBonuses = new Dictionary<string, EraBonus>();
                            parser.ReadInsideBrackets(eraParser =>
                            {
                                var bonus = new EraBonus();
                                bonus.Era = eraParser.ReadString();
                                eraParser.ReadInsideBrackets(p =>
                                {
                                    switch (p.ReadString())
                                    {
                                        case "damage":
                                            bonus.Damage = p.ReadInt32();
                                            break;
                                        case "toughness":
                                            bonus.Toughness = p.ReadInt32();
                                            break;
                                        case "pursuit":
                                            bonus.Pursuit = p.ReadInt32();
                                            break;
                                        case "screen":
                                            bonus.Screen = p.ReadInt32();
                                            break;
                                        default:
                                            break;
                                    }
                                });
                                regiment.EraBonuses[bonus.Era] = bonus;
                            });
                            break;
                        case "stack":
                            regiment.Stack = parser.ReadInt32();
                            break;
                        case "buy_cost":
                            regiment.BuyCost = ReadCost(p);
                            break;
                        case "low_maintenance_cost":
                            regiment.LowMaintenanceCost = ReadCost(p);
                            break;
                        case "high_maintenance_cost":
                            regiment.HighMaintenanceCost = ReadCost(p);
                            break;
                        default:
                            break;
                    }

                    if (regimentLocalization.Localizations.TryGetValue(regiment.Name, out string localizedName))
                        regiment.LocalizedName = localizedName;

                });
            }
        }

        // doesn't work
        double ReadCost(ParadoxParser parser)
        {
            var val = 0.0;
            parser.ReadInsideBrackets(p =>
            {
                if (p.ReadString() == "gold")
                {
                    var valString = p.ReadString();
                    if (!double.TryParse(valString, out val))
                    {
                        Variables.TryGetValue(valString, out val);
                    }
                }
            });
            return val;
        }
    }
}
