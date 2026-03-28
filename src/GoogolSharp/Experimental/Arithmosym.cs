using System.Globalization;

namespace GoogolSharp.Experimental
{
    public abstract class Arithmosym : IParsable<Arithmosym>
    {
        public static Arithmosym Zero => new ArithmosymInteger(0);

        public Arithmosym GetSimplified()
        {
            // No rewrite rules — just internal simplification
            return GetSimplifiedInternal();
        }

        internal abstract Arithmosym GetSimplifiedInternal();
        internal abstract string ToInternalString();

        public Arithmonym Evaluate()
        {
            return EvaluateInternal();
        }

        internal abstract Arithmonym EvaluateInternal();


        public override string ToString()
        {
            string s = ToInternalString();
            if (s.StartsWith("(") && s.EndsWith(")"))
                return s.Substring(1, s.Length - 2);
            return s;
        }

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

        public static bool TryParse(string? s, IFormatProvider? provider, out Arithmosym result)
        {
            try
            {
                result = Parse(s, provider);
                return true;
            }
            catch
            {
                result = Zero;
                return false;
            }
        }

        private sealed class Parser
        {
            private readonly string _s;
            private readonly CultureInfo _culture;
            private int _i;

            public Parser(string s, CultureInfo culture)
            {
                _s = s;
                _culture = culture;
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

            public Arithmosym ParseExpression()
            {
                var terms = new List<Arithmosym> { ParseTerm() };

                while (true)
                {
                    if (Eat('+'))
                        terms.Add(ParseTerm());
                    else if (Eat('-'))
                        terms.Add(new ArithmosymProduct(new Arithmosym[]
                        {
                            new ArithmosymInteger(-1),
                            ParseTerm()
                        }));
                    else break;
                }

                return terms.Count == 1 ? terms[0] : new ArithmosymSum(terms);
            }

            private Arithmosym ParseTerm()
            {
                var factors = new List<Arithmosym> { ParseFactor() };

                while (true)
                {
                    if (Eat('*'))
                        factors.Add(ParseFactor());
                    else if (Eat('/'))
                        factors.Add(new ArithmosymReciprocal(ParseFactor()));
                    else if (IsImplicitMultiplication())
                        factors.Add(ParseFactor());
                    else break;
                }

                return factors.Count == 1 ? factors[0] : new ArithmosymProduct(factors);
            }

            private bool IsImplicitMultiplication()
            {
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
                if (Eat('-')) { }

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
