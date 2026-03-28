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

namespace GoogolSharp
{
    internal readonly struct ArithmonymScg : IArithmonymOperation
    {
        private readonly Arithmonym inner;
        public Arithmonym Operand => inner;

        internal ArithmonymScg(Arithmonym inner)
        {
            this.inner = inner;
        }

        public Arithmonym Evaluate()
        {
            if (Operand < Arithmonym.NegativeOne) throw new Exception("SCG input must be >=0");
            if (Operand == Arithmonym.NegativeOne) return Arithmonym.One;
            if (Operand == Arithmonym.Zero) return Arithmonym.Six;
            if (Operand == Arithmonym.One) return new Arithmonym(false, false, 0x14, 0);
            if (Operand == Arithmonym.Two) return Arithmonym.Scg2LowerBound;
            if (Operand < Arithmonym.Thirteen && !Arithmonym.IsInteger(Operand)) throw new ArgumentException("SSCG not defined for fractional operands.");
            if (Operand < Arithmonym.Thirteen)
            {
                // idk the lower bound
                return Arithmonym.Scg2LowerBound;
            }
            if (Operand <= new Arithmonym(false, false, 0x19, 0))
            {
                // idk?!
                return new(false, false, 0x19, 0);
            }

            // Operand is too big to be affected by SSCG.
            return Operand;
        }
    }
}