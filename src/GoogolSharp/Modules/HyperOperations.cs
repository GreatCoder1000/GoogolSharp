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

using System.Xml.XPath;
using GoogolSharp.Helpers;
using SQLitePCL;
using Float128 = QuadrupleLib.Float128<QuadrupleLib.Accelerators.DefaultAccelerator>;

namespace GoogolSharp
{
    partial struct Arithmonym
    {
        // Convergence and tolerance constants
        private const double CONVERGENCE_TOLERANCE = 1e-10;
        private const double TIGHT_CONVERGENCE = 1e-12;
        private const double ANALYTICAL_BASE_TOLERANCE = 1e-3;
        private const double PENTATION_LARGE_HEIGHT = 100;
        private const int NEWTON_ITERATION_STEPS = 12;

        // Polynomial coefficients for InterestingCurve (10^^(value-1) analytical approximation)
        private const double CURVE_COEFF_LINEAR = 1.45373;
        private const double CURVE_COEFF_QUADRATIC = -0.4618;
        private const double CURVE_COEFF_CUBIC = 0.0080691;

        // Polynomial coefficients for AnotherInterestingCurve (10^^^(value-1) analytical approximation)
        private static class AnotherInterestingCurveCoefficients
        {
            public const double C10 = -1.58961365e+04;
            public const double C9 = 1.00218925e+05;
            public const double C8 = -2.76624816e+05;
            public const double C7 = 4.38599788e+05;
            public const double C6 = -4.40413666e+05;
            public const double C5 = 2.91056375e+05;
            public const double C4 = -1.27363047e+05;
            public const double C3 = 3.61665463e+04;
            public const double C2 = -6.34231255e+03;
            public const double C1 = 6.29537046e+02;
            public const double C0_2 = -3.30294377e+01;
            public const double C_1 = 2.83793626e+00;
            public const double C_0 = -2.00790645e-03;
        }

        public static Arithmonym Hyper(Arithmonym a, Arithmonym b, int c)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(c, 1);
            if (c == 1) return a + b;
            if (c == 2) return a * b;
            if (c == 3) return Pow(a, b);
            if (c == 4) return Tetration(a, b);
            if (c == 5) return Pentation(a, b);

            // TODO
            throw new NotImplementedException();
        }

        public static Arithmonym Pentation(Arithmonym baseV, Arithmonym heightV)
            => Pentation(baseV, heightV, analytical: false);

        public static Arithmonym Pentation(Arithmonym baseV, Arithmonym heightV, bool analytical)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(baseV, Zero);

            // Handle special base cases
            if (HandlePentationZeroBase(heightV, out var zeroBaseResult))
                return zeroBaseResult;
            
            if (baseV == One) return One;
            if (IsZero(heightV)) return One;
            if (heightV == One) return baseV;

            // Validate analytical mode
            if (analytical && Abs(baseV - Ten) > (Arithmonym)ANALYTICAL_BASE_TOLERANCE)
                throw new Exception("Can't pentate analytically for bases other than 10");

            // Handle extreme negative heights
            if (heightV < -PENTATION_LARGE_HEIGHT)
                return Parse("-1.8414056604369606378466046580124861060503713143776396", null);

            // Handle negative heights recursively
            if (heightV < Zero)
                return HandlePentationNegativeHeight(baseV, heightV, analytical);

            // Handle small positive heights via tetration recursion
            if (heightV < Ten)
                return Tetration(baseV, Pentation(baseV, heightV - One, analytical));

