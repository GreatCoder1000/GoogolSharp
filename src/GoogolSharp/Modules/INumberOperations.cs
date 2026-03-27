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
using System.Globalization;
using System.Numerics;

namespace GoogolSharp
{
    partial struct Arithmonym
    {

        /// <summary>
        /// Returns the absolute value (magnitude) of <paramref name="value"/>.
        /// This is a small helper that forwards to the instance-level <see cref="AbsoluteValue"/> property.
        /// </summary>
        /// <param name="value">The value to take the absolute of.</param>
        /// <returns>A non-negative <see cref="Arithmonym"/> with the same magnitude as <paramref name="value"/>.</returns>
        public static Arithmonym Abs(Arithmonym value) => value.AbsoluteValue;

        /// <summary>
        /// Returns the additive negation of <paramref name="value"/>.
        /// This is a convenience wrapper around the unary minus operator.
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated <see cref="Arithmonym"/>.</returns>
        public static Arithmonym Neg(Arithmonym value) => -value;

        /// <summary>
        /// Returns 10 raised to the power <paramref name="value"/>.
        /// This static helper forwards to the instance-level <see cref="_Exp10"/> behavior.
        /// </summary>
        /// <param name="value">The exponent value (base 10).</param>
        /// <returns>An <see cref="Arithmonym"/> representing 10^<paramref name="value"/>.</returns>
        public static Arithmonym Exp10(Arithmonym value) => value._Exp10;

        /// <summary>
        /// Returns the base-10 logarithm of <paramref name="value"/>.
        /// This static helper forwards to the instance-level <see cref="_Log10"/> behavior.
        /// </summary>
        /// <param name="value">The positive value to take the base-10 logarithm of.</param>
        /// <returns>An <see cref="Arithmonym"/> representing log₁₀(<paramref name="value"/>).</returns>
        public static Arithmonym Log10(Arithmonym value) => value._Log10;

        /// <summary>
        /// Returns 2 raised to the power <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The exponent value (base 2).</param>
        /// <returns>An <see cref="Arithmonym"/> representing 2^<paramref name="value"/>.</returns>
        public static Arithmonym Exp2(Arithmonym value) => (value / Log2_10)._Exp10;

        /// <summary>
        /// Returns the base-2 logarithm of <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The positive value to take the base-2 logarithm of.</param>
        /// <returns>An <see cref="Arithmonym"/> representing log₂(<paramref name="value"/>).</returns>
        public static Arithmonym Log2(Arithmonym value) => value._Log10 * Log2_10;

        /// <summary>
        /// Returns e raised to the power <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The exponent value (base e).</param>
        /// <returns>An <see cref="Arithmonym"/> representing e^<paramref name="value"/>.</returns>
        public static Arithmonym Exp(Arithmonym value) => (value / Ln10)._Exp10;

        /// <summary>
        /// Returns the natural (base-e) logarithm of <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The positive value to take the natural logarithm of.</param>
        /// <returns>An <see cref="Arithmonym"/> representing ln(<paramref name="value"/>).</returns>
        public static Arithmonym Log(Arithmonym value) => value._Log10 * Ln10;

        /// <summary>
        /// Returns <paramref name="left"/> exponentiated to <paramref name="right"/>
        /// </summary>
        public static Arithmonym Pow(Arithmonym left, Arithmonym right) => (left._Log10 * right)._Exp10;

        /// <summary>
        /// Returns the square root of <paramref name="value"/>
        /// </summary>
        /// <param name="value">The non-negative value to take the square root of.</param>
        /// <returns>The positive square root of <paramref name="value"/>.</returns>
        public static Arithmonym Sqrt(Arithmonym value) => new ArithmonymSqrt(value).Evaluate();

        /// <summary>
        /// Returns the cube root of <paramref name="value"/>
        /// </summary>
        /// <param name="value">The value to take the cube root of.</param>
        /// <returns>The cube root of <paramref name="value"/>.</returns>
        public static Arithmonym Cbrt(Arithmonym value) => new ArithmonymCbrt(value).Evaluate();

