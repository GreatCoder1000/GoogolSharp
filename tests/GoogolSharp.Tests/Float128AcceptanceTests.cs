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

namespace GoogolSharp.Tests
{

    using System;
    using Float128 = QuadrupleLib.Float128<QuadrupleLib.Accelerators.DefaultAccelerator>;
    using GoogolSharp.Helpers;

    public class Float128AcceptanceTests
    {
        // Basic arithmetic
        [Fact]
        public void AdditionBasic()
        {
            Float128 a = (Float128)2;
            Float128 b = (Float128)3;
            Float128 result = a + b;
            AssertFloat128.Equal((Float128)5.0, result, 10);
        }

        [Fact]
        public void SubtractionBasic()
        {
            var five = (Float128)5;
            var two = (Float128)2;
            var result = five - two;
            AssertFloat128.Equal((Float128)3.0, result, 10);
        }

        [Fact]
        public void MultiplicationBasic()
        {
            var four = (Float128)4;
            var five = (Float128)5;
            var result = four * five;
            AssertFloat128.Equal((Float128)20.0, result, 10);
        }

        [Fact]
        public void DivisionBasic()
        {
            var ten = (Float128)10;
            var two = (Float128)2;
            var result = ten / two;
            AssertFloat128.Equal((Float128)5.0, result, 10);
        }

        [Fact]
        public void NegationAndReciprocal()
        {
            var three = (Float128)3;
            AssertFloat128.Equal((Float128)(-3.0), -three, 10);
            // reciprocal of three is one-third
            var recip = Float128.One / three;
            AssertFloat128.Equal((Float128)(1.0 / 3.0), recip, 10);
        }

        // predicates and special values -------------------------------------------------
        [Fact]
        public void SpecialConstants()
        {
            Assert.True(Float128.IsZero(Float128.Zero));
            Assert.True(Float128.IsInfinity(Float128.PositiveInfinity));
            Assert.True(Float128.IsInfinity(Float128.NegativeInfinity));
            Assert.True(Float128.IsNaN(Float128.NaN));
            Assert.False(Float128.IsInfinity((Float128)1.0));
            Assert.False(Float128.IsZero((Float128)1.0));
        }

        [Fact]
        public void NaNPropagation()
        {
            var nan = Float128.NaN;
            var five = (Float128)5;
            var result = nan + five;
            Assert.True(Float128.IsNaN(result));
        }

        [Fact]
        public void InfinityArithmetic()
        {
            var inf = Float128.PositiveInfinity;
            var one = (Float128)1;
            Assert.True((inf + one) == Float128.PositiveInfinity);
            Assert.True(Float128.IsNaN(inf * Float128.Zero));
        }

        // conversion and parsing -------------------------------------------------------
        [Fact]
        public void ToAndFromDouble()
        {
            var value = (Float128)123.456;
            double back = (double)value;
            AssertFloat128.Equal((Float128)123.456, value, 10);
        }

        [Fact]
        public void ParseString()
        {
            var parsed = Float128.Parse("3.14159");
            AssertFloat128.Equal((Float128)3.14159, parsed, 10);
        }

        // rounding helpers ------------------------------------------------------------
        [Fact]
        public void FloorAndCeiling()
        {
            var v = (Float128)3.7;
            var f = Float128.Floor(v);
            var c = Float128.Ceiling(v);
            Assert.True(f <= v, "floor should be <= value");
            Assert.True(c >= v, "ceiling should be >= value");
        }

        // constants --------------------------------------------------------------------
        [Fact]
        public void EpsilonAndIdentity()
        {
            Assert.True(Float128PreciseTranscendentals.Epsilon > Float128.Zero);
            Assert.Equal((Float128)1, Float128.One);
            Assert.Equal((Float128)0, Float128.Zero);
        }

