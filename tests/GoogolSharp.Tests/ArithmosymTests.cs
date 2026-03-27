using Xunit;
using GoogolSharp.Experimental;
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
    public class ArithmosymTests
    {
        [Fact]
        public void Parse_ShouldNormalizeExpressionCorrectly()
        {
            // Arrange
            string input = "5+7*(pi-1)";
            string expected = "-2+(7*π)";

            // Act
            var result = Arithmosym.Parse(input, null);
            string? actual = result.ToString();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}