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

using System.Globalization;

namespace GoogolSharp.Experimental
{
    /// <summary>
    /// A *precise* number. If ultra precision is not your thing, then use <see cref="Arithmonym"/>.
    /// Still experimental.
    /// </summary>
    public abstract class Arithmosym : IParsable<Arithmosym>
    {
        public static Arithmosym Zero => new ArithmosymInteger(0);
        public abstract Arithmosym GetSimplified();

        public static Arithmosym Parse(string? s, IFormatProvider? provider)
        {
            if (string.IsNullOrWhiteSpace(s))
                return Zero;

            var culture = provider as CultureInfo ?? CultureInfo.InvariantCulture;

            string input = s
                .Replace("pi", "π", StringComparison.OrdinalIgnoreCase)
                .Replace(" ", "")
                .Trim();

            var parser = new Parser(input, culture);
            return parser.ParseExpression().GetSimplified();
        }

        public static bool TryParse(string? s, IFormatProvider? provider, out Arithmosym arithmosym)
        {
            try
            {
                arithmosym = Parse(s, provider);
                return true;
            }
            catch
            {
                arithmosym = Zero;
                return false;
            }
        }

        /// <summary>
        /// Very small recursive-descent parser for symbolic expressions.
        /// Supports +, -, *, /, parentheses, integers, π, e.
        /// </summary>
        private sealed class Parser
        {
            private readonly string _s;
            private readonly CultureInfo _culture;
            private int _i;

            public Parser(string s, CultureInfo culture)
            {
                _s = s;
                _culture = culture;
                _i = 0;
            }

            private bool End => _i >= _s.Length;
            private char Current => End ? '\0' : _s[_i];

            private void Eat() => _i++;

            private bool Eat(char c)
            {
                if (Current == c)
                {
                    _i++;
                    return true;
                }
                return false;
            }

            // ---------------------------
            // Grammar:
            //   Expr   := Term (('+' | '-') Term)*
            //   Term   := Factor (('*' | '/') Factor | implicit-mul Factor)*
            //   Factor := Number | Constant | '(' Expr ')'
            // ---------------------------

            public Arithmosym ParseExpression()
            {
                var terms = new List<Arithmosym> { ParseTerm() };

                while (true)
                {
                    if (Eat('+'))
                    {
                        terms.Add(ParseTerm());
                    }
                    else if (Eat('-'))
                    {
                        var right = ParseTerm();
                        // represent subtraction as adding (-1 * right)
                        terms.Add(new ArithmosymProduct(new[] {
                new ArithmosymInteger(-1),
                right
            }));
                    }
                    else break;
                }

                return terms.Count == 1
                    ? terms[0]
                    : new ArithmosymSum(terms);
            }

            private Arithmosym ParseTerm()
            {
                var factors = new List<Arithmosym> { ParseFactor() };

                while (true)
                {
                    if (Eat('*'))
                    {
                        factors.Add(ParseFactor());
                    }
                    else if (Eat('/'))
                    {
                        var right = ParseFactor();
                        factors.Add(new ArithmosymReciprocal(right));
                    }
                    else if (IsImplicitMultiplication())
                    {
                        factors.Add(ParseFactor());
                    }
                    else break;
                }

                return factors.Count == 1
                    ? factors[0]
                    : new ArithmosymProduct(factors);
            }

            private bool IsImplicitMultiplication()
            {
                // Examples:
                //   2π
                //   3(4+5)
                //   πx   (if you later add variables)
                if (End) return false;

                char c = Current;
                return char.IsDigit(c) || c == '(' || c == 'π' || char.ToLowerInvariant(c) == 'e';
            }

            private Arithmosym ParseFactor()
            {
                if (Eat('('))
                {
                    var expr = ParseExpression();
                    if (!Eat(')'))
                        throw new FormatException("Missing closing parenthesis.");
                    return expr;
                }

                if (Current == 'π')
                {
                    Eat();
                    return new ArithmosymPi();
                }

                if (char.ToLowerInvariant(Current) == 'e')
                {
                    Eat();
                    return new ArithmosymE();
                }

                return ParseNumber();
            }

            private Arithmosym ParseNumber()
            {
                int start = _i;

                if (Eat('-')) { } // allow leading minus

                while (!End && char.IsDigit(Current))
                    Eat();

                string slice = _s.Substring(start, _i - start);

                if (!int.TryParse(slice, NumberStyles.Integer, _culture, out int value))
                    throw new FormatException($"Invalid number literal: '{slice}'");

                return new ArithmosymInteger(value);
            }
        }
    }
}