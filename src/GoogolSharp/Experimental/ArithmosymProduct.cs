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
    public class ArithmosymProduct : Arithmosym
    {
        public readonly List<Arithmosym> factors;

        public ArithmosymProduct(IEnumerable<Arithmosym> items)
        {
            factors = [..items];
        }

        public override Arithmosym GetSimplified()
        {
            var flat = new List<Arithmosym>();

            Int128 integerAccumulator = 1;
            bool overflow = false;

            foreach (var f in factors)
            {
                var s = f.GetSimplified();

                switch (s)
                {
                    case ArithmosymInteger ai:
                        if (ai.value == 0)
                            return new ArithmosymInteger(0);

                        if (ai.value == 1)
                            continue;

                        if (!overflow)
                        {
                            try
                            {
                                integerAccumulator = checked(integerAccumulator * ai.value);
                                continue;
                            }
                            catch (OverflowException)
                            {
                                overflow = true;
                            }
                        }

                        flat.Add(ai);
                        break;

                    case ArithmosymProduct p:
                        flat.AddRange(p.factors);
                        break;

                    default:
                        flat.Add(s);
                        break;
                }
            }

            if (!overflow && integerAccumulator != 1)
                flat.Insert(0, new ArithmosymInteger(integerAccumulator));
            else if (overflow)
                flat.Insert(0, new ArithmosymInteger(integerAccumulator)); // keep overflowed integer as-is

            if (flat.Count == 0)
                return new ArithmosymInteger(1);

            if (flat.Count == 1)
                return flat[0];

            return new ArithmosymProduct(flat);
        }

        public override string ToString()
            => "(" + string.Join("*", factors.Select(f => f.ToString())) + ")";
    }
}
