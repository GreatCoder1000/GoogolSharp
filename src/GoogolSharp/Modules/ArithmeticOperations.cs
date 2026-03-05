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
        /// Adds two <see cref="Arithmonym"/> values.
        /// </summary>
        /// <remarks>
        /// This operator implements fast-path logic for common cases and falls back to <see cref="Float128"/> arithmetic
        /// for complex or extreme values; the fallback may lose precision for ultra-large or ultra-small values.
        /// </remarks>
        public static Arithmonym operator +(Arithmonym left, Arithmonym right)
        {
            if (IsNaN(left) || IsNaN(right)) return NaN;
            if (IsInfinity(left) && IsInfinity(right))
            {
                if (left._IsNegative == right._IsNegative)
                    return left;
                else return NaN;
            }
            if (IsInfinity(left)) return left;

            // Bugfix 1: IsInfinity(right) means right is returned.
            if (IsInfinity(right)) return right;

            if (IsZero(left) && IsZero(right))
            {
                // IEEE754 Zero handling
                if (!left._IsNegative || (left._IsNegative == right._IsNegative)) return Zero;
                return Zero.Negated;
            }

            if (IsZero(left)) return right;
            if (IsZero(right)) return left;
            if (left._IsNegative)
            {
                if (left.Negated == right)
                    return Zero;
                else if (right._IsNegative || left.Negated > right)
                    return (left.Negated + right.Negated).Negated;
                else
                    return right + left;
            }
            if (!right._IsNegative && left < right) return right + left;

            /* 
             * Now there are two branches of logic. 
             * Thanks to the previous few if statements,
             * we can guarantee that, if a >= b and a>0 and b>0, 
             * then we have to perform either a + b or a - b.
             * 
             * We also know b is not 0.
             * 
             * Here's the branching...
             */
            if (right < Zero)
            {
                // Subtraction (left + right where right is negative => left - |right|)


                Arithmonym b = right.Negated;
                if (left == b) return Zero;

                if (left._IsReciprocal)
                {
                    if (Float128.Abs(left.ToFloat128()) >= Float128PreciseTranscendentals.SafeExp2(-16382))
                        return new(left.ToFloat128() + right.ToFloat128());
                    Float128 llog = left._Log10.ToFloat128();
                    Float128 rlog = right._Log10.ToFloat128();
                    Float128 olog = llog + Float128PreciseTranscendentals.SafeLog10(
                        1 - Float128PreciseTranscendentals.SafeExp10(rlog - llog)
                    );
                    if (Float128.IsInfinity(olog) || Float128.IsNaN(olog))
                    {
                        return left;
                    }
                    else
                    {
                        return new Arithmonym(olog)._Exp10;
                    }

                }

                // General fallback (may lose extreme precision for ultra-large/small values)
                return new(left.ToFloat128() + right.ToFloat128());
            }
            else
            {
                // Addition

                // Small enough / common cases: if left is reciprocal use Float128 arithmetic
                if (left._IsReciprocal)
                {
                    if (Float128.Abs(left.ToFloat128()) >= Float128PreciseTranscendentals.SafeExp2(-16382))
                        return new(left.ToFloat128() + right.ToFloat128());
                    Float128 llog = left._Log10.ToFloat128();
                    Float128 rlog = right._Log10.ToFloat128();
                    Float128 olog = llog + Float128PreciseTranscendentals.SafeLog10(
                        1 + Float128PreciseTranscendentals.SafeExp10(rlog - llog)
                    );
                    if (Float128.IsInfinity(olog) || Float128.IsNaN(olog))
                    {
                        return left;
                    }
                    else
                    {
                        return new Arithmonym(olog)._Exp10;
                    }
                }
                else
                {
                    if (right._IsReciprocal)
                    {
                        if (Float128.IsInfinity(left.ToFloat128())) return left;
                        return new(left.ToFloat128() + right.ToFloat128());
                    }
                    Float128 sum = left.ToFloat128() + right.ToFloat128();
                    if (sum == Float128.PositiveInfinity)
                    {
                        Float128 llog = left._Log10.ToFloat128();
                        Float128 rlog = right._Log10.ToFloat128();
                        Float128 olog = llog + Float128PreciseTranscendentals.SafeLog10(
                            1 + Float128PreciseTranscendentals.SafeExp10(rlog - llog)
                        );
                        if (olog == Float128.PositiveInfinity)
                        {
                            return left;
                        }
                        return new Arithmonym(olog)._Exp10;
                    }
                    else
                    {
                        return new(sum);
                    }
                }
            }
        }

        /// <summary>
        /// Subtracts <paramref name="right"/> from <paramref name="left"/>.
        /// </summary>
        public static Arithmonym operator -(Arithmonym left, Arithmonym right)
            => left + right.Negated;

        /// <summary>
        /// Multiplies two <see cref="Arithmonym"/> values.
        /// </summary>
        /// <remarks>
        /// This implementation uses logarithmic addition (log10) on the absolute values and
        /// then exponentiates the result. The resulting sign is the XOR of operand signs.
        /// Special values (NaN, zero, infinity) are handled explicitly.
        /// For finite, reasonably-small values, direct Float128 multiplication is used.
        /// </remarks>
        public static Arithmonym operator *(Arithmonym left, Arithmonym right)
        {
            // Top-level special cases
            if (IsNaN(left) || IsNaN(right)) return NaN;
            if ((IsInfinity(left) && IsZero(right)) || (IsZero(left) && IsInfinity(right)))
                return NaN;
            if (IsZero(left) || IsZero(right)) return Zero;
            if (left == One) return right;
            if (right == One) return left;
            if (left == NegativeOne) return right.Negated;
            if (right == NegativeOne) return left.Negated;

            bool resultNegative = IsNegative(left) ^ IsNegative(right);

            if (IsInfinity(left) || IsInfinity(right))
                return resultNegative ? NegativeInfinity : PositiveInfinity;

            // Fast-path for small, representable values: direct Float128 arithmetic
            Float128 a = left.ToFloat128();
            Float128 b = right.ToFloat128();
            if (Float128.IsFinite(a) && Float128.IsFinite(b) &&
                Float128.Abs(a) < (Float128)1e20 && Float128.Abs(b) < (Float128)1e20)
            {
                Float128 prod = a * b;
                if (Float128.IsFinite(prod))
                {
                    return new Arithmonym(prod);
                }
            }

            // Multiply via logarithms: (left.AbsoluteValue._Log10 + right.AbsoluteValue._Log10)._Exp10
            Arithmonym sumLog = left.AbsoluteValue._Log10 + right.AbsoluteValue._Log10;
            if (IsNaN(sumLog)) return NaN;

            Arithmonym result = sumLog._Exp10;
            return resultNegative ? result.Negated : result;
        }

        /// <summary>
        /// Divides two <see cref="Arithmonym"/> values.
        /// </summary>
        /// <remarks>
        /// This implementation uses logarithmic subtraction (log10) on the absolute values and
        /// then exponentiates the result. The resulting sign is the XOR of operand signs.
        /// Special values (NaN, zero, infinity) are handled explicitly.
        /// </remarks>
        public static Arithmonym operator /(Arithmonym left, Arithmonym right)
        {
            // Top-level special cases
            if (IsNaN(left) || IsNaN(right)) return NaN;
            if (left == right) return One;
            if (IsZero(left)) return Zero;
            if (IsInfinity(right)) return Zero;

            // Division by zero
            if (IsZero(right)) return right._IsNegative ? NegativeInfinity : PositiveInfinity;

            // Shortcuts
            if (right == One) return left;
            if (right == NegativeOne) return left.Negated;
            if (left == One) return right.Reciprocal;
            if (left == NegativeOne) return right.Reciprocal.Negated;

            bool resultNegative = IsNegative(left) ^ IsNegative(right);

            if (IsInfinity(left))
                return resultNegative ? NegativeInfinity : PositiveInfinity;

            // Fast-path for well-behaved finite values - avoids log/exp noise
            Float128 af = left.ToFloat128();
            Float128 bf = right.ToFloat128();
            if (Float128.IsFinite(af) && Float128.IsFinite(bf) && bf != Float128.Zero)
            {
                Float128 div = af / bf;
                if (Float128.IsFinite(div))
                {
                    var r = new Arithmonym(div);
                    return resultNegative ? r.Negated : r;
                }
            }

            // Divide via logarithms: (left._Log10 - right._Log10)._Exp10
            Arithmonym diffLog = left._Log10 - right._Log10;
            if (IsNaN(diffLog)) return NaN;

            Arithmonym result = diffLog._Exp10;
            return resultNegative ? result.Negated : result;
        }

        public static Arithmonym operator ++(Arithmonym v) => v + One;
        public static Arithmonym operator --(Arithmonym v) => v - One;

        public static Arithmonym Floor(Arithmonym v)
        {
            if (Float128.IsFinite(v.ToFloat128())) return new(Float128.Floor(v.ToFloat128()));
            return v;
        }

        public static Arithmonym operator %(Arithmonym a, Arithmonym b)
        {
            if (b == Arithmonym.Zero)
                throw new DivideByZeroException();

            // quotient = Floor(a / b)
            Arithmonym quotient = Floor(a / b);

            // remainder = a - b * quotient
            return a - b * quotient;
        }

        public static Arithmonym operator +(Arithmonym value) => value;
        public static Arithmonym operator -(Arithmonym value) => value.Negated;

    }
}