        /// <summary>
        /// Determines whether the specified <see cref="Arithmonym"/> represents positive or negative infinity.
        /// </summary>
        public static bool IsInfinity(Arithmonym v)
            => (v.Letter == 0x3f) && !v._IsReciprocal;

        /// <summary>
        /// Determines whether the specified <see cref="Arithmonym"/> is Not-a-Number (NaN).
        /// </summary>
        public static bool IsNaN(Arithmonym v)
            => (v.Letter == 0x3f) && v._IsReciprocal && v.OperandFloored != 2;

        /// <summary>
        /// Determines whether the specified <see cref="Arithmonym"/> is a quiet NaN (QNaN).
        /// </summary>
        public static bool IsQNaN(Arithmonym v)
            => IsNaN(v) && v._IsReciprocal;

        /// <summary>
        /// Determines whether the specified <see cref="Arithmonym"/> represents zero.
        /// </summary>
        public static bool IsZero(Arithmonym v)
            => (v.Letter == 0x3f) && v._IsReciprocal && v.OperandFloored == 2;

        /// <summary>
        /// Determines whether the specified <see cref="Arithmonym"/> is negative (and not zero).
        /// </summary>
        public static bool IsNegative(Arithmonym v) => v._IsNegative && !IsZero(v);

        /// <summary>
        /// Determines whether the specified <see cref="Arithmonym"/> is positive (and not zero).
        /// </summary>
        public static bool IsPositive(Arithmonym v) => !v._IsNegative && !IsZero(v);

        public static bool IsCanonical(Arithmonym v) => (IsZero(v) && v._IsNegative) || (v.AbsoluteValue == One && v._IsReciprocal);
        public static bool IsComplexNumber(Arithmonym v) => false;
        public static bool IsEvenInteger(Arithmonym v) => IsZero(v) || (!v._IsReciprocal && (v.AbsoluteValue <= Hundred ? Float128.IsEvenInteger(v) : Float128.IsInteger(v.Operand)));
        public static bool IsFinite(Arithmonym v) => !IsInfinity(v) && !IsNaN(v);
        public static bool IsImaginaryNumber(Arithmonym v) => false;
        public static bool IsInteger(Arithmonym v) => IsEvenInteger(v) || IsOddInteger(v);
        public static bool IsNegativeInfinity(Arithmonym v) => IsInfinity(v) && IsNegative(v);
        public static bool IsNormal(Arithmonym v) => !IsNaN(v) && !IsZero(v) && !IsInfinity(v);
        public static bool IsOddInteger(Arithmonym v) => !v._IsReciprocal && v.AbsoluteValue <= Hundred && Float128.IsOddInteger(v);
        public static bool IsPositiveInfinity(Arithmonym v) => IsInfinity(v) && IsPositive(v);
        public static bool IsRealNumber(Arithmonym v) => !IsNaN(v) && !IsInfinity(v);
        public static bool IsSubnormal(Arithmonym v) => false;

        public static Arithmonym MinMagnitude(Arithmonym x, Arithmonym y)
        {
            if (IsNaN(x) || IsNaN(y)) return Arithmonym.NaN;

            var ax = Abs(x);
            var ay = Abs(y);

            int cmp = ax.CompareTo(ay);
            if (cmp < 0) return x;
            if (cmp > 0) return y;

            // Magnitudes equal → return the smaller numeric value
            return x.CompareTo(y) <= 0 ? x : y;
        }

        public static Arithmonym MaxMagnitude(Arithmonym x, Arithmonym y)
        {
            if (IsNaN(x) || IsNaN(y)) return Arithmonym.NaN;

            var ax = Abs(x);
            var ay = Abs(y);

            int cmp = ax.CompareTo(ay);
            if (cmp > 0) return x;
            if (cmp < 0) return y;

            // Magnitudes equal → return the larger numeric value
            return x.CompareTo(y) >= 0 ? x : y;
        }

        public static Arithmonym MinMagnitudeNumber(Arithmonym x, Arithmonym y)
        {
            if (IsNaN(x)) return y;
            if (IsNaN(y)) return x;
            return MinMagnitude(x, y);
        }

        public static Arithmonym MaxMagnitudeNumber(Arithmonym x, Arithmonym y)
        {
            if (IsNaN(x)) return y;
            if (IsNaN(y)) return x;
            return MaxMagnitude(x, y);
        }

