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
    public class ArithmosymSum(IEnumerable<Arithmosym> items) : Arithmosym
    {
        public readonly List<Arithmosym> terms = [.. items];

        internal override Arithmosym GetSimplifiedInternal()
        {
            var flat = new List<Arithmosym>();

            Int128 integerAccumulator = 0;
            bool overflow = false;

            foreach (var t in terms)
            {
                var s = t.GetSimplifiedInternal();

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

        internal override string ToInternalString()
            => "(" + string.Join("+", terms.Select(t => t.ToInternalString())) + ")";

        internal override Arithmonym EvaluateInternal()
        {
            Arithmonym acc = Arithmonym.Zero;
            foreach (var t in terms)
                acc += t.EvaluateInternal();
            return acc;
        }
    }
}
