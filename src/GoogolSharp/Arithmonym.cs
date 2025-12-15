using System.Numerics;
using QuadrupleLib;
using GoogolSharp.Helpers;

namespace GoogolSharp
{
    /// <summary>
    /// A large number.
    /// </summary>
    /// <remarks>
    /// A number is represented using 4 basic fields.
    /// _IsNegative (tracks sign), _IsReciprocal (allows for numbers below 1),
    /// Letter (allows for both tiny numbers like 100, and googological giants),
    /// Operand (>=2, <10)
    /// 
    /// Bit layout in a 96-bit word:
    /// [n 1][r 1][l 6][i 3][f 85]
    /// 
    /// - n: _IsNegative
    /// - r: _IsReciprocal
    /// - l: Letter
    /// - i: OperandFloored-2 (3 bits)
    /// - f: Fraction (Q3.85)
    /// </remarks>
    public readonly struct Arithmonym :
        IEquatable<Arithmonym>,
        IEqualityOperators<Arithmonym, Arithmonym, bool>,
        IComparable,
        IComparisonOperators<Arithmonym, Arithmonym, bool>
    {
        #region Core functionality
        private readonly uint squishedLo;
        private readonly uint squishedMid;
        private readonly uint squishedHi;

        // Reconstruct the original 96-bit stored value as a UInt128 when needed.
        /// <summary>
        /// Reconstructs the packed internal representation into a <see cref="UInt128"/> value.
        /// The implementation stores the value across three 32-bit words; this property
        /// recombines them into the original integer layout used elsewhere in the type.
        /// </summary>
        private UInt128 Squished
        {
            get
            {
                return (((UInt128)squishedHi) << 64) | (((UInt128)squishedMid) << 32) | ((UInt128)squishedLo);
            }
        }
        private const int FRACTION_BITS = 85;

        #region Internal properties

        internal readonly byte OperandFloored
        {
            get
            {
                UInt128 mask = ((UInt128)0x7) << FRACTION_BITS;
                return (byte)(2 + (byte)((Squished & mask) >> FRACTION_BITS));
            }
        }

        internal readonly byte Letter
        {
            get
            {
                UInt128 mask = ((UInt128)0x3F) << (FRACTION_BITS + 3);
                return (byte)((Squished & mask) >> (FRACTION_BITS + 3));
            }
        }

        internal readonly bool _IsReciprocal
        {
            get
            {
                return ((Squished >> (FRACTION_BITS + 9)) & (UInt128)1) == (UInt128)1;
            }
        }

        internal readonly UInt128 OperandFraction128
        {
            get
            {
                // Mask the lower FRACTION_BITS to get the fraction field
                UInt128 mask = (((UInt128)1) << FRACTION_BITS) - 1;
                return Squished & mask;
            }
        }

        internal readonly Float128 Operand
        {
            get
            {
                return OperandFloored + (UInt128ToFloat128(OperandFraction128) / Float128PreciseTranscendentals.SafeExp2(FRACTION_BITS));
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents a quiet Not-a-Number (QNaN).
        /// </summary>
        public static readonly Arithmonym NaN = new(
            (((UInt128)1) << (FRACTION_BITS + 9)) |
            (((UInt128)0x3f) << (FRACTION_BITS + 3)) |
            (((UInt128)1) << (FRACTION_BITS + 2))
        );

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents zero.
        /// </summary>
        public static readonly Arithmonym Zero = new(isNegative: false, _IsReciprocal: true, 0x3f, 0);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value one.
        /// </summary>
        public static readonly Arithmonym One = new(isNegative: false, _IsReciprocal: false, 0x01, 0);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value negative one (-1).
        /// </summary>
        public static readonly Arithmonym NegativeOne = new(isNegative: true, _IsReciprocal: false, 0x01, 0);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value two.
        /// </summary>
        public static readonly Arithmonym Two = new(isNegative: false, _IsReciprocal: false, 0x02, 0);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value ten.
        /// </summary>
        public static readonly Arithmonym Ten = new(isNegative: false, _IsReciprocal: false, 0x03, EncodeOperand((Float128)5));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents the value 100.
        /// </summary>
        public static readonly Arithmonym Hundred = new(isNegative: false, _IsReciprocal: false, 0x05, EncodeOperand((Float128)2));

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents positive infinity (+∞).
        /// </summary>
        public static readonly Arithmonym PositiveInfinity = new(isNegative: false, _IsReciprocal: false, 0x3f, 0);

        /// <summary>
        /// A constant <see cref="Arithmonym"/> that represents negative infinity (-∞).
        /// </summary>
        public static readonly Arithmonym NegativeInfinity = new(isNegative: true, _IsReciprocal: false, 0x3f, 0);

        /// <summary>
        /// Gets the multiplicative reciprocal (1 / value) of this <see cref="Arithmonym"/>.
        /// </summary>
        public readonly Arithmonym Reciprocal
        {
            get
            {
                if (IsNaN(this)) return NaN;
                return new Arithmonym(Squished ^ (((UInt128)1) << (FRACTION_BITS + 9)));
            }
        }

        /// <summary>
        /// Gets the additive negation (-value) of this <see cref="Arithmonym"/>.
        /// </summary>
        public readonly Arithmonym Negated
        {
            get
            {
                return new Arithmonym(Squished ^ (((UInt128)1) << (FRACTION_BITS + 10)));
            }
        }

        /// <summary>
        /// Gets the absolute value (magnitude) of this <see cref="Arithmonym"/>.
        /// </summary>
        public readonly Arithmonym AbsoluteValue
        {
            get
            {
                return _IsNegative ? Negated : this;
            }
        }

        /// <summary>
        /// Returns the base-10 logarithm of this value as an <see cref="Arithmonym"/>.
        /// </summary>
        public readonly Arithmonym _Log10
        {
            get
            {
                if (_IsNegative || IsZero(this) || IsNaN(this)) return NaN;
                if (IsInfinity(this) && IsNegative(this)) return Zero;
                if (_IsReciprocal) return Reciprocal._Log10.Negated;
                if (Letter < 0x05)
                    return new Arithmonym(Float128PreciseTranscendentals.SafeLog10(ToFloat128()));
                if (Letter == 0x05)
                    return new Arithmonym(Operand);
                if (Letter == 0x06)
                {
                    Float128 newOperand = Operand - Float128.One;
                    byte newLetter = 0x06;
                    if (newOperand < (Float128)2)
                    {
                        newOperand = Float128PreciseTranscendentals.SafeExp10(newOperand - Float128.One);
                        newLetter = 0x05;
                    }
                    return new Arithmonym(false, false, newLetter, EncodeOperand(newOperand));
                }
                if (Letter == 0x07)
                {
                    Float128 op = Float128HyperTranscendentals.LetterJToLetterG(Operand);
                    if (op < 3)
                    {
                        Float128 letterFOperand = Float128HyperTranscendentals.LetterF(Float128PreciseTranscendentals.SafeExp10(op - (Float128)2));
                        if (Float128.IsInfinity(letterFOperand))
                        {
                            return this;
                        }
                        letterFOperand--;
                        if (letterFOperand < 10)
                        {
                            return new(false, false, 0x06, EncodeOperand(letterFOperand));
                        }
                        Float128 letterGOperand = Float128PreciseTranscendentals.SafeLog10(Float128HyperTranscendentals.SuperLog10(letterFOperand));
                        letterGOperand += 2;
                        return new(false, false, 0x07, EncodeOperand(Float128HyperTranscendentals.LetterGToLetterJ(letterGOperand)));
                    }
                    return this;
                }
                return this;
            }
        }

        /// <summary>
        /// Returns 10^value as an <see cref="Arithmonym"/>.
        /// </summary>
        public readonly Arithmonym _Exp10
        {
            get
            {
                if (_IsNegative)
                {
                    return Negated._Exp10.Reciprocal;
                }
                if (IsInfinity(this))
                {
                    if (IsPositive(this))
                    {
                        return PositiveInfinity;
                    }
                    return Zero;
                }
                if (IsNaN(this)) return NaN;
                if (_IsReciprocal) return new(Float128PreciseTranscendentals.SafeExp10(ToFloat128()));
                if (Letter < 0x05)
                    return new(Float128PreciseTranscendentals.SafeExp10(ToFloat128()));
                if (Letter == 0x05)
                    return new(false, false, 0x06, EncodeOperand(2 + Float128PreciseTranscendentals.SafeLog10(Operand)));
                if (Letter == 0x06)
                {
                    if (Operand < 9)
                    {
                        return new(false, false, 0x06, EncodeOperand(Operand + 1));
                    }
                    return new(false, false, 0x07, EncodeOperand(Float128HyperTranscendentals.LetterGToLetterJ(2 + Float128PreciseTranscendentals.SafeLog10(Float128HyperTranscendentals.SuperLog10(Operand + 1)))));
                }
                if (Letter == 0x07)
                {
                    Float128 op = Float128HyperTranscendentals.LetterJToLetterG(Operand);
                    if (op < 3)
                    {
                        Float128 letterFOperand = Float128HyperTranscendentals.LetterF(Float128PreciseTranscendentals.SafeExp10(op - (Float128)2));
                        if (Float128.IsInfinity(letterFOperand))
                            return this;
                        letterFOperand++;
                        Float128 letterGOperand = Float128PreciseTranscendentals.SafeLog10(Float128HyperTranscendentals.SuperLog10(letterFOperand));
                        letterGOperand += 2;
                        return new(false, false, 0x07, EncodeOperand(Float128HyperTranscendentals.LetterGToLetterJ(letterGOperand)));
                    }
                    return this;
                }
                return this;
            }
        }

        /// <summary>
        /// Converts this instance to an unsigned 64-bit integer by converting to <see cref="Float128"/> then casting.
        /// </summary>
        public ulong ToUlong() => (ulong)ToFloat128();

        /// <summary>
        /// Converts this instance to a signed 64-bit integer by converting to <see cref="Float128"/> then casting.
        /// </summary>
        public long ToLong() => (long)ToFloat128();

        /// <summary>
        /// Converts this instance to an unsigned 32-bit integer by converting to <see cref="Float128"/> then casting.
        /// </summary>
        public uint ToUint() => (uint)ToFloat128();

        /// <summary>
        /// Converts this instance to a signed 32-bit integer by converting to <see cref="Float128"/> then casting.
        /// </summary>
        public int ToInt() => (int)ToFloat128();

        /// <summary>
        /// Converts this instance to a double-precision floating-point number.
        /// </summary>
        public double ToDouble() => (double)ToFloat128();

        #region Predicates
        /// <summary>
        /// Determines whether the specified <see cref="Arithmonym"/> represents positive or negative infinity.
        /// </summary>
        public static bool IsInfinity(Arithmonym v) => (v.Letter == 0x3f) && (((Float128)2) == v.Operand) && (!v._IsReciprocal);

        /// <summary>
        /// Determines whether the specified <see cref="Arithmonym"/> is Not-a-Number (NaN).
        /// </summary>
        public static bool IsNaN(Arithmonym v) => (v.Letter == 0x3f) && (((Float128)2) != v.Operand);

        /// <summary>
        /// Determines whether the specified <see cref="Arithmonym"/> is a quiet NaN (QNaN).
        /// </summary>
        public static bool IsQNaN(Arithmonym v) => (v.Letter == 0x3f) && (((Float128)2) != v.Operand) && v._IsReciprocal;

        /// <summary>
        /// Determines whether the specified <see cref="Arithmonym"/> represents zero.
        /// </summary>
        public static bool IsZero(Arithmonym v) => (v.Letter == 0x3f) && (((Float128)2) == v.Operand) && v._IsReciprocal;

        /// <summary>
        /// Determines whether the specified <see cref="Arithmonym"/> is negative (and not zero).
        /// </summary>
        public static bool IsNegative(Arithmonym v) => v._IsNegative && !IsZero(v);

        /// <summary>
        /// Determines whether the specified <see cref="Arithmonym"/> is positive (and not zero).
        /// </summary>
        public static bool IsPositive(Arithmonym v) => !v._IsNegative && !IsZero(v);
        #endregion

        /// <summary>
        /// Converts this <see cref="Arithmonym"/> to the underlying <see cref="Float128"/> value.
        /// Special values such as infinities and NaN are preserved.
        /// 
        /// Note that infinity/zero is very much possible due to overflow/underflow.
        /// </summary>
        public Float128 ToFloat128()
        {
            if (IsInfinity(this) && IsPositive(this)) return Float128.PositiveInfinity;
            if (IsInfinity(this) && IsNegative(this)) return Float128.NegativeInfinity;
            if (IsNaN(this)) return Float128.NaN;
            if (IsZero(this)) return Float128.Zero;
            Float128 output;
            Float128 value = UInt128ToFloat128(OperandFraction128) / Float128PreciseTranscendentals.SafeExp2(FRACTION_BITS);
            value += OperandFloored;
            switch (Letter)
            {
                case 0x01:
                    output = _IsReciprocal
                        ? 1 / (1 + ((value - 2) / 8))
                        : 1 + ((value - 2) / 8);
                    break;
                case 0x02:
                    output = _IsReciprocal
                        ? 1 / (2 + ((value - 2) / 4))
                        : 2 + ((value - 2) / 4);
                    break;
                case 0x03:
                    output = _IsReciprocal
                        ? 1 / (value * 2)
                        : value * 2;
                    break;
                case 0x04:
                    output = _IsReciprocal
                        ? 1 / (value * 10)
                        : value * 10;
                    break;
                case 0x05:
                    output = _IsReciprocal
                        ? Float128PreciseTranscendentals.SafeExp10(-value) : Float128PreciseTranscendentals.SafeExp10(value);
                    break;
                case 0x06:
                    output = _IsReciprocal
                        ? 1 / Float128PreciseTranscendentals.SafeExp10(Float128PreciseTranscendentals.SafeExp10(Float128PreciseTranscendentals.SafeExp10(value - 2)))
                        : Float128PreciseTranscendentals.SafeExp10(Float128PreciseTranscendentals.SafeExp10(Float128PreciseTranscendentals.SafeExp10(value - 2)));
                    break;
                default:
                    output = _IsReciprocal ? Float128.Zero : Float128.PositiveInfinity;
                    break;

            }
            if (_IsNegative) output = -output;
            return output;
        }

        /// <summary>
        /// Returns a human-readable string representation of this <see cref="Arithmonym"/>.
        /// </summary>
        public override string ToString()
        {
            if (IsNaN(this)) return "NaN";
            if (this == PositiveInfinity) return "∞";
            if (this == NegativeInfinity) return "-∞";
            if (this == Zero) return "0";

            // Reconstruct operand in [2, 10)
            Float128 value = UInt128ToFloat128(OperandFraction128) / Float128PreciseTranscendentals.SafeExp2(FRACTION_BITS);
            value += OperandFloored;

            string output = "";
            if (_IsNegative)
                output += "-";

            string[] letters = ["", "A", "B", "C", "D", "E", "F", "J", "K", "L", "M", "N", "P"];

            switch (Letter)
            {
                case 0x01:
                    output += _IsReciprocal
                        ? 1 / (1 + ((value - 2) / 8))
                        : 1 + ((value - 2) / 8);
                    break;
                case 0x02:
                    output += _IsReciprocal
                        ? 1 / (2 + ((value - 2) / 4))
                        : 2 + ((value - 2) / 4);
                    break;
                case 0x03:
                    output += _IsReciprocal
                        ? 1 / (value * 2)
                        : value * 2;
                    break;
                case 0x04:
                    output += _IsReciprocal
                        ? 1 / (value * 10)
                        : value * 10;
                    break;
                case 0x05:
                    output += _IsReciprocal
                        ? Float128PreciseTranscendentals.SafeExp10(-value) : Float128PreciseTranscendentals.SafeExp10(value);
                    break;
                case 0x06:
                    output += ArithmonymFormattingUtils.FormatArithmonymFromLetterF(Operand, _IsReciprocal);
                    break;
                default:
                    if (_IsReciprocal)
                        output += "1 / ";
                    output += letters[Letter];
                    output += value;
                    break;
            }
            return output;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Arithmonym"/> from a <see cref="double"/> value.
        /// </summary>
        /// <param name="v">The double value to convert.</param>
        public Arithmonym(double v) : this((Float128)v) { }

        #endregion
        /// <summary>
        /// Initializes a new instance by splitting a packed <see cref="UInt128"/> value
        /// into the internal three 32-bit words. This constructor is used internally to
        /// create an instance from the packed bit-layout representation.
        /// </summary>
        private Arithmonym(UInt128 squished)
        {
            squishedLo = (uint)squished;
            squishedMid = (uint)(squished >> 32);
            squishedHi = (uint)(squished >> 64);
        }

        private static UInt128 EncodeOperand(Float128 value)
        {
            // Encode as Q3.85 on (value - 2)
            value -= 2;
            value = SnapToInt(value);

            Float128 floored = Float128.Floor(value);
            Float128 fraction = value - floored;

            if (fraction < 0) fraction = 0;
            if (fraction >= 1) { fraction = 0; floored += 1; }

            // Scale fraction to 85-bit fixed-point with truncation
            Float128 scaled = fraction * Float128PreciseTranscendentals.SafeExp2(FRACTION_BITS);

            // Carry if scaled == 2^85
            Float128 twoPow = Float128PreciseTranscendentals.SafeExp2(FRACTION_BITS);
            if (scaled >= twoPow)
            {
                scaled = 0;
                floored += 1;
            }
            else
            {
                scaled = Float128.Floor(scaled);
            }

            // Build UInt128 exactly
            ulong hi = (ulong)Float128.Floor(scaled / Float128PreciseTranscendentals.SafeExp2(64));
            ulong lo = (ulong)(scaled - (((Float128)hi) * Float128PreciseTranscendentals.SafeExp2(64)));
            UInt128 fractionBits = ((UInt128)hi << 64) + lo;

            byte flooredByte = (byte)floored;

            // Pack fields
            return fractionBits
                + ((UInt128)flooredByte << FRACTION_BITS);
        }

        private static Float128 UInt128ToFloat128(UInt128 v)
        {
            // Split into hi/lo 64-bit words and accumulate exactly
            ulong lo = (ulong)v;
            ulong hi = (ulong)(v >> 64);
            Float128 result = (Float128)hi * Float128PreciseTranscendentals.SafeExp2(64) + (Float128)lo;
            return result;
        }


        /// <summary>
        /// Initializes a new instance of <see cref="Arithmonym"/> from a <see cref="Float128"/> value.
        /// </summary>
        /// <param name="v">The <see cref="Float128"/> value to convert.</param>
        public Arithmonym(Float128 v)
        {
            if (Float128.IsInfinity(v))
            {
                if (Float128.IsPositive(v))
                {
                    squishedLo = PositiveInfinity.squishedLo;
                    squishedMid = PositiveInfinity.squishedMid;
                    squishedHi = PositiveInfinity.squishedHi;
                }
                if (Float128.IsNegative(v))
                {
                    squishedLo = NegativeInfinity.squishedLo;
                    squishedMid = NegativeInfinity.squishedMid;
                    squishedHi = NegativeInfinity.squishedHi;
                }
            }
            if (Float128.IsNaN(v))
            {
                squishedLo = NaN.squishedLo;
                squishedMid = NaN.squishedMid;
                squishedHi = NaN.squishedHi;
            }
            if (Float128.IsZero(v))
            {
                squishedLo = Zero.squishedLo;
                squishedMid = Zero.squishedMid;
                squishedHi = Zero.squishedHi;
            }

            Float128 value = v;
            byte letter = 0;
            bool isNegative = false;
            bool _IsReciprocal = false;

            if (value < 0)
            {
                value = -value;
                isNegative = true;
            }
            if (value < 1)
            {
                value = 1 / value;
                _IsReciprocal = true;
            }

            // Map into [2,10) and set letter
            if (value < 2)
            {
                value = (value * 8) - 6;
                letter = 1;
            }
            else if (value < 4)
            {
                value = (value * 4) - 6;
                letter = 2;
            }
            else if (value < 20)
            {
                value /= 2;
                letter = 3;
            }
            else if (value < 100)
            {
                value /= 10;
                letter = 4;
            }
            else if (value < (Float128)1e10)
            {
                value = Float128PreciseTranscendentals.SafeLog10(value);
                value = SnapToInt(value);
                letter = 5;
            }
            else
            {
                value = Float128HyperTranscendentals.SuperLog10(value);
                value = SnapToInt(value);
                letter = 6;
            }

            // Encode operand into Q3.85 format and pack fields
            UInt128 operandBits = EncodeOperand(value);
            UInt128 s =
                operandBits
                + ((UInt128)letter << (FRACTION_BITS + 3))
                + ((UInt128)(_IsReciprocal ? 1 : 0) << (FRACTION_BITS + 9))
                + ((UInt128)(isNegative ? 1 : 0) << (FRACTION_BITS + 10));

            squishedLo = (uint)s;
            squishedMid = (uint)(s >> 32);
            squishedHi = (uint)(s >> 64);
        }

        /// <summary>
        /// Gets the normalized form of this <see cref="Arithmonym"/>.
        /// </summary>
        public readonly Arithmonym Normalized
        {
            get
            {
                if (_IsReciprocal && (ToFloat128() == 1)) return One;
                if (_IsReciprocal && (ToFloat128() == -1)) return NegativeOne;
                if (IsNaN(this)) return NaN;
                if (IsInfinity(this)) return _IsNegative ? NegativeInfinity : PositiveInfinity;
                return this;
            }
        }

        private Arithmonym(bool isNegative, bool _IsReciprocal, byte letter, UInt128 operand)
            : this(
                operand
                + ((UInt128)letter << (FRACTION_BITS + 3))
                + ((UInt128)(_IsReciprocal ? 1 : 0) << (FRACTION_BITS + 9))
                + ((UInt128)(isNegative ? 1 : 0) << (FRACTION_BITS + 10)))
        {
        }

        // Snap values that are within tolerance of an integer
        private static Float128 SnapToInt(Float128 x)
        {
            Float128 n = Float128.Round(x);
            return (Float128.Abs(x - n) < Float128PreciseTranscendentals.SafeExp2(-40)) ? n : x;
        }

        private readonly bool _IsNegative
        {
            get
            {
                return ((Squished >> (FRACTION_BITS + 10)) & (UInt128)1) == (UInt128)1;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="Arithmonym"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is an <see cref="Arithmonym"/> equal to this instance; otherwise <see langword="false"/>.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is Arithmonym other) return !IsNaN(this) && !IsNaN(other) && ((IsZero(this) && IsZero(other)) ? Squished == other.Squished : Normalized.Squished == other.Normalized.Squished);
            return false;
        }

        /// <summary>
        /// Returns the hash code for this <see cref="Arithmonym"/>.
        /// Combines the three internal 32-bit words that together represent the packed value.
        /// </summary>
        public override int GetHashCode() => HashCode.Combine(squishedLo, squishedMid, squishedHi);


        /// <summary>
        /// Determines whether the specified <see cref="Arithmonym"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Arithmonym"/> to compare with.</param>
        /// <returns><see langword="true"/> if <paramref name="other"/> equals this instance; otherwise <see langword="false"/>.</returns>
        public bool Equals(Arithmonym other) => !IsNaN(this) && !IsNaN(other) && ((IsZero(this) && IsZero(other)) ? Squished == other.Squished : Normalized.Squished == other.Normalized.Squished);

        /// <summary>
        /// Compares this instance to another <see cref="Arithmonym"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Arithmonym"/> to compare to.</param>
        /// <returns>
        /// -1 if this instance is less than <paramref name="other"/>, 0 if equal, 1 if greater.
        /// Returns <see cref="int.MinValue"/> when either operand is NaN.
        /// </returns>
        public int CompareTo(Arithmonym other)
        {
            if (IsNaN(this) || IsNaN(other)) return int.MinValue;
            if (IsZero(other)) return IsZero(this) ? 0 : _IsNegative ? -1 : 1;
            if (_IsNegative)
            {
                if (other._IsNegative) return other.Negated.CompareTo(Negated);
                return -1;
            }
            if (_IsReciprocal)
            {
                if (other._IsReciprocal) return other.Reciprocal.CompareTo(Reciprocal);
                return -1;
            }

            if (Letter > other.Letter) return 1;
            if (Letter < other.Letter) return -1;
            if (OperandFloored > other.OperandFloored) return 1;
            if (OperandFloored < other.OperandFloored) return -1;
            if (OperandFraction128 > other.OperandFraction128) return 1;
            if (OperandFraction128 < other.OperandFraction128) return -1;

            return 0;
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="object"/>, which should be an <see cref="Arithmonym"/>.
        /// </summary>
        /// <param name="other">The object to compare to.</param>
        /// <returns>Comparison result as with <see cref="CompareTo(Arithmonym)"/>, or <see cref="int.MinValue"/> for invalid types.</returns>
        public int CompareTo(object? other)
            => other is Arithmonym a
                ? CompareTo(a)
                : int.MinValue;

        /// <summary>
        /// Determines whether two <see cref="Arithmonym"/> instances are equal.
        /// </summary>
        public static bool operator ==(Arithmonym left, Arithmonym right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="Arithmonym"/> instances are not equal.
        /// </summary>
        public static bool operator !=(Arithmonym left, Arithmonym right) => !left.Equals(right);

        /// <summary>
        /// Determines whether <paramref name="left"/> is less than <paramref name="right"/>.
        /// </summary>
        public static bool operator <(Arithmonym left, Arithmonym right) => left.CompareTo(right) == -1;

        /// <summary>
        /// Determines whether <paramref name="left"/> is greater than <paramref name="right"/>.
        /// </summary>
        public static bool operator >(Arithmonym left, Arithmonym right) => left.CompareTo(right) == 1;

        /// <summary>
        /// Determines whether <paramref name="left"/> is less than or equal to <paramref name="right"/>.
        /// </summary>
        public static bool operator <=(Arithmonym left, Arithmonym right) => (left < right) || (left == right);

        /// <summary>
        /// Determines whether <paramref name="left"/> is greater than or equal to <paramref name="right"/>.
        /// </summary>
        public static bool operator >=(Arithmonym left, Arithmonym right) => (left > right) || (left == right);

        /// <summary>
        /// Adds two <see cref="Arithmonym"/> values.
        /// </summary>
        /// <remarks>
        /// This operator implements fast-path logic for common cases and falls back to <see cref="Float128"/> arithmetic
        /// for complex or extreme values; the fallback may lose precision for ultra-large or ultra-small values.
        /// </remarks>
        public static Arithmonym operator +(Arithmonym left, Arithmonym right)
        {
            if (IsNaN(left) || IsNaN(right)) return NaN;
            if (IsInfinity(left) && IsInfinity(right))
            {
                if (left._IsNegative == right._IsNegative)
                    return left;
                else return NaN;
            }
            if (IsInfinity(left)) return left;
            if (IsInfinity(right)) return left;

            if (IsZero(left) && IsZero(right))
            {
                // IEEE754 Zero handling
                if (!left._IsNegative || (left._IsNegative == right._IsNegative)) return Zero;
                return Zero.Negated;
            }

            if (IsZero(left)) return right;
            if (IsZero(right)) return left;
            if (left._IsNegative)
            {
                if (left.Negated == right)
                    return Zero;
                else if (right._IsNegative || left.Negated > right)
                    return (left.Negated + right.Negated).Negated;
                else
                    return right + left;
            }
            if (!right._IsNegative && left < right) return right + left;

            /* 
             * Now there are two branches of logic. 
             * Thanks to the previous few if statements,
             * we can guarantee that, if a >= b and a>0 and b>0, 
             * then we have to perform either a + b or a - b.
             * 
             * We also know b is not 0.
             * 
             * Here's the branching...
             */
            if (right < Zero)
            {
                // Subtraction (left + right where right is negative => left - |right|)


                Arithmonym b = right.Negated;
                if (left == b) return Zero;

                if (left._IsReciprocal)
                {
                    if (Float128.Abs(left.ToFloat128()) >= Float128PreciseTranscendentals.SafeExp2(-16382))
                        return new(left.ToFloat128() + right.ToFloat128());
                    Float128 llog = left._Log10.ToFloat128();
                    Float128 rlog = right._Log10.ToFloat128();
                    Float128 olog = llog + Float128PreciseTranscendentals.SafeLog10(
                        1 - Float128PreciseTranscendentals.SafeExp10(rlog - llog)
                    );
                    if (Float128.IsInfinity(olog) || Float128.IsNaN(olog))
                    {
                        return left;
                    }
                    else
                    {
                        return new Arithmonym(olog)._Exp10;
                    }

                }

                // General fallback (may lose extreme precision for ultra-large/small values)
                return new(left.ToFloat128() + right.ToFloat128());
            }
            else
            {
                // Addition

                // Small enough / common cases: if left is reciprocal use Float128 arithmetic
                if (left._IsReciprocal)
                {
                    if (Float128.Abs(left.ToFloat128()) >= Float128PreciseTranscendentals.SafeExp2(-16382))
                        return new(left.ToFloat128() + right.ToFloat128());
                    Float128 llog = left._Log10.ToFloat128();
                    Float128 rlog = right._Log10.ToFloat128();
                    Float128 olog = llog + Float128PreciseTranscendentals.SafeLog10(
                        1 + Float128PreciseTranscendentals.SafeExp10(rlog - llog)
                    );
                    if (Float128.IsInfinity(olog) || Float128.IsNaN(olog))
                    {
                        return left;
                    }
                    else
                    {
                        return new Arithmonym(olog)._Exp10;
                    }
                }
                else
                {
                    if (right._IsReciprocal)
                    {
                        if (Float128.IsInfinity(left.ToFloat128())) return left;
                        return new(left.ToFloat128() + right.ToFloat128());
                    }
                    Float128 sum = left.ToFloat128() + right.ToFloat128();
                    if (sum == Float128.PositiveInfinity)
                    {
                        Float128 llog = left._Log10.ToFloat128();
                        Float128 rlog = right._Log10.ToFloat128();
                        Float128 olog = llog + Float128PreciseTranscendentals.SafeLog10(
                            1 + Float128PreciseTranscendentals.SafeExp10(rlog - llog)
                        );
                        if (olog == Float128.PositiveInfinity)
                        {
                            return left;
                        }
                        return new Arithmonym(olog)._Exp10;
                    }
                    else
                    {
                        return new(sum);
                    }
                }
            }
        }

        /// <summary>
        /// Subtracts <paramref name="right"/> from <paramref name="left"/>.
        /// </summary>
        public static Arithmonym operator -(Arithmonym left, Arithmonym right)
            => left + right.Negated;

        /// <summary>
        /// Multiplies two <see cref="Arithmonym"/> values.
        /// </summary>
        /// <remarks>
        /// This implementation uses logarithmic addition (log10) on the absolute values and
        /// then exponentiates the result. The resulting sign is the XOR of operand signs.
        /// Special values (NaN, zero, infinity) are handled explicitly.
        /// </remarks>
        public static Arithmonym operator *(Arithmonym left, Arithmonym right)
        {
            // Top-level special cases
            if (IsNaN(left) || IsNaN(right)) return NaN;
            if ((IsInfinity(left) && IsZero(right)) || (IsZero(left) && IsInfinity(right)))
                return NaN;
            if (IsZero(left) || IsZero(right)) return Zero;
            if (left == One) return right;
            if (right == One) return left;
            if (left == NegativeOne) return right.Negated;
            if (right == NegativeOne) return left.Negated;

            bool resultNegative = IsNegative(left) ^ IsNegative(right);

            if (IsInfinity(left) || IsInfinity(right))
                return resultNegative ? NegativeInfinity : PositiveInfinity;

            // Multiply via logarithms: (left.AbsoluteValue._Log10 + right.AbsoluteValue._Log10)._Exp10
            Arithmonym sumLog = left.AbsoluteValue._Log10 + right.AbsoluteValue._Log10;
            if (IsNaN(sumLog)) return NaN;

            Arithmonym result = sumLog._Exp10;
            return resultNegative ? result.Negated : result;
        }

        /// <summary>
        /// Divides two <see cref="Arithmonym"/> values.
        /// </summary>
        /// <remarks>
        /// This implementation uses logarithmic subtraction (log10) on the absolute values and
        /// then exponentiates the result. The resulting sign is the XOR of operand signs.
        /// Special values (NaN, zero, infinity) are handled explicitly.
        /// </remarks>
        public static Arithmonym operator /(Arithmonym left, Arithmonym right)
        {
            // Top-level special cases
            if (IsNaN(left) || IsNaN(right)) return NaN;
            if (left == right) return One;
            if (IsZero(left)) return Zero;
            if (IsInfinity(right)) return Zero;

            // Division by zero
            if (IsZero(right)) return right._IsNegative ? NegativeInfinity : PositiveInfinity;

            // Shortcuts
            if (right == One) return left;
            if (right == NegativeOne) return left.Negated;
            if (left == One) return right.Reciprocal;
            if (left == NegativeOne) return right.Reciprocal.Negated;

            bool resultNegative = IsNegative(left) ^ IsNegative(right);

            if (IsInfinity(left))
                return resultNegative ? NegativeInfinity : PositiveInfinity;

            // Divide via logarithms: (left._Log10 - right._Log10)._Exp10
            Arithmonym diffLog = left._Log10 - right._Log10;
            if (IsNaN(diffLog)) return NaN;

            Arithmonym result = diffLog._Exp10;
            return resultNegative ? result.Negated : result;
        }

        public static Arithmonym operator +(Arithmonym value) => value;
        public static Arithmonym operator -(Arithmonym value) => value.Negated;

        /// <summary>
        /// Returns the absolute value (magnitude) of <paramref name="value"/>.
        /// This is a small helper that forwards to the instance-level <see cref="AbsoluteValue"/> property.
        /// </summary>
        /// <param name="value">The value to take the absolute of.</param>
        /// <returns>A non-negative <see cref="Arithmonym"/> with the same magnitude as <paramref name="value"/>.</returns>
        public static Arithmonym Abs(Arithmonym value) => value.AbsoluteValue;

        /// <summary>
        /// Returns the additive negation of <paramref name="value"/>.
        /// This is a convenience wrapper around the unary minus operator.
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated <see cref="Arithmonym"/>.</returns>
        public static Arithmonym Neg(Arithmonym value) => -value;

        /// <summary>
        /// Returns 10 raised to the power <paramref name="value"/>.
        /// This static helper forwards to the instance-level <see cref="_Exp10"/> behavior.
        /// </summary>
        /// <param name="value">The exponent value (base 10).</param>
        /// <returns>An <see cref="Arithmonym"/> representing 10^<paramref name="value"/>.</returns>
        public static Arithmonym Exp10(Arithmonym value) => value._Exp10;

        /// <summary>
        /// Returns the base-10 logarithm of <paramref name="value"/>.
        /// This static helper forwards to the instance-level <see cref="_Exp10"/> behavior.
        /// </summary>
        /// <param name="value">The logarithm (base 10).</param>
        /// <returns>An <see cref="Arithmonym"/> representing Log10(<paramref name="value"/>).</returns>
        public static Arithmonym Log10(Arithmonym value) => value._Log10;

        public static explicit operator double(Arithmonym value)
        {
            return value.ToDouble();
        }

        public static implicit operator Float128(Arithmonym value)
        {
            return value.ToFloat128();
        }

        public static implicit operator Arithmonym(double value)
        {
            return new(value);
        }

        public static explicit operator Arithmonym(Float128 value)
        {
            return new(value);
        }

        #endregion

        #region Implementing INumber
        //TODO
        #endregion
    }
}
