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
        // These are used literally everywhere. Take great care when changing!
        internal readonly byte OperandFloored
        {
            get
            {
                UInt128 mask = ((UInt128)0x7) << FRACTION_BITS;
                return (byte)(2 + (byte)((Squished & mask) >> FRACTION_BITS));
            }
        }

        internal readonly byte Letter
        {
            get
            {
                UInt128 mask = ((UInt128)0x3F) << (FRACTION_BITS + 3);
                return (byte)((Squished & mask) >> (FRACTION_BITS + 3));
            }
        }

        internal readonly bool _IsReciprocal
        {
            get
            {
                return ((Squished >> (FRACTION_BITS + 9)) & (UInt128)1) == (UInt128)1;
            }
        }

        internal readonly UInt128 OperandFraction128
        {
            get
            {
                // Mask the lower FRACTION_BITS to get the fraction field
                UInt128 mask = (((UInt128)1) << FRACTION_BITS) - 1;
                return Squished & mask;
            }
        }

        internal readonly Float128 Operand
        {
            get
            {
                // OperandFloored already includes the +2 offset, so just add the fractional part
                // If fractional part is zero, return just OperandFloored (no additional +2)
                if (OperandFraction128 == 0)
                {
                    return OperandFloored;
                }

                // Otherwise decode fractional part and add to the floored value
                return OperandFloored + (Float128ExtendedConversions.UInt128ToFloat128(OperandFraction128) / FRACTION_BITS_EXP2);
            }
        }

        private readonly bool _IsNegative
        {
            get
            {
                return ((Squished >> (FRACTION_BITS + 10)) & (UInt128)1) == (UInt128)1;
            }
        }
    }
}