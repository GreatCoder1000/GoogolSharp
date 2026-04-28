/*
 *  Copyright 2025 @GreatCoder1000
 *  This file is part of GoogolSharp.
 *
 *  GoogolSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  GoogolSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with GoogolSharp.  If not, see <https://www.gnu.org/licenses/>.
 */

using GoogolSharp.Helpers;
using Float128 = QuadrupleLib.Float128<QuadrupleLib.Accelerators.DefaultAccelerator>;
using System.Globalization;

namespace GoogolSharp
{
    partial struct Arithmonym
    {
        private static readonly string[] AllowedE = ["E", "e", "10^"];
        private static readonly string[] AllowedF = ["F", "f", "10^^"];

        public static Arithmonym Parse(string? s, NumberStyles styles, IFormatProvider? provider)
        {
            ArgumentNullException.ThrowIfNull(s);

            // Direct Float128 parse ONLY IF NO SCIENTIFIC
            // Old bug: eeee5e12345 -> 5 in Float128 parsing! Causing a big bug.
            if (!s.Contains("10^") && !s.Contains('e') && !s.Contains('E') && !s.Contains('F'))
                if (Float128.TryParse(s, null, out Float128 floatValue))
                    return new Arithmonym(floatValue);

            // Leading sign handling
            if (s.StartsWith('-'))
                return Parse(s[1..], styles, provider).Negated;

            if (s.StartsWith('+'))
                return Parse(s[1..], styles, provider);

            // Scientific notation: a*10^b or aEb
            if (TryParseScientific(s, out Arithmonym sci))
                return sci;

            // E-prefix (10^x)
            if (TryStripPrefix(s, AllowedE, out string? eRest))
                return Parse(eRest, styles, provider)._Exp10;

            // F-prefix (10^^x)
            if (TryStripPrefix(s, AllowedF, out string? fRest))
                return Tetration10Linear(Parse(fRest, styles, provider));

            // Nothing matched
            throw new FormatException("Input string was not in a correct format.");
        }

        // -----------------------
        // Helpers
        // -----------------------

        private static bool TryStripPrefix(string s, string[] prefixes, out string? remainder)
        {
            foreach (var prefix in prefixes)
            {
                if (s.StartsWith(prefix))
                {
                    remainder = s[prefix.Length..];
                    return true;
                }
            }

            remainder = null;
            return false;
        }

        private static bool TryParseScientific(string s, out Arithmonym result)
        {
            // Normalize: replace *10^ with e, remove parentheses
            string normalized = s.Replace("*10^", "e")
                                 .Replace("10^", "e")
                                 .Replace("(", "")
                                 .Replace(")", "");

            string[] parts = normalized.ToLower().Split('e');

            if (parts.Length == 2)
            {
                Float128 significand = Float128.Parse(parts[0]);
                Float128 exponent = Float128.Parse(parts[1]);

                bool reciprocal = false;

                // Right now letterF is misleading, it is actually storing letter E!
                Float128 letterE = exponent + Float128PreciseTranscendentals.SafeLog10(significand);

                if (letterE < 0)
                {
                    letterE = -letterE;
                    reciprocal = true;
                }

                if (letterE < 10)
                {
                    result = new Arithmonym(letterE)._Exp10;
                    return true;
                }

                // and NOW we calculate letter F.

                // We do a +1 because letterE = log(actual value)
                Float128 letterF = 1 + Float128HyperTranscendentals.SuperLog10(letterE);

                // No Exp10 needed here. Because we already did it in letterF
                result = new Arithmonym(false, reciprocal, LETTERCODE_F, EncodeOperand(letterF));
                return true;
            }

            result = default;
            return false;
        }
    }
}
