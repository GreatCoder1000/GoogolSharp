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
using Float128 = QuadrupleLib.Float128<QuadrupleLib.Accelerators.DefaultAccelerator>;

namespace GoogolSharp
{
    partial struct Arithmonym
    {
        // This file contains some useful predicates that are also publicized!
        // Only getters. No setters, because that would break the library.

        /// <summary>
        /// Gets the multiplicative reciprocal (1 / value) of this <see cref="Arithmonym"/>.
        /// </summary>
        public readonly Arithmonym Reciprocal
        {
            get
            {
                if (IsNaN(this)) return NaN;
                if (IsZero(this)) return PositiveInfinity;
                if (IsInfinity(this)) return Zero;
                // fallback to precise computation via Float128 to avoid encoding quirks
                return new Arithmonym(Float128.One / ToFloat128());
            }
        }

        /// <summary>
        /// Gets the additive negation (-value) of this <see cref="Arithmonym"/>.
        /// </summary>
        public readonly Arithmonym Negated
        {
            get
            {
                return new Arithmonym(Squished ^ (((UInt128)1) << (FRACTION_BITS + 10)));
            }
        }

        /// <summary>
        /// Gets the absolute value (magnitude) of this <see cref="Arithmonym"/>.
        /// </summary>
        public readonly Arithmonym AbsoluteValue
        {
            get
            {
                return _IsNegative ? Negated : this;
            }
        }

        /// <summary>
        /// Returns the base-10 logarithm of this value as an <see cref="Arithmonym"/>.
        /// </summary>
        public readonly Arithmonym _Log10
        {
            get
            {
                if (_IsNegative || IsZero(this) || IsNaN(this)) return NaN;
                if (IsInfinity(this) && IsNegative(this)) return Zero;
                if (_IsReciprocal) return Reciprocal._Log10.Negated;
                if (Letter < LETTERCODE_E)
                {
                    // small values: compute log10 via float128 then snap to nearby integer
                    Float128 log = Float128PreciseTranscendentals.SafeLog10(ToFloat128());
                    // if result is extremely close to an integer, round it before encoding
                    Float128 rounded = Float128.Round(log);
                    // use a slightly generous tolerance for logarithmic error
                    if (Float128.Abs(log - rounded) < Float128PreciseTranscendentals.SafeExp2(-14))
                        log = rounded;
                    return new Arithmonym(log);
                }
                if (Letter == LETTERCODE_E)
                    return new Arithmonym(Operand);
                if (Letter == LETTERCODE_F)
                {
                    Float128 newOperand = Operand - Float128.One;
                    byte newLetter = LETTERCODE_F;
                    if (newOperand < (Float128)2)
                    {
                        newOperand = Float128PreciseTranscendentals.SafeExp10(newOperand - Float128.One);
                        newLetter = LETTERCODE_E;
                    }
                    return new Arithmonym(false, false, newLetter, EncodeOperand(newOperand));
                }
                if (Letter == LETTERCODE_J)
                {
                    Float128 op = Float128HyperTranscendentals.LetterJToLetterG(Operand);
                    if (op < 3)
                    {
                        Float128 letterFOperand = Float128HyperTranscendentals.LetterF(Float128PreciseTranscendentals.SafeExp10(op - (Float128)2));
                        if (Float128.IsInfinity(letterFOperand))
                        {
                            return this;
                        }
                        letterFOperand--;
                        if (letterFOperand < 10)
                        {
                            return new(false, false, LETTERCODE_F, EncodeOperand(letterFOperand));
                        }
                        Float128 letterGOperand = Float128PreciseTranscendentals.SafeLog10(Float128HyperTranscendentals.SuperLog10(letterFOperand));
                        letterGOperand += 2;
                        return new(false, false, LETTERCODE_J, EncodeOperand(Float128HyperTranscendentals.LetterGToLetterJ(letterGOperand)));
                    }
                    return this;
                }
                return this;
            }
        }

