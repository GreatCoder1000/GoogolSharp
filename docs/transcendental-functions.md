# High-Precision Transcendental Functions

GoogolSharp provides advanced implementations of logarithmic and exponential functions using **128-bit IEEE 754 binary floating-point** through the `Float128` type from **QuadrupleLib**. These functions achieve **25+ significant digits of precision**, far exceeding standard `double` accuracy.

---

## 📚 Overview

The `Float128PreciseTranscendentals` helper class provides implementations of fundamental transcendental functions with extended precision:

| Function | Formula | Notes |
|----------|---------|-------|
| **Log₂(x)** | Logarithm base 2 | Uses atanh-based range reduction |
| **Log₁₀(x)** | Logarithm base 10 | Uses natural log with conversion |
| **Log(x)** | Natural logarithm | Core atanh-based implementation |
| **Exp₂(x)** | 2 to the power x | Newton-Raphson with binary scaling |
| **Exp(x)** | e to the power x | Uses range reduction and iteration |
| **Exp₁₀(x)** | 10 to the power x | Converts via Exp₂ for precision |
| **Pow(x, y)** | x to the power y | Logarithmic decomposition: e^(y·Log(x)) |

---

## 🔍 Core Algorithm: Atanh-Based Logarithm

The foundation of high-precision logarithmic computation uses the **atanh (inverse hyperbolic tangent)** method:

$$\ln(x) = 2 \cdot \text{atanh}\left(\frac{x-1}{x+1}\right)$$

This approach is ideal because:

1. **Range Reduction**: Input is scaled to $[1, \sqrt{2})$ where atanh converges rapidly
2. **Series Convergence**: The series $\text{atanh}(t) = t + \frac{t^3}{3} + \frac{t^5}{5} + \cdots$ converges quickly for small $|t|$
3. **High Precision**: 60 iterations achieve full 128-bit precision

**Implementation Steps**:
- Reduce $x$ to $m \in [1, \sqrt{2})$ using binary scaling
- Compute atanh series for high precision
- Scale result back: $\ln(x) = 2 \cdot \text{atanh}(\ldots) + k \cdot \ln(2)$

---

## 🔄 Exponential Functions: Newton-Raphson Method

Exponential functions are computed using **Newton-Raphson iteration**, which solves $f(x) = 0$ where $f(x) = \log_b(x) - y$.

### Exp₂(x) - Binary Exponential

For $\text{Exp}_2(x) = 2^x$:

1. **Split**: Separate into integer and fractional parts: $x = n + f$ where $n = \lfloor x \rfloor$ and $f \in [0, 1)$
2. **Scale Integer**: Use binary scaling: $2^n = \text{ScaleB}(1, n)$
3. **Fractional Part**: Solve $f(x) = \log_2(x) - f = 0$ using Newton-Raphson:
   - Initial guess: $x_0 = 1 + f \cdot \ln(2)$
   - Iteration: $x_{n+1} = x_n - \frac{\log_2(x_n) - f}{1/(x_n \cdot \ln(2))}$
4. **Combine**: Result = Exp₂(f) × 2^n

### Exp(x) - Natural Exponential

For $\text{Exp}(x) = e^x$:

1. **Split**: $x = n + f$ where $n = \lfloor x \rfloor$
2. **Fractional Exp**: Solve $f(x) = \ln(x) - f = 0$ using Newton-Raphson:
   - Initial guess: $x_0 = 1 + f$
   - Iteration: $x_{n+1} = x_n + x_n(f - \ln(x_n))$
3. **Scale by Powers**: Multiply by $e^n$ (using $E$ constant multiplied $|n|$ times)

### Exp₁₀(x) - Base-10 Exponential

For efficiency, uses conversion:

$$10^x = 2^{x \cdot \log_2(10)}$$

This leverages the optimized Exp₂ implementation, achieving better precision than direct computation.

---

## 🛡️ Input Validation & Error Handling

