# GoogolSharp Debugging & Development Tools

This directory contains specialized utilities for testing, debugging, and validating the GoogolSharp library's implementations of transcendental functions and advanced operations.

## 📂 Directory Contents

### `ArithmonymDebug/`

**Comprehensive diagnostic tool for validating precision and correctness of transcendental functions.**

#### Building
```bash
cd ArithmonymDebug
dotnet build -c Debug
```

#### Running
```bash
dotnet run -c Debug
```

#### Features

The ArithmonymDebug utility performs six comprehensive test suites:

1. **TEST 1: Exp10 Convergence Analysis**
   - Tests base-10 exponential: 10^x = 2^(x * log₂(10))
   - Validates against known values (10⁰=1, 10¹=10, 10²=100, 10⁻¹=0.1)
   - Measures convergence precision and relative errors

2. **TEST 2: Exp2 Convergence Analysis**
   - Tests binary exponential: 2^x
   - Validates Newton-Raphson iteration accuracy
   - Tests special cases (2⁰=1, 2¹=2, 2^0.5=√2)

3. **TEST 3: Exp Convergence Analysis**
   - Tests natural exponential: e^x
   - Validates against double-precision reference
   - Measures error accumulation in range reduction

4. **TEST 4: Log Consistency Checks**
   - Validates mathematical relationships between logarithm bases
   - Verifies: log₂(x)*ln(2) = ln(x) and log₁₀(x)*ln(10) = ln(x)
   - Detects base conversion errors

5. **TEST 5: Roundtrip Consistency**
   - Tests inverse function relationships
   - Validates: Exp(Log(x)) ≈ x, Exp₁₀(Log₁₀(x)) ≈ x, etc.
   - Measures error accumulation through function composition

6. **TEST 6: Tetration Test**
   - Tests hyperexponentiation: Tetration(2, 4) = 2↑↑4 = 65536
   - Validates power tower computation

#### Output Example

```
╔════════════════════════════════════════════════════════════╗
║         EXHAUSTIVE EXPONENTIAL FUNCTION DEBUGGING          ║
╚════════════════════════════════════════════════════════════╝

▶ TEST 1: Exp10 Convergence Analysis
═══════════════════════════════════════════════════════════

Exp10(1):
  SafeExp10 result:  10.000024900130644
  Expected (double): 10
  Difference:        2.490013064488401E-005
  Relative error:    2.490013064488400E-006
  Log2(10):          3.321928094887362
  y * Log2(10):      3.321928094887362
  Exp2(y*Log2(10)): 10.000008002511988
```

### `ArithmonymDebug/TetrationTest.cs`

**Focused validation for tetration (power tower) operations.**

#### Building
```bash
cd ArithmonymDebug
dotnet build TetrationTest.cs
```

#### Running
```bash
dotnet run TetrationTest.cs
```

#### Purpose

Validates the `Arithmonym.Tetration(base, height)` method for fundamental tower operations.

**Test Case**: Tetration(2, 4)

Mathematical representation:
- **2↑↑4** = 2^(2^(2^2)) = 2^(2^4) = 2^16 = 65536

#### Expected Output

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

## 🔍 Interpreting Results

### Error Metrics

| Relative Error | Interpretation | Status |
|---|---|---|
| < 1e-15 | Excellent (< 1 PPQ) | ✓ Ideal |
| < 1e-10 | Good (< 1 PPB) | ✓ Acceptable |
| < 1e-6 | Fair (< 1 PPM) | ⚠ Investigate |
| > 1e-4 | Poor | ✗ Problem |

**PPQ** = Parts Per Quadrillion, **PPB** = Parts Per Billion, **PPM** = Parts Per Million

### Precision Limits

- **IEEE 754 binary128 machine epsilon**: ε ≈ 2⁻¹¹³ ≈ 9.63×10⁻³⁵
- **Typical achievable precision**: 25-30 significant digits (out of 34 theoretical maximum)
- **Logarithm methods**: 60 atanh series iterations
- **Exponential methods**: 30 Newton-Raphson iterations

---

## 🐛 Debugging Guide

### When Tests Fail

1. **High Relative Errors (> 1e-10)**
   - Check mathematical constants have 50+ digit precision
   - Verify iteration count is sufficient (30-60)
   - Test with different input ranges
   - Check epsilon value (should be ~2⁻¹¹³)

2. **Convergence Doesn't Occur**
   - Monitor intermediate values for overflow/underflow
   - Verify correction calculation denominators aren't zero
   - Check loop termination conditions
   - Confirm iteration start values

3. **Tetration Failures**
   - Super-exponential growth occurs quickly
   - Tetration(2, 5) exceeds Float128 range
   - Values beyond height=4 use Arithmonym's extended representation
   - Verify base value is in valid tetration range

### Common Issues & Solutions

| Issue | Cause | Solution |
|-------|-------|----------|
| Roundtrip error > 1e-6 | Function composition error | Check each function individually |
| Log consistency fails | Base conversion incorrect | Verify Ln10 and Ln2 precision |
| Exp doesn't converge | Large exponent | Split integer and fractional parts |
| Tetration returns NaN | Input domain error | Validate base/height constraints |

---

## 📊 Performance Notes

- **ArithmonymDebug**: 1-2 seconds typical completion
- **TetrationTest**: < 100ms typical completion
- **Memory usage**: Minimal (<10 MB)

---

## 📖 Documentation References

- [Transcendental Functions](../docs/transcendental-functions.md) - Algorithm details and precision analysis
- [Debugging Tools & Testing](../docs/debugging-tools.md) - Comprehensive testing documentation
- [Introduction](../docs/introduction.md) - Library overview

---

## 🔗 Related Files

- Main implementation: [`src/GoogolSharp/Helpers/Float128PreciseTranscendentals.cs`](../src/GoogolSharp/Helpers/Float128PreciseTranscendentals.cs)
- Tests: [`tests/GoogolSharp.Tests/`](../tests/GoogolSharp.Tests/)
- Arithmonym core: [`src/GoogolSharp/Arithmonym.cs`](../src/GoogolSharp/Arithmonym.cs)

