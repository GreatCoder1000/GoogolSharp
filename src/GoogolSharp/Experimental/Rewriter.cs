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
    public static class Rewriter
    {
        public static Arithmosym Rewrite(Arithmosym expr, IEnumerable<RewriteRule> rules)
        {
            // Try rules at this level
            foreach (var rule in rules)
                if (rule.TryApply(expr, out var replaced) && !ReferenceEquals(replaced, expr))
                    return Rewrite(replaced, rules);

            // Recurse into children
            switch (expr)
            {
                case ArithmosymSum s:
                    return new ArithmosymSum(s.terms.Select(t => Rewrite(t, rules))).GetSimplified();

                case ArithmosymProduct p:
                    return new ArithmosymProduct(p.factors.Select(f => Rewrite(f, rules))).GetSimplified();

                case ArithmosymReciprocal r:
                    return new ArithmosymReciprocal(Rewrite(r.inner, rules)).GetSimplified();

                default:
                    return expr;
            }
        }
    }
}