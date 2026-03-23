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

using QuadrupleLib;
using QuadrupleLib.Accelerators;
using Float128 = QuadrupleLib.Float128<QuadrupleLib.Accelerators.DefaultAccelerator>;

namespace GoogolSharp.Helpers
{
    public static class ArithmonymFormattingUtils
    {
        public static string FormatArithmonymFromLetterF(
            Float128 letterF,
            bool isReciprocal,
            string placeholder = "E",
            bool showExponentSignIfPositive = true)
        {
            if (letterF < 2)
                return new Arithmonym(Float128HyperTranscendentals.LetterF(letterF)).ToString();

            if (letterF < 3)
            {
                Float128 letterE = Float128PreciseTranscendentals.SafeExp10(
                    Float128PreciseTranscendentals.SafeExp10(
                        Float128.FusedMultiplyAdd(letterF, 1, -2)));

                return FormatArithmonymScientific(letterE, isReciprocal, placeholder, showExponentSignIfPositive);
            }

            if (letterF < 7)
            {
                string sign = isReciprocal ? "-" :
                              showExponentSignIfPositive ? "+" : "";

                if (placeholder == "*10^")
                    return $"10^({sign}{FormatArithmonymFromLetterF(letterF - 1, false, placeholder, showExponentSignIfPositive)})";

                return $"{placeholder}{sign}{FormatArithmonymFromLetterF(letterF - 1, false, placeholder, showExponentSignIfPositive)}";
            }

            if (letterF < 100000000000000000000.0)
            {
                Float128 right = Float128.Floor(letterF);
                Float128 left = Float128PreciseTranscendentals.SafeExp10(
                    Float128.FusedMultiplyAdd(letterF, 1, -right));

                // Normalize
                if (left < 1)
                {
                    right -= 1;
                    left *= 10;
                }
                else if (left >= 10)
                {
                    right += 1;
                    left /= 10;
                }

                string leftStr = left.ToString();

                return $"{(isReciprocal ? "1 / (" : "")}{leftStr}F+{right}{(isReciprocal ? ")" : "")}";
            }

            return $"{(isReciprocal ? "1 / " : "")}F+{letterF.ToString("R", null)}";
        }


        public static string FormatArithmonymScientific(
    Float128 letterE,
    bool isReciprocal,
    string placeholder = "E",
    bool showExponentSignIfPositive = true)
        {
            // exponent = floor(letterE)
            Float128 exponent = Float128.Floor(letterE);

            // significand = 10^(letterE - exponent)
            Float128 significand = Float128PreciseTranscendentals.SafeExp10(
                Float128.FusedMultiplyAdd(letterE, 1, -exponent));

            // Normalize significand into [1, 10)
            if (significand < 1)
            {
                significand *= 10;
                exponent -= 1;
            }
            else if (significand > 9.99999)
            {
                significand = 1;
                exponent += 1;
            }
            else if (significand >= 10)
            {
                significand /= 10;
                exponent += 1;
            }

            string sig = significand.ToString("F6", null);

            string sign = isReciprocal ? "-" :
                          showExponentSignIfPositive ? "+" : "";

            return $"{sig}{placeholder}{sign}{(ulong)exponent}";
        }


        /// <summary>
        /// Formats a Float128 value, rounding to integer if it is very close to an integer (within precision tolerance).
        /// This prevents floating-point artifacts like "5.000000000000000000000000025..." from being displayed.
        /// </summary>
        public static string FormatNearInteger(Float128 value)
        {
            Float128 rounded = Float128.Round(value);
            Float128 error = Float128.Abs(value - rounded);

            // If error is extremely small (less than ~2e-21 which is typical for Float128 precision artifacts),
            // return the integer. For values like 5.000000...0026, error ≈ 2.58e-21 triggers this.
            if (error < (Float128)1e-20)
            {
                return ((long)rounded).ToString();
            }

            return value.ToString();
        }
    }
}