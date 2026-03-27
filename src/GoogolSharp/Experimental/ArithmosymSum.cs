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
    public class ArithmosymSum : Arithmosym
    {
        public readonly List<Arithmosym> terms;

        public ArithmosymSum(IEnumerable<Arithmosym> items)
        {
            terms = [..items];
        }

        public override Arithmosym GetSimplified()
        {
            var flat = new List<Arithmosym>();

            Int128 integerAccumulator = 0;
            bool overflow = false;

            foreach (var t in terms)
            {
                var s = t.GetSimplified();

                switch (s)
                {
                    case ArithmosymInteger ai:
                        if (!overflow)
                        {
                            try
                            {
                                integerAccumulator = checked(integerAccumulator + ai.value);
                                continue;
                            }
                            catch (OverflowException)
                            {
                                overflow = true;
                            }
                        }

                        flat.Add(ai);
                        break;

                    case ArithmosymSum sum:
                        flat.AddRange(sum.terms);
                        break;

                    default:
                        flat.Add(s);
                        break;
                }
            }

            // Insert accumulated integer if nonzero OR if overflow occurred
            if (integerAccumulator != 0 || overflow)
                flat.Insert(0, new ArithmosymInteger(integerAccumulator));

            // If everything cancelled out
            if (flat.Count == 0)
                return new ArithmosymInteger(0);

            // Single term → return it directly
            if (flat.Count == 1)
                return flat[0];

            return new ArithmosymSum(flat);
        }

        public override string ToString()
            => "(" + string.Join("+", terms.Select(t => t.ToString())) + ")";
    }
}