        public static Arithmonym Parse(ReadOnlySpan<char> chars, NumberStyles styles, IFormatProvider? provider)
        {
            return Parse(chars.ToString(), styles, provider);
        }

        public static Arithmonym Parse(ReadOnlySpan<char> chars, IFormatProvider? provider)
        {
            return Parse(chars.ToString(), provider);
        }

        public static bool TryConvertFromChecked<TFrom>(TFrom value, out Arithmonym result)
            where TFrom : INumberBase<TFrom>
        {
            // Handle NaN/Infinity if your type supports them
            if (TFrom.IsNaN(value) || TFrom.IsInfinity(value))
            {
                result = default;
                return false;
            }

            if (typeof(TFrom) == typeof(double))
            {
                double original = (double)(object)value;
                result = new Arithmonym(original);
                return (double)result == original;
            }
            else if (typeof(TFrom) == typeof(Float128))
            {
                var original = (Float128)(object)value;
                result = new Arithmonym(original);
                return (Float128)result == original;
            }
            else if (typeof(TFrom) == typeof(int))
            {
                int original = (int)(object)value;
                result = new Arithmonym(original);
                return true; // integers are exact
            }
            else if (typeof(TFrom) == typeof(long))
            {
                long original = (long)(object)value;
                result = new Arithmonym(original);
                return true;
            }
            else
            {
                // Unsupported type (like decimal)
                result = default;
                return false;
            }
        }

        public static bool TryConvertFromSaturating<TFrom>(TFrom value, out Arithmonym result)
            where TFrom : INumberBase<TFrom>
        {
            // Handle NaN
            if (TFrom.IsNaN(value))
            {
                result = Arithmonym.NaN; // or default if you don’t support NaN
                return true;
            }

            // Handle Infinity
            if (TFrom.IsInfinity(value))
            {
                result = TFrom.IsNegative(value) ? Arithmonym.MinValue : Arithmonym.MaxValue;
                return true;
            }

            // Double
            if (typeof(TFrom) == typeof(double))
            {
                double original = (double)(object)value;
                if (original > (double)Arithmonym.MaxValue)
                    result = Arithmonym.MaxValue;
                else if (original < (double)Arithmonym.MinValue)
                    result = Arithmonym.MinValue;
                else
                    result = new Arithmonym(original);
                return true;
            }

            // Quadruple precision (Float128)
            if (typeof(TFrom) == typeof(Float128))
            {
                var original = (Float128)(object)value;
                if (original > (Float128)Arithmonym.MaxValue)
                    result = Arithmonym.MaxValue;
                else if (original < (Float128)Arithmonym.MinValue)
                    result = Arithmonym.MinValue;
                else
                    result = new Arithmonym(original);
                return true;
            }

            // Integers (safe, no clamping needed)
            if (typeof(TFrom) == typeof(int))
            {
                int original = (int)(object)value;
                result = new Arithmonym(original);
                return true;
            }
            if (typeof(TFrom) == typeof(long))
            {
                long original = (long)(object)value;
                result = new Arithmonym(original);
                return true;
            }

            // Unsupported types (like decimal, unless you add support)
            result = default;
            return false;
        }

        public static bool TryConvertFromTruncating<TFrom>(TFrom value, out Arithmonym result)
            where TFrom : INumberBase<TFrom>
        {
            // Handle NaN
            if (TFrom.IsNaN(value))
            {
                result = Arithmonym.NaN; // or default if you don’t support NaN
                return true;
            }

            // Handle Infinity
            if (TFrom.IsInfinity(value))
            {
                result = default; // truncating discards infinities
                return false;
            }

            // Double
            if (typeof(TFrom) == typeof(double))
            {
                double original = (double)(object)value;
                if (original > (double)Arithmonym.MaxValue)
                    result = Arithmonym.MaxValue; // truncate down
                else if (original < (double)Arithmonym.MinValue)
                    result = Arithmonym.MinValue; // truncate up
                else
                    result = new Arithmonym(original);
                return true;
            }

            // Quadruple precision (Float128)
            if (typeof(TFrom) == typeof(Float128))
            {
                var original = (Float128)(object)value;
                if (original > (Float128)Arithmonym.MaxValue)
                    result = Arithmonym.MaxValue;
                else if (original < (Float128)Arithmonym.MinValue)
                    result = Arithmonym.MinValue;
                else
                    result = new Arithmonym(original);
                return true;
            }

            // Integers (safe, truncation not needed)
            if (typeof(TFrom) == typeof(int))
            {
                result = new Arithmonym((int)(object)value);
                return true;
            }
            if (typeof(TFrom) == typeof(long))
            {
                result = new Arithmonym((long)(object)value);
                return true;
            }

            // Unsupported types
            result = default;
            return false;
        }

