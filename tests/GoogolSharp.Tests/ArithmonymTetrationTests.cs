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
    public class ArithmonymTetrationTests
    {
        // Provided test
        [Fact]
        public void TestTwoTetratedToFour()
        {
            Arithmonym twoTetratedToFour = Arithmonym.Tetration(Arithmonym.Two, Arithmonym.Four);
            AssertArithmonym.NearlyEqual(65536, twoTetratedToFour, 0.01);
        }

        // 1 ↑↑ n = 1 for all n ≥ 1
        [Fact]
        public void TestOneTetratedToFive()
        {
            var result = Arithmonym.Tetration(Arithmonym.One, Arithmonym.Five);
            AssertArithmonym.Equal(Arithmonym.One, result);
        }

        // a ↑↑ 1 = a
        [Fact]
        public void TestThreeTetratedToOne()
        {
            var result = Arithmonym.Tetration(Arithmonym.Three, Arithmonym.One);
            AssertArithmonym.NearlyEqual(Arithmonym.Three, result, 1e-15);
        }

        // 2 ↑↑ 2 = 4
        [Fact]
        public void TestTwoTetratedToTwo()
        {
            var result = Arithmonym.Tetration(Arithmonym.Two, Arithmonym.Two);
            AssertArithmonym.NearlyEqual(Arithmonym.Four, result, 1e-3);
        }

        // 3 ↑↑ 2 = 27
        [Fact]
        public void TestThreeTetratedToTwo()
        {
            var result = Arithmonym.Tetration(Arithmonym.Three, Arithmonym.Two);
            AssertArithmonym.NearlyEqual(Arithmonym.TwentySeven, result, 1e-4);
        }

        // 2 ↑↑ 3 = 16
        [Fact]
        public void TestTwoTetratedToThree()
        {
            var result = Arithmonym.Tetration(Arithmonym.Two, Arithmonym.Three);
            AssertArithmonym.NearlyEqual(Arithmonym.Sixteen, result, 3e-4);
        }

        // 3 ↑↑ 3 = 3^(3^3) = 3^27 = 7625597484987
        [Fact]
        public void TestThreeTetratedToThree()
        {
            var result = Arithmonym.Tetration(Arithmonym.Three, Arithmonym.Three);
            AssertArithmonym.NearlyEqual(7625597484987, result, 1e-2);
        }

        // Edge case: a ↑↑ 0 is often defined as 1 (empty power tower)
        [Fact]
        public void TestTetrationHeightZero()
        {
            var result = Arithmonym.Tetration(Arithmonym.Five, Arithmonym.Zero);
            AssertArithmonym.Equal(Arithmonym.One, result);
        }

        // Check that tetration grows extremely fast but still returns a finite number for small inputs
        [Fact]
        public void TestFourTetratedToThree()
        {
            // 4 ↑↑ 3 = 4^(4^4) = 4^256
            double expected = Math.Pow(4, Math.Pow(4, 4)); // 4^256
            var result = Arithmonym.Tetration(Arithmonym.Four, Arithmonym.Three);
            AssertArithmonym.NearlyEqual(expected, result, 1e-3);
        }

        // Check that tetration grows extremely fast but still returns a finite number for small inputs
        [Fact]
        public void TestSixTetratedToSeven()
        {
            // 6 ↑↑ 7 = eeee2.069197e36305
            Arithmonym expected = Arithmonym.Parse("eeee2.069197e36305", null);
            var result = Arithmonym.Tetration(Arithmonym.Six, Arithmonym.Seven);
            AssertArithmonym.NearlyEqual(expected, result, 1e-3);
        }

        [Fact]
        public void TestFiveTetratedToFour()
        {
            // 5 ↑↑ 4 = e1.3357404839e2184
            Arithmonym expected = Arithmonym.Parse("e1.3357404839e2184", null);
            var result = Arithmonym.Tetration(Arithmonym.Five, Arithmonym.Four);
            AssertArithmonym.NearlyEqual(expected, result, 1e-3);
        }

        [Fact]
        public void TestEightTetratedToFour()
        {
            // 8 ↑↑ 4 = e5.431651e15151335
            Arithmonym expected = Arithmonym.Parse("e5.431651e15151335", null);
            var result = Arithmonym.Tetration(Arithmonym.Eight, Arithmonym.Four);
            AssertArithmonym.NearlyEqual(expected, result, 1e-3);
        }

        [Fact]
        public void TestSevenTetratedToEleven()
        {
            // 7 ↑↑ 11 = eeeeeeee3.17741949e695974
            Arithmonym expected = Arithmonym.Parse("eeeeeeee3.17741949e695974", null);
            var result = Arithmonym.Tetration(Arithmonym.Seven, Arithmonym.Eleven);
            AssertArithmonym.NearlyEqual(expected, result, 1e-3);
        }

        // Symmetry check: tetration is NOT commutative
        [Fact]
        public void TestTetrationIsNotCommutative()
        {
            var a = Arithmonym.Tetration(Arithmonym.Two, Arithmonym.Three); // 16
            var b = Arithmonym.Tetration(Arithmonym.Three, Arithmonym.Two); // 27
            Assert.NotEqual(a, b);
        }
    }
}