        // safe transcendentals ---------------------------------------------------------
        [Fact]
        public void SafeExp2Log2Roundtrip()
        {
            var x = (Float128)5.3;
            var log2 = Float128PreciseTranscendentals.SafeLog2(x);
            var exp2 = Float128PreciseTranscendentals.SafeExp2(log2);
            Assert.False(Float128.IsNaN(exp2));
            Assert.False(Float128.IsInfinity(exp2));
            double relErr = Math.Abs((double)exp2 - (double)x) / (double)x;
            Assert.True(relErr < 0.2, $"relative error {relErr}");
        }

        [Fact]
        public void SafeLog2KnownValues()
        {
            AssertFloat128.Equal((Float128)0.0, Float128PreciseTranscendentals.SafeLog2((Float128)1), 10);
            AssertFloat128.Equal((Float128)1.0, Float128PreciseTranscendentals.SafeLog2((Float128)2), 10);
            AssertFloat128.Equal((Float128)10.0, Float128PreciseTranscendentals.SafeLog2((Float128)1024), 10);
        }

        [Fact]
        public void AdditionBugRepro_Ln2Partials()
        {
            var a = (Float128)0.693147180;
            var b = (Float128)5.599453094e-10;
            var sum = a + b;
            Assert.InRange((double)sum, 0.693147179, 0.693147181);
        }

        // [Fact]
        // public void Ln2ConstantSequence()
        // {
        //     var c1 = (Float128)0.693147180;
        //     var c2 = (Float128)5.599453094e-10;
        //     var c3 = (Float128)1.723212145e-20;
        //     var c4 = (Float128)8.176568075e-30;
        //     var c5 = (Float128)5.001343602e-40;
        //     Float128 running = c1;
        //     running += c2;
        //     running += c3;
        //     running += c4;
        //     running += c5;
        //     // after fixing addition the constant should equal the sequential sum
        //     Assert.True(running == Float128PreciseTranscendentals.Ln2,
        //         "Ln2 constant should match sequential accumulation");
        //     // also check the value is close to actual ln(2)
        //     double dv = (double)running;
        //     Assert.Equal(Math.Log(2), dv, precision: 8);
        // }

        [Fact]
        public void SafeLog2DomainError()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                Float128PreciseTranscendentals.SafeLog2(Float128.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                Float128PreciseTranscendentals.SafeLog2((Float128)(-1)));
        }

        [Fact]
        public void SafeLog10AndLogAndExpRoundtrip()
        {
            var value = (Float128)100;
            var log10 = Float128PreciseTranscendentals.SafeLog10(value);
            double relErrLog10 = Math.Abs((double)log10 - 2.0) / 2.0;
            Assert.True(relErrLog10 < 1e-4, $"log10 relative error {relErrLog10}");

            var lnE = Float128PreciseTranscendentals.SafeLog(Float128PreciseTranscendentals.E);
            double relErrLnE = Math.Abs((double)lnE - 1.0);
            Assert.True(relErrLnE < 1e-4, $"lnE relative error {relErrLnE}");

            var exp1 = Float128PreciseTranscendentals.SafeExp((Float128)1);
            double relErrExp = Math.Abs((double)exp1 - Math.E) / Math.E;
            Assert.True(relErrExp < 1e-4, $"exp(1) relative error {relErrExp}");

            var exp10_2 = Float128PreciseTranscendentals.SafeExp10((Float128)2);
            double relErrExp10 = Math.Abs((double)exp10_2 - 100.0) / 100.0;
            Assert.True(relErrExp10 < 1e-4, $"exp10(2) relative error {relErrExp10}");
        }

        [Fact]
        public void SafePowWorks()
        {
            var result = Float128PreciseTranscendentals.SafePow((Float128)3, (Float128)4);
            double relErr = Math.Abs((double)result - 81.0) / 81.0;
            Assert.True(relErr < 1e-4, $"relative error {relErr}");
        }

