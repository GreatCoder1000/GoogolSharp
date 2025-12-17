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

namespace GoogolSharp.Helpers
{
    public static class ArithmonymFormattingUtils
    {
        public static string FormatArithmonymFromLetterF(Float128 letterF, bool isReciprocal)
        {
            if (letterF < 2) return new Arithmonym(Float128HyperTranscendentals.LetterF(letterF)).ToString();
            if (letterF < 3)
            {
                Float128 letterE = letterF - 2;
                letterE = Float128PreciseTranscendentals.SafeExp10(
                    Float128PreciseTranscendentals.SafeExp10(
                        Float128PreciseTranscendentals.SafeExp10(
                            letterE
                        )
                    )
                );
                return FormatArithmonymScientific(letterE, isReciprocal);
            }

            // TODO
            return $"{(isReciprocal ? "1 / " : "")}F{letterF}";
        }

        public static string FormatArithmonymScientific(Float128 letterE, bool isReciprocal)
        {
            letterE = Float128PreciseTranscendentals.SafeExp10(
                Float128PreciseTranscendentals.SafeExp10(
                    Float128PreciseTranscendentals.SafeExp10(
                        letterE
                    )
                )
            );
            Float128 exponent = Float128.Floor(letterE);
            Float128 significand = Float128PreciseTranscendentals.SafeExp10(
                letterE - exponent);
            return $"{significand}e{(isReciprocal ? "-" : "+")}{(ulong)exponent}";
        }
    }
}