        public static bool TryConvertToChecked<TTo>(Arithmonym value, out TTo result)
            where TTo : INumberBase<TTo>
        {
            // Handle NaN/Infinity
            if (IsNaN(value) || IsInfinity(value))
            {
                result = default!;
                return false;
            }

            if (typeof(TTo) == typeof(double))
            {
                double candidate = (double)value;
                result = (TTo)(object)candidate;
                return candidate == (double)(object)result; // exact round-trip
            }
            else if (typeof(TTo) == typeof(Float128))
            {
                var candidate = (Float128)value;
                result = (TTo)(object)candidate;
                return candidate == (Float128)(object)result;
            }
            else if (typeof(TTo) == typeof(int))
            {
                int candidate = (int)value;
                result = (TTo)(object)candidate;
                return new Arithmonym(candidate) == value;
            }
            else if (typeof(TTo) == typeof(long))
            {
                long candidate = (long)value;
                result = (TTo)(object)candidate;
                return new Arithmonym(candidate) == value;
            }

            result = default!;
            return false;
        }

        public static bool TryConvertToSaturating<TTo>(Arithmonym value, out TTo result)
            where TTo : INumberBase<TTo>
        {
            if (IsNaN(value))
            {
                result = default!;
                return false;
            }

            if (typeof(TTo) == typeof(double))
            {
                double candidate;
                if (value > new Arithmonym(double.MaxValue))
                    candidate = double.MaxValue;
                else if (value < new Arithmonym(double.MinValue))
                    candidate = double.MinValue;
                else
                    candidate = (double)value;

                result = (TTo)(object)candidate;
                return true;
            }
            else if (typeof(TTo) == typeof(Float128))
            {
                // Float128 has much larger range, so usually direct
                var candidate = (Float128)value;
                result = (TTo)(object)candidate;
                return true;
            }
            else if (typeof(TTo) == typeof(int))
            {
                int candidate;
                if (value > new Arithmonym((Float128)int.MaxValue))
                    candidate = int.MaxValue;
                else if (value < new Arithmonym((Float128)int.MinValue))
                    candidate = int.MinValue;
                else
                    candidate = (int)value;

                result = (TTo)(object)candidate;
                return true;
            }
            else if (typeof(TTo) == typeof(long))
            {
                long candidate;
                if (value > new Arithmonym((Float128)long.MaxValue))
                    candidate = long.MaxValue;
                else if (value < new Arithmonym((Float128)long.MinValue))
                    candidate = long.MinValue;
                else
                    candidate = (long)value;

                result = (TTo)(object)candidate;
                return true;
            }

            result = default!;
            return false;
        }

        public static bool TryConvertToTruncating<TTo>(Arithmonym value, out TTo result)
            where TTo : INumberBase<TTo>
        {
            if (IsNaN(value) || IsInfinity(value))
            {
                result = default!;
                return false;
            }

            if (typeof(TTo) == typeof(double))
            {
                // Truncate by casting directly
                result = (TTo)(object)(double)value;
                return true;
            }
            else if (typeof(TTo) == typeof(Float128))
            {
                result = (TTo)(object)(Float128)value;
                return true;
            }
            else if (typeof(TTo) == typeof(int))
            {
                // Drop fractional part
                result = (TTo)(object)(int)value;
                return true;
            }
            else if (typeof(TTo) == typeof(long))
            {
                result = (TTo)(object)(long)value;
                return true;
            }

            result = default!;
            return false;
        }

