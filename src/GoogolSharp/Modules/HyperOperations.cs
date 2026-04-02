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
            throw new NotImplementedException("Not Implemented Yet.");
        }

        public static Arithmonym Pentation(Arithmonym baseV, Arithmonym heightV)
            => Pentation(baseV, heightV, analytical: false);

        public static Arithmonym Pentation(Arithmonym baseV, Arithmonym heightV, bool analytical)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(baseV, Zero);
            if (IsZero(baseV))
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(heightV, Zero);
                if (IsEvenInteger(heightV)) return One;
                if (IsOddInteger(heightV)) return Zero;
                throw new Exception("0^^^n : n ∈ R and n ∉ Z is undefined.");
            }
            if (baseV == One) return One;
            if (IsZero(heightV)) return One;
            if (heightV == One) return baseV;
            if (Abs(baseV - Ten) > (Arithmonym)ANALYTICAL_BASE_TOLERANCE && analytical)
                throw new Exception("Can't pentate analytically for bases other than 10");
            if (heightV < -PENTATION_LARGE_HEIGHT) return Parse("-1.8414056604369606378466046580124861060503713143776396", null);
            if (heightV < NegativeOne) return Slog10(Pentation(baseV, heightV + One, analytical), analytical);
            if (heightV < Zero) return analytical ? AnotherInterestingCurve(heightV + One) : heightV + One;
            // curve = height -1 to 0
            if (heightV < Ten) return Tetration(baseV, Pentation(baseV, heightV - One, analytical));

            // TODO!! [tip: either iterate Tetration or create a custom TetraTower impl]"
            throw new NotImplementedException();
        }

        public static Arithmonym Tetration(Arithmonym baseV, Arithmonym heightV)
            => Tetration(baseV, heightV, analytical: false);

        public static Arithmonym Tetration(Arithmonym baseV, Arithmonym heightV, bool analytical)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(baseV, Zero);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(heightV, NegativeTwo);
            if (IsZero(baseV))
            {
                if (IsEvenInteger(heightV)) return One;
                if (IsOddInteger(heightV)) return Zero;
                throw new Exception("0^^n : n ∈ R and n ∉ Z is undefined.");
            }
            if (baseV == One) return One;
            if (heightV == Zero) return One;
            if (heightV == One) return baseV;

            bool shouldUseAnalytical = analytical;
            if (Abs(baseV - Ten) > (Arithmonym)ANALYTICAL_BASE_TOLERANCE && analytical)
            { if (Abs(baseV - E) > (Arithmonym)ANALYTICAL_BASE_TOLERANCE) shouldUseAnalytical = false; else throw new Exception("Can't tetrate analytically for bases other than 10 or e"); }

            if (!shouldUseAnalytical)
            {
                // Fast path
                if (heightV <= NegativeOne) return (heightV + Two)._Log10 / baseV._Log10;
                if (heightV <= Zero) return heightV + One;
                if (heightV <= One) return Pow(baseV, heightV);
                if (heightV <= Two) return Pow(baseV, Pow(baseV, heightV - One));
                if (heightV <= Three) return Pow(baseV, Pow(baseV, Pow(baseV, heightV - Two)));
                if (heightV <= Four) return Pow(baseV, Pow(baseV, Pow(baseV, Pow(baseV, heightV - Three))));
            }

            if (heightV < Zero && shouldUseAnalytical)
                return Tetration(baseV, heightV + 1, shouldUseAnalytical)._Log10 + baseV._Log10;

            Arithmonym iterationCount = Floor(heightV);
            Arithmonym start = heightV - iterationCount;
            if (shouldUseAnalytical) start = InterestingCurve(start);
            return PowerTower(baseV, iterationCount, start);
        }

        public static Arithmonym TetraTower(Arithmonym a, Arithmonym b, Arithmonym c, bool analytical = false)
        {
            // This way to do it "works" but can very quickly lose precision.
            // Trying to find a better way.
            Arithmonym iterationCount = b;
            Arithmonym result = c;
            for (int i = 0; i <= iterationCount; i++)
            {
                Arithmonym newResult = Tetration(a, result, analytical);

                // Shortcut.
                if (Abs(newResult - result) < 1e-10) break;

                // We can guess the outcome from here
                if (Abs(Slog10Linear(newResult) - result) < 1e-10)
                {
                    return result.AddToItsNlog(iterationCount - i, 3, analytical);
                }
                result = newResult;
            }
            return result;
        }

        public static Arithmonym PowerTower(Arithmonym a, Arithmonym b, Arithmonym c)
        {
            // This way to do it "works" but can very quickly lose precision.
            // Trying to find a better way.

            // Example 6^6^6^6^6^6^6^0 a=6 b=7 c=0
            /*
             * Iteration 1: newResult = 1,
             */
            Arithmonym iterationCount = b;
            Arithmonym result = c;
            for (int i = 0; i <= iterationCount; i++)
            {
                Arithmonym newResult = Pow(a, result);

                // Shortcut. Doesn't work well if base is slightly greater than e^e^-1 though
                if (Abs(newResult - result) < 1e-10) break;

                // We can guess the outcome from here (!! only if newResult != One)
                // Bugfix: add &&newResult!=One
                if (Abs(newResult._Log10 - result) < 1e-10 && newResult != One && result != Zero)
                {
                    return result.AddToItsSlog(iterationCount - i, false);
                }
                result = newResult;
            }
            return result;
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

        private Arithmonym AddToItsSlog(Arithmonym value, bool analytical)
        {
            return AddToItsNlog(value, 2, analytical);
        }

        private Arithmonym AddToItsNlog(Arithmonym value, int n, bool analytical)
        {
            // For now this only does addition, no subtraction yet
            ArgumentOutOfRangeException.ThrowIfLessThan(value, 0);
            ArgumentOutOfRangeException.ThrowIfLessThan(n, 1);
            if (n == 1) return this * value._Exp10;
            if (IsZero(value)) return this;
            if (n == 2)
            {
                if (value == One) return _Exp10;
                if (value == Two) return _Exp10._Exp10;
                if (value == Three) return _Exp10._Exp10._Exp10;
                if (value == Four) return _Exp10._Exp10._Exp10._Exp10;
                if (value == Five) return _Exp10._Exp10._Exp10._Exp10._Exp10;
                if (value == Six) return _Exp10._Exp10._Exp10._Exp10._Exp10._Exp10;
            }
            Arithmonym slog = Slog10(this, analytical) + value;
            return analytical ? Tetration10Linear(InverseAnalyticify(slog)) : Tetration10Linear(slog);
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
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(n, 10);
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

        public static Arithmonym Slog10(Arithmonym value)
        {
            if (value < TenBillion)
            {
                return new(Float128HyperTranscendentals.SuperLog10(value));
            }
            if (value < Dekalogue)
            {
                // value >= 10^10 and value < 10^10^10^10^10^10^10^10^10^10
                return new(value.Operand);
            }
            if (value < Triapetaksys)
            {
                Float128 letterG;
                if (value < Dekateraksys)
                    letterG = Float128PreciseTranscendentals.SafePow(5, value.Operand - 2) * 2;
                else letterG = Float128HyperTranscendentals.LetterG(Float128PreciseTranscendentals.SafeExp10(Float128PreciseTranscendentals.SafePow(5, value.Operand - 3) * 2));
                letterG--;
                if (Float128.IsInfinity(letterG)) return value;
                if (letterG < 2) return Tetration10Linear((Arithmonym)Float128PreciseTranscendentals.SafeExp10(letterG - 1));
                return new(false, false, LETTERCODE_J, EncodeOperand(2 + (Float128PreciseTranscendentals.SafeLog(letterG / 2) / Float128PreciseTranscendentals.SafeLog(5))));
            }
            return value;
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

        public static Arithmonym Slog10Linear(Arithmonym value)
        {
            if (value < TenBillion)
            {
                return new(Float128HyperTranscendentals.SuperLog10(value));
            }
            if (value < Dekalogue)
            {
                // value >= 10^10 and value < 10^10^10^10^10^10^10^10^10^10
                return new(value.Operand);
            }
            if (value < Triapetaksys)
            {
                Float128 letterG;
                if (value < Dekateraksys)
                    letterG = Float128PreciseTranscendentals.SafePow(5, value.Operand - 2) * 2;
                else letterG = Float128HyperTranscendentals.LetterG(Float128PreciseTranscendentals.SafeExp10(Float128PreciseTranscendentals.SafePow(5, value.Operand - 3) * 2));
                letterG--;
                if (Float128.IsInfinity(letterG)) return value;
                if (letterG < 2) return Tetration10Linear((Arithmonym)Float128PreciseTranscendentals.SafeExp10(letterG - 1));
                return new(false, false, LETTERCODE_J, EncodeOperand(2 + (Float128PreciseTranscendentals.SafeLog(letterG / 2) / Float128PreciseTranscendentals.SafeLog(5))));
            }
            return value;
        }

        public static Arithmonym Tetration10Linear(Arithmonym value)
        {
            if (value < Two)
            {
                return new(Float128HyperTranscendentals.LetterF(value));
            }
            if (value < Ten)
            {
                return new(false, false, LETTERCODE_F, EncodeOperand(value.ToFloat128()));
            }
            if (value < Dekateraksys)
            {
                // Hi anyone reading this!
                // I'm a comment! The compiler ignores me :(
                // You won't, luckily.
                // I'm here to tell you that please don't look at that line of code
                // It's confusing
                // Yet it works. And it satisfies tests. Leave that line alone
                // Thank you!!

                /*
                  Hi guys it's another me
                  I'm another comment
                  Don't look at the comment above me
                  That comment above me is not even multi line

                  Cuz you're smart enough to understand this
                  Remember that we're using linear approximation inside
                  So remember also this function is 10^^x
                  If that x is >=10 then its not so easy
                  We have to turn the F-form (e.g. F-form of 10 is 1,
                  of 10^10 is 2, of 10^10^10 is 3, etc) into a J-form
                  because J is after F in this system

                  The maintainer of this code is too lazy to use conventions
                  because that would make a bit too much edge cases

                  It's still a bit hard to understand right

                  You want an equation? Alright!
                  [Formula works assuming x is between 10 and 10^10]

                  10^^x = 10^^10^10^log(log(x)) = 10^^10^^(1+log(log(x)))
                  = 10^^10^^10^^(log(1+log(log(x)))) = 10^^^(2+log(1+log(log(x))))
                  
                  Now we converted it to G form. On to J form!
                  Remember if the number the G form outputs is in [2,10) we can use
                  a cool shortie cuttie (the whole program is one)

                  let a = log(1+log(log(x))) <-- you can guess its a small number
                  10^^^(2+a) = 10^... 3 + log5(1+(a/2)) arrows ...^10
                  = 10^...(2 + log5(1+((log(1+log(log(x))))/2)))...^10

                  😰 That was some serious computation!

                  *LMAO*
                */

                // That comment above me is too long. Clean Code would delete it right away.

                // Yes, my twin! I do agree. But sadly we can't talk about this to the author of the code.

                // Isn't the code more self-documenting? Just look below ME!
                return new(
                    false,
                    false,
                    LETTERCODE_J,
                    EncodeOperand(
                        LetterGToLetterJ(LetterFToLetterG(value)).ToFloat128()
                    )
                );
            }
            if (value < Triapetaksys)
            {
                Float128 letterH = Float128PreciseTranscendentals.SafePow(5, value.Operand - 3) * 2;
                if (letterH < 2)
                {
                    letterH = 2;
                }
                if (letterH >= 3)
                {
                    letterH = 3 - Float128.Epsilon;
                }

                // H2.301.. -> GGG0.301.. -> GG2
                Float128 letterG = Float128HyperTranscendentals.LetterG(Float128PreciseTranscendentals.SafeExp10(letterH - 2));

                if (Float128.IsInfinity(letterG)) { return value; }
                letterG++;

                letterH = 2 + Float128PreciseTranscendentals.SafeLog10(Float128HyperTranscendentals.InverseLetterG(letterG));
                return new(false, false, LETTERCODE_J, EncodeOperand(3 + (Float128PreciseTranscendentals.SafeLog(letterH / 2) / Float128PreciseTranscendentals.SafeLog(5))));
            }
            return value;
        }

        private static Arithmonym LetterFToLetterG(Arithmonym value)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, Zero);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, Dekalogue);
            // 10^^(10^^2) = 10^^^(2+log(2))
            // 10^^(10^^^3) = 10^^^operand+1
            return (value < One) ? value :
                (value < Ten) ? value._Log10 + 1 :
                (value < TenBillion) ? ((value._Log10._Log10 + One)._Log10 + Two) : new Arithmonym(Float128PreciseTranscendentals.SafeLog10(value.Operand)) + Two;
        }

        private static Arithmonym LetterGToLetterJ(Arithmonym value)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, Ten);
            if (value <= Two) return value;
            return (value / Two)._Log10 / Five._Log10;
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