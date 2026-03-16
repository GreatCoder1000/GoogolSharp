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

using QuadrupleLib;
using QuadrupleLib.Accelerators;
using Float128 = QuadrupleLib.Float128<QuadrupleLib.Accelerators.DefaultAccelerator>;

namespace GoogolSharp.Helpers
{
    public static class Float128PreciseTranscendentals
    {
        public static Float128 SafeExp(Float128 v) => Float128.Exp(v);
        public static Float128 SafeExp2(Float128 v) => Float128.Exp2(v);
        public static Float128 SafeExp10(Float128 v) => Float128.Exp10(v);
        public static Float128 SafeLog(Float128 v) => Float128.Log(v);
        public static Float128 SafeLog2(Float128 v) => Float128.Log2(v);
        public static Float128 SafeLog10(Float128 v) => Float128.Log10(v);
    }
}
