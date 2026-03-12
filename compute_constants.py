#!/usr/bin/env python3
"""
Compute high-precision mathematical constants for Float128 (128-bit float, ~34 decimal digits)
Using mpmath library for arbitrary precision
"""

try:
    from mpmath import mp, log, pi, e, sqrt
except ImportError:
    print("Error: mpmath not found. Install with: pip install mpfr")
    exit(1)

# Set precision to 150 bits (roughly 45 decimal digits)
mp.dps = 50  # decimal places

# Compute constants
ln_2 = log(2)
ln_10 = log(10)
log2_e = 1 / ln_2
log2_10 = log(10) / log(2)
sqrt_2 = sqrt(2)
ln_sqrt_2 = ln_2 / 2
pi_const = pi
e_const = e

print("=" * 80)
print("HIGH-PRECISION MATHEMATICAL CONSTANTS for Float128")
print("=" * 80)
print()

print("// Ln(2) = 0.693147180559945309417232121458176...")
print(f"// {ln_2}")
print()

print("// Ln(10) = 2.30258509299404568401799145468436...")
print(f"// {ln_10}")
print()

print("// Log2(e) = 1.44269504088896340735992468100189...")
print(f"// {log2_e}")
print()

print("// Log2(10) = 3.32192809488736234787031942948939...")
print(f"// {log2_10}")
print()

print("// Sqrt(2) = 1.41421356237309504880168872420969...")
print(f"// {sqrt_2}")
print()

print("// Pi = 3.14159265358979323846264338327950...")
print(f"// {pi_const}")
print()

print("// e = 2.71828182845904523536028747135266...")
print(f"// {e_const}")
print()

print("// Ln(Sqrt(2)) = Ln(2)/2")
print(f"// {ln_sqrt_2}")
print()

# Now generate C# code
print("\n" + "=" * 80)
print("C# CODE TEMPLATE")
print("=" * 80)
print()

def to_float128_parts(value, num_parts=10):
    """Convert a high-precision decimal to Float128 parts"""
    # For now, return the main value as a string that can go into a Float128 literal
    str_val = str(value)
    print(f"Main value as string: {str_val}")
    return str_val

print("// Using single-part high-precision definition (recommended):")
print("// This avoids issues with cascading precision loss\n")

print(f"public static readonly Float128 Ln2 = (Float128)\"{ln_2}\";")
print(f"public static readonly Float128 Ln10 = (Float128)\"{ln_10}\";")
print(f"public static readonly Float128 Log2_E = (Float128)\"{log2_e}\";")
print(f"public static readonly Float128 Log2_10 = (Float128)\"{log2_10}\";")
print(f"public static readonly Float128 Sqrt2 = (Float128)\"{sqrt_2}\";")
print(f"public static readonly Float128 Pi = (Float128)\"{pi_const}\";")
print(f"public static readonly Float128 E = (Float128)\"{e_const}\";")
