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
    internal readonly struct ArithmonymPsiLevel : IArithmonymOperation
    {
        private readonly Arithmonym inner;
        public Arithmonym Operand => inner;

        internal ArithmonymPsiLevel(Arithmonym inner)
        {
            this.inner = inner;
        }

        public Arithmonym Evaluate()
        {
            int n = (int)(Operand + (Arithmonym)1e-15);
            if (n == 0) return Arithmonym.Zero;
            if (n == 1) return Arithmonym.Ten;
            if (n == 2) return Arithmonym.TenBillion;
            if (n == 3) return Arithmonym.Trialogue;
            if (n == 4) return Arithmonym.Tetralogue;
            if (n == 5) return Arithmonym.Pentalogue;
            if (n == 6) return Arithmonym.Dekalogue;
            throw new NotImplementedException();
        }
    }
}