            // General case: decompose and iterate via TetraTower
            Arithmonym integerHeight = Floor(heightV);
            Arithmonym fractionalHeight = heightV - integerHeight;
            return TetraTower(baseV, ++integerHeight, fractionalHeight, analytical);
        }

        private static bool HandlePentationZeroBase(Arithmonym heightV, out Arithmonym result)
        {
            result = default;
            ArgumentOutOfRangeException.ThrowIfLessThan(heightV, Zero);
            
            if (!IsZero(heightV))
                return false;
                
            if (IsEvenInteger(heightV)) { result = One; return true; }
            if (IsOddInteger(heightV)) { result = Zero; return true; }
            throw new Exception("0^^^n : n ∈ R and n ∉ Z is undefined.");
        }

        private static Arithmonym HandlePentationNegativeHeight(Arithmonym baseV, Arithmonym heightV, bool analytical)
        {
            if (heightV < NegativeOne)
                return Slog10(Pentation(baseV, heightV + One, analytical), analytical);

            // heightV in [-1, 0), use analytical approximation if requested
            return analytical ? AnotherInterestingCurve(heightV + One) : heightV + One;
        }

        public static Arithmonym Tetration(Arithmonym baseV, Arithmonym heightV)
            => Tetration(baseV, heightV, analytical: false);

        public static Arithmonym Tetration(Arithmonym baseV, Arithmonym heightV, bool analytical)
        {
            // Handle edge cases and special values
            ValidateTetrationInputs(baseV, heightV);
            if (baseV == One) return One;
            if (heightV == Zero) return One;
            if (heightV == One) return baseV;

            // Attempt fast path computation for non-analytical tetration
            if (!analytical)
            {
                if (TryTetrationFastPath(baseV, heightV, out var fastResult))
                    return fastResult;
            }

            // Validate and adjust analytical mode
            bool useAnalytical = ValidateAnalyticalMode(baseV, analytical);

            // Handle negative heights in analytical mode
            if (heightV < Zero && useAnalytical)
                return Tetration(baseV, heightV + 1, useAnalytical)._Log10 + baseV._Log10;

            // General case: decompose height into integer + fractional parts
            Arithmonym integerHeight = Floor(heightV);
            Arithmonym fractionalHeight = heightV - integerHeight;
            if (useAnalytical) 
                fractionalHeight = InterestingCurve(fractionalHeight);

            return PowerTower(baseV, ++integerHeight, fractionalHeight);
        }

        private static void ValidateTetrationInputs(Arithmonym baseV, Arithmonym heightV)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(baseV, Zero);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(heightV, NegativeTwo);
            if (IsZero(baseV))
            {
                if (IsEvenInteger(heightV)) return;
                if (IsOddInteger(heightV)) return;
                throw new Exception("0^^n : n ∈ R and n ∉ Z is undefined.");
            }
        }

        private static bool TryTetrationFastPath(Arithmonym baseV, Arithmonym heightV, out Arithmonym result)
        {
            result = default;
            
            if (heightV <= NegativeOne) { result = (heightV + Two)._Log10 / baseV._Log10; return true; }
            if (heightV <= Zero) { result = heightV + One; return true; }
            if (heightV <= One) { result = Pow(baseV, heightV); return true; }
            if (heightV <= Two) { result = Pow(baseV, Pow(baseV, heightV - One)); return true; }
            if (heightV <= Three) { result = Pow(baseV, Pow(baseV, Pow(baseV, heightV - Two))); return true; }
            if (heightV <= Four) { result = Pow(baseV, Pow(baseV, Pow(baseV, Pow(baseV, heightV - Three)))); return true; }
            
            return false;
        }

        private static bool ValidateAnalyticalMode(Arithmonym baseV, bool requestAnalytical)
        {
            if (!requestAnalytical) return false;
            
            if (Abs(baseV - Ten) <= (Arithmonym)ANALYTICAL_BASE_TOLERANCE) return true;
            if (Abs(baseV - E) <= (Arithmonym)ANALYTICAL_BASE_TOLERANCE) return true;
            
            throw new Exception("Can't tetrate analytically for bases other than 10 or e");
        }

        public static Arithmonym TetraTower(Arithmonym a, Arithmonym b, Arithmonym c, bool analytical = false)
        {
            // This way to do it "works" but can very quickly lose precision.
            // Trying to find a better way.
            Arithmonym iterationCount = b;
            Arithmonym result = c;
            for (int i = 0; i < iterationCount; i++)
            {
                Arithmonym newResult = Tetration(a, result, analytical);

                // Shortcut.
                if (Abs(newResult - result) < 1e-10) break;

                // We can guess the outcome from here
                if (Abs(Slog10Linear(newResult) - result) < 1e-10)
                {
                    return result.AddToItsPlog(iterationCount - i, analytical);
                }
                result = newResult;
            }
            return result;
        }

        /// <summary>
        /// Computes a^a^a^...^a (b times, starting from c)
        /// Example: PowerTower(6, 7, 0) computes 6^6^6^6^6^6^6 via iteration
        /// </summary>
        public static Arithmonym PowerTower(Arithmonym baseValue, Arithmonym iterationCount, Arithmonym startingValue)
        {
            Arithmonym currentValue = startingValue;
            
            for (int iteration = 0; iteration < iterationCount; iteration++)
            {
                Arithmonym nextValue = Pow(baseValue, currentValue);

                // Check if sequence has converged to a fixed point (newValue ≈ currentValue)
                // This is valid for bases < e^e^-1 ≈ 1.444
                if (HasConvergedToFixedPoint(nextValue, currentValue))
                    break;

                // Check if the logarithm has converged, indicating we can use AddToItsSlog acceleration
                if (ShouldUseAcceleration(nextValue, currentValue))
                {
                    return currentValue.AddToItsSlog(iterationCount - iteration, false);
                }
                
                currentValue = nextValue;
            }
            
            return currentValue;
        }

        private static bool HasConvergedToFixedPoint(Arithmonym nextValue, Arithmonym currentValue)
        {
            // Convergence tolerance: if the tower stops growing, we've found the fixed point
            return Abs(nextValue - currentValue) < CONVERGENCE_TOLERANCE;
        }

        private static bool ShouldUseAcceleration(Arithmonym nextValue, Arithmonym currentValue)
        {
            // Acceleration technique: if log10 has converged, use AddToItsSlog
            bool logConverged = Abs(nextValue._Log10 - currentValue) < CONVERGENCE_TOLERANCE;
            bool nextIsNotOne = nextValue != One;
            bool currentIsNotZero = currentValue != Zero;
            
            return logConverged && nextIsNotOne && currentIsNotZero;
        }

        private static Arithmonym InterestingCurve(Arithmonym value)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, Zero);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, One);
            if (value == One) return One;

            // Polynomial approximating 10^^(value - 1) analytically
            return value * 1.45373 + value * value * -0.4618 + value * value * value * 0.0080691;
        }
        private static Arithmonym InterestingCurveInverse(Arithmonym y)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(y, Zero);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(y, One);
            if (y == Zero) return Zero;
            if (y == One) return One;

            // Coefficients of the cubic
            Arithmonym a = 0.0080691;
            Arithmonym b = -0.4618;
            Arithmonym c = 1.45373;

            // Newton iteration starting guess
            Arithmonym x = y; // good initial guess since f(x) ~ x

            for (int i = 0; i < 12; i++)
            {
                // f(x)
                Arithmonym fx = c * x + b * x * x + a * x * x * x;

                // f'(x)
                Arithmonym fpx = c + 2 * b * x + 3 * a * x * x;

                // Newton step
                Arithmonym dx = (fx - y) / fpx;
                x -= dx;

                // Converged?
                if (Abs(dx) < 1e-12) break;
            }

            // Clamp to [0,1] just in case of tiny numerical drift
            if (x < Zero) x = Zero;
            if (x > One) x = One;

            return x;
        }


        private static Arithmonym AnotherInterestingCurve(Arithmonym value)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, Zero);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, One);
            if (value == One) return One;

            // Polynomial approximating 10^^^(value - 1) analytically
            return
                ((((((((((
                    (-1.58961365e+04 * value + 1.00218925e+05)
                    * value - 2.76624816e+05)
                    * value + 4.38599788e+05)
                    * value - 4.40413666e+05)
                    * value + 2.91056375e+05)
                    * value - 1.27363047e+05)
                    * value + 3.61665463e+04)
                    * value - 6.34231255e+03)
                    * value + 6.29537046e+02)
                    * value - 3.30294377e+01)
                    * value + 2.83793626e+00)
                    * value - 2.00790645e-03;
        }

        private static Arithmonym AnotherInterestingCurveInverse(Arithmonym y)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(y, Zero);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(y, One);
            if (y == Zero) return Zero;
            if (y == One) return One;

            // Polynomial coefficients (degree 12 → degree 0)
            Arithmonym[] c =
            {
        -1.58961365e+04,
         1.00218925e+05,
        -2.76624816e+05,
         4.38599788e+05,
        -4.40413666e+05,
         2.91056375e+05,
        -1.27363047e+05,
         3.61665463e+04,
        -6.34231255e+03,
         6.29537046e+02,
        -3.30294377e+01,
         2.83793626e+00,
        -2.00790645e-03
    };

            // Derivative coefficients
            Arithmonym[] d = new Arithmonym[c.Length - 1];
            for (int i = 0; i < d.Length; i++)
                d[i] = c[i] * (c.Length - 1 - i);

            // Newton iteration
            Arithmonym x = y; // good initial guess for monotonic functions

            for (int i = 0; i < 20; i++)
            {
                // Evaluate f(x) using Horner's method
                Arithmonym fx = 0;
                for (int j = 0; j < c.Length; j++)
                    fx = fx * x + c[j];

                // Evaluate f'(x)
                Arithmonym fpx = 0;
                for (int j = 0; j < d.Length; j++)
                    fpx = fpx * x + d[j];

                Arithmonym dx = (fx - y) / fpx;
                x -= dx;

                if (Abs(dx) < 1e-14)
                    break;
            }

            // Clamp to [0,1]
            if (x < Zero) x = Zero;
            if (x > One) x = One;

            return x;
        }

        /// <summary>
        /// Adds integer values to the super-logarithm (slog) of this number.
        /// Interprets the result as 10↑↑(slog(this) + value).
        /// </summary>
        private Arithmonym AddToItsSlog(Arithmonym integersToAdd, bool analytical)
        {
            // Small integer optimizations: compute directly via repeated Exp10
            if (integersToAdd == One) return _Exp10;
            if (integersToAdd == Two) return _Exp10._Exp10;
            if (integersToAdd == Three) return _Exp10._Exp10._Exp10;
            if (integersToAdd == Four) return _Exp10._Exp10._Exp10._Exp10;
            if (integersToAdd == Five) return _Exp10._Exp10._Exp10._Exp10._Exp10;
            if (integersToAdd == Six) return _Exp10._Exp10._Exp10._Exp10._Exp10._Exp10;

            // General case: slog(result) = slog(this) + integersToAdd
            Arithmonym resultSlog = Slog10(this, analytical) + integersToAdd;
            if (analytical)
                resultSlog = InverseAnalyticify(resultSlog);
            
            return Tetration10Linear(resultSlog);
        }

        private Arithmonym AddToItsPlog(Arithmonym value, bool analytical)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, Zero);
            // 1.AddToItsPlog(1) = 10
            // 10.AddToItsPlog(1) = 10^^10
            // 10^^10.AddToItsPlog(1) = 10^^(10^^10)
            if (value == One) return analytical ? Tetration10Linear(InverseAnalyticify(value)) : Tetration10Linear(value);
            Arithmonym plog = Nlog10(this, 3) + value;
            return analytical ? AntiNlog10(InversePentaAnalyticify(plog), 3) : AntiNlog10(plog, 3);
        }

        private Arithmonym AddToItsNlog(Arithmonym value, int n)
        {
            // For now this only does addition, no subtraction yet
            ArgumentOutOfRangeException.ThrowIfLessThan(value, 0);
            ArgumentOutOfRangeException.ThrowIfLessThan(n, 1);
            if (n == 1) return this * value._Exp10;
            if (IsZero(value)) return this;
            if (n == 2) return AddToItsSlog(value, false);
            if (n == 3) return AddToItsPlog(value, false);
            throw new NotImplementedException("TODO");
        }

        public static Arithmonym Slog10(Arithmonym value, bool analytical)
        => analytical ? Analyticify(Slog10(value)) : Slog10(value);

        /// <summary>
        /// Returns the <paramref name="n"/>-hyperlogarithm of <paramref name="value"/>
        /// </summary>
        /// <param name="value">The number to take the hyperlogarithm of</param>
        /// <param name="n">The number of arrows to inverse</param>
        /// <returns>The result of taking the <paramref name="n"/>-hyperlogarithm of <paramref name="value"/></returns>
        /// <exception cref="NotImplementedException">This annoying part is not implemented yet. It's an edge case.</exception>
        public static Arithmonym Nlog10(Arithmonym value, int n)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(n, 1);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(n, 9);
            if (n == 1) return value._Log10;
            if (n == 2) return Slog10(value);
            if (value.Letter == LETTERCODE_J && (n == (value.OperandFloored + 1)))
                return (Arithmonym)Float128PreciseTranscendentals.SafePow(5, value.Operand + 1 - n) * 2;
            if (value.Letter == LETTERCODE_J && (n == value.Operand))
                return Ten;
            if (value.Letter < LETTERCODE_F)
            {
                if (n == 3)
                    return (Arithmonym)Float128HyperTranscendentals.InverseLetterG((Float128)value);
                if (value < 1)
                {
                    /* This depends on n... I'm way too lazy to implement this */
                    throw new NotImplementedException("Don't call the general hyperlogarithm with value < 1");
                }
                if (value <= 10) return value._Log10;

                // This solution works. So please, don't question it. Remember we assume value is a small number here.
                Arithmonym result = Slog10(value);
                for (int i = 2; i < n; i++)
                    result = One + result._Log10;
                return result;
            }

            // value>=10^^10, n>=3
            // Now we enumerate the possibility value.OperandFloored + 1 < n
            // like value=10^^100, n==4 for instance
            // 10^^100 = 10^^10^^1.301 = 10^^10^^10^^0.114 = 10^^^2.114;
            // Operand is in 2 to 10 range so we can do that interesting 1+log thing
            // 10^^^2.114 = 10^^^10^^^0.
            if (value.Letter == LETTERCODE_J)
            {
                if (n > (value.OperandFloored + 1))
                {
                    Arithmonym a = (Arithmonym)(2 * Float128PreciseTranscendentals.SafePow(5, value.Operand - value.OperandFloored));
                    for (int i = value.OperandFloored + 1; i < n; i++)
                        a = 1 + a._Log10;
                    return a;
                }
                if (n == value.OperandFloored)
                {
                    // value = 10^^^^3
                    // n = 3
                    // value = 10^^^^2
                    Arithmonym a = (Arithmonym)(2 * Float128PreciseTranscendentals.SafePow(5, value.Operand - value.OperandFloored));
                    if (a >= 3) return new(false, false, LETTERCODE_J, EncodeOperand(n + Float128PreciseTranscendentals.SafeLog((a - 1) / 2) / Float128PreciseTranscendentals.SafeLog(5)));
                    // 10^^^^2.5 = 10^^^10^^^(10^0.5)
                    return new(false, false, LETTERCODE_J, EncodeOperand(-1 + n + Float128PreciseTranscendentals.SafeLog(Float128PreciseTranscendentals.SafeExp10(a - 2) / 2) / Float128PreciseTranscendentals.SafeLog(5)));
                }
            }
            // number is too big to be affected by nlog unless rare edge case
            return value;
        }

        public static Arithmonym AntiNlog10(Arithmonym value, int n)
        {
            // if n == 1 this function is 10^value
            // if n == 2 this function is 10^^value
            // if n == 3 this function is 10^^^value
            // and so on
            ArgumentOutOfRangeException.ThrowIfLessThan(n, 1);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(n, 9);
            ArgumentOutOfRangeException.ThrowIfLessThan(value, 0);
            if (n == 1) return value._Exp10;
            if (n == 2) return Tetration10Linear(value);

            // value is between 0 and 1.
            // in this case the answer is the same as 10^value
            // no matter what n is
            if (value <= One) return value._Exp10;

            // now value should be >1
            // and n >=3 && n < 9
            if (value == Two) return AntiNlog10(Ten, --n);

            // 10^^^^1.5 = 10^^^(10^^^^0.5) = 10^^^(10^0.5)
            if (value < Two) return AntiNlog10(value._Exp10, --n);
            if (value < Ten) return AntiNlog10(AntiNlog10(value, n - 1), n);

            // TODO
            throw new NotImplementedException();
        }

        public static Arithmonym Slog10(Arithmonym value)
        {
            // Small values: use QuadrupleLib's SuperLog10 for high precision
            if (value < TenBillion)
                return new(Float128HyperTranscendentals.SuperLog10(value));

            // F-letter range: value is 10^^x where x is representable as the operand
            if (value < Dekalogue)
                return new(value.Operand);

            // G/J-letter range: convert through intermediate representations
            if (value < Triapetaksys)
                return ComputeSlog10ForLargeValues(value);

            // Unreachable: value too large to compute further
            return value;
        }

        private static Arithmonym ComputeSlog10ForLargeValues(Arithmonym value)
        {
            // Extract the letter-G representation from the F-letter value
            Float128 letterGValue;
            if (value < Dekateraksys)
            {
                // value is in F-letter range: convert F operand to letter-G
                letterGValue = Float128PreciseTranscendentals.SafePow(5, value.Operand - 2) * 2;
            }
            else
            {
                // value is in J-letter range: convert J operand back to letter-G for analysis
                letterGValue = Float128HyperTranscendentals.LetterG(
                    Float128PreciseTranscendentals.SafeExp10(
                        Float128PreciseTranscendentals.SafePow(5, value.Operand - 3) * 2
                    )
                );
            }

            // Adjust for the conversion formula
            letterGValue--;
            
            if (Float128.IsInfinity(letterGValue))
                return value;
            
            // Recursively convert back to tetration form
            if (letterGValue < 2)
                return Tetration10Linear((Arithmonym)Float128PreciseTranscendentals.SafeExp10(letterGValue - 1));

            // Return as J-letter encoding
            return new(
                false, 
                false, 
                LETTERCODE_J, 
                EncodeOperand(2 + (Float128PreciseTranscendentals.SafeLog(letterGValue / 2) / Float128PreciseTranscendentals.SafeLog(5)))
            );
        }

        private static Arithmonym Analyticify(Arithmonym value)
        {
            // Not gonna do anything to change stuff after decimal point to a number this large...
            if (value > Trialogue) return value;
            Arithmonym breakdown_floor = Floor(value);
            Arithmonym breakdown_mod = value - breakdown_floor;
            return breakdown_floor + InterestingCurve(breakdown_mod);
        }

        private static Arithmonym InverseAnalyticify(Arithmonym value)
        {
            // Not gonna do anything to change stuff after decimal point to a number this large...
            if (value > Trialogue) return value;
            Arithmonym breakdown_floor = Floor(value);
            Arithmonym breakdown_mod = value - breakdown_floor;
            return breakdown_floor + InterestingCurveInverse(breakdown_mod);
        }

        private static Arithmonym InversePentaAnalyticify(Arithmonym value)
        {
            // Not gonna do anything to change stuff after decimal point to a number this large...
            if (value > Trialogue) return value;
            Arithmonym breakdown_floor = Floor(value);
            Arithmonym breakdown_mod = value - breakdown_floor;
            return breakdown_floor + AnotherInterestingCurveInverse(breakdown_mod);
        }

        public static Arithmonym Slog10Linear(Arithmonym value)
        {
            // Small values: use QuadrupleLib's SuperLog10 for high precision
            if (value < TenBillion)
                return new(Float128HyperTranscendentals.SuperLog10(value));

            // F-letter range: value is 10^^x where x is representable as the operand
            if (value < Dekalogue)
                return new(value.Operand);

            // G/J-letter range: convert through intermediate representations (same as non-linear version)
            if (value < Triapetaksys)
                return ComputeSlog10ForLargeValues(value);

            // Unreachable: value too large to compute further
            return value;
        }

        public static Arithmonym Tetration10Linear(Arithmonym exponent)
        {
            // Small exponents: use LetterF helper for x in [0, 2)
            if (exponent < Two)
                return new(Float128HyperTranscendentals.LetterF(exponent));

            // Medium exponents: F-letter encoding for x in [2, 10)
            if (exponent < Ten)
                return new(false, false, LETTERCODE_F, EncodeOperand(exponent.ToFloat128()));

            // Large exponents: convert F-representation through G-representation to J-representation
            if (exponent < Dekateraksys)
                return ConvertToJLetterViaPaths(exponent);

            // Very large exponents: multi-step conversion through intermediate forms
            if (exponent < Triapetaksys)
                return ConvertVeryLargeExponentToJLetter(exponent);

            // Unreachable: exponent too large
            return exponent;
        }

        private static Arithmonym ConvertToJLetterViaPaths(Arithmonym exponent)
        {
            // Convert through: F-letter (exponent) -> G-representation -> J-letter
            Arithmonym asG = LetterFToLetterG(exponent);
            Arithmonym asJ = LetterGToLetterJ(asG);
            
            return new(
                false,
                false,
                LETTERCODE_J,
                EncodeOperand(asJ.ToFloat128())
            );
        }

        private static Arithmonym ConvertVeryLargeExponentToJLetter(Arithmonym exponent)
        {
            // For exponents >= 10^10^10, use interval arithmetic to constrain letter-H
            Float128 letterHValue = Float128PreciseTranscendentals.SafePow(5, exponent.Operand - 3) * 2;
            
            if (letterHValue < 2)
                letterHValue = 2;
            if (letterHValue >= 3)
                letterHValue = 3 - Float128.Epsilon;

            // Convert letter-H through letter-G to letter-J
            Float128 letterGValue = Float128HyperTranscendentals.LetterG(
                Float128PreciseTranscendentals.SafeExp10(letterHValue - 2)
            );

            if (Float128.IsInfinity(letterGValue))
                return exponent;

            letterGValue++;

            // Final conversion formula: letterH = 2 + log10(InverseLetterG(letterG))
            letterHValue = 2 + Float128PreciseTranscendentals.SafeLog10(
                Float128HyperTranscendentals.InverseLetterG(letterGValue)
            );

            return new(
                false,
                false,
                LETTERCODE_J,
                EncodeOperand(3 + (Float128PreciseTranscendentals.SafeLog(letterHValue / 2) / Float128PreciseTranscendentals.SafeLog(5)))
            );
        }

        /// <summary>
        /// Converts a number from F-letter representation (10^^x) to G-letter representation (10^^^y).
        /// Domain: exponent in [0, 10^10)
        /// </summary>
        private static Arithmonym LetterFToLetterG(Arithmonym exponent)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(exponent, Zero);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(exponent, Dekalogue);
            
            if (exponent < One)
                return exponent;  // 10^x -> G representation is still x for small x
            
            if (exponent < Ten)
                return exponent._Log10 + 1;  // 10^^(10^a) = 10^^^(Log10(10^a)+1) = 10^^^(a+1)
            
            // Large exponents: use logarithm chain
            if (exponent < TenBillion)
                return ((exponent._Log10._Log10 + One)._Log10 + Two);
            
            // Very large exponents: directly compute from F-letter operand
            return new Arithmonym(Float128PreciseTranscendentals.SafeLog10(exponent.Operand)) + Two;
        }

        /// <summary>
        /// Converts a number from G-letter representation (10^^^y) to J-letter representation.
        /// Domain: representation value in [0, 10)
        /// </summary>
        private static Arithmonym LetterGToLetterJ(Arithmonym gRepresentation)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(gRepresentation, Ten);
            
            if (gRepresentation <= Two)
                return gRepresentation;  // No conversion needed for small G values
            
            // Logarithmic conversion formula for larger G values
            return (gRepresentation / Two)._Log10 / Five._Log10;
        }

        /// <summary>
        /// Friedman's TREE function. Notable number: TREE(3)
        /// </summary>        
        public static Arithmonym Tree(Arithmonym x) => new ArithmonymTree(x).Evaluate();


        /// <summary>
        /// Simple SubCubic Graph function. Notable number: SSCG(3)
        /// </summary>
        public static Arithmonym Sscg(Arithmonym x) => new ArithmonymSscg(x).Evaluate();

        /// <summary>
        /// SubCubic Graph function. Notable number: SCG(13)
        /// </summary>
        public static Arithmonym Scg(Arithmonym x) => new ArithmonymScg(x).Evaluate();

        /// <summary>
        /// Busy Beaver function (Sigma, not the frantic frog)
        /// 
        /// Learn more: https://googology.fandom.com/wiki/Busy_beaver_function
        /// </summary>
        public static Arithmonym BusyBeaver(Arithmonym x) => new ArithmonymBusyBeaver(x).Evaluate();

        /// <summary>
        /// Psi Level of x. Note that x is treated as an integer, so 4.2 -> 4.
        /// 
        /// Learn more: https://googology.fandom.com/wiki/User_blog:PsiCubed2/For_Newbies_(and_Veterans_too):_The_Great_Scale_of_Googology
        /// </summary>
        /// <param name="x">The value</param>
        /// <returns>An <see cref="Arithmonym"/> that returns approximately the psi level of x.</returns>
        public static Arithmonym PsiLevel(Arithmonym x) => new ArithmonymPsiLevel(x).Evaluate();
    }
}