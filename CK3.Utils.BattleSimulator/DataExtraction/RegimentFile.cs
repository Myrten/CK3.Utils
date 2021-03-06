﻿using System.Collections.Generic;
using CK3.Utils.BattleSimulator.Data;
using Pdoxcl2Sharp;

namespace CK3.Utils.BattleSimulator.DataExtraction
{
    public class RegimentFile :  IParadoxRead
    {
        readonly RegimentLocalization regimentLocalization;

        public RegimentFile(RegimentLocalization regimentLocalization)
        {
            this.regimentLocalization = regimentLocalization;
        }
        public List<Regiment> Regiments { get; set; } = new List<Regiment>();
        public Dictionary<string, double> Variables { get; set; } = new Dictionary<string, double>();
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
                        case "stack":
                            regiment.Stack = parser.ReadInt32();
                            break;
                        case "buy_cost":
                            regiment.BuyCost = ReadCost(p);
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