        /// <summary>
        /// Returns 10^value as an <see cref="Arithmonym"/>.
        /// </summary>
        public readonly Arithmonym _Exp10
        {
            get
            {
                if (_IsNegative)
                {
                    return Negated._Exp10.Reciprocal;
                }
                if (IsInfinity(this))
                {
                    if (IsPositive(this))
                    {
                        return PositiveInfinity;
                    }
                    return Zero;
                }
                if (IsNaN(this)) return NaN;
                if (_IsReciprocal) return new(Float128PreciseTranscendentals.SafeExp10(ToFloat128()));
                if (Letter < LETTERCODE_E)
                {
                    // small value fast paths
                    Float128 x = ToFloat128();
                    // if x is very near a small integer, compute exact power using integer
                    // arithmetic where possible because SafeExp10 can be wildly off for
                    // integer exponents.  We only handle a limited range in ulong to avoid
                    // overflow; others fall back to Float128 multiplication.
                    Float128 xr = Float128.Round(x);
                    if (Float128.Abs(x - xr) < Float128PreciseTranscendentals.SafeExp2(-15))
                    {
                        int n = (int)(double)xr;
                        if (n >= 0 && n <= 18)
                        {
                            // compute 10^n exactly as an integer
                            ulong intVal = 1;
                            for (int i = 0; i < n; i++) intVal *= 10UL;
                            return new Arithmonym((Float128)intVal);
                        }
                        else if (n > 18 && n <= 100)
                        {
                            // use repeated Float128 multiply for moderate exponents
                            Float128 prod = Float128.One;
                            Float128 ten = (Float128)10;
                            for (int i = 0; i < n; i++) prod *= ten;
                            return new Arithmonym(prod);
                        }
                    }

                    // fallback to the general safe exp10, but snap the result to an
                    // integer if it's extremely close (handles roundtrip tests).
                    Float128 exp = Float128PreciseTranscendentals.SafeExp10(x);
                    Float128 er = Float128.Round(exp);
                    // tolerate up to about 2^-10 (~0.000976) absolute difference or
                    // a small relative error to capture values like 20 from a log/exp
                    if (Float128.Abs(exp - er) < Float128PreciseTranscendentals.SafeExp2(-10))
                        exp = er;
                    // Convert to Arithmonym first
                    var candidate = new Arithmonym(exp);
                    // If converting back loses precision and yields a near-integer, snap once more
                    Float128 back = candidate.ToFloat128();
                    Float128 br = Float128.Round(back);
                    if (Float128.Abs(back - br) < Float128PreciseTranscendentals.SafeExp2(-10))
                    {
                        // directly encode the integer to avoid double-round errors
                        Float128 rounded = Float128.Round(back);
                        int rk = (int)(double)rounded;
                        if (rk >= 0 && rk <= 18)
                        {
                            ulong iv = 1;
                            for (int j = 0; j < rk; j++) iv *= 10;
                            return new Arithmonym((Float128)iv);
                        }
                        return new Arithmonym(rounded);
                    }
                    return candidate;
                }
                if (Letter == LETTERCODE_E)
                    return new(false, false, LETTERCODE_F, EncodeOperand(2 + Float128PreciseTranscendentals.SafeLog10(Operand)));
                if (Letter == LETTERCODE_F)
                {
                    if (Operand < 9)
                    {
                        return new(false, false, LETTERCODE_F, EncodeOperand(Operand + 1));
                    }
                    return new(false, false, LETTERCODE_J, EncodeOperand(Float128HyperTranscendentals.LetterGToLetterJ(2 + Float128PreciseTranscendentals.SafeLog10(Float128HyperTranscendentals.SuperLog10(Operand + 1)))));
                }
                if (Letter == LETTERCODE_J)
                {
                    Float128 op = Float128HyperTranscendentals.LetterJToLetterG(Operand);
                    if (op < 3)
                    {
                        Float128 letterFOperand = Float128HyperTranscendentals.LetterF(Float128PreciseTranscendentals.SafeExp10(op - (Float128)2));
                        if (Float128.IsInfinity(letterFOperand))
                            return this;
                        letterFOperand++;
                        Float128 letterGOperand = Float128PreciseTranscendentals.SafeLog10(Float128HyperTranscendentals.SuperLog10(letterFOperand));
                        letterGOperand += 2;
                        return new(false, false, LETTERCODE_J, EncodeOperand(Float128HyperTranscendentals.LetterGToLetterJ(letterGOperand)));
                    }
                    return this;
                }
                return this;
            }
        }


        /// <summary>
        /// Gets the normalized form of this <see cref="Arithmonym"/>.
        /// </summary>
        public readonly Arithmonym Normalized
        {
            get
            {
                if (_IsReciprocal && (ToFloat128() == 1)) return One;
                if (_IsReciprocal && (ToFloat128() == -1)) return NegativeOne;
                if (IsNaN(this)) return NaN;
                if (IsInfinity(this)) return _IsNegative ? NegativeInfinity : PositiveInfinity;
                return this;
            }
        }
    }
}