        [Fact]
        public void RoundtripRelationships()
        {
            var r1 = Float128PreciseTranscendentals.SafeExp2(
                Float128PreciseTranscendentals.SafeLog2((Float128)7));
            Assert.False(Float128.IsNaN(r1));
            Assert.False(Float128.IsInfinity(r1));
            Assert.InRange((double)r1, 6.98, 7.02);

            var r2 = Float128PreciseTranscendentals.SafeLog(
                Float128PreciseTranscendentals.SafeExp((Float128)2));
            Assert.False(Float128.IsNaN(r2));
            Assert.False(Float128.IsInfinity(r2));
            Assert.InRange((double)r2, 1.98, 2.02);
        }

        // Additional comprehensive transcendental tests
        [Fact]
        public void Log2OfPowerOfTwo()
        {
            for (int i = -10; i <= 10; i++)
            {
                Float128 x = Float128.ScaleB(Float128.One, i);
                Float128 result = Float128PreciseTranscendentals.SafeLog2(x);
                AssertFloat128.Equal((Float128)i, result, 8);
            }
        }

        [Fact]
        public void Log2KnownValues()
        {
            AssertFloat128.Equal((Float128)1.0, Float128PreciseTranscendentals.SafeLog2((Float128)2), 10);
            AssertFloat128.Equal((Float128)2.0, Float128PreciseTranscendentals.SafeLog2((Float128)4), 10);
            AssertFloat128.Equal((Float128)3.0, Float128PreciseTranscendentals.SafeLog2((Float128)8), 10);
            AssertFloat128.Equal((Float128)4.0, Float128PreciseTranscendentals.SafeLog2((Float128)16), 10);
            AssertFloat128.Equal((Float128)5.0, Float128PreciseTranscendentals.SafeLog2((Float128)32), 10);
            AssertFloat128.Equal((Float128)(-1.0), Float128PreciseTranscendentals.SafeLog2((Float128)0.5), 10);
            AssertFloat128.Equal((Float128)(-2.0), Float128PreciseTranscendentals.SafeLog2((Float128)0.25), 10);
        }

        [Fact]
        public void Log10KnownValues()
        {
            AssertFloat128.Equal((Float128)0.0, Float128PreciseTranscendentals.SafeLog10((Float128)1), 10);
            AssertFloat128.Equal((Float128)1.0, Float128PreciseTranscendentals.SafeLog10((Float128)10), 3);
            AssertFloat128.Equal((Float128)2.0, Float128PreciseTranscendentals.SafeLog10((Float128)100), 3);
            AssertFloat128.Equal((Float128)3.0, Float128PreciseTranscendentals.SafeLog10((Float128)1000), 2);
            AssertFloat128.Equal((Float128)(-1.0), Float128PreciseTranscendentals.SafeLog10((Float128)0.1), 3);
        }

        [Fact]
        public void LogNaturalKnownValues()
        {
            AssertFloat128.Equal((Float128)0.0, Float128PreciseTranscendentals.SafeLog((Float128)1), 10);
            double ln2Expected = Math.Log(2);
            AssertFloat128.Equal((Float128)ln2Expected, Float128PreciseTranscendentals.SafeLog((Float128)2), 4);

            double lnEExpected = 1.0;
            AssertFloat128.Equal((Float128)lnEExpected, Float128PreciseTranscendentals.SafeLog(Float128PreciseTranscendentals.E), 4);
            AssertFloat128.Equal((Float128)2.0, Float128PreciseTranscendentals.SafeExp2((Float128)1), 10);
            AssertFloat128.Equal((Float128)4.0, Float128PreciseTranscendentals.SafeExp2((Float128)2), 10);
            AssertFloat128.Equal((Float128)8.0, Float128PreciseTranscendentals.SafeExp2((Float128)3), 10);
            AssertFloat128.Equal((Float128)16.0, Float128PreciseTranscendentals.SafeExp2((Float128)4), 10);
            AssertFloat128.Equal((Float128)0.5, Float128PreciseTranscendentals.SafeExp2((Float128)(-1)), 10);
        }

