using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using org.mariuszgromada.math.mxparser;
using Pdoxcl2Sharp;

namespace CK3.Utils.BattleSimulator.DataExtraction
{
    internal class RegimentCostFileParser : IParadoxRead
    {
        public Dictionary<string, double> Variables { get; }
        bool complete = false;

        public RegimentCostFileParser(Dictionary<string, double> variables)
        {
            Variables = variables;
        }
        public void TokenCallback(ParadoxParser parser, string token)
        {
            token = token.Replace(@"ï»¿", "");
            if (token == "culture_ai_weight_skirmishers")
                complete = true;
            if (complete || string.IsNullOrWhiteSpace(token) || token.StartsWith("#"))
                return;

            var variableName = token.Replace("@", "");
            double variableValue;
            var valueToken = parser.ReadString();

            if (valueToken.StartsWith("@"))
            {
                var tokenList = new List<string>();

                var expressionBuilder = new StringBuilder();

                tokenList.Add(CleanToken(valueToken));
                expressionBuilder.Append(CleanToken(valueToken));
                while (!valueToken.Contains("]"))
                {
                    valueToken = parser.ReadString();

                    var expressionToken = CleanToken(valueToken);
                    tokenList.Add(expressionToken);
                    expressionBuilder.Append(expressionToken);
                }

                var exp = new Expression(expressionBuilder.ToString());
                Variables[variableName] = exp.calculate();
            }
            else
            {
                variableValue = double.Parse(valueToken);
                Variables[variableName] = variableValue;
            }
        }

        static readonly char[] SpecialChars = new char[] { '@', '[', ']' };


        string CleanToken(string token)
        {
            var sb = new StringBuilder();
            foreach (var c in token)
            {
                if (!SpecialChars.Contains(c))
                    sb.Append(c);
            }
            var expressionToken =  sb.ToString();

            if (Variables.TryGetValue(expressionToken, out var val))
            {
                expressionToken = val.ToString(CultureInfo.InvariantCulture);
            }

            return expressionToken;
        }
    }
}
