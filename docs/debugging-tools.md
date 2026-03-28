# Debugging Tools & Testing

GoogolSharp includes specialized debugging and testing tools located in the `tools/` directory to validate precision and convergence of transcendental functions and other operations.

---

## 📊 ArithmonymDebug Tool

**Location**: `tools/ArithmonymDebug/`

A comprehensive diagnostic utility for validating high-precision transcendental function implementations and Arithmonym operations.

### Purpose

The ArithmonymDebug tool systematically validates:
1. **Exponential function convergence** - Exp, Exp2, Exp10
2. **Logarithmic function accuracy** - Log, Log2, Log10 (including input validation)
3. **Function consistency** - Mathematical relationships (e.g., Log2(x) * Ln(2) ≈ Ln(x))
4. **Roundtrip operations** - Exp(Log(x)) ≈ x, Exp10(Log10(x)) ≈ x, etc.
5. **Tetration operations** - Hyperoperations and extreme value testing

### Running the Tool

```bash
cd tools/ArithmonymDebug
dotnet run -c Debug
```

### Test Modules

#### TEST 1: Exp10 Convergence Analysis
Tests base-10 exponential against known values:
- **Exp10(0)** → 1 (expected)
- **Exp10(1)** → 10 (expected)
- **Exp10(2)** → 100 (expected)
- **Exp10(-1)** → 0.1 (expected)

**Validates**: Precision of 10^y via 2^(y*log₂(10)) conversion.

#### TEST 2: Exp2 Convergence Analysis
Tests binary exponential:
- **Exp2(0)** → 1 (expected)
- **Exp2(1)** → 2 (expected)
- **Exp2(0.5)** → √2 ≈ 1.414... (expected)
- **Exp2(log₂(10))** → 10 (expected)

**Validates**: Newton-Raphson iteration precision for fractional exponents.

#### TEST 3: Exp Convergence Analysis
Tests natural exponential:
- **Exp(0)** → 1 (expected)
- **Exp(1)** → e ≈ 2.71828... (expected)
- **Exp(2)** → e² ≈ 7.389... (expected)
- **Exp(ln(10))** → 10 (expected)

**Validates**: Range reduction accuracy for natural exponential.

#### TEST 4: Log Consistency Checks
Verifies logarithmic relationships hold mathematically:
- **Log10(x) * Ln(10)** should equal **Log(x)**
- **Log2(x) * Ln(2)** should equal **Log(x)**

**Tests**: Consistency of base conversions (error should be near epsilon).

#### TEST 5: Roundtrip Consistency
Validates inverse function relationships:
- **Exp(Log(x))** should equal **x** (typically error < 10⁻⁶)
- **Exp10(Log10(x))** should equal **x** (typically error < 10⁻⁴)
- **Exp2(Log2(x))** should equal **x** (typically error < 10⁻⁶)

**Tests**: Composed function accuracy and error accumulation.

#### TEST 6: Tetration Test
Tests hyperoperation (power tower):
- **Tetration(2, 4)** → 2↑↑4 = 2^(2^(2^2)) = 65536 (expected)

**Validates**: Advanced construction operations on Arithmonym.

### Output Interpretation

Each test produces detailed metrics:

```
SafeExp10 result:  10.000024900130644
Expected (double): 10
Difference:        2.490013064488401E-005
Relative error:    2.490013064488400E-006  ← Error in parts per million
```

**Error evaluation**:
- **Relative error < 1e-15**: Excellent (< 1 part per quadrillion)
- **Relative error < 1e-10**: Good (< 1 part per 10 billion)
- **Relative error < 1e-6**: Acceptable (< 1 part per million)
- **Relative error > 1e-4**: Consider investigation

---

## 🔧 TetrationTest

**Location**: `tools/ArithmonymDebug/TetrationTest.cs`

A focused test for tetration (hyperexponentiation) operations.

### Purpose

Validates the `Arithmonym.Tetration(base, height)` method for fundamental cases.

### Mathematical Background

Tetration is repeated exponentiation (right-associative):
- **2↑↑1** = 2 (single tower)
- **2↑↑2** = 2² = 4
- **2↑↑3** = 2^(2²) = 2^4 = 16
- **2↑↑4** = 2^(2^4) = 2^16 = 65536
- **2↑↑5** = 2^(2^16) = 2^65536 ≈ 10^19728 (astronomically large)

### Running the Test

```bash
cd tools/ArithmonymDebug
dotnet run --project TetrationTest.cs
```

Or compile and run:
```bash
dotnet build TetrationTest.cs
dotnet run TetrationTest.cs
```

### Expected Output

```
╔════════════════════════════════════════════════════════════╗
║   Arithmonym Tetration Debugging: 2↑↑4 = 65536             ║
╚════════════════════════════════════════════════════════════╝

Test: Tetration(2, 4)
Mathematical operation: 2↑↑4 = 2^(2^(2^2)) = 2^16 = 65536

Base (2):   2
Height (4): 4

Computing result...

Result:    65536
Expected:  65536

Verification:
  Equals 65536:     True
  As Int64:         65536
  Decimal match:    True

✓ TEST PASSED: Tetration(2, 4) = 65536
```

---

## 📈 Precision Metrics

### Machine Epsilon

IEEE 754 binary128 machine epsilon: **ε ≈ 2⁻¹¹³ ≈ 9.63×10⁻³⁵**

This represents the smallest relative difference distinguishable between values.

### Convergence Criteria

Tests use convergence criterion: **|correction| < ε * |value|**

This ensures iteration continues until machine precision is exhausted.

### Typical Accuracy Achieved

| Function | Iteration Count | Precision |
|----------|-----------------|-----------|
| Log/Log2/Log10 | 60 (atanh series) | 25-30 significant digits |
| Exp2 | 30 (Newton-Raphson) | 25-30 significant digits |
| Exp | 30 + n (E^n multiplications) | 25-30 significant digits |
| Exp10 | 30 (via Exp2 conversion) | 25-30 significant digits |
| Pow | 60 + 30 | 25-30 significant digits |

---

## 🐛 Debugging Tips

### High Error Rates

If you observe errors > 1e-10:

1. **Check constants**: Verify mathematical constants have 50+ digits of precision
2. **Review iteration counts**: May need to increase convergence iterations
3. **Validate input ranges**: Some functions perform better in specific domains
4. **Test roundtrips**: Compose functions to isolate error sources

### Convergence Issues

If iteration doesn't converge:

1. **Verify epsilon value**: Should be ~2⁻¹¹³ for Float128
2. **Check loop indices**: Iteration counters should be sufficient (30-60 typical)
3. **Inspect correction calculation**: Ensure denominator isn't near zero
4. **Monitor overflow/underflow**: Intermediate values shouldn't exceed range

### Tetration Edge Cases

Test additional tower heights carefully:

```csharp
// These compute correctly
Tetration(2, 0) → 1
Tetration(2, 1) → 2
Tetration(2, 2) → 4
Tetration(2, 3) → 16
Tetration(2, 4) → 65536

// Higher values exceed float128 range
Tetration(2, 5) → Overflows to Arithmonym's extended representation
```

---

## 🔗 Related Topics

- [Transcendental Functions](transcendental-functions.md) - Algorithm documentation
- [Introduction](introduction.md) - Project overview
- [Getting Started](getting-started.md) - Setup instructions

