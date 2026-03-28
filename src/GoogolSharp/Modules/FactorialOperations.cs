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
    partial struct Arithmonym
    {
        public static Arithmonym Factorial(Arithmonym n) => new ArithmonymFactorial(n).Evaluate();

        public static Arithmonym Permutations(Arithmonym n, Arithmonym r)
        {
            return Factorial(n) / Factorial(n - r);
        }

        public static Arithmonym Combinations(Arithmonym n, Arithmonym r)
        {
            return Factorial(n) / (Factorial(r) * Factorial(n - r));
        }
    }
}