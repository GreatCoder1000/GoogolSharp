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
            AssertArithmonym.NearlyEqual(65536, twoTetratedToFour, 1e-10);
        }

        // 1 ↑↑ n = 1 for all n ≥ 1
        [Fact]
        public void TestOneTetratedToFive()
        {
            var result = Arithmonym.Tetration(Arithmonym.One, Arithmonym.Five);
            AssertArithmonym.NearlyEqual(1, result, 1e-10);
        }

        // a ↑↑ 1 = a
        [Fact]
        public void TestThreeTetratedToOne()
        {
            var result = Arithmonym.Tetration(Arithmonym.Three, Arithmonym.One);
            AssertArithmonym.NearlyEqual(3, result, 1e-10);
        }

        // 2 ↑↑ 2 = 4
        [Fact]
        public void TestTwoTetratedToTwo()
        {
            var result = Arithmonym.Tetration(Arithmonym.Two, Arithmonym.Two);
            AssertArithmonym.NearlyEqual(4, result, 1e-10);
        }

        // 3 ↑↑ 2 = 27
        [Fact]
        public void TestThreeTetratedToTwo()
        {
            var result = Arithmonym.Tetration(Arithmonym.Three, Arithmonym.Two);
            AssertArithmonym.NearlyEqual(27, result, 1e-10);
        }

        // 2 ↑↑ 3 = 16
        [Fact]
        public void TestTwoTetratedToThree()
        {
            var result = Arithmonym.Tetration(Arithmonym.Two, Arithmonym.Three);
            AssertArithmonym.NearlyEqual(16, result, 1e-10);
        }

        // 3 ↑↑ 3 = 3^(3^3) = 3^27 = 7625597484987
        [Fact]
        public void TestThreeTetratedToThree()
        {
            var result = Arithmonym.Tetration(Arithmonym.Three, Arithmonym.Three);
            AssertArithmonym.NearlyEqual(7625597484987d, result, 1e-5);
        }

        // Edge case: a ↑↑ 0 is often defined as 1 (empty power tower)
        [Fact]
        public void TestTetrationHeightZero()
        {
            var result = Arithmonym.Tetration(Arithmonym.Five, Arithmonym.Zero);
            AssertArithmonym.NearlyEqual(1, result, 1e-10);
        }

        // Check that tetration grows extremely fast but still returns a finite number for small inputs
        [Fact]
        public void TestFourTetratedToThree()
        {
            // 4 ↑↑ 3 = 4^(4^4) = 4^256
            double expected = Math.Pow(4, Math.Pow(4, 4)); // 4^256
            var result = Arithmonym.Tetration(Arithmonym.Four, Arithmonym.Three);
            AssertArithmonym.NearlyEqual(expected, result, 1e-5);
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