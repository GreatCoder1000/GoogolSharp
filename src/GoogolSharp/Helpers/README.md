# GoogolSharp Helper Classes

This directory contains helper classes and utilities that support the core GoogolSharp functionality. These are primarily focused on high-precision mathematical computations.

## 📂 Contents

### `Float128PreciseTranscendentals.cs`

**Core class providing ultra-high-precision transcendental mathematical functions using 128-bit IEEE 754 Float128.**

#### Purpose

Extends GoogolSharp's mathematical capabilities with functions achieving **25+ significant digits of precision**, far exceeding standard `double` accuracy (15-17 digits).

#### Public API

##### Logarithmic Functions

All logarithmic functions:
- Throw `ArgumentOutOfRangeException` for non-positive inputs
- Achieve 25+ significant digits of precision
- Use atanh (inverse hyperbolic tangent) based range reduction

```csharp
// Natural logarithm: ln(x)
public static Float128 SafeLog(Float128 x);

// Binary logarithm: log₂(x)  
public static Float128 SafeLog2(Float128 x);

// Decimal logarithm: log₁₀(x)
public static Float128 SafeLog10(Float128 x);
```

##### Exponential Functions

All exponential functions:
- Achieve 25+ significant digits of precision
- Handle overflow/underflow gracefully
- Use Newton-Raphson iteration with binary scaling

```csharp
// Natural exponential: e^y
public static Float128 SafeExp(Float128 y);

// Binary exponential: 2^y
public static Float128 SafeExp2(Float128 y);

// Decimal exponential: 10^y (via 2^(y*log₂(10)))
public static Float128 SafeExp10(Float128 y);
```

##### Power Function

```csharp
// General power: x^y = e^(y*ln(x))
// Throws ArgumentOutOfRangeException if x ≤ 0
public static Float128 SafePow(Float128 x, Float128 y);
```

#### Mathematical Constants

Ultra-high-precision constants (50+ significant digits):

```csharp
// Logarithm constants
public static readonly Float128 Ln2;              // ln(2) = 0.693147...
public static readonly Float128 Ln10;             // ln(10) = 2.302585...
public static readonly Float128 Log2_E;           // log₂(e) = 1.442695...
public static readonly Float128 Log2_10;          // log₂(10) = 3.321928...

// Transcendental constants
public static readonly Float128 E;                // e = 2.718281...
public static readonly Float128 Pi;               // π = 3.141592...

// Algebraic constants
public static readonly Float128 Sqrt2;            // √2 = 1.414213...
public static readonly Float128 SqrtSqrt2;        // ⁴√2 = 1.189207...
public static readonly Float128 LnSqrt2;          // ln(√2) = ln(2)/2

// System constant
public static readonly Float128 Epsilon;          // Machine epsilon ~ 2⁻¹¹³
```

---

## 🔬 Algorithm Details

### Logarithm Implementation: Atanh Method

**Formula**: ln(x) = 2 * atanh((x-1)/(x+1))

**Process**:
1. **Range Reduction**: Scale x to mantissa m ∈ [1, √2) via binary scaling
   - Track exponent k: x = 2^k * m
2. **Atanh Series**: Compute with convergence criterion
   - atanh(t) = t + t³/3 + t⁵/5 + t⁷/7 + ...
   - 60 iterations achieve full 128-bit precision
3. **Scale Result**: ln(x) = 2*atanh(...) + k*ln(2)

**Why atanh?**
- Rapid convergence for |t| < 0.5 after range reduction
- Numerically stable (no catastrophic cancellation)
- Iteration count independent of input magnitude

### Exponential Implementation: Newton-Raphson

**Problem**: Solve ln(x) = y for x, i.e., find x = e^y

**Iteration**: 
```
x_{n+1} = x_n - f(x_n)/f'(x_n)
where f(x) = ln(x) - y
and f'(x) = 1/x
```

For **Exp₂**:
- Split: y = n + f where n = ⌊y⌋, f ∈ [0,1)
- Solve for 2^f using Newton-Raphson
- Scale: result = 2^f * 2^n (via binary shifting)

For **Exp**:
- Similar approach with reference point at E constant
- Multiply by E^n iteratively for integer scaling

