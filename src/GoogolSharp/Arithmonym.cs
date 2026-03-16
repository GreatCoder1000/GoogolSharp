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

using GoogolSharp.Helpers;
using QuadrupleLib;
using QuadrupleLib.Accelerators;
using Float128 = QuadrupleLib.Float128<QuadrupleLib.Accelerators.DefaultAccelerator>;
using System.Globalization;
using System.Numerics;

namespace GoogolSharp
{
    /// <summary>
    /// A large number.
    /// </summary>
    /// <remarks>
    /// A number is represented using 4 basic fields.
    /// _IsNegative (tracks sign), _IsReciprocal (allows for numbers below 1),
    /// Letter (allows for both tiny numbers like 100, and googological giants),
    /// Operand (>=2, <10)
    /// 
    /// Bit layout in a 96-bit word:
    /// [n 1][r 1][l 6][i 3][f 85]
    /// 
    /// - n: _IsNegative
    /// - r: _IsReciprocal
    /// - l: Letter
    /// - i: OperandFloored-2 (3 bits)
    /// - f: Fraction (Q3.85)
    /// </remarks>
    public readonly partial struct Arithmonym :
        IEquatable<Arithmonym>,
        IEqualityOperators<Arithmonym, Arithmonym, bool>,
        IComparable,
        IComparisonOperators<Arithmonym, Arithmonym, bool>,
        IAdditionOperators<Arithmonym, Arithmonym, Arithmonym>,
        IAdditiveIdentity<Arithmonym, Arithmonym>,
        ISubtractionOperators<Arithmonym, Arithmonym, Arithmonym>,
        IMultiplyOperators<Arithmonym, Arithmonym, Arithmonym>,
        IMultiplicativeIdentity<Arithmonym, Arithmonym>,
        IDivisionOperators<Arithmonym, Arithmonym, Arithmonym>,
        IExponentialFunctions<Arithmonym>,
        INumber<Arithmonym>,
        INumberBase<Arithmonym>
    {
        // See Modules/ for implementation.
        // For some unincluded stuff like Float128PreciseTranscendentals look in Helpers/
    }
}
