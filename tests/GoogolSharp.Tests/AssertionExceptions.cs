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


using Xunit.Sdk;
namespace GoogolSharp.Tests
{

    /// <summary>
    /// Exception thrown when two Arithmonym values are not equal.
    /// </summary>
    internal class ArithmonymEqualException(string message) : XunitException(message)
    {
    }

    /// <summary>
    /// Exception thrown when two Arithmonym values differ more than the allowed tolerance.
    /// </summary>
    internal class ArithmonymNearlyEqualException(string message) : XunitException(message)
    {
    }

    /// <summary>
    /// Exception thrown when two Float128 values are not equal.
    /// </summary>
    internal class Float128EqualException(string message) : XunitException(message)
    {
    }

    /// <summary>
    /// Exception thrown when two Float128 values differ more than the allowed tolerance.
    /// </summary>
    internal class Float128NearlyEqualException(string message) : XunitException(message)
    {
    }
}