**Why Newton-Raphson?**
- Quadratic convergence (error squares each iteration)
- 30 iterations sufficient for machine precision
- Works well with precomputed ln/log₂ implementations

### Conversion Strategies

**Base Conversions**:
- log₂(x) = ln(x) * log₂(e)
- log₁₀(x) = ln(x) / ln(10)
- 10^x = 2^(x * log₂(10)) [for better precision]

**Why 10^x via 2^(x*log₂(10))?**
- Leverages optimized Exp₂ implementation
- Avoids separate E^(x*ln(10)) computation
- Reduces function composition error

---

## 🎯 Precision Analysis

### Convergence Criteria

Iterations continue while:
```
|correction| < ε * |current_value|
where ε ≈ 2⁻¹¹³ (machine epsilon)
```

This ensures utilization of full 128-bit precision.

### Typical Accuracy Achieved

| Operation | Relative Error | Decimal Places |
|-----------|---|---|
| Individual Log/Exp | < 1e-25 | 25+ |
| Roundtrip Exp(Log(x)) | < 1e-15 | 15-17 |
| Roundtrip Exp10(Log10(x)) | < 1e-10 | 10+ |
| Composition (3+ functions) | < 1e-10 | 10+ |

Error growth is sub-linear due to:
- Compensation in Newton-Raphson iteration
- Precise reference constants (50+ digits)
- Range reduction minimizing growth

---

## ⚠️ Limitations & Edge Cases

### Domain Restrictions

```csharp
SafeLog(x)     // x > 0 only
SafeLog2(x)    // x > 0 only
SafeLog10(x)   // x > 0 only
SafePow(x, y)  // x > 0 only
```

Special values:
- Log(1) = 0 (exact)
- Exp(0) = 1 (exact)
- Pow(1, y) = 1 (exact)
- Pow(x, 0) = 1 (exact)

### Range Limitations

```csharp
SafeExp(y)     // y ∈ (-11356, 11356) roughly
SafeExp2(y)    // y ∈ (-16384, 16384) roughly
SafeExp10(y)   // y ∈ (-4932, 4932) roughly
```

Outside these ranges:
- Returns PositiveInfinity on overflow
- Returns Zero on underflow

### Performance Characteristics

| Function | Time Complexity | Notes |
|----------|---|---|
| Log/Log2/Log10 | O(1) | 60 iterations constant |
| Exp2/Exp10 | O(1) | 30 iterations constant |
| Exp | O(n) | n = ⌊exponent⌋ for E^n scaling |
| Pow | O(1) | Combines O(1) Log and Exp |

---

## 🔗 Integration with GoogolSharp

### Where Used

These functions support:
- **Arithmonym operations**: Logarithmic decomposition for power operations
- **Tetration calculations**: Exponential towers depend on Exp/Log
- **Mathematical constants**: Extended precision for calibration
- **Test validation**: Precision verification in unit tests

### Example Usage

```csharp
using GoogolSharp.Helpers;
using QuadrupleLib;

// Create large precision values
Float128 x = (Float128)1234.5678;

// Compute logarithms
Float128 ln_x = Float128PreciseTranscendentals.SafeLog(x);
Float128 log2_x = Float128PreciseTranscendentals.SafeLog2(x);
Float128 log10_x = Float128PreciseTranscendentals.SafeLog10(x);

// Verify relationships
Float128 check = log2_x * Float128PreciseTranscendentals.Ln2;
// check ≈ ln_x (within 1e-25 relative error)

// Compute exponentials
Float128 exp_result = Float128PreciseTranscendentals.SafeExp(ln_x);
// exp_result ≈ x (original value recovered)

// Power operations
Float128 power = Float128PreciseTranscendentals.SafePow((Float128)2, (Float128)10);
// power ≈ 1024
```

---

## 📚 References

- [IEEE 754 Floating Point Standard](https://en.wikipedia.org/wiki/IEEE_754)
- [QuadrupleLib Documentation](https://github.com/IsaMorphic/QuadrupleLib)
- [Transcendental Functions Guide](../docs/transcendental-functions.md)
- [High-Precision Arithmetic Methods](https://en.wikipedia.org/wiki/Arbitrary-precision_arithmetic)

