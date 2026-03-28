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

namespace GoogolSharp.Experimental
{
    public readonly struct ArithmonymAngle
    {
        private readonly Arithmonym _value;
        public ArithmonymAngle Normalized => new(_value % Arithmonym.Tau);
        public static ArithmonymAngle Pi => Arithmonym.Pi;
        public static ArithmonymAngle Tau => Arithmonym.Tau;

        public static implicit operator ArithmonymAngle(Arithmonym value)
        {
            return new(value);
        }

        public static implicit operator Arithmonym(ArithmonymAngle value)
        {
            return value._value;
        }

        private ArithmonymAngle(Arithmonym value)
        {
            _value = value;
        }
    }
}