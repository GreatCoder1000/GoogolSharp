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
    partial struct Arithmonym
    {
        
        // Lanczos coefficients (g=7, n=9 is common choice)
        private static readonly double[] lanczosCoefficients =
        {
            0.99999999999980993,
            676.5203681218851,
            -1259.1392167224028,
            771.32342877765313,
            -176.61502916214059,
            12.507343278686905,
            -0.13857109526572012,
            9.9843695780195716e-6,
            1.5056327351493116e-7
        };

        /// <summary>
        /// Factorial using Lanczos approximation via Gamma(n+1).
        /// </summary>
        public static Arithmonym Factorial(Arithmonym n)
        {
            // Convert to double for approximation
            double x = (double)n;

            if (x < 0.0)
                throw new ArgumentException("Factorial not defined for negative values.");

            // For integer values, handle small n directly
            if (x == Math.Floor(x) && x <= 20)
            {
                double exact = 1.0;
                for (int i = 2; i <= (int)x; i++)
                    exact *= i;
                return (Arithmonym)exact;
            }

            // Lanczos approximation for Gamma(n+1)
            return (Arithmonym)GammaLanczos(x + 1.0);
        }

        private static double GammaLanczos(double z)
        {
            if (z < 0.5)
            {
                // Reflection formula for stability
                return Math.PI / (Math.Sin(Math.PI * z) * GammaLanczos(1 - z));
            }

            z--;
            double x = lanczosCoefficients[0];
            for (int i = 1; i < lanczosCoefficients.Length; i++)
            {
                x += lanczosCoefficients[i] / (z + i);
            }

            double g = 7.0;
            double t = z + g + 0.5;
            return Math.Sqrt(2 * Math.PI) * Math.Pow(t, z + 0.5) * Math.Exp(-t) * x;
        }
    }
}