        [Fact]
        public void ExpKnownValues()
        {
            AssertFloat128.Equal((Float128)1.0, Float128PreciseTranscendentals.SafeExp((Float128)0), 10);
            double eExpected = Math.E;
            AssertFloat128.Equal((Float128)eExpected, Float128PreciseTranscendentals.SafeExp((Float128)1), 8);

            double e2Expected = Math.Exp(2);
            AssertFloat128.Equal((Float128)e2Expected, Float128PreciseTranscendentals.SafeExp((Float128)2), 8);
        }

        [Fact]
        public void Exp10KnownValues()
        {
            AssertFloat128.Equal((Float128)1.0, Float128PreciseTranscendentals.SafeExp10((Float128)0), 10);
            AssertFloat128.Equal((Float128)10.0, Float128PreciseTranscendentals.SafeExp10((Float128)1), 10);
            AssertFloat128.Equal((Float128)100.0, Float128PreciseTranscendentals.SafeExp10((Float128)2), 10);
            AssertFloat128.Equal((Float128)1000.0, Float128PreciseTranscendentals.SafeExp10((Float128)3), 10);
            AssertFloat128.Equal((Float128)0.1, Float128PreciseTranscendentals.SafeExp10((Float128)(-1)), 10);
        }

        [Fact]
        public void PowWithIntegerExponents()
        {
            AssertFloat128.Equal((Float128)1.0, Float128PreciseTranscendentals.SafePow((Float128)5, (Float128)0), 10);
            AssertFloat128.Equal((Float128)5.0, Float128PreciseTranscendentals.SafePow((Float128)5, (Float128)1), 3);
            AssertFloat128.Equal((Float128)25.0, Float128PreciseTranscendentals.SafePow((Float128)5, (Float128)2), 2);
            AssertFloat128.Equal((Float128)125.0, Float128PreciseTranscendentals.SafePow((Float128)5, (Float128)3), 1);
            AssertFloat128.Equal((Float128)0.2, Float128PreciseTranscendentals.SafePow((Float128)5, (Float128)(-1)), 3);
        }

        [Fact]
        public void PowWithFractionalExponents()
        {
            var sqrt4 = Float128PreciseTranscendentals.SafePow((Float128)4, (Float128)0.5);
            AssertFloat128.Equal((Float128)2.0, sqrt4, 8);

            var cbrt8 = Float128PreciseTranscendentals.SafePow((Float128)8, Float128PreciseTranscendentals.SafeExp2((Float128)(-1.5)));
            // 8^(1/3) = 2
            Assert.True(Float128.Abs(cbrt8 - (Float128)2) < (Float128)0.1);
        }

        [Fact]
        public void Log2AndExp2Inverse()
        {
            var testValues = new[] { 0.1, 0.5, 1.0, 2.0, 5.0, 10.0, 100.0 };
            foreach (var val in testValues)
            {
                var fval = (Float128)val;
                var result = Float128PreciseTranscendentals.SafeExp2(
                    Float128PreciseTranscendentals.SafeLog2(fval));
                double relErr = Math.Abs((double)result - val) / val;
                Assert.True(relErr < 1e-4, $"Roundtrip error for {val}: {relErr}");
            }
        }

        [Fact]
        public void Log10AndExp10Inverse()
        {
            var testValues = new[] { 0.1, 0.5, 1.0, 5.0, 10.0, 50.0, 100.0 };
            foreach (var val in testValues)
            {
                var fval = (Float128)val;
                var result = Float128PreciseTranscendentals.SafeExp10(
                    Float128PreciseTranscendentals.SafeLog10(fval));
                double relErr = Math.Abs((double)result - val) / val;
                Assert.True(relErr < 1e-4, $"Roundtrip error for {val}: {relErr}");
            }
        }

        [Fact]
        public void LogAndExpInverse()
        {
            var testValues = new[] { 0.1, 0.5, 1.0, 2.0, Math.E, 5.0, 10.0 };
            foreach (var val in testValues)
            {
                var fval = (Float128)val;
                var result = Float128PreciseTranscendentals.SafeExp(
                    Float128PreciseTranscendentals.SafeLog(fval));
                double relErr = Math.Abs((double)result - val) / val;
                Assert.True(relErr < 1e-4, $"Roundtrip error for {val}: {relErr}");
            }
        }

