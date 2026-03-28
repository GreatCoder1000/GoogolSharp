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
    internal readonly struct ArithmonymBusyBeaver : IArithmonymOperation
    {
        private readonly Arithmonym inner;
        public Arithmonym Operand => inner;

        internal ArithmonymBusyBeaver(Arithmonym inner)
        {
            this.inner = inner;
        }

        public Arithmonym Evaluate()
        {
            if (Operand < Arithmonym.One) throw new Exception("Busy Beaver Input must be integer >=1");
            if (Operand == Arithmonym.One) return Arithmonym.One;
            if (Operand == Arithmonym.Two) return Arithmonym.Four;
            if (Operand == Arithmonym.Three) return Arithmonym.Six;
            if (Operand == Arithmonym.Four) return Arithmonym.Fourteen;
            if (Operand == Arithmonym.Five) return new(4098L);
            return Arithmonym.Tetration(Arithmonym.Two, Arithmonym.Tetration(Arithmonym.Two, Arithmonym.Tetration(Arithmonym.Two, Arithmonym.Nine)));
            // TODO
        }
    }
}