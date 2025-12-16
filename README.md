# 📐 GoogolSharp

GoogolSharp is a C# library for working with **extremely** large numbers — inspired by googology and advanced numeric representations. It introduces a numeric type called `Arithmonym`, designed to efficiently encode and manipulate values **far beyond conventional floating‑point ranges**.

## ✨ Features

* Custom struct `Arithmonym` for representing *very* large or *very* small numbers.

* Compact 96‑bit word layout for efficient storage.

* Support for:

  * Negative numbers

  * Reciprocal values (numbers below 1)

  * Fractional precision

  * Googological giants (numbers with symbolic “letters”)

* Extensible design for mathematical operations and future numeric extensions.

## 🔢 Bit Layout (96 bits total)

|Bits|Description|
|----|-----------|
|1   |`_IsNegative` - Sign Bit|
|1   |`_IsReciprocal` - reciprocal flag (for numbers < 1)|
|6|`Letter` - symbolic representation for scale (tiny numbers to googological)|
|3|`OperandFloored` - Since `Operand` is always less than 10, and >=2, `Floor(Operand) - 2` encodes cleanly in 3 bits. |
|85|`OperandFraction128` - The 128 is because the property returns a `UInt128`, and this is the fractional part of the `Operand`.|

All this cleanly fits into 96 bits. Since this is not a power of two it is represented in three `uint`s.

## ⚖️ Dependencies

* `.NET 7` or later
* `C# 11` or later
* `QuadrupleLib.Float128` (Download the library with `git clone https://github.com/IsaMorphic/QuadrupleLib.git`)

## 📄 License

GoogolSharp is licensed under Apache License (2.0), but it uses **QuadrupleLib**, which is licensed under the LGPL-3.0 license. You can find the full license and source code for QuadrupleLib here:

- [QuadrupleLib GitHub Repository](https://link-to-quadruplelib-repo)
- License: [LGPL-3.0](https://www.gnu.org/licenses/lgpl-3.0.html)
