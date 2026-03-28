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
using Float128 = QuadrupleLib.Float128<QuadrupleLib.Accelerators.DefaultAccelerator>;

namespace GoogolSharp
{
    partial struct Arithmonym
    {
        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents a quiet Not-a-Number (QNaN).
        /// </summary>
        public static Arithmonym NaN => new(
            (((UInt128)1) << (FRACTION_BITS + 9)) |
            (((UInt128)0x3f) << (FRACTION_BITS + 3)) |
            (((UInt128)1) << (FRACTION_BITS + 2))
        );

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents zero.
        /// </summary>
        public static Arithmonym Zero => new(isNegative: false, _IsReciprocal: true, 0x3f, 0);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the additive identity (zero).
        /// </summary>
        public static Arithmonym AdditiveIdentity => new(isNegative: false, _IsReciprocal: true, 0x3f, 0);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value one.
        /// </summary>
        public static Arithmonym One => new(isNegative: false, _IsReciprocal: false, 0x01, 0);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the multiplicative identity (one).
        /// </summary>
        public static Arithmonym MultiplicativeIdentity => new(isNegative: false, _IsReciprocal: false, 0x01, 0);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value negative one (-1).
        /// </summary>
        public static Arithmonym NegativeOne => new(isNegative: true, _IsReciprocal: false, 0x01, 0);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value negative two.
        /// </summary>
        public static Arithmonym NegativeTwo => new(isNegative: true, _IsReciprocal: false, 0x02, 0);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value two.
        /// </summary>
        public static Arithmonym Two => new(isNegative: false, _IsReciprocal: false, 0x02, 0);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the natural logarithm of 10.
        /// </summary>
        public static Arithmonym Ln10 => new(Float128PreciseTranscendentals.SafeLog(10));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents Euler's number (the Napier Constant).
        /// </summary>
        public static Arithmonym E => new(Float128.E);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value three.
        /// </summary>
        public static Arithmonym Three => new(isNegative: false, _IsReciprocal: false, 0x02, EncodeOperand(6));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents pi.
        /// </summary>
        public static Arithmonym Pi => new(Float128.Pi);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the base-2 logarithm of 10.
        /// </summary>
        public static Arithmonym Log2_10 => new(Float128PreciseTranscendentals.Log2_10);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value four.
        /// </summary>
        public static Arithmonym Four => new(isNegative: false, _IsReciprocal: false, 0x03, 0);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value five.
        /// </summary>
        public static Arithmonym Five => new(isNegative: false, _IsReciprocal: false, 0x03, EncodeOperand((Float128)2.5));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value six.
        /// </summary>
        public static Arithmonym Six => new(isNegative: false, _IsReciprocal: false, 0x03, EncodeOperand((Float128)3));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents tau.
        /// </summary>
        public static Arithmonym Tau => new(Float128.Tau);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value seven.
        /// </summary>
        public static Arithmonym Seven => new(isNegative: false, _IsReciprocal: false, 0x03, EncodeOperand((Float128)3.5));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value eight.
        /// </summary>
        public static Arithmonym Eight => new(isNegative: false, _IsReciprocal: false, 0x03, EncodeOperand((Float128)4));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value nine.
        /// </summary>
        public static Arithmonym Nine => new(isNegative: false, _IsReciprocal: false, 0x03, EncodeOperand((Float128)4.5));


        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value ten.
        /// </summary>
        public static Arithmonym Ten => new(isNegative: false, _IsReciprocal: false, 0x03, EncodeOperand((Float128)5));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value 11.
        /// </summary>
        public static Arithmonym Eleven => new(isNegative: false, _IsReciprocal: false, 0x03, EncodeOperand((Float128)5.5));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value 12.
        /// </summary>
        public static Arithmonym Twelve => new(isNegative: false, _IsReciprocal: false, 0x03, EncodeOperand((Float128)6));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value 13.
        /// </summary>
        public static Arithmonym Thirteen => new(isNegative: false, _IsReciprocal: false, 0x03, EncodeOperand((Float128)6.5));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value 14.
        /// </summary>
        public static Arithmonym Fourteen => new(isNegative: false, _IsReciprocal: false, 0x03, EncodeOperand((Float128)7));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value 15.
        /// </summary>
        public static Arithmonym Fifteen => new(isNegative: false, _IsReciprocal: false, 0x03, EncodeOperand((Float128)7.5));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value 16.
        /// </summary>
        public static Arithmonym Sixteen => new(isNegative: false, _IsReciprocal: false, 0x03, EncodeOperand((Float128)8));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value 20.
        /// </summary>
        public static Arithmonym Twenty => new(isNegative: false, _IsReciprocal: false, 0x04, 0);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value 27.
        /// </summary>
        public static Arithmonym TwentySeven => new(isNegative: false, _IsReciprocal: false, 0x04, EncodeOperand((Float128)2.7));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value 100.
        /// </summary>
        public static Arithmonym Hundred => new(isNegative: false, _IsReciprocal: false, 0x05, 0);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value 10^10.
        /// 
        /// The name Dialogue was also suggested for this constant, but it is a bit ambiguous and used in other places too.
        /// </summary>
        public static Arithmonym TenBillion => new(isNegative: false, _IsReciprocal: false, 0x06, 0);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value 10^(10^10).
        /// </summary>
        public static Arithmonym Trialogue => new(isNegative: false, _IsReciprocal: false, 0x06, EncodeOperand((Float128)3));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value 10^(10^(10^10)).
        /// </summary>
        public static Arithmonym Tetralogue => new(isNegative: false, _IsReciprocal: false, 0x06, EncodeOperand((Float128)4));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value 10^(10^(10^(10^10))).
        /// </summary>
        public static Arithmonym Pentalogue => new(isNegative: false, _IsReciprocal: false, 0x06, EncodeOperand((Float128)5));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value 10^(10^(10^(10^(10^(10^(10^(10^(10^10)))))))).
        /// </summary>
        public static Arithmonym Dekalogue => new(isNegative: false, _IsReciprocal: false, 0x07, 0);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value 10^^^3
        /// </summary>
        public static Arithmonym Triateraksys => new(isNegative: false, _IsReciprocal: false, 0x07, EncodeOperand(2 + Float128PreciseTranscendentals.SafeLog2(1.5) / Float128PreciseTranscendentals.SafeLog2(5)));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value 10^^^10 (or J3)
        /// </summary>
        public static Arithmonym Dekateraksys => new(isNegative: false, _IsReciprocal: false, 0x07, EncodeOperand(3));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value 10^^^^3
        /// </summary>
        public static Arithmonym Triapetaksys => new(isNegative: false, _IsReciprocal: false, 0x07, EncodeOperand(3 + Float128PreciseTranscendentals.SafeLog2(1.5) / Float128PreciseTranscendentals.SafeLog2(5)));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents SCG(2)'s lower bound ~ T2
        /// </summary>
        public static Arithmonym Scg2LowerBound => new(isNegative: false, _IsReciprocal: false, 0x10, 0);

