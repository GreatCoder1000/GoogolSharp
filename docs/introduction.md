# Introduction to GoogolSharp

GoogolSharp is a C# library designed for working with **extremely large and small numbers**, inspired by the field of *googology* and advanced numeric representations.  

At its core, GoogolSharp introduces a custom numeric type called **`Arithmonym`**, which encodes values far beyond conventional floating‑point ranges. This makes it possible to represent numbers that are:

- Larger than `double.MaxValue`
- Smaller than `double.MinValue`
- Symbolically extended into googological scales (e.g., numbers represented with “letters”)

Unlike traditional floating‑point types, `Arithmonym` uses a **compact 96‑bit word layout** to efficiently store sign, reciprocal flags, symbolic scales, and fractional precision.  

GoogolSharp is built for researchers, hobbyists, and developers who want to explore **numeric frontiers** without being constrained by standard floating‑point limitations.

---

## ✨ Key Features

- **Custom struct `Arithmonym`** for representing very large or very small numbers.
- **96‑bit word layout** for efficient storage and manipulation.
- Support for:
  - Negative numbers
  - Reciprocal values (numbers below 1)
  - Fractional precision
  - Googological giants (numbers with symbolic “letters”)
- Extensible design for future mathematical operations.

---

## ⚖️ Dependencies

- `.NET 8` or later
- `C# 12` or later
- [`QuadrupleLib.Float128`](https://github.com/IsaMorphic/QuadrupleLib) for high‑precision floating‑point support.

---

## 📄 License

GoogolSharp is licensed under **LGPL‑3.0**.  
It also depends on **QuadrupleLib**, which is licensed under the same terms.  

- [LGPL‑3.0 License](https://www.gnu.org/licenses/lgpl-3.0.html)