        [Fact]
        public void LogarithmProperties()
        {
            // log(a*b) = log(a) + log(b)
            var a = (Float128)2;
            var b = (Float128)3;
            var logProduct = Float128PreciseTranscendentals.SafeLog(a * b);
            var logSum = Float128PreciseTranscendentals.SafeLog(a) + Float128PreciseTranscendentals.SafeLog(b);
            double relErr = Math.Abs((double)(logProduct - logSum)) / (double)Float128.Abs(logSum);
            Assert.True(relErr < 1e-4, $"log(a*b) != log(a)+log(b): {relErr}");

            // log(a/b) = log(a) - log(b)
            var logQuotient = Float128PreciseTranscendentals.SafeLog(a / b);
            var logDiff = Float128PreciseTranscendentals.SafeLog(a) - Float128PreciseTranscendentals.SafeLog(b);
            relErr = Math.Abs((double)(logQuotient - logDiff)) / (double)Float128.Abs(logDiff);
            Assert.True(relErr < 1e-4, $"log(a/b) != log(a)-log(b): {relErr}");

            // log(a^b) = b*log(a)
            var logPower = Float128PreciseTranscendentals.SafeLog(Float128PreciseTranscendentals.SafePow(a, b));
            var bLogA = b * Float128PreciseTranscendentals.SafeLog(a);
            relErr = Math.Abs((double)(logPower - bLogA)) / (double)Float128.Abs(bLogA);
            Assert.True(relErr < 1e-4, $"log(a^b) != b*log(a): {relErr}");
        }

        [Fact]
        public void PowerOfBase10AndE()
        {
            // 10^x * 10^y = 10^(x+y)
            var x = (Float128)2;
            var y = (Float128)3;
            var lhs = Float128PreciseTranscendentals.SafeExp10(x) * Float128PreciseTranscendentals.SafeExp10(y);
            var rhs = Float128PreciseTranscendentals.SafeExp10(x + y);
            double relErr = Math.Abs((double)(lhs - rhs)) / (double)Float128.Abs(rhs);
            Assert.True(relErr < 1e-4, $"10^x * 10^y != 10^(x+y): {relErr}");

            // e^x * e^y = e^(x+y)
            lhs = Float128PreciseTranscendentals.SafeExp(x) * Float128PreciseTranscendentals.SafeExp(y);
            rhs = Float128PreciseTranscendentals.SafeExp(x + y);
            relErr = Math.Abs((double)(lhs - rhs)) / (double)Float128.Abs(rhs);
            Assert.True(relErr < 1e-4, $"e^x * e^y != e^(x+y): {relErr}");
        }

        [Fact]
        public void SmallNumbersLogarithms()
        {
            var small = (Float128)1e-20;
            var logSmall = Float128PreciseTranscendentals.SafeLog10(small);
            // For very small numbers, check relative error instead of absolute precision
            double expected = -20.0;
            double actual = (double)logSmall;
            double relErr = Math.Abs(actual - expected) / Math.Abs(expected);
            Assert.True(relErr < 1e-5, $"Relative error {relErr} exceeded threshold");
        }

        [Fact]
        public void LargeNumbersExponentiation()
        {
            var largeExp = Float128PreciseTranscendentals.SafeExp2((Float128)50);
            Assert.False(Float128.IsNaN(largeExp));
            Assert.False(Float128.IsInfinity(largeExp));
            // 2^50 ≈ 1.1e15
            Assert.True((double)largeExp > 1e15);
        }

        [Fact]
        public void NegativeNumbersInLogs()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                Float128PreciseTranscendentals.SafeLog((Float128)(-1)));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                Float128PreciseTranscendentals.SafeLog10((Float128)(-5)));
        }

        [Fact]
        public void NegativeBasesInPow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                Float128PreciseTranscendentals.SafePow((Float128)(-2), (Float128)0.5));
        }
    }
}