using QuadrupleLib;

namespace GoogolSharp.Helpers
{
    public static class Float128PreciseTranscendentals
    {
        // Machine epsilon for IEEE 754 binary128 (approx 2^-113)
        public static readonly Float128 Epsilon = Float128.ScaleB(Float128.One, -113);
        public static readonly Float128 Log2_E = (Float128)1.442695040 + (Float128)8.889634073e-10 + (Float128)5.992468100e-20 + (Float128)1.892137426e-30 + (Float128)6.459541529e-40;
        public static readonly Float128 Log2_10 = (Float128)3.321928094 + (Float128)8.873623478e-10 + (Float128)7.031942948e-20 + (Float128)9.390175864e-30 + (Float128)8.313930245e-40;
        public static readonly Float128 Ln2 = (Float128)0.693147180 + (Float128)5.599453094e-10 + (Float128)1.723212145e-20 + (Float128)8.176568075e-30 + (Float128)5.001343602e-40;

        /// <summary>
        /// Improved Exp2(y) using Newton iteration with adaptive stopping.
        /// </summary>
        public static Float128 SafeExp2(Float128 y)
        {
            Float128 x_n = Float128.ScaleB(Float128.One, (int)Float128.Floor(y));

            for (int n = 0; n < 10; n++)
            {
                Float128 delta = y - Float128.Log2(x_n);
                if (Float128.Abs(delta) < Epsilon) break;
                x_n = Float128.FusedMultiplyAdd(x_n * Ln2, delta, x_n);
            }

            return x_n;
        }

        /// <summary>
        /// Improved Log2(x) wrapper.
        /// </summary>
        public static Float128 SafeLog2(Float128 x)
        {
            if (x <= Float128.Zero)
                throw new ArgumentOutOfRangeException(nameof(x), "Log2 undefined for non-positive values.");

            // Decompose x = m * 2^e
            Decompose(x, out Float128 m, out int e);

            // ε = m - 1, with m in [0.5,1)
            Float128 epsilon = m - Float128.One;

            // Series expansion for log2(1+ε)
            Float128 term = epsilon;
            Float128 sum = Float128.Zero;
            int k = 1;

            while (Float128.Abs(term) > Float128.ScaleB(Float128.One, -120)) // stop near binary128 epsilon
            {
                sum += term / k;
                k++;
                term *= -epsilon; // alternating series
            }

            Float128 log2Mantissa = sum / Ln2;

            return e + log2Mantissa;
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
            return SafeExp2(y * Float128.Log2(x));
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