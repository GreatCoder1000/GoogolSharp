using GoogolSharp.Helpers;
using QuadrupleLib;
using Float128 = QuadrupleLib.Float128<QuadrupleLib.Accelerators.DefaultAccelerator>;

class PrecisionTest
{
    static void Main()
    {
        Console.WriteLine("=== Float128PreciseTranscendentals Precision Tests ===\n");

        // Test 1: Natural logarithm
        Console.WriteLine("Test 1: Natural Logarithm (SafeLog)");
        var ln10 = Float128PreciseTranscendentals.SafeLog((Float128)10);
        Console.WriteLine($"  ln(10) = {ln10}");
        Console.WriteLine($"  Expected ≈ 2.30258509299404568401799145468436");

        // Test 2: Log base 2
        Console.WriteLine("\nTest 2: Logarithm Base 2 (SafeLog2)");
        var log2_8 = Float128PreciseTranscendentals.SafeLog2((Float128)8);
        Console.WriteLine($"  log₂(8) = {log2_8}");
        Console.WriteLine($"  Expected = 3.0 (exact)");

        var log2_10 = Float128PreciseTranscendentals.SafeLog2((Float128)10);
        Console.WriteLine($"  log₂(10) = {log2_10}");
        Console.WriteLine($"  Expected ≈ 3.32192809488736234787031942948939");

        // Test 3: Log base 10
        Console.WriteLine("\nTest 3: Logarithm Base 10 (SafeLog10)");
        var log10_100 = Float128PreciseTranscendentals.SafeLog10((Float128)100);
        Console.WriteLine($"  log₁₀(100) = {log10_100}");
        Console.WriteLine($"  Expected = 2.0 (exact)");

        var log10_10 = Float128PreciseTranscendentals.SafeLog10((Float128)10);
        Console.WriteLine($"  log₁₀(10) = {log10_10}");
        Console.WriteLine($"  Expected = 1.0 (exact)");

        // Test 4: Exponential base e
        Console.WriteLine("\nTest 4: Exponential Base e (SafeExp)");
        var exp_1 = Float128PreciseTranscendentals.SafeExp((Float128)1);
        Console.WriteLine($"  e^1 = {exp_1}");
        Console.WriteLine($"  Expected ≈ 2.71828182845904523536028747135266");

        var exp_ln10 = Float128PreciseTranscendentals.SafeExp(Float128PreciseTranscendentals.SafeLog((Float128)10));
        Console.WriteLine($"  e^(ln(10)) = {exp_ln10}");
        Console.WriteLine($"  Expected ≈ 10.0");

        // Test 5: Exponential base 2
        Console.WriteLine("\nTest 5: Exponential Base 2 (SafeExp2)");
        var exp2_10 = Float128PreciseTranscendentals.SafeExp2((Float128)10);
        Console.WriteLine($"  2^10 = {exp2_10}");
        Console.WriteLine($"  Expected = 1024.0 (exact)");

        var exp2_half = Float128PreciseTranscendentals.SafeExp2((Float128)0.5m);
        Console.WriteLine($"  2^0.5 = {exp2_half}");
        Console.WriteLine($"  Expected ≈ 1.41421356237309504880168872420969 (√2)");

        // Test 6: Exponential base 10
        Console.WriteLine("\nTest 6: Exponential Base 10 (SafeExp10)");
        var exp10_2 = Float128PreciseTranscendentals.SafeExp10((Float128)2);
        Console.WriteLine($"  10^2 = {exp10_2}");
        Console.WriteLine($"  Expected = 100.0 (exact)");

        var exp10_1 = Float128PreciseTranscendentals.SafeExp10((Float128)1);
        Console.WriteLine($"  10^1 = {exp10_1}");
        Console.WriteLine($"  Expected = 10.0 (exact)");

        // Test 7: Power function
        Console.WriteLine("\nTest 7: Power Function (SafePow)");
        var pow_2_3 = Float128PreciseTranscendentals.SafePow((Float128)2, (Float128)3);
        Console.WriteLine($"  2^3 = {pow_2_3}");
        Console.WriteLine($"  Expected = 8.0 (exact)");

        var pow_16_half = Float128PreciseTranscendentals.SafePow((Float128)16, (Float128)0.5m);
        Console.WriteLine($"  16^0.5 = {pow_16_half}");
        Console.WriteLine($"  Expected = 4.0 (exact)");

        Console.WriteLine("\n=== Precision Tests Complete ===");
    }
}
