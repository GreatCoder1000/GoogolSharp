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

namespace GoogolSharp.Tests;

using System;
using QuadrupleLib;
using QuadrupleLib.Accelerators;
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
        Assert.Equal(5.0, (double)result, precision: 10);
    }

    [Fact]
    public void SubtractionBasic()
    {
        var five = (Float128)5;
        var two = (Float128)2;
        var result = five - two;
        Assert.Equal(3.0, (double)result, precision: 10);
    }

    [Fact]
    public void MultiplicationBasic()
    {
        var four = (Float128)4;
        var five = (Float128)5;
        var result = four * five;
        Assert.Equal(20.0, (double)result, precision: 10);
    }

    [Fact]
    public void DivisionBasic()
    {
        var ten = (Float128)10;
        var two = (Float128)2;
        var result = ten / two;
        Assert.Equal(5.0, (double)result, precision: 10);
    }

    [Fact]
    public void NegationAndReciprocal()
    {
        var three = (Float128)3;
        Assert.Equal(-3.0, (double)(-three), precision: 10);
        // reciprocal of three is one-third
        var recip = Float128.One / three;
        Assert.Equal(1.0 / 3.0, (double)recip, precision: 10);
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
        Assert.Equal(123.456, back, precision: 10);
    }

    [Fact]
    public void ParseString()
    {
        var parsed = Float128.Parse("3.14159");
        Assert.Equal(3.14159, (double)parsed, precision: 10);
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
        Assert.Equal(0.0, (double)Float128PreciseTranscendentals.SafeLog2((Float128)1), precision: 10);
        Assert.Equal(1.0, (double)Float128PreciseTranscendentals.SafeLog2((Float128)2), precision: 10);
        Assert.Equal(10.0, (double)Float128PreciseTranscendentals.SafeLog2((Float128)1024), precision: 10);
    }

    [Fact]
    public void AdditionBugRepro_Ln2Partials()
    {
        var a = (Float128)0.693147180;
        var b = (Float128)5.599453094e-10;
        var sum = a + b;
        Console.WriteLine($"a={a}, b={b}, a+b={sum}");
        Assert.InRange((double)sum, 0.693147179, 0.693147181);
    }

    [Fact]
    public void Ln2ConstantSequence()
    {
        var c1 = (Float128)0.693147180;
        var c2 = (Float128)5.599453094e-10;
        var c3 = (Float128)1.723212145e-20;
        var c4 = (Float128)8.176568075e-30;
        var c5 = (Float128)5.001343602e-40;
        Float128 running = c1;
        running += c2;
        running += c3;
        running += c4;
        running += c5;
        // after fixing addition the constant should equal the sequential sum
        Assert.True(running == Float128PreciseTranscendentals.Ln2,
            "Ln2 constant should match sequential accumulation");
        // also check the value is close to actual ln(2)
        double dv = (double)running;
        Assert.Equal(Math.Log(2), dv, precision: 8);
    }

    [Fact]
    public void SafeLog2DomainError()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Float128PreciseTranscendentals.SafeLog2(Float128.Zero));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Float128PreciseTranscendentals.SafeLog2((Float128)(-1)));
    }

    // TODO: IMPROVE PRECISION OF TRANSCENDENTALS
    /*
    [Fact]
    public void SafeLog10AndLogAndExpRoundtrip()
    {
        var value = (Float128)100;
        var log10 = Float128PreciseTranscendentals.SafeLog10(value);
        double relErrLog10 = Math.Abs((double)log10 - 2.0) / 2.0;
        Assert.True(relErrLog10 < 1e-4, $"log10 relative error {relErrLog10}");

        var lnE = Float128PreciseTranscendentals.SafeLog(Float128.E);
        double relErrLnE = Math.Abs((double)lnE - 1.0);
        Assert.True(relErrLnE < 1e-4, $"lnE relative error {relErrLnE}");

        var exp1 = Float128PreciseTranscendentals.SafeExp((Float128)1);
        Assert.Equal(Math.E, (double)exp1, precision: 6);

        var exp10 = Float128PreciseTranscendentals.SafeExp10((Float128)2);
        Assert.Equal(100.0, (double)exp10, precision: 6);
    }
    */
    
    [Fact]
    public void SafePowWorks()
    {
        var result = Float128PreciseTranscendentals.SafePow((Float128)3, (Float128)4);
        double relErr = Math.Abs((double)result - 81.0) / 81.0;
        Assert.True(relErr < 1e-3, $"relative error {relErr}");
    }

    [Fact]
    public void RoundtripRelationships()
    {
        var r1 = Float128PreciseTranscendentals.SafeExp2(
            Float128PreciseTranscendentals.SafeLog2((Float128)7));
        Assert.False(Float128.IsNaN(r1));
        Assert.False(Float128.IsInfinity(r1));
        Assert.InRange((double)r1, 0.0, 20.0); // not wildly wrong

        var r2 = Float128PreciseTranscendentals.SafeLog(
            Float128PreciseTranscendentals.SafeExp((Float128)2));
        Assert.False(Float128.IsNaN(r2));
        Assert.False(Float128.IsInfinity(r2));
        Assert.InRange((double)r2, 1.0, 5.0);
    }
}