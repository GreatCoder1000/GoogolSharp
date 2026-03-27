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


using System;
using QuadrupleLib;
using QuadrupleLib.Accelerators;
using Float128 = QuadrupleLib.Float128<QuadrupleLib.Accelerators.DefaultAccelerator>;
using GoogolSharp.Helpers;
using Xunit.Sdk;
namespace GoogolSharp.Tests
{

    internal static class AssertArithmonym
    {
        /// <summary>
        /// Asserts that two Arithmonym values are equal.
        /// </summary>
        internal static void Equal(Arithmonym expected, Arithmonym actual)
        {
            if (expected != actual)
                throw new ArithmonymEqualException($"{nameof(AssertArithmonym)}.{nameof(Equal)} failure: Values differ\nExpected: {expected}\nActual: {actual}.");
        }

        /// <summary>
        /// Asserts that two Arithmonym values are nearly equal within a specified tolerance.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="operandTolerance">The maximum allowed difference.</param>
        internal static void NearlyEqual(Arithmonym expected, Arithmonym actual, Float128 operandTolerance)
        {
            if (!Arithmonym.NearlyEqual(expected, actual, operandTolerance))
                throw new ArithmonymNearlyEqualException($"{nameof(AssertArithmonym)}.{nameof(NearlyEqual)} failure: Values differ more than operandTolerance {operandTolerance}\nExpected: {expected}\nActual: {actual}.");
        }
    }
}