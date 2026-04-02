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

using Float128 = QuadrupleLib.Float128<QuadrupleLib.Accelerators.DefaultAccelerator>;

namespace GoogolSharp.Helpers
{
    public static class Float128HyperTranscendentals
    {
        public static Float128 SuperLog10(Float128 v)
        {
            // 

            if (v < 0) return Float128PreciseTranscendentals.SafeExp10(v) - 2;
            if (v < 1) return v - 1;
            if (v < 10) return Float128PreciseTranscendentals.SafeLog10(v);
            if (v < (Float128)1e10) return 1 + Float128PreciseTranscendentals.SafeLog10(Float128PreciseTranscendentals.SafeLog10(v));
            return 2 + Float128PreciseTranscendentals.SafeLog10(Float128PreciseTranscendentals.SafeLog10(Float128PreciseTranscendentals.SafeLog10(v)));
        }

        public static Float128 InverseLetterG(Float128 v)
        {
            // 10^^^0 = 1
            // 10^^^-1 = slog(1) = 0
            // 10^^^-2 = slog(0) = -1
            // 10^^^-3 = slog(-1) = something!

            // Approximation for fast-ness
            if (v < -100) return Float128.Parse("-1.8414056604369606378466046580124861060503713143776396695648500895481840812183170005103456716913040161870560012665879494546091");
            if (v < 0) return SuperLog10(v + 1);
            if (v < 1) return v - 1;
            if (v < 10) return Float128PreciseTranscendentals.SafeLog10(v);
            return 1 + Float128PreciseTranscendentals.SafeLog10(Float128HyperTranscendentals.SuperLog10(v));
        }

        public static Float128 LetterJToLetterG(Float128 v)
        {
            if (v < 2) return v;
            if (v < 3) return 2 * Float128PreciseTranscendentals.SafePow(5, v - 2);
            Float128 letterH = 2 * Float128PreciseTranscendentals.SafePow(5, v - 3);
            Float128 letterG = LetterG(Float128PreciseTranscendentals.SafeExp10(letterH - 2));
            return letterG;
        }

        public static Float128 LetterGToLetterJ(Float128 v)
        {
            if (v < 2) return v;
            if (v < 10) return 2 + (Float128PreciseTranscendentals.SafeLog2(v / 2) / Float128PreciseTranscendentals.SafeLog2(5));
            Float128 letterH = 2 * Float128PreciseTranscendentals.SafePow(5, v - 3);
            Float128 letterG = LetterG(Float128PreciseTranscendentals.SafeExp10(letterH - 2));
            return letterG;
        }

        public static Float128 LetterF(Float128 v)
        {
            if (v < -1) return Float128PreciseTranscendentals.SafeLog10(v + 2);
            if (v < 0) return v + 1;
            if (v < 1) return Float128PreciseTranscendentals.SafeExp10(v);
            if (v < 2) return Float128PreciseTranscendentals.SafeExp10(Float128PreciseTranscendentals.SafeExp10(v - 1));
            return Float128PreciseTranscendentals.SafeExp10(Float128PreciseTranscendentals.SafeExp10(Float128PreciseTranscendentals.SafeExp10(v - 2)));
        }

        public static Float128 LetterG(Float128 v)
        {
            if (v < -1) return SuperLog10(v + 2);
            if (v < 0) return v + 1;
            if (v < 1) return LetterF(v);
            if (v < 2) return LetterF(LetterF(v - 1));
            return LetterF(LetterF(LetterF(v - 2)));
        }
    }
}