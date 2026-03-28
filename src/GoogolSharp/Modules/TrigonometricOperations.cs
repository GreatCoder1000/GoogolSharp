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

namespace GoogolSharp
{
    partial struct Arithmonym
    {// =========================
     // TRIGONOMETRIC FUNCTIONS
     // =========================

        // --- Sin ---
        public static Arithmonym Sin(Arithmonym value) => Sin(value, 20);
        public static Arithmonym Sin(Arithmonym value, int terms = 20)
        {
            value %= Tau;
            Arithmonym result = Zero;
            Arithmonym numerator = value;
            Arithmonym denominator = One;
            Arithmonym sign = One;

            for (int term = 0; term < terms; term++)
            {
                result += sign * (numerator / denominator);
                numerator *= value * value;
                denominator *= (2 * term + 2) * (2 * term + 3);
                sign *= -1;
            }
            return result;
        }

        // --- Cos ---
        public static Arithmonym Cos(Arithmonym value) => Cos(value, 20);
        public static Arithmonym Cos(Arithmonym value, int terms = 20)
        {
            value %= Tau;
            Arithmonym result = Zero;
            Arithmonym numerator = One;
            Arithmonym denominator = One;
            Arithmonym sign = One;

            for (int term = 0; term < terms; term++)
            {
                result += sign * (numerator / denominator);
                numerator *= value * value;
                denominator *= (2 * term + 1) * (2 * term + 2);
                sign *= -1;
            }
            return result;
        }

        // --- Tan ---
        public static Arithmonym Tan(Arithmonym value) => Tan(value, 20);
        public static Arithmonym Tan(Arithmonym value, int terms = 20)
        {
            value %= Tau;
            Arithmonym c = Cos(value, terms);
            if (IsZero(c)) throw new ArgumentException("Tan undefined for 90°, 270°, etc.");
            return Sin(value, terms) / c;
        }

        // =========================
        // RECIPROCAL TRIG FUNCTIONS
        // =========================

        public static Arithmonym Csc(Arithmonym value) => Csc(value, 20);
        public static Arithmonym Csc(Arithmonym value, int terms = 20)
        {
            Arithmonym s = Sin(value, terms);
            if (IsZero(s)) throw new ArgumentException("Csc undefined at multiples of π");
            return One / s;
        }

        public static Arithmonym Sec(Arithmonym value) => Sec(value, 20);
        public static Arithmonym Sec(Arithmonym value, int terms = 20)
        {
            Arithmonym c = Cos(value, terms);
            if (IsZero(c)) throw new ArgumentException("Sec undefined at π/2 + kπ");
            return One / c;
        }

        public static Arithmonym Cot(Arithmonym value) => Cot(value, 20);
        public static Arithmonym Cot(Arithmonym value, int terms = 20)
        {
            Arithmonym t = Tan(value, terms);
            if (IsZero(t)) throw new ArgumentException("Cot undefined at multiples of π");
            return One / t;
        }

        // =========================
        // INVERSE TRIG FUNCTIONS
        // =========================

        public static Arithmonym Asin(Arithmonym x) => Asin(x, 20);
        public static Arithmonym Asin(Arithmonym x, int terms = 20)
        {
            if (x < -One || x > One) throw new ArgumentException("Asin domain is [-1,1]");
            return Atan(x / Sqrt(One - x * x), terms);
        }

        public static Arithmonym Acos(Arithmonym x) => Acos(x, 20);
        public static Arithmonym Acos(Arithmonym x, int terms = 20)
        {
            if (x < -One || x > One) throw new ArgumentException("Acos domain is [-1,1]");
            return (Pi / 2) - Asin(x, terms);
        }

        public static Arithmonym Atan(Arithmonym x) => Atan(x, 20);
        public static Arithmonym Atan(Arithmonym x, int terms = 20)
        {
            if (Abs(x) > One)
                return (Pi / 2) - Atan(One / x, terms);

            Arithmonym result = Zero;
            Arithmonym power = x;
            Arithmonym sign = One;

            for (int n = 0; n < terms; n++)
            {
                result += sign * power / (2 * n + 1);
                power *= x * x;
                sign *= -1;
            }
            return result;
        }

        public static Arithmonym Atan2(Arithmonym y, Arithmonym x) => Atan2(y, x, 20);
        public static Arithmonym Atan2(Arithmonym y, Arithmonym x, int terms = 20)
        {
            if (IsZero(x))
            {
                if (IsZero(y)) return Zero;
                return y > Zero ? Pi / 2 : -Pi / 2;
            }

            Arithmonym atan = Atan(y / x, terms);

            if (x > Zero) return atan;
            if (y >= Zero) return atan + Pi;
            return atan - Pi;
        }

        // =========================
        // Pi-SCALED TRIG FUNCTIONS
        // =========================

        public static Arithmonym SinPi(Arithmonym x) => SinPi(x, 20);
        public static Arithmonym SinPi(Arithmonym x, int terms = 20) => Sin(Pi * x, terms);

        public static Arithmonym CosPi(Arithmonym x) => CosPi(x, 20);
        public static Arithmonym CosPi(Arithmonym x, int terms = 20) => Cos(Pi * x, terms);

