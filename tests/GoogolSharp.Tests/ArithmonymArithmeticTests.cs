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
    public class ArithmonymArithmeticTests
    {
        [Fact]
        public void AdditionOfTwoAndThreeIsFive()
        {
            var two = new Arithmonym(2);
            var three = new Arithmonym(3);
            var result = two + three;

            Assert.Equal(5, (double)result, precision: 10);
        }

        [Fact]
        public void SubtractionOfFiveAndTwoIsThree()
        {
            var five = new Arithmonym(5);
            var two = new Arithmonym(2);
            var result = five - two;

            Assert.Equal(3, (double)result, precision: 10);
        }

        [Fact]
        public void MultiplicationOfFourAndFiveIsTwenty()
        {
            var four = new Arithmonym(4);
            var five = new Arithmonym(5);
            var result = four * five;

            Assert.Equal(20, (double)result, precision: 10);
        }

        [Fact]
        public void DivisionOfTenByTwoIsFive()
        {
            var ten = new Arithmonym(10);
            var two = new Arithmonym(2);
            var result = ten / two;

            Assert.Equal(5, (double)result, precision: 10);
        }

        [Fact]
        public void ReciprocalOfFourIsQuarter()
        {
            var four = new Arithmonym(4);
            var result = four.Reciprocal;

            Assert.Equal(0.25, (double)result, precision: 10);
        }

        [Fact]
        public void NegationOfThreeIsNegativeThree()
        {
            var three = new Arithmonym(3);
            var result = -three;

            Assert.Equal(-3, (double)result, precision: 10);
        }

        [Fact]
        public void Log10Of100IsTwo()
        {
            var hundred = new Arithmonym(100);
            var result = Arithmonym.Log10(hundred);

            Assert.Equal(2, (double)result, precision: 10);
        }

        [Fact]
        public void ZeroTimesAnyIsZero()
        {
            var zero = Arithmonym.Zero;
            var big = new Arithmonym(123456123456);
            var result = zero * big;

            Assert.Equal(0, (double)result, precision: 10);
        }

        [Fact]
        public void InfinityPlusOneIsInfinity()
        {
            var inf = Arithmonym.PositiveInfinity;
            var one = new Arithmonym(1);
            var result = inf + one;

            Assert.True(result == Arithmonym.PositiveInfinity);
        }

        [Fact]
        public void NaNPropagatesThroughAddition()
        {
            var nan = Arithmonym.NaN;
            var five = new Arithmonym(5);
            var result = nan + five;

            Assert.True(Arithmonym.IsNaN(result));
        }

        // Existing tests...

        [Fact]
        public void AdditionWithZeroLeavesValueUnchanged()
        {
            var zero = Arithmonym.Zero;
            var seven = new Arithmonym(7);
            var result = seven + zero;

            Assert.Equal(7, (double)result, precision: 10);
        }

        [Fact]
        public void MultiplicationWithOneLeavesValueUnchanged()
        {
            var one = new Arithmonym(1);
            var nine = new Arithmonym(9);
            var result = one * nine;

            Assert.Equal(9, (double)result, precision: 10);
        }

        [Fact]
        public void DivisionByOneLeavesValueUnchanged()
        {
            var one = new Arithmonym(1);
            var twelve = new Arithmonym(12);
            var result = twelve / one;

            Assert.Equal(12, (double)result, precision: 10);
        }

        [Fact]
        public void DivisionByZeroIsInfinity()
        {
            var ten = new Arithmonym(10);
            var zero = Arithmonym.Zero;
            var result = ten / zero;

            Assert.True(result == Arithmonym.PositiveInfinity);
        }

        [Fact]
        public void ZeroDividedByNonZeroIsZero()
        {
            var zero = Arithmonym.Zero;
            var five = new Arithmonym(5);
            var result = zero / five;

            Assert.True(Arithmonym.IsZero(result));
        }

        [Fact]
        public void NegativeTimesNegativeIsPositive()
        {
            var negTwo = new Arithmonym(-2);
            var negThree = new Arithmonym(-3);
            var result = negTwo * negThree;

            Assert.Equal(6, (double)result, precision: 10);
        }

        [Fact]
        public void NegativeTimesPositiveIsNegative()
        {
            var negFour = new Arithmonym(-4);
            var five = new Arithmonym(5);
            var result = negFour * five;

            Assert.Equal(-20, (double)result, precision: 10);
        }

        [Fact]
        public void Log10AndExp10RoundTrip()
        {
            var twenty = new Arithmonym(20);
            var log = Arithmonym.Log10(twenty);
            var exp = Arithmonym.Exp10(log);

            Assert.Equal((double)twenty, (double)exp, precision: 10);
        }

        [Fact]
        public void ReciprocalOfReciprocalIsOriginal()
        {
            var seven = new Arithmonym(7);
            var result = seven.Reciprocal.Reciprocal;

            Assert.Equal((double)seven, (double)result, precision: 10);
        }

        [Fact]
        public void InfinityTimesZeroIsNaN()
        {
            var inf = Arithmonym.PositiveInfinity;
            var zero = Arithmonym.Zero;
            var result = inf * zero;

            Assert.True(Arithmonym.IsNaN(result));
        }

        [Fact]
        public void InfinityMinusInfinityIsNaN()
        {
            var inf = Arithmonym.PositiveInfinity;
            var result = inf - inf;

            Assert.True(Arithmonym.IsNaN(result));
        }

        [Fact]
        public void LargeExponentiationWorks()
        {
            var ten = new Arithmonym(10);
            var result = Arithmonym.Exp10(ten); // 10^10

            Assert.Equal(1e10, (double)result, precision: 4);
        }
    }
}
