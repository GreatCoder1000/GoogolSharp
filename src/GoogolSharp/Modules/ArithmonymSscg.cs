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
using System.Numerics;
namespace GoogolSharp
{
    internal readonly struct ArithmonymSscg : IArithmonymOperation
    {
        private readonly Arithmonym inner;
        public Arithmonym Operand => inner;

        internal ArithmonymSscg(Arithmonym inner)
        {
            this.inner = inner;
        }

        public Arithmonym Evaluate()
        {
            if (Operand < Arithmonym.Zero) throw new Exception("SSCG input must be >=0");
            if (Operand == Arithmonym.Zero) return Arithmonym.Two;
            if (Operand == Arithmonym.One) return Arithmonym.Five;

            // 3*2^(3*2^95) - 8. The 8 is ignored because the number is at the scale of double exponentials,
            // where the last bunch of digits doesn't matter in this mantissa size.
            if (Operand == Arithmonym.Two) return Arithmonym.Three * Arithmonym.Exp2(Arithmonym.Three * Arithmonym.Exp2(new(95)));
            if (Operand < Arithmonym.Three && !Arithmonym.IsInteger(Operand)) throw new ArgumentException("SSCG not defined for fractional operands.");
            if (Operand <= Arithmonym.Scg2LowerBound)
            {
                // STILL too close to differentiate from T2.
                return Arithmonym.Scg2LowerBound;
            }

            // Operand is too big to be affected by SSCG.
            return Operand;
        }
    }
}