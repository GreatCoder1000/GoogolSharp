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
        public static Arithmonym Sin(Arithmonym value, int terms = 20)
        {
            value %= Tau;
            Arithmonym result = Zero;
            Arithmonym numerator = value;
            Arithmonym denominator = One;
            Arithmonym sign = One;

            for (int term = 0; term < terms; term++)
            {
                result += sign * (numerator / denominator);
                numerator *= value * value;
                denominator *= (2 * term + 2) * (2 * term + 3);
                sign *= -1;
            }
            return result;
        }

        public static Arithmonym Cos(Arithmonym value, int terms = 20)
        {
            value %= Tau;
            Arithmonym result = Zero;
            Arithmonym numerator = One;
            Arithmonym denominator = One;
            Arithmonym sign = One;

            for (int term = 0; term < terms; term++)
            {
                result += sign * (numerator / denominator);
                numerator *= value * value;
                denominator *= (2 * term + 1) * (2 * term + 2);
                sign *= -1;
            }
            return result;
        }

        public static Arithmonym Tan(Arithmonym value, int terms = 20)
        {
            value %= Tau;
            Arithmonym c = Cos(value, terms);
            if (IsZero(c)) throw new ArgumentException("Tan undefined for 90, 270 degrees");
            return Sin(value, terms) / c;
        }
    }
}