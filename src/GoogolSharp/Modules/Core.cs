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
        private readonly uint squishedLo;
        private readonly uint squishedMid;
        private readonly uint squishedHi;

        // Reconstruct the original 96-bit stored value as a UInt128 when needed.
        /// <summary>
        /// Reconstructs the packed internal representation into a <see cref="UInt128"/> value.
        /// The implementation stores the value across three 32-bit words; this property
        /// recombines them into the original integer layout used elsewhere in the type.
        /// </summary>
        private UInt128 Squished
        {
            get
            {
                return (((UInt128)squishedHi) << 64) | (((UInt128)squishedMid) << 32) | ((UInt128)squishedLo);
            }
        }
        private const int FRACTION_BITS = 85;
        private static readonly Float128 FRACTION_BITS_EXP2 = Float128ExtendedConversions.LdexpLoop(1, FRACTION_BITS);

        private static UInt128 EncodeOperand(Float128 value)
        {
            // Encode as Q3.85 on (value - 2)
            value -= 2;

            // integer fast-path: if value is already very near an integer we can
            // bypass the messy fractional scaling entirely. This avoids stepping
            // on the 2^-21 boundary that was plaguing small integers like 2.0.
            Float128 rounded = Float128.Round(value);
            Float128 diff = value - rounded;
            if (Float128.Abs(diff) < Float128PreciseTranscendentals.SafeExp2(-40))
            {
                // floored part is simply the integer itself
                long iv = (long)(double)rounded; // safe for small operand ranges
                if (iv < 0) iv = 0;
                UInt128 operandBits = (UInt128)iv << FRACTION_BITS;
                return operandBits;
            }

#if DEBUG
            // report cases where value is very close to integer but missed the
            // fast path; these are usually due to upstream mapping rounding.
            if (Float128.Abs(diff) < Float128PreciseTranscendentals.SafeExp2(-20))
            {
                Console.WriteLine($"[EncodeOperand] near-integer input {value} diff {diff}");
            }
#endif

            value = SnapToInt(value);

            Float128 floored = Float128.Floor(value);
            Float128 fraction = value - floored;

            if (fraction < 0) fraction = 0;
            if (fraction >= 1) { fraction = 0; floored += 1; }

            // Scale fraction to 85-bit fixed-point with truncation
            Float128 scaled = fraction * FRACTION_BITS_EXP2;

            // Carry if scaled == 2^85
            Float128 twoPow = FRACTION_BITS_EXP2;
            if (scaled >= twoPow)
            {
                scaled = 0;
                floored += 1;
            }
            else
            {
                scaled = Float128.Floor(scaled);
            }

            // Build UInt128 exactly
            ulong hi = (ulong)Float128.Floor(scaled / Float128PreciseTranscendentals.SafeExp2(64));
            ulong lo = (ulong)(scaled - (((Float128)hi) * Float128PreciseTranscendentals.SafeExp2(64)));
            UInt128 fractionBits = ((UInt128)hi << 64) + lo;

            byte flooredByte = (byte)floored;

            // Pack fields
            return fractionBits
                + ((UInt128)flooredByte << FRACTION_BITS);
        }

        // Snap values that are within tolerance of an integer
        private static Float128 SnapToInt(Float128 x)
        {
            Float128 n = Float128.Round(x);
            // tolerance widened to -15 to catch log10 errors slightly larger than 2^-16
            return (Float128.Abs(x - n) < Float128PreciseTranscendentals.SafeExp2(-15)) ? n : x;
        }
    }
}