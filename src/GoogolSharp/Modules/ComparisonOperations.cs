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
        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="Arithmonym"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is an <see cref="Arithmonym"/> equal to this instance; otherwise <see langword="false"/>.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is Arithmonym other) return !IsNaN(this) && !IsNaN(other) && ((IsZero(this) && IsZero(other)) ? Squished == other.Squished : Normalized.Squished == other.Normalized.Squished);
            return false;
        }

        /// <summary>
        /// Returns the hash code for this <see cref="Arithmonym"/>.
        /// Combines the three internal 32-bit words that together represent the packed value.
        /// </summary>
        public override int GetHashCode() => HashCode.Combine(squishedLo, squishedMid, squishedHi);


        /// <summary>
        /// Determines whether the specified <see cref="Arithmonym"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Arithmonym"/> to compare with.</param>
        /// <returns><see langword="true"/> if <paramref name="other"/> equals this instance; otherwise <see langword="false"/>.</returns>
        public bool Equals(Arithmonym other) => !IsNaN(this) && !IsNaN(other) && ((IsZero(this) && IsZero(other)) ? Squished == other.Squished : Normalized.Squished == other.Normalized.Squished);

        /// <summary>
        /// Compares this instance to another <see cref="Arithmonym"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Arithmonym"/> to compare to.</param>
        /// <returns>
        /// -1 if this instance is less than <paramref name="other"/>, 0 if equal, 1 if greater.
        /// Returns <see cref="int.MinValue"/> when either operand is NaN.
        /// </returns>
        public int CompareTo(Arithmonym other)
        {
            if (IsNaN(this) || IsNaN(other)) return int.MinValue;
            if (IsZero(other)) return IsZero(this) ? 0 : _IsNegative ? -1 : 1;
            if (_IsNegative)
            {
                if (other._IsNegative) return other.Negated.CompareTo(Negated);
                return -1;
            }
            if (_IsReciprocal)
            {
                if (other._IsReciprocal) return other.Reciprocal.CompareTo(Reciprocal);
                return -1;
            }

            if (Letter > other.Letter) return 1;
            if (Letter < other.Letter) return -1;
            if (OperandFloored > other.OperandFloored) return 1;
            if (OperandFloored < other.OperandFloored) return -1;
            if (OperandFraction128 > other.OperandFraction128) return 1;
            if (OperandFraction128 < other.OperandFraction128) return -1;

            return 0;
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="object"/>, which should be an <see cref="Arithmonym"/>.
        /// </summary>
        /// <param name="other">The object to compare to.</param>
        /// <returns>Comparison result as with <see cref="CompareTo(Arithmonym)"/>, or <see cref="int.MinValue"/> for invalid types.</returns>
        public int CompareTo(object? other)
            => other is Arithmonym a
                ? CompareTo(a)
                : int.MinValue;

        /// <summary>
        /// Determines whether two <see cref="Arithmonym"/> instances are equal.
        /// </summary>
        public static bool operator ==(Arithmonym left, Arithmonym right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="Arithmonym"/> instances are not equal.
        /// </summary>
        public static bool operator !=(Arithmonym left, Arithmonym right) => !left.Equals(right);

        /// <summary>
        /// Determines whether <paramref name="left"/> is less than <paramref name="right"/>.
        /// </summary>
        public static bool operator <(Arithmonym left, Arithmonym right) => left.CompareTo(right) == -1;

        /// <summary>
        /// Determines whether <paramref name="left"/> is greater than <paramref name="right"/>.
        /// </summary>
        public static bool operator >(Arithmonym left, Arithmonym right) => left.CompareTo(right) == 1;

        /// <summary>
        /// Determines whether <paramref name="left"/> is less than or equal to <paramref name="right"/>.
        /// </summary>
        public static bool operator <=(Arithmonym left, Arithmonym right) => (left < right) || (left == right);

        /// <summary>
        /// Determines whether <paramref name="left"/> is greater than or equal to <paramref name="right"/>.
        /// </summary>
        public static bool operator >=(Arithmonym left, Arithmonym right) => (left > right) || (left == right);

        public static bool NearlyEqual(Arithmonym left, Arithmonym right, Float128 operandTolerance)
        {
            Arithmonym lhsNmlzd = left.Normalized;
            Arithmonym rhsNmlzd = right.Normalized;
            if (IsZero(lhsNmlzd) != IsZero(rhsNmlzd)) return false;
            if (IsNaN(lhsNmlzd) != IsNaN(rhsNmlzd)) return false;
            if (IsPositiveInfinity(lhsNmlzd) != IsPositiveInfinity(rhsNmlzd)) return false;
            if (IsNegativeInfinity(lhsNmlzd) != IsNegativeInfinity(rhsNmlzd)) return false;

            if (lhsNmlzd._IsNegative != rhsNmlzd._IsNegative) return false;
            if (lhsNmlzd._IsReciprocal != rhsNmlzd._IsReciprocal) return false;
            
            Float128 lhsCompId = lhsNmlzd.Letter + ((lhsNmlzd.Operand - 2) / 8);
            Float128 rhsCompId = rhsNmlzd.Letter + ((rhsNmlzd.Operand - 2) / 8);
            if (Float128.Abs(lhsCompId - rhsCompId) > operandTolerance)
                return false;
            return true;
        }
    }
}