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
    public class ArithmosymReciprocal : Arithmosym
    {
        public readonly Arithmosym inner;

        public ArithmosymReciprocal(Arithmosym inner)
        {
            this.inner = inner;
        }

        public override Arithmosym GetSimplified()
        {
            var s = inner.GetSimplified();

            switch (s)
            {
                case ArithmosymInteger ai:
                    // 1 / 0 → keep symbolic (no exception)
                    if (ai.value == 1)
                        return new ArithmosymInteger(1);

                    return new ArithmosymProduct(new Arithmosym[]
                    {
                        new ArithmosymInteger(1),
                        new ArithmosymReciprocal(ai)
                    });

                case ArithmosymReciprocal r:
                    // 1 / (1/x) → x
                    return r.inner;

                case ArithmosymProduct p:
                    // 1/(a*b*c) → (1/a)*(1/b)*(1/c)
                    return new ArithmosymProduct(
                        p.factors.Select(f => new ArithmosymReciprocal(f))
                    ).GetSimplified();

                default:
                    return new ArithmosymReciprocal(s);
            }
        }

        public override string ToString()
            => "(1/" + inner.ToString() + ")";
    }
}
