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
        /// Initializes a new instance of <see cref="Arithmonym"/> from a <see cref="Float128"/> value.
        /// </summary>
        /// <param name="v">The <see cref="Float128"/> value to convert.</param>
        public Arithmonym(Float128 v)
        {
            if (Float128.IsInfinity(v))
            {
                if (Float128.IsPositive(v))
                {
                    squishedLo = PositiveInfinity.squishedLo;
                    squishedMid = PositiveInfinity.squishedMid;
                    squishedHi = PositiveInfinity.squishedHi;
                }
                if (Float128.IsNegative(v))
                {
                    squishedLo = NegativeInfinity.squishedLo;
                    squishedMid = NegativeInfinity.squishedMid;
                    squishedHi = NegativeInfinity.squishedHi;
                }
            }
            if (Float128.IsNaN(v))
            {
                squishedLo = NaN.squishedLo;
                squishedMid = NaN.squishedMid;
                squishedHi = NaN.squishedHi;
            }
            if (Float128.IsZero(v))
            {
                squishedLo = Zero.squishedLo;
                squishedMid = Zero.squishedMid;
                squishedHi = Zero.squishedHi;
            }

            Float128 value = v;
            byte letter = 0;
            bool isNegative = false;
            bool _IsReciprocal = false;

            if (value < 0)
            {
                value = -value;
                isNegative = true;
            }
            if (value < 1)
            {
                value = 1 / value;
                _IsReciprocal = true;
            }

            // Map into [2,10) and set letter
            if (value < 2)
            {
                value = (value * 8) - 6;
                value = SnapToInt(value);
                letter = 1;
            }
            else if (value < 4)
            {
                value = (value * 4) - 6;
                value = SnapToInt(value);
                letter = 2;
            }
            else if (value < 20)
            {
                value /= 2;
                value = SnapToInt(value);
                letter = 3;
            }
            else if (value < 100)
            {
                value /= 10;
                value = SnapToInt(value);
                letter = 4;
            }
            else if (value < (Float128)1e10)
            {
#if DEBUG
                if (Float128.Abs(v - (Float128)100) < (Float128)0.1)
                    Console.WriteLine($"[Arithmonym constructor] Input={v}, SafeLog10={Float128PreciseTranscendentals.SafeLog10(value)}, SnapToInt result={SnapToInt(Float128PreciseTranscendentals.SafeLog10(value))}");
#endif
                value = Float128PreciseTranscendentals.SafeLog10(value);
                value = SnapToInt(value);
                // clamp any rounding error that pushes us below 2.0
                if (value < (Float128)2)
                    value = (Float128)2;
                letter = 5;
            }
            else
            {
                value = Float128HyperTranscendentals.SuperLog10(value);
                value = SnapToInt(value);
                letter = 6;
            }

            // Encode operand into Q3.85 format and pack fields
            UInt128 operandBits = EncodeOperand(value);
            UInt128 s =
                operandBits
                + ((UInt128)letter << (FRACTION_BITS + 3))
                + ((UInt128)(_IsReciprocal ? 1 : 0) << (FRACTION_BITS + 9))
                + ((UInt128)(isNegative ? 1 : 0) << (FRACTION_BITS + 10));

            squishedLo = (uint)s;
            squishedMid = (uint)(s >> 32);
            squishedHi = (uint)(s >> 64);
        }
        
        internal Arithmonym(bool isNegative, bool _IsReciprocal, byte letter, UInt128 operand)
            : this(
                operand
                + ((UInt128)letter << (FRACTION_BITS + 3))
                + ((UInt128)(_IsReciprocal ? 1 : 0) << (FRACTION_BITS + 9))
                + ((UInt128)(isNegative ? 1 : 0) << (FRACTION_BITS + 10)))
        {
        }

        
        /// <summary>
        /// Initializes a new instance by splitting a packed <see cref="UInt128"/> value
        /// into the internal three 32-bit words. This constructor is used internally to
        /// create an instance from the packed bit-layout representation.
        /// </summary>
        private Arithmonym(UInt128 squished)
        {
            squishedLo = (uint)squished;
            squishedMid = (uint)(squished >> 32);
            squishedHi = (uint)(squished >> 64);
        }

        

        /// <summary>
        /// Initializes a new instance of <see cref="Arithmonym"/> from a <see cref="double"/> value.
        /// </summary>
        /// <param name="v">The double value to convert.</param>
        public Arithmonym(double v) : this((Float128)v) { }

    }
}