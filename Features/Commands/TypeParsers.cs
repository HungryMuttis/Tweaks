using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Zorro.Core.CLI;

namespace Tweaks.Features.Commands
{
    [TypeParser(typeof(Vector3))]
    public class TypeParsers : CLITypeParser
    {
        private const NumberStyles Style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands;
        private static readonly SortedDictionary<string, Vector3> Constants = new()
        {
            {"Back", Vector3.back},
            {"Down", Vector3.down},
            {"Forward", Vector3.forward},
            {"Left", Vector3.left},
            {"One", Vector3.one},
            {"Right", Vector3.right},
            {"Up", Vector3.up},
            {"Zero", Vector3.zero}
        };

        public override List<ParameterAutocomplete> FindAutocomplete(string parameterText)
        {
            List<ParameterAutocomplete> autocompletes = parameterText == "" ? [new("0")] : [];
            foreach (KeyValuePair<string, Vector3> pair in Constants)
                if (pair.Key.StartsWith(parameterText)) autocompletes.Add(new(pair.Key));
                else if (string.Compare(pair.Key, parameterText) > 0) break;

            if (autocompletes.Count > 0) return autocompletes;

            parameterText = parameterText.Replace(',', '.');
            string? str = GetAutocomplete(parameterText);
            if (str == null) return [];
            else return [new(str)];
        }

        public override object Parse(string str)
        {
            if (Constants.TryGetValue(str, out Vector3 vector)) return vector;

            str = str.Replace(",", ".");
            string[] parts = str.Split(';');
            if (parts.Length != 3) throw new ArgumentException("Wrong number of axies", nameof(str));
            float[] parsed = new float[3];
            for (int i = 0; i < 3; i++)
                if (!float.TryParse(parts[i], Style, CultureInfo.InvariantCulture, out parsed[i])) throw new ArgumentException("Cannot parse floats", nameof(str));
            return new Vector3(parsed[0], parsed[1], parsed[2]);
        }

        // HELPER METHODS //
        private static string? GetAutocomplete(string parameterText)
        {
            string[] parts = parameterText.Split(';');
            bool prevb = false;
            if (parts.Length > 3 || parts.Any(s =>
            {
                if (prevb) return true;
                if (s == "" || s == "-")
                {
                    prevb = true;
                    return false;
                }
                return !float.TryParse(s, Style, CultureInfo.InvariantCulture, out _);
            })) return null;

            if (parts.Last().Length == 0 || parts.Last() == "-") return new(parameterText + '0');
            else if (parts.Length != 3) return new(parameterText + ';');
            else return new(parameterText);
        }
    }
}
