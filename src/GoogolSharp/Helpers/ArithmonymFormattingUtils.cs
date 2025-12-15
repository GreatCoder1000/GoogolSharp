using QuadrupleLib;

namespace GoogolSharp.Helpers
{
    public static class ArithmonymFormattingUtils
    {
        public static string FormatArithmonymFromLetterF(Float128 letterF, bool isReciprocal)
        {
            if (letterF < 2) return new Arithmonym(Float128HyperTranscendentals.LetterF(letterF)).ToString();
            if (letterF < 3)
            {
                Float128 letterE = letterF - 2;
                letterE = Float128PreciseTranscendentals.SafeExp10(
                    Float128PreciseTranscendentals.SafeExp10(
                        Float128PreciseTranscendentals.SafeExp10(
                            letterE
                        )
                    )
                );
                return FormatArithmonymScientific(letterE, isReciprocal);
            }

            // TODO
            return $"{(isReciprocal ? "1 / " : "")}F{letterF}";
        }

        public static string FormatArithmonymScientific(Float128 letterE, bool isReciprocal)
        {
            letterE = Float128PreciseTranscendentals.SafeExp10(
                Float128PreciseTranscendentals.SafeExp10(
                    Float128PreciseTranscendentals.SafeExp10(
                        letterE
                    )
                )
            );
            Float128 exponent = Float128.Floor(letterE);
            Float128 significand = Float128PreciseTranscendentals.SafeExp10(
                letterE - exponent);
            return $"{significand}e{(isReciprocal ? "-" : "+")}{(ulong)exponent}";
        }
    }
}