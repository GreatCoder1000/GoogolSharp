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
    public class ArithmonymPentationTests
    {
        // Provided test
        [Fact]
        public void TestTwoPentatedToThree()
        {
            Arithmonym twoPentatedToThree = Arithmonym.Pentation(Arithmonym.Two, Arithmonym.Three);
            AssertArithmonym.NearlyEqual(65536, twoPentatedToThree, 0.1);
        }

        // 1 ↑↑ n = 1 for all n ≥ 1
        [Fact]
        public void TestOnePentatedToFive()
        {
            var result = Arithmonym.Pentation(Arithmonym.One, Arithmonym.Five);
            AssertArithmonym.Equal(Arithmonym.One, result);
        }

        // a ↑↑ 1 = a
        [Fact]
        public void TestThreePentatedToOne()
        {
            var result = Arithmonym.Pentation(Arithmonym.Three, Arithmonym.One);
            AssertArithmonym.NearlyEqual(Arithmonym.Three, result, 1e-15);
        }

        // 2 ↑↑ 2 = 4
        [Fact]
        public void TestTwoPentatedToTwo()
        {
            var result = Arithmonym.Pentation(Arithmonym.Two, Arithmonym.Two);
            AssertArithmonym.NearlyEqual(Arithmonym.Four, result, 1e-3);
        }

        [Fact]
        public void TestThreePentatedToTwo()
        {
            var result = Arithmonym.Pentation(Arithmonym.Three, Arithmonym.Two);
            AssertArithmonym.NearlyEqual(7625597484987L, result, 1e-3);
        }

        // Edge case: a ↑↑ 0 is often defined as 1 (empty power tower)
        [Fact]
        public void TestPentationHeightZero()
        {
            var result = Arithmonym.Pentation(Arithmonym.Five, Arithmonym.Zero);
            AssertArithmonym.Equal(Arithmonym.One, result);
        }

        // Check that pentation grows extremely fast but still returns a finite number for small inputs
        [Fact]
        public void TestThreePentatedToThree()
        {
            // 3 ↑↑↑ 3 approximation
            Arithmonym expected = Arithmonym.Parse("F7625597484986.041", null);
            var result = Arithmonym.Pentation(Arithmonym.Three, Arithmonym.Three);
            AssertArithmonym.NearlyEqual(expected, result, 1e-3);
        }

        [Fact]
        public void TestFourPentatedToThree()
        {
            // 4 ↑↑↑ 3 approximation
            Arithmonym expected = Arithmonym.Parse("Fe8.07230472603e153", null);
            var result = Arithmonym.Pentation(Arithmonym.Four, Arithmonym.Three);
            AssertArithmonym.NearlyEqual(expected, result, 1e-3);
        }
        
        // Symmetry check: tetration is NOT commutative
        [Fact]
        public void TestTetrationIsNotCommutative()
        {
            var a = Arithmonym.Pentation(Arithmonym.Two, Arithmonym.Three); // 65536
            var b = Arithmonym.Pentation(Arithmonym.Three, Arithmonym.Two); // 7625597484987
            Assert.NotEqual(a, b);
        }
    }
}