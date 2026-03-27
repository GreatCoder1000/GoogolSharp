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
    public static class Float128ExtendedConversions
    {
        public static Float128 twoRaisedTo64 = Float128.One + (Float128)ulong.MaxValue;
        public static Float128 UInt128ToFloat128(UInt128 value)
        {
            ulong lo = (ulong)value;
            ulong hi = (ulong)(value >> 64);
            return (Float128)lo + (Float128)hi * twoRaisedTo64;
        }
        public static Float128 LdexpLoop(Float128 x, int exponent)
        {
            for (int i = 0; i < exponent; i++) x *= 2;
            return x;
        }
    }
}