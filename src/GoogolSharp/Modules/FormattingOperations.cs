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
using System.Text;

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
            if (format == "A") return ToAbbreviatedString();
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

            Float128 value = Operand;

            var sb = new StringBuilder();
            if (_IsNegative)
                sb.Append('-');

            string[] letters = ["", "A", "B", "C", "D", "E", "F", "J", "K", "L", "M", "N", "P"];
            while (letters.Length < 63)
                letters = [.. letters, $"[{letters.Length}]"];

            // unified special‑letter handler
            string? special = FormatSpecialLetter(Letter, value, _IsReciprocal, commonString: false, abbreviate: false);
            if (special != null)
            {
                sb.Append(special);
                return sb.ToString();
            }

            // default formatting
            if (_IsReciprocal)
                sb.Append("1 / ");

            sb.Append(letters[Letter]);
            sb.Append(value.ToString("R", null));

            return sb.ToString();
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

            Float128 value = Operand;

            if (Letter == 0x0C)
                value += 2;

            var sb = new StringBuilder();
            if (_IsNegative)
                sb.Append('-');

            string[] prefixes = ["", "A", "B", "C", "D", "10^", "10^^", "{10,10,", "{10,", "{10,", "{10,10,", "{10,10,10,", "{10,", "X^^", "X^^^", "{X,", "{X,", "X_2^^", "{10,"];
            while (prefixes.Length < 63)
                prefixes = [.. prefixes, $"[{prefixes.Length}]"];

            string[] suffixes = ["", "", "", "", "", "", "", "}", ",1,2}", ",2,2}", ",2}", "}", "(1)2}", " & 10", " & 10", "(1)2} & 10", ",2(1)2} & 10", " & X & 10", "/2}"];
            while (suffixes.Length < 63)
                suffixes = [.. suffixes, $"[{suffixes.Length}]"];

            // unified special‑letter handler
            string? special = FormatSpecialLetter(Letter, value, _IsReciprocal, commonString: true, abbreviate: false);
            if (special != null)
            {
                sb.Append(special);
                return sb.ToString();
            }

            // default formatting
            if (_IsReciprocal)
                sb.Append("1 / ");

            sb.Append(prefixes[Letter]);
            sb.Append(value.ToString("R", null));
            sb.Append(suffixes[Letter]);

            return sb.ToString();
        }

        /// <summary>
        /// Returns a human-readable string representation of this <see cref="Arithmonym"/>.
        /// </summary>
        public string ToAbbreviatedString()
        {
            if (IsNaN(this)) return "NaN";
            if (this == PositiveInfinity) return "∞";
            if (this == NegativeInfinity) return "-∞";
            if (this == Zero) return "0";

            Float128 value = Operand;

            if (Letter == 0x0C)
                value += 2;

            var sb = new StringBuilder();
            if (_IsNegative)
                sb.Append('-');

            string[] prefixes = ["", "A", "B", "C", "D", "10^", "10^^", "{10,10,", "{10,", "{10,", "{10,10,", "{10,10,10,", "{10,", "X^^", "X^^^", "{X,", "{X,", "X_2^^", "{10,"];
            while (prefixes.Length < 63)
                prefixes = [.. prefixes, $"[{prefixes.Length}]"];

            string[] suffixes = ["", "", "", "", "", "", "", "}", ",1,2}", ",2,2}", ",2}", "}", "(1)2}", " & 10", " & 10", "(1)2} & 10", ",2(1)2} & 10", " & X & 10", "/2}"];
            while (suffixes.Length < 63)
                suffixes = [.. suffixes, $"[{suffixes.Length}]"];

            // unified special‑letter handler
            string? special = FormatSpecialLetter(Letter, value, _IsReciprocal, commonString: true, abbreviate: true);
            if (special != null)
            {
                sb.Append(special);
                return sb.ToString();
            }

            // default formatting
            if (_IsReciprocal)
                sb.Append("1 / ");

            sb.Append(prefixes[Letter]);
            sb.Append(value.ToString("R", null));
            sb.Append(suffixes[Letter]);

            return sb.ToString();
        }


        private static string? FormatSpecialLetter(byte letter, Float128 value, bool isReciprocal, bool commonString, bool abbreviate)
        {
            switch (letter)
            {
                case 0x01:
                    {
                        Float128 t = Float128.FusedMultiplyAdd(value, Float128.One / 8, 1 - (Float128)2 / 8);
                        return (isReciprocal ? (1 / t) : t).ToString("R", null);
                    }

                case 0x02:
                    {
                        Float128 t = Float128.FusedMultiplyAdd(value, Float128.One / 4, 2 - (Float128)2 / 4);
                        return (isReciprocal ? (1 / t) : t).ToString("R", null);
                    }

                case 0x03:
                    {
                        Float128 t = value * 2;
                        if (commonString)
                            return ArithmonymFormattingUtils.FormatNearInteger(isReciprocal ? 1 / t : t);

                        return (isReciprocal ? (1 / t) : t).ToString("R", null);
                    }

                case 0x04:
                    {
                        Float128 t = value * 10;
                        return (isReciprocal ? (1 / t) : t).ToString("R", null);
                    }

                case LETTERCODE_E:
                    {
                        Float128 t = isReciprocal
                            ? Float128PreciseTranscendentals.SafeExp10(-value)
                            : Float128PreciseTranscendentals.SafeExp10(value);

                        if (value >= 3 && abbreviate && !isReciprocal)
                        {
                            return FormatFloat128AbbreviateFromLog10(value);
                        }

                        return t.ToString("R", null);
                    }

                case LETTERCODE_F:
                    return commonString
                        ? ArithmonymFormattingUtils.FormatArithmonymFromLetterF(value, isReciprocal, "*10^", false, abbreviate)
                        : ArithmonymFormattingUtils.FormatArithmonymFromLetterF(value, isReciprocal, "E", true, abbreviate);

                case LETTERCODE_J:
                    {
                        if (value < 3)
                        {
                            Float128 letterG = Float128PreciseTranscendentals.SafePow(5, value - 2) * 2;
                            if (letterG < 3)
                            {
                                // All part of 𝔊𝔬𝔬𝔤𝔬𝔩𝔖𝔥𝔞𝔯𝔭, 𝔱𝔥𝔢 𝔞𝔯𝔟𝔦𝔱𝔯𝔞𝔯𝔶-𝔯𝔞𝔫𝔤𝔢 𝔫𝔲𝔪𝔢𝔯𝔦𝔠 𝔱𝔶𝔭𝔢.
                                // G2.301... = FFF0.301... =  you kinda get the idea... 
                                Float128 letterFF = Float128PreciseTranscendentals.SafeExp10(letterG - 2);
                                return commonString ? "10^^" + ArithmonymFormattingUtils.FormatArithmonymFromLetterF(letterFF, isReciprocal, "*10^", false, abbreviate) : "F" + ArithmonymFormattingUtils.FormatArithmonymFromLetterF(letterFF, isReciprocal, "E", true, abbreviate);
                            }
                            return null;
                        }
                        return null;
                    }
                default:
                    return null;
            }
        }

        internal static string FormatFloat128AbbreviateFromLog10(Float128 value)
        {
            int vf = (int)value;
            int vfloor = vf / 3;
            Float128 left = Float128PreciseTranscendentals.SafeExp10(value - (3 * vfloor));

            return left.ToString($"F{3 + (3 * vfloor) - vf}", null) + new string[] { "", "K", "M", "B", "T" }[vfloor];
        }
    }
}