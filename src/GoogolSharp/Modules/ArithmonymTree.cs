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

namespace GoogolSharp
{
    internal readonly struct ArithmonymTree : IArithmonymOperation
    {
        private readonly Arithmonym inner;
        public Arithmonym Operand => inner;

        internal ArithmonymTree(Arithmonym inner)
        {
            this.inner = inner;
        }

        public Arithmonym Evaluate()
        {
            if (Operand < Arithmonym.Zero) throw new Exception("TREE input must be >=0");
            if (Operand == Arithmonym.Zero || Operand == Arithmonym.One) return Arithmonym.One;
            if (Operand == Arithmonym.Two) return Arithmonym.Three;
            if (Operand < Arithmonym.Three && !Arithmonym.IsInteger(Operand)) throw new ArgumentException("TREE not defined for fractional operands.");
            if (Operand <= Arithmonym.Scg2LowerBound)
            {
                // STILL too close to differentiate from T2.
                return Arithmonym.Scg2LowerBound;
            }

            // Operand is too big to be affected by TREE
            return Operand;
        }
    }
}