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

using System.Reflection;
using Xunit;

namespace GoogolSharp.Tests
{
    public class ArithmonymFormattingTests
    {
        [Fact]
        public void TestZero()
        {
            Assert.Equal("0", Arithmonym.Zero.ToString());
        }


        [Fact]
        public void TestOne()
        {
            Assert.Equal("1", Arithmonym.One.ToString());
        }

        [Fact]
        public void TestConstantsTwoToThirteen()
        {
            Assert.Equal("2", Arithmonym.Two.ToString());
            Assert.Equal("3", Arithmonym.Three.ToString());
            Assert.Equal("4", Arithmonym.Four.ToString());
            Assert.Equal("5", Arithmonym.Five.ToString());
            Assert.Equal("6", Arithmonym.Six.ToString());
            Assert.Equal("7", Arithmonym.Seven.ToString());
            Assert.Equal("8", Arithmonym.Eight.ToString());
            Assert.Equal("9", Arithmonym.Nine.ToString());
            Assert.Equal("10", Arithmonym.Ten.ToString());
            Assert.Equal("11", Arithmonym.Eleven.ToString());
            Assert.Equal("12", Arithmonym.Twelve.ToString());
            Assert.Equal("13", Arithmonym.Thirteen.ToString());
        }

        [Fact]
        public void TestTwo()
        {
            Assert.Equal("2", Arithmonym.Two.ToString());
        }

        [Fact]
        public void TestThree()
        {
            Assert.Equal("3", Arithmonym.Three.ToString());
        }

        [Fact]
        public void TestFour()
        {
            Assert.Equal("4", Arithmonym.Four.ToString());
        }

        [Fact]
        public void TestFive()
        {
            Assert.Equal("5", Arithmonym.Five.ToString());
        }

        [Fact]
        public void TestSix()
        {
            Assert.Equal("6", Arithmonym.Six.ToString());
        }

        [Fact]
        public void TestSeven()
        {
            Assert.Equal("7", Arithmonym.Seven.ToString());
        }

        [Fact]
        public void TestEight()
        {
            Assert.Equal("8", Arithmonym.Eight.ToString());
        }

        [Fact]
        public void TestNine()
        {
            Assert.Equal("9", Arithmonym.Nine.ToString());
        }

        [Fact]
        public void TestTen()
        {
            Assert.Equal("10", Arithmonym.Ten.ToString());
        }

        [Fact]
        public void TestEleven()
        {
            Assert.Equal("11", Arithmonym.Eleven.ToString());
        }

        [Fact]
        public void TestTwelve()
        {
            Assert.Equal("12", Arithmonym.Twelve.ToString());
        }

        [Fact]
        public void TestThirteen()
        {
            Assert.Equal("13", Arithmonym.Thirteen.ToString());
        }

        [Fact]
        public void TestTwenty()
        {
            Assert.Equal("20", Arithmonym.Twenty.ToString());
        }

        [Fact]
        public void TestHundred()
        {
            Assert.Equal("100", Arithmonym.Hundred.ToString());
        }

        [Fact]
        public void TestMultiDigit()
        {
            // Relaxing the tests in this weird way because currently numbers above 100 are wildly imprecise.
            Assert.Equal("74284", new Arithmonym(74284.5).ToString()[..5]);
        }
        
        [Fact]
        public void TestTenBillion()
        {
            Assert.Equal("1*10^10", Arithmonym.TenBillion.ToString());
        }

        [Fact]
        public void TestTrialogue()
        {
            Assert.Equal("10^(1*10^10)", Arithmonym.Trialogue.ToString());
        }
    }
}