        /// <summary>
        /// The radix.
        /// 
        /// Note that exponential scaling uses base 10, but mantissas/operands are still encoded in base 2.
        /// Since all base 2 values can be represented exactly in base 10 (although more digits may be required),
        /// radix is set to 10.
        /// </summary>
        public static int Radix => 10;

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents positive infinity (+∞).
        /// </summary>
        public static Arithmonym PositiveInfinity => new(isNegative: false, _IsReciprocal: false, 0x3f, 0);


        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents negative infinity (-∞).
        /// </summary>
        public static Arithmonym NegativeInfinity => new(isNegative: true, _IsReciprocal: false, 0x3f, 0);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the largest technically representable value.
        /// Notice that a number this big has absolutely NO mathematical or googological definition,
        /// and it surpasses everything, even Rayo's Number. In fact, we can't be sure about anything at this range.
        /// 
        /// Plus, the largest allowed letter, 62, is already too much. Probably a letter number less than 30 is enough.
        /// As said, nothing is sure at this level. We are doing guesswork with common sense, which isn't so useful
        /// at this level.
        /// 
        /// See also: <seealso cref="MinValue"/>, <seealso cref="Epsilon"/>
        /// </summary>
        public static Arithmonym MaxValue => new(isNegative: false, _IsReciprocal: false, 0x3e, UInt128.MaxValue);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the smallest technically representable value.
        /// Note this is negative. For the smallest POSITIVE representible number, see <see cref="Epsilon"/>.
        /// 
        /// See also: <seealso cref="MaxValue"/>, <seealso cref="Epsilon"/>
        /// </summary>
        public static Arithmonym MinValue => -MaxValue;

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the smallest positive, technically representable value.
        /// 
        /// It's so tiny, it's smaller than the reciprocal of Rayo's Number.
        /// Remember in calculus we used Δx = 0.01, 0.000001, 0.00000000000000001, etc.?
        /// Here this is the tiniest number, and the tiniest number in the whole of math.
        /// (NOT AS MUCH OF AN EXAGGERATION AS YOU THINK!!)
        /// 
        /// In fact, due to its immense tininess you can actually use it in place of Δx.
        /// Whatever you do to it -- *2, sqrt, ^2, etc. it's still the same Epsilon.
        /// However, it may not be the best design
        /// 
        /// See also: <seealso cref="MaxValue"/>, <seealso cref="MinValue"/>
        /// </summary>
        public static Arithmonym Epsilon => MaxValue.Reciprocal;
    }
}