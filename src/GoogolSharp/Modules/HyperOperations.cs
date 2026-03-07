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
    partial struct Arithmonym
    {
        public static Arithmonym Tetration(Arithmonym baseV, Arithmonym heightV)
        {
            throw new NotImplementedException("TODO: Get Tetration to work");
        }

        public static Arithmonym Tetration10Linear(Arithmonym value)
        {
            if (value < Two)
            {
                return new(Float128HyperTranscendentals.LetterF(value));
            }
            if (value < Ten)
            {
                return new(false, false, 0x06, EncodeOperand(value.ToFloat128()));
            }
            if (value < Dekalogue)
            {
                // Hi anyone reading this!
                // I'm a comment! The compiler ignores me :(
                // You won't, luckily.
                // I'm here to tell you that please don't look at that line of code
                // It's confusing
                // Yet it works. And it satisfies tests. Leave that line alone
                // Thank you!!

                /*
                  Hi guys it's another me
                  I'm another comment
                  Don't look at the comment above me
                  That comment above me is not even multi line

                  Cuz you're smart enough to understand this
                  Remember that we're using linear approximation inside
                  So remember also this function is 10^^x
                  If that x is >=10 then its not so easy
                  We have to turn the F-form (e.g. F-form of 10 is 1,
                  of 10^10 is 2, of 10^10^10 is 3, etc) into a J-form
                  because J is after F in this system

                  The maintainer of this code is too lazy to use conventions
                  because that would make a bit too much edge cases

                  It's still a bit hard to understand right

                  You want an equation? Alright!
                  [Formula works assuming x is between 10 and 10^10]

                  10^^x = 10^^10^10^log(log(x)) = 10^^10^^(1+log(log(x)))
                  = 10^^10^^10^^(log(1+log(log(x)))) = 10^^^(2+log(1+log(log(x))))
                  
                  Now we converted it to G form. On to J form!
                  Remember if the number the G form outputs is in [2,10) we can use
                  a cool shortie cuttie (the whole program is one)

                  let a = log(1+log(log(x))) <-- you can guess its a small number
                  10^^^(2+a) = 10^... 3 + log5(1+(a/2)) arrows ...^10
                  = 10^...(2 + log5(1+((log(1+log(log(x))))/2)))...^10

                  😰 That was some serious computation!

                  *LMAO*
                */

                // That comment above me is too long. Clean Code would delete it right away.

                // Yes, my twin! I do agree. But sadly we can't talk about this to the author of the code.

                // Isn't the code more self-documenting? Just look below ME!
                return new(
                    false,
                    false,
                    0x07,
                    EncodeOperand(
                        LetterGToLetterJ(LetterFToLetterG(value)).ToFloat128()
                    )
                );
            }
            throw new ArgumentOutOfRangeException("TODO");
        }

        private static Arithmonym LetterFToLetterG(Arithmonym value)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, Zero);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, Dekalogue);
            // 10^^(10^^2) = 10^^^(2+log(2))
            // 10^^(10^^^3) = 10^^^operand+1
            return (value < One) ? value : 
                (value < Ten) ? value._Log10 + 1 : 
                (value < TenBillion) ? ((value._Log10._Log10 + One)._Log10 + Two) : new Arithmonym(Float128PreciseTranscendentals.SafeLog10(value.Operand)) + Two;
        }

        private static Arithmonym LetterGToLetterJ(Arithmonym value)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, Ten);
            if (value <= Two) return value;
            return (value / Two)._Log10 / Five._Log10;
        }
    }
}