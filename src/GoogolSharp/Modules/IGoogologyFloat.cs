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

using System.Numerics;

namespace GoogolSharp
{
    /// <summary>
    /// Marker interface for googological number types that implement all standard numeric operations.
    /// This allows any implementation to satisfy the numeric interface contract.
    /// </summary>
    public interface IGoogologyFloat<TSelf> : 
        System.Numerics.IExponentialFunctions<TSelf>, 
        System.Numerics.IFloatingPointConstants<TSelf>, 
        System.Numerics.INumber<TSelf>, 
        IComparable, 
        IComparable<TSelf>, 
        System.Numerics.IComparisonOperators<TSelf, TSelf, bool>, 
        System.Numerics.IModulusOperators<TSelf, TSelf, TSelf>, 
        System.Numerics.INumberBase<TSelf>, 
        IEquatable<TSelf>, 
        ISpanFormattable, 
        IFormattable, 
        ISpanParsable<TSelf>, 
        IParsable<TSelf>, 
        System.Numerics.IAdditionOperators<TSelf, TSelf, TSelf>, 
        System.Numerics.IAdditiveIdentity<TSelf, TSelf>, 
        System.Numerics.IDecrementOperators<TSelf>, 
        System.Numerics.IDivisionOperators<TSelf, TSelf, TSelf>, 
        System.Numerics.IEqualityOperators<TSelf, TSelf, bool>, 
        System.Numerics.IIncrementOperators<TSelf>, 
        System.Numerics.IMultiplicativeIdentity<TSelf, TSelf>, 
        System.Numerics.IMultiplyOperators<TSelf, TSelf, TSelf>, 
        System.Numerics.ISubtractionOperators<TSelf, TSelf, TSelf>, 
        System.Numerics.IUnaryNegationOperators<TSelf, TSelf>, 
        System.Numerics.IUnaryPlusOperators<TSelf, TSelf>, 
        IUtf8SpanFormattable, 
        IUtf8SpanParsable<TSelf>,
        ILogarithmicFunctions<TSelf>,
        IPowerFunctions<TSelf>,
        IRootFunctions<TSelf>,
        ISignedNumber<TSelf>
        where TSelf : IGoogologyFloat<TSelf>
    {
        static abstract TSelf Neg(TSelf value);
        static abstract TSelf Factorial(TSelf value);
        static abstract TSelf Permutations(TSelf n, TSelf r);
        static abstract TSelf Combinations(TSelf n, TSelf r);
        static abstract TSelf Tetration(TSelf baseV, TSelf heightV);
        static abstract TSelf PowerTower(TSelf a, TSelf b, TSelf c);
    }
}