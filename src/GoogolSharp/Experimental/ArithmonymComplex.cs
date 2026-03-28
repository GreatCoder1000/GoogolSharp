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

namespace GoogolSharp.Experimental
{
    public readonly struct ArithmonymComplex(Arithmonym real, Arithmonym imaginary)
    {
        private readonly Arithmonym real = real;
        private readonly Arithmonym imaginary = imaginary;

        public Arithmonym Real => real;
        public Arithmonym Imaginary => imaginary;
        public Arithmonym Magnitude => Arithmonym.Sqrt(Real*Real + Imaginary*Imaginary);
        
        // bd - ac / ad + bc
        public Arithmonym Phase => Arithmonym.Atan2(Imaginary, Real);
        public ArithmonymAngle Theta => Phase;
    }
}