All logarithmic functions include **strict input validation**:

```csharp
// Throws ArgumentOutOfRangeException for invalid inputs
SafeLog(x)     // x ≤ 0 → exception
SafeLog2(x)    // x ≤ 0 → exception  
SafeLog10(x)   // x ≤ 0 → exception
SafePow(x, y)  // x ≤ 0 → exception
```

Exponential functions include **overflow/underflow handling**:

```csharp
SafeExp(y)     // y > 11356 → PositiveInfinity, y < -11356 → Zero
SafeExp2(y)    // y > 16384 → PositiveInfinity, y < -16384 → Zero
SafeExp10(y)   // y > 4932 → PositiveInfinity, y < -4932 → Zero
```

---

## 📐 Mathematical Constants

GoogolSharp provides ultra-high-precision mathematical constants (50+ significant digits):

| Constant | Value | Precision |
|----------|-------|-----------|
| **Ln(2)** | 0.693147180559945... | 50 digits |
| **Ln(10)** | 2.302585092994046... | 50 digits |
| **Log₂(e)** | 1.442695040888963... | 50 digits |
| **Log₂(10)** | 3.321928094887362... | 50 digits |
| **e** | 2.718281828459045... | 50 digits |
| **π** | 3.141592653589793... | 50 digits |
| **√2** | 1.414213562373095... | 50 digits |
| **⁴√2** | 1.189207115002721... | 50 digits |

---

## ✅ Precision Verification

To verify precision, roundtrip tests validate the relationship $\text{Exp}(\log(x)) \approx x$:

```csharp
var x = 10.0d;
var y = Float128PreciseTranscendentals.SafeLog((Float128)x);
var recovered = Float128PreciseTranscendentals.SafeExp(y);
// Typically matches to 15-17 significant digits
```

**Typical Error Rates**:
- Roundtrip errors: $< 10^{-15}$ (15 decimal places)
- Individual function errors: $< 10^{-25}$ in isolation
- Composition errors accumulate through series

---

## 🚀 Performance Characteristics

| Function | Iterations | Time Complexity | Notes |
|----------|-----------|-----------------|-------|
| **SafeLog** | 60 (atanh series) | O(1) | Constant iterations for full precision |
| **SafeLog2** | 60 (atanh series) | O(1) | Uses SafeLog with constant conversion |
| **SafeLog10** | 60 (atanh series) | O(1) | Uses SafeLog with constant conversion |
| **SafeExp2** | 30 (Newton-Raphson) | O(1) | Scales with integer part |
| **SafeExp** | 30 + n iterations | O(n) where n = ⌊exponent⌋ | Dominated by E^n multiplication |
| **SafeExp10** | 30 (Newton-Raphson) | O(1) | Converts to Exp2 efficiently |
| **SafePow** | 60 + 30 iterations | O(1) | Combines Log and Exp paths |

---

## 🎯 Use Cases

1. **Scientific Computation**: Extended precision for research calculations
2. **Financial Modeling**: Precise compound interest and growth calculations
3. **Cryptography**: Logarithm-based security parameters with full precision
4. **Astronomical Calculations**: Large number representations (googological scales)
5. **Optimization Algorithms**: Gradient descent with extended precision numerical stability

---

## ⚠️ Known Limitations

1. **Precision Ceiling**: Limited to ~34 significant digits (128-bit double+exponent overhead)
2. **QuadrupleLib Dependencies**: Precision depends on QuadrupleLib's underlying implementations
3. **Exp1Iteration Complexity**: Natural exponential has O(n) complexity for large exponents
4. **Rounding Errors**: Series methods accumulate rounding errors; 25+ digit accuracy is conservative estimate

---

## 📖 Related Documentation

- [Getting Started](getting-started.md) - Introduction to GoogolSharp
- [Introduction](introduction.md) - Project overview
- [Arithmonym Casts](arithmonym-casts.md) - Type conversions