        public static Arithmonym TanPi(Arithmonym x) => TanPi(x, 20);
        public static Arithmonym TanPi(Arithmonym x, int terms = 20) => Tan(Pi * x, terms);

        public static Arithmonym AsinPi(Arithmonym x) => AsinPi(x, 20);
        public static Arithmonym AsinPi(Arithmonym x, int terms = 20) => Asin(x, terms) / Pi;

        public static Arithmonym AcosPi(Arithmonym x) => AcosPi(x, 20);
        public static Arithmonym AcosPi(Arithmonym x, int terms = 20) => Acos(x, terms) / Pi;

        public static Arithmonym AtanPi(Arithmonym x) => AtanPi(x, 20);
        public static Arithmonym AtanPi(Arithmonym x, int terms = 20) => Atan(x, terms) / Pi;

        public static Arithmonym Atan2Pi(Arithmonym y, Arithmonym x) => Atan2Pi(y, x, 20);
        public static Arithmonym Atan2Pi(Arithmonym y, Arithmonym x, int terms = 20) => Atan2(y, x, terms) / Pi;

        // =========================
        // HYPERBOLIC FUNCTIONS
        // =========================

        public static Arithmonym Sinh(Arithmonym x) => Sinh(x, 20);
        public static Arithmonym Sinh(Arithmonym x, int terms = 20)
        {
            Arithmonym result = Zero;
            Arithmonym numerator = x;
            Arithmonym denominator = One;

            for (int n = 0; n < terms; n++)
            {
                result += numerator / denominator;
                numerator *= x * x;
                denominator *= (2 * n + 2) * (2 * n + 3);
            }
            return result;
        }

        public static Arithmonym Cosh(Arithmonym x) => Cosh(x, 20);
        public static Arithmonym Cosh(Arithmonym x, int terms = 20)
        {
            Arithmonym result = Zero;
            Arithmonym numerator = One;
            Arithmonym denominator = One;

            for (int n = 0; n < terms; n++)
            {
                result += numerator / denominator;
                numerator *= x * x;
                denominator *= (2 * n + 1) * (2 * n + 2);
            }
            return result;
        }

        public static Arithmonym Tanh(Arithmonym x) => Tanh(x, 20);
        public static Arithmonym Tanh(Arithmonym x, int terms = 20)
        {
            Arithmonym c = Cosh(x, terms);
            if (IsZero(c)) throw new ArgumentException("Tanh undefined");
            return Sinh(x, terms) / c;
        }

        // =========================
        // RECIPROCAL HYPERBOLIC
        // =========================

        public static Arithmonym Csch(Arithmonym x) => Csch(x, 20);
        public static Arithmonym Csch(Arithmonym x, int terms = 20)
        {
            Arithmonym s = Sinh(x, terms);
            if (IsZero(s)) throw new ArgumentException("Csch undefined at x = 0");
            return One / s;
        }

        public static Arithmonym Sech(Arithmonym x) => Sech(x, 20);
        public static Arithmonym Sech(Arithmonym x, int terms = 20)
        {
            Arithmonym c = Cosh(x, terms);
            if (IsZero(c)) throw new ArgumentException("Sech undefined");
            return One / c;
        }

        public static Arithmonym Coth(Arithmonym x) => Coth(x, 20);
        public static Arithmonym Coth(Arithmonym x, int terms = 20)
        {
            Arithmonym t = Tanh(x, terms);
            if (IsZero(t)) throw new ArgumentException("Coth undefined at x = 0");
            return One / t;
        }

        // =========================
        // INVERSE HYPERBOLIC
        // =========================

        public static Arithmonym Asinh(Arithmonym x) => Log(x + Sqrt(x * x + One));

        public static Arithmonym Acosh(Arithmonym x)
        {
            if (x < One) throw new ArgumentException("Acosh domain is x >= 1");
            return Log(x + Sqrt((x - One) * (x + One)));
        }

        public static Arithmonym Atanh(Arithmonym x)
        {
            if (x <= -One || x >= One)
                throw new ArgumentException("Atanh domain is (-1,1)");
            return One / 2 * Log((One + x) / (One - x));
        }

        // =========================
        // COMBINED TRIG FUNCTIONS
        // =========================

        // Returns (sin(x), cos(x))
        public static (Arithmonym Sin, Arithmonym Cos) SinCos(Arithmonym value)
            => SinCos(value, 20);

        public static (Arithmonym Sin, Arithmonym Cos) SinCos(Arithmonym value, int terms = 20)
        {
            value %= Tau;
            return (Sin(value, terms), Cos(value, terms));
        }

        // =========================
        // PI-SCALED COMBINED TRIG
        // =========================

        // Returns (sin(πx), cos(πx))
        public static (Arithmonym SinPi, Arithmonym CosPi) SinCosPi(Arithmonym x)
            => SinCosPi(x, 20);

        public static (Arithmonym SinPi, Arithmonym CosPi) SinCosPi(Arithmonym x, int terms = 20)
        {
            Arithmonym v = Pi * x;
            v %= Tau;
            return (Sin(v, terms), Cos(v, terms));
        }
    }
}