        /// <summary>
        /// Tries to parse the specified string into an <see cref="Arithmonym"/> using the
        /// provided <see cref="NumberStyles"/> and <see cref="IFormatProvider"/>.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="styles">The number styles to allow when parsing.</param>
        /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information, or <c>null</c>.</param>
        /// <param name="result">When this method returns, contains the parsed <see cref="Arithmonym"/> if the parse succeeded; otherwise the default value.</param>
        /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
        public static bool TryParse(string? s, NumberStyles styles, IFormatProvider? provider, out Arithmonym result)
        {
            try
            {
                result = Parse(s, styles, provider);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// Tries to parse the characters in <paramref name="chars"/> into an <see cref="Arithmonym"/>
        /// using the provided <see cref="NumberStyles"/> and <see cref="IFormatProvider"/>.
        /// </summary>
        /// <param name="chars">The span of characters to parse.</param>
        /// <param name="styles">The number styles to allow when parsing.</param>
        /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information, or <c>null</c>.</param>
        /// <param name="result">When this method returns, contains the parsed <see cref="Arithmonym"/> if the parse succeeded; otherwise the default value.</param>
        /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
        public static bool TryParse(ReadOnlySpan<char> chars, NumberStyles styles, IFormatProvider? provider, out Arithmonym result)
        {
            try
            {
                result = Parse(chars, styles, provider);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// Tries to parse the characters in <paramref name="chars"/> into an <see cref="Arithmonym"/>
        /// using the specified <see cref="IFormatProvider"/>.
        /// </summary>
        /// <param name="chars">The span of characters to parse.</param>
        /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information, or <c>null</c>.</param>
        /// <param name="result">When this method returns, contains the parsed <see cref="Arithmonym"/> if the parse succeeded; otherwise the default value.</param>
        /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
        public static bool TryParse(ReadOnlySpan<char> chars, IFormatProvider? provider, out Arithmonym result)
        {
            try
            {
                result = Parse(chars, provider);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// Attempts to format the current <see cref="Arithmonym"/> into the provided
        /// <paramref name="destination"/> buffer using the specified <paramref name="format"/> and <paramref name="provider"/>.
        /// </summary>
        /// <param name="destination">The destination buffer to write the formatted string into.</param>
        /// <param name="charsWritten">When this method returns, contains the number of characters written to <paramref name="destination"/>.</param>
        /// <param name="format">The format to use (may be empty).</param>
        /// <param name="provider">An optional format provider that supplies culture-specific formatting information.</param>
        /// <returns><c>true</c> if the value was successfully formatted into <paramref name="destination"/>; otherwise <c>false</c>.</returns>
        public bool TryFormat(
            Span<char> destination,
            out int charsWritten,
            ReadOnlySpan<char> format,
            IFormatProvider? provider)
        {
            // Step 1: Get canonical string form
            string s = ToString(format.ToString(), provider);

            // Step 2: Check buffer size
            if (s.Length > destination.Length)
            {
                charsWritten = 0;
                return false;
            }

            // Step 3: Copy into destination
            s.AsSpan().CopyTo(destination);
            charsWritten = s.Length;
            return true;
        }

        /// <summary>
        /// Parses the specified string into an <see cref="Arithmonym"/> using the provided
        /// <see cref="IFormatProvider"/> and default number styles.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information, or <c>null</c>.</param>
        /// <returns>The parsed <see cref="Arithmonym"/>.</returns>
        /// <exception cref="FormatException">Thrown when <paramref name="s"/> is not in a correct format.</exception>
        public static Arithmonym Parse(string s, IFormatProvider? provider)
        {
            return Parse(s, NumberStyles.None, provider);
        }

        /// <summary>
        /// Tries to parse the specified string into an <see cref="Arithmonym"/> using the provided
        /// <see cref="IFormatProvider"/> and default number styles.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information, or <c>null</c>.</param>
        /// <param name="value">When this method returns, contains the parsed <see cref="Arithmonym"/> if the parse succeeded; otherwise the default value.</param>
        /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
        public static bool TryParse(string? s, IFormatProvider? provider, out Arithmonym value)
        {
            return TryParse(s, NumberStyles.None, provider, out value);
        }
    }
}