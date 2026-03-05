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
        /// <summary>
        /// Returns a human-readable string representation of this <see cref="Arithmonym"/>.
        /// </summary>
        public override string ToString()
        {
            if (IsNaN(this)) return "NaN";
            if (this == PositiveInfinity) return "∞";
            if (this == NegativeInfinity) return "-∞";
            if (this == Zero) return "0";

            // Reconstruct operand in [2, 10)
            Float128 value = Operand;

            string output = "";
            if (_IsNegative)
                output += "-";

            string[] letters = ["", "A", "B", "C", "D", "E", "F", "J", "K", "L", "M", "N", "P"];
            while (letters.Length < 63)
                letters = [.. letters, $"[{letters.Length}]"];

            switch (Letter)
            {
                case 0x01:
                    output += _IsReciprocal
                        ? 1 / (1 + ((value - 2) / 8))
                        : 1 + ((value - 2) / 8);
                    break;
                case 0x02:
                    output += _IsReciprocal
                        ? 1 / (2 + ((value - 2) / 4))
                        : 2 + ((value - 2) / 4);
                    break;
                case 0x03:
                    output += _IsReciprocal
                        ? 1 / (value * 2)
                        : value * 2;
                    break;
                case 0x04:
                    output += _IsReciprocal
                        ? 1 / (value * 10)
                        : value * 10;
                    break;
                case 0x05:
                    output += _IsReciprocal
                        ? Float128PreciseTranscendentals.SafeExp10(-value) : Float128PreciseTranscendentals.SafeExp10(value);
                    break;
                case 0x06:
                    output += ArithmonymFormattingUtils.FormatArithmonymFromLetterF(Operand, _IsReciprocal);
                    break;
                default:
                    if (_IsReciprocal)
                        output += "1 / ";
                    output += letters[Letter];
                    output += value;
                    break;
            }
            return output;
        }
    }
}