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


using Float128 = QuadrupleLib.Float128<QuadrupleLib.Accelerators.DefaultAccelerator>;

namespace GoogolSharp.Tests
{
    internal static class AssertFloat128
    {
        /// <summary>
        /// Asserts that two Float128 values are equal.
        /// </summary>
        public static void Equal(Float128 expected, Float128 actual)
        {
            if (expected != actual)
                throw new Float128EqualException($"{nameof(AssertFloat128)}.{nameof(Equal)} failure: Values differ\nExpected: {expected}\nActual: {actual}.");
        }

        /// <summary>
        /// Asserts that two Float128 values are equal within a specified precision.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="precision">Number of decimal places to compare (0-15 for typical use).</param>
        public static void Equal(Float128 expected, Float128 actual, int precision)
        {
            // Convert to double for precision-based comparison
            double expectedDouble = (double)expected;
            double actualDouble = (double)actual;

            // Calculate the tolerance based on precision
            double tolerance = Math.Pow(10, -precision);
            double difference = Math.Abs(expectedDouble - actualDouble);

            if (difference > tolerance)
                throw new Float128EqualException($"{nameof(AssertFloat128)}.{nameof(Equal)} failure: Values differ beyond tolerance of {tolerance}\nExpected: {expected} ({expectedDouble})\nActual: {actual} ({actualDouble})\nDifference: {difference}.");
        }

        /// <summary>
        /// Asserts that two Float128 values are nearly equal within a specified tolerance.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="tolerance">The maximum allowed difference.</param>
        public static void NearlyEqual(Float128 expected, Float128 actual, Float128 tolerance)
        {
            Float128 difference = Float128.Abs(expected - actual);

            if (difference > tolerance)
                throw new Float128NearlyEqualException($"{nameof(AssertFloat128)}.{nameof(NearlyEqual)} failure: Values differ more than tolerance {tolerance}\nExpected: {expected}\nActual: {actual}\nDifference: {difference}.");
        }

        /// <summary>
        /// Asserts that a Float128 value is zero.
        /// </summary>
        public static void Zero(Float128 value)
        {
            if (value != Float128.Zero)
                throw new Float128EqualException($"{nameof(AssertFloat128)}.{nameof(Zero)} failure: Expected zero but got {value}.");
        }

        /// <summary>
        /// Asserts that a Float128 value is not zero.
        /// </summary>
        public static void NotZero(Float128 value)
        {
            if (value == Float128.Zero)
                throw new Float128EqualException($"{nameof(AssertFloat128)}.{nameof(NotZero)} failure: Expected non-zero value but got zero.");
        }

        /// <summary>
        /// Asserts that a Float128 value is positive.
        /// </summary>
        public static void Positive(Float128 value)
        {
            if (value <= Float128.Zero)
                throw new Float128EqualException($"{nameof(AssertFloat128)}.{nameof(Positive)} failure: Expected positive value but got {value}.");
        }

        /// <summary>
        /// Asserts that a Float128 value is negative.
        /// </summary>
        public static void Negative(Float128 value)
        {
            if (value >= Float128.Zero)
                throw new Float128EqualException($"{nameof(AssertFloat128)}.{nameof(Negative)} failure: Expected negative value but got {value}.");
        }

        /// <summary>
        /// Asserts that a Float128 value is NaN.
        /// </summary>
        public static void NaN(Float128 value)
        {
            if (!Float128.IsNaN(value))
                throw new Float128EqualException($"{nameof(AssertFloat128)}.{nameof(NaN)} failure: Expected NaN but got {value}.");
        }

        /// <summary>
        /// Asserts that a Float128 value is not NaN.
        /// </summary>
        public static void NotNaN(Float128 value)
        {
            if (Float128.IsNaN(value))
                throw new Float128EqualException($"{nameof(AssertFloat128)}.{nameof(NotNaN)} failure: Expected non-NaN value but got NaN.");
        }

        /// <summary>
        /// Asserts that a Float128 value is positive infinity.
        /// </summary>
        public static void PositiveInfinity(Float128 value)
        {
            if (value != Float128.PositiveInfinity)
                throw new Float128EqualException($"{nameof(AssertFloat128)}.{nameof(PositiveInfinity)} failure: Expected positive infinity but got {value}.");
        }

        /// <summary>
        /// Asserts that a Float128 value is negative infinity.
        /// </summary>
        public static void NegativeInfinity(Float128 value)
        {
            if (value != Float128.NegativeInfinity)
                throw new Float128EqualException($"{nameof(AssertFloat128)}.{nameof(NegativeInfinity)} failure: Expected negative infinity but got {value}.");
        }
    }
}