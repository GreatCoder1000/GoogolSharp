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
using QuadrupleLib;
using QuadrupleLib.Accelerators;
using Float128 = QuadrupleLib.Float128<QuadrupleLib.Accelerators.DefaultAccelerator>;
using System.Globalization;
using System.Numerics;

namespace GoogolSharp
{
    partial struct Arithmonym
    {
        private static readonly string[] AllowedE = ["E", "e", "10^"];
        private static readonly string[] AllowedF = ["F", "f", "10^^"];

        public static Arithmonym Parse(string? s, NumberStyles styles, IFormatProvider? provider)
        {
            ArgumentNullException.ThrowIfNull(s);

            // Direct Float128 parse
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
                                 .Replace("(", "")
                                 .Replace(")", "");

            string[] parts = normalized.Split(['e', 'E']);

            if (parts.Length == 2)
            {
                Float128 significand = Float128.Parse(parts[0]);
                Float128 exponent = Float128.Parse(parts[1]);

                bool reciprocal = false;

                // Convert to "F‑notation" internal representation
                Float128 letterF = exponent + Float128PreciseTranscendentals.SafeLog10(significand);

                if (letterF < 0)
                {
                    letterF = -letterF;
                    reciprocal = true;
                }

                if (letterF < 10)
                {
                    result = new Arithmonym(letterF)._Exp10;
                    return true;
                }

                letterF = 1 + Float128HyperTranscendentals.SuperLog10(letterF);

                result = new Arithmonym(false, reciprocal, 0x06, EncodeOperand(letterF));
                return true;
            }

            result = default;
            return false;
        }
    }
}
