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

namespace GoogolSharp
{
    partial struct Arithmonym
    {

        /// <summary>
        /// Returns a string representation of the current <see cref="Arithmonym"/>,
        /// formatted according to <paramref name="format"/> and <paramref name="provider"/> if provided.
        /// </summary>
        /// <param name="format">A format string (currently unused); may be <c>null</c>.</param>
        /// <param name="provider">An optional format provider that supplies culture-specific formatting information.</param>
        /// <returns>A formatted string representation of this <see cref="Arithmonym"/>.</returns>
        public string ToString(string? format, IFormatProvider? provider)
        {
            if (format is null) return ToString();
            if (format == "B") return ToBinaryString(squishedHi, 32) + ToBinaryString(squishedMid, 32) + ToBinaryString(squishedLo, 32);
            if (format == "L") return ToLetterString();
            return ToString();
        }

        // Converts an integer to a binary string with optional fixed width
        private static string ToBinaryString(uint number, int bitWidth = 0)
        {
            // Convert to binary without leading zeros
            string binary = Convert.ToString(number, 2);

            // If a fixed width is specified, pad with leading zeros
            if (bitWidth > 0)
            {
                // Ensure bitWidth is reasonable (1 to 64 for int)
                if (bitWidth < 1 || bitWidth > 64)
                    throw new ArgumentOutOfRangeException(nameof(bitWidth), "Bit width must be between 1 and 64.");

                binary = binary.PadLeft(bitWidth, '0');
            }

            return binary;
        }

        /// <summary>
        /// Returns a human-readable string representation of this <see cref="Arithmonym"/>.
        /// </summary>
        public override string ToString() => ToCommonString();

        /// <summary>
        /// Returns a human-readable string representation of this <see cref="Arithmonym"/>.
        /// </summary>
        public string ToLetterString()
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

        /// <summary>
        /// Returns a human-readable string representation of this <see cref="Arithmonym"/>.
        /// </summary>
        public string ToCommonString()
        {
            if (IsNaN(this)) return "NaN";
            if (this == PositiveInfinity) return "∞";
            if (this == NegativeInfinity) return "-∞";
            if (this == Zero) return "0";

            // Reconstruct operand in [2, 10)
            Float128 value = Operand;

            if (Letter == 0x0C) value += 2;

            string output = "";
            if (_IsNegative)
                output += "-";

            string[] prefixes = ["", "A", "B", "C", "D", "10^", "10^^", "{10,10,", "{10,", "{10,", "{10,10,", "{10,10,10,", "{10,", "X^^", "X^^^", "{X,"];
            while (prefixes.Length < 63)
                prefixes = [.. prefixes, $"[{prefixes.Length}]"];
            string[] suffixes = ["", "", "", "", "", "", "", "}", ",1,2}", ",2,2}", ",2}", "}", "(1)2}", " & 10", " & 10"];
            while (suffixes.Length < 63)
                suffixes = [.. suffixes, $"[{suffixes.Length}]"];

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
                    {
                        Float128 result = _IsReciprocal
                            ? 1 / (value * 2)
                            : value * 2;
                        output += ArithmonymFormattingUtils.FormatNearInteger(result);
                        break;
                    }
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
                    output += ArithmonymFormattingUtils.FormatArithmonymFromLetterF(Operand, _IsReciprocal, "*10^", false);
                    break;
                default:
                    if (_IsReciprocal)
                        output += "1 / ";
                    output += prefixes[Letter];
                    output += value;
                    output += suffixes[Letter];
                    break;
            }
            return output;
        }
    }
}