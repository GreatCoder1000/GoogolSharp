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
        // Machine epsilon for IEEE 754 binary128 (approx 2^-113)
        public static readonly Float128 Epsilon = Float128.ScaleB(Float128.One, -113);

        // High-precision constants using multi-part representation
        // Log2(e) = 1.44269504088896340735992468100189214...
        public static readonly Float128 Log2_E = (Float128)1.442695040888963407 +
                                               (Float128)3.59667e-18 +
                                               (Float128)9.4850e-27;

        // Log2(10) = 3.32192809488736234787031942948939398...
        public static readonly Float128 Log2_10 = (Float128)3.321928094887362347 +
                                                 (Float128)8.704e-18 +
                                                 (Float128)1.092e-27;

        // Ln(2) = 0.69314718055994530941723212145817656...
        public static readonly Float128 Ln2 = (Float128)0.693147180559945309 +
                                             (Float128)4.1747e-18 +
                                             (Float128)1.2382e-27;

        // Euler's number constants
        public static readonly Float128 E = (Float128)2.718281828459045234 +
                                           (Float128)6.02214e-18 +
                                           (Float128)1.9927e-27;

        public static readonly Float128 Pi = (Float128)3.141592653589793238 +
                                            (Float128)4.6264e-18 +
                                            (Float128)2.8467e-27;

        /// <summary>
        /// Improved Exp2(y) using Taylor series with 13 terms for better precision.
        /// </summary>
        public static Float128 SafeExp2(Float128 y)
        {
            int n = (int)Float128.Floor(y);
            Float128 f = y - n;

            // small-range exp(f*ln2) using Taylor series
            Float128 z = f * Ln2;
            Float128 z2 = z * z;
            Float128 z3 = z2 * z;
            Float128 z4 = z2 * z2;

            // exp(z) = 1 + z + z^2/2! + z^3/3! + z^4/4! + ... with 13 terms
            Float128 r = (Float128)1.0;
            r += z;
            r += z2 / (Float128)2.0;
            r += z3 / (Float128)6.0;
            r += z4 / (Float128)24.0;
            r += z4 * z / (Float128)120.0;
            r += z4 * z2 / (Float128)720.0;
            r += z4 * z3 / (Float128)5040.0;
            r += z4 * z4 / (Float128)40320.0;
            r += z4 * z4 * z / (Float128)362880.0;
            r += z4 * z4 * z2 / (Float128)3628800.0;
            r += z4 * z4 * z3 / (Float128)39916800.0;
            r += z4 * z4 * z4 / (Float128)479001600.0;
            r += z4 * z4 * z4 * z / (Float128)6227020800.0;

            return Float128.ScaleB(r, n);
        }

        /// <summary>
        /// Improved Log2(x) with better convergence and more iterations.
        /// </summary>
        public static Float128 SafeLog2(Float128 x)
        {
            if (x <= Float128.Zero)
                throw new ArgumentOutOfRangeException(nameof(x),
                    "Log2 undefined for non-positive values.");

            // Decompose x = m * 2^e, with m in [0.5, 1)
            Decompose(x, out Float128 m, out int e);

            // Shift to m in [sqrt(0.5), sqrt(2))
            Float128 sqrtHalf = Float128.Sqrt(Float128.ScaleB(Float128.One, -1));
            if (m < sqrtHalf)
            {
                m *= 2;
                e--;
            }

            // atanh-style transform: ln(x) = 2 * sum_{k=0}^{inf} t^(2k+1)/(2k+1) where t = (x-1)/(x+1)
            Float128 t = (m - Float128.One) / (m + Float128.One);
            Float128 t2 = t * t;

            Float128 sum = t;
            Float128 term = t;

            // Use more terms for better precision
            for (int k = 1; k < 100; k++)
            {
                term *= t2;
                Float128 contrib = term / (2 * k + 1);
                if (Float128.Abs(contrib) < Epsilon * Float128.Abs(sum))
                    break;
                sum += contrib;
            }

            Float128 ln_m = 2 * sum;
            Float128 log2_m = ln_m / Ln2;

            return e + log2_m;
        }

        /// <summary>
        /// Improved Log10(x) using precomputed Log2(10).
        /// </summary>
        public static Float128 SafeLog10(Float128 x)
        {
            return SafeLog2(x) / Log2_10;
        }

        /// <summary>
        /// Improved Log(x) using precomputed Log2(e).
        /// </summary>
        public static Float128 SafeLog(Float128 x)
        {
            return SafeLog2(x) / Log2_E;
        }

        /// <summary>
        /// Safe Pow(x, y) = Exp2(y * Log2(x)).
        /// </summary>
        public static Float128 SafePow(Float128 x, Float128 y)
        {
            if (x <= Float128.Zero)
                throw new ArgumentOutOfRangeException(nameof(x),
                    "Pow undefined for non-positive base.");

            return SafeExp2(y * SafeLog2(x));
        }

        /// <summary>
        /// Safe Exp(y) = Exp2(y * Log2(e)).
        /// </summary>
        public static Float128 SafeExp(Float128 y)
        {
            return SafeExp2(y * Log2_E);
        }

        /// <summary>
        /// Safe Exp10(y) = Exp2(y * Log2(10)).
        /// </summary>
        public static Float128 SafeExp10(Float128 y)
        {
            return SafeExp2(y * Log2_10);
        }

        private static void Decompose(Float128 x, out Float128 mantissa, out int exponent)
        {
            exponent = Float128.ILogB(x);         // integer exponent
            mantissa = Float128.ScaleB(x, -exponent); // normalized mantissa in [0.5, 1)
        }

    }
}