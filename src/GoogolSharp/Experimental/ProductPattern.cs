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
    public class ProductPattern : ArithmosymPattern
    {
        public readonly List<ArithmosymPattern> factors;

        public ProductPattern(IEnumerable<ArithmosymPattern> factors)
        {
            this.factors = factors.ToList();
        }

        public override bool Match(Arithmosym expr, Dictionary<string, Arithmosym> bindings)
        {
            if (expr is not ArithmosymProduct p)
                return false;

            if (p.factors.Count != factors.Count)
                return false;

            for (int i = 0; i < factors.Count; i++)
                if (!factors[i].Match(p.factors[i], bindings))
                    return false;

            return true;
        }
    }
}