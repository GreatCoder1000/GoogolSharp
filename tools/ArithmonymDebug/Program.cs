using QuadrupleLib;
using QuadrupleLib.Accelerators;
using GoogolSharp.Helpers;
using GoogolSharp;
using Float128 = QuadrupleLib.Float128<QuadrupleLib.Accelerators.DefaultAccelerator>;
using System;

namespace TranscendentalsTest
{
    class Program
    {
        // A small epsilon value for comparison
        static readonly Float128 Epsilon = Float128.ScaleB(Float128.One, -113);  // ~2^-113 for Float128 precision
public static void RunSmallNumberTests()
{
    // List of small numbers to test
    Float128[] testValues = new Float128[]
    {
        1e-10,
        1e-100,
        1e-1000,
        0.1,
        0.01
    };

    // Run SafeExp on each of these values and print the result
    foreach (var x in testValues)
    {
        var result = Float128PreciseTranscendentals.SafeExp(x);
        Console.WriteLine($"SafeExp({x}): {result}");
    }
}

        // Helper function for assertions with margin tolerance
        static void AssertEqual(Float128 expected, Float128 actual, string message, bool relaxTolerance = false)
        {
            // If the 'relaxTolerance' flag is set, we use a larger epsilon for comparison
            var tolerance = relaxTolerance ? Epsilon * Float128.ScaleB(Float128.One, 5) : Epsilon;

            // Perform the comparison with the given tolerance
            if (Float128.Abs(expected - actual) > tolerance)
            {
                Console.WriteLine($"FAIL: {message}\nExpected: {expected}\nActual: {actual}");
            }
            else
            {
                Console.WriteLine($"PASS: {message}");
            }
        }

        static void RunTests()
        {
            // Known constants and expected values
            Float128 exp1Expected = Float128.Parse("2.71828182845904523536028747135266249775724709369995", null); // exp(1)
            Float128 exp1e500Expected = Float128.One; // exp(1e-500) should return approximately 1
            Float128 expSubnormalExpected = Float128.One; // exp(subnormal value) should return approximately 1
            Float128 log2Expected = Float128.Parse("10", null); // log2(1024)
            Float128 log10Expected = Float128.Parse("3", null); // log10(1000)

            // Run tests with proper expected values

            // Test 1: exp(1)
            AssertEqual(exp1Expected, Float128PreciseTranscendentals.SafeExp(Float128.One), "exp(1)");

            // Test 2: exp(1e-500)
            AssertEqual(exp1e500Expected, Float128PreciseTranscendentals.SafeExp(Float128.Parse("1e-500", null)), "exp(1e-500)");

            // Test 3: exp(subnormal value) - we assume subnormal values are extremely small and close to 0
            AssertEqual(expSubnormalExpected, Float128PreciseTranscendentals.SafeExp(Float128.Parse("1e-5000", null)), "exp(subnormal value)");

            // Test 4: log2(1024) - should be exactly 10, but due to precision we use a slightly relaxed tolerance
            AssertEqual(log2Expected, Float128PreciseTranscendentals.SafeLog2(Float128.Parse("1024", null)), "log2(1024)", relaxTolerance: true);

            // Test 5: log10(1000) - should be exactly 3, but again allow for a slight tolerance
            AssertEqual(log10Expected, Float128PreciseTranscendentals.SafeLog10(Float128.Parse("1000", null)), "log10(1000)", relaxTolerance: true);

            // Test 6: exp(3) - Verify a larger exponential value
            AssertEqual(Float128.Parse("20.085536923187667740928809115179859794829456933775", null), Float128PreciseTranscendentals.SafeExp(Float128.Parse("3", null)), "exp(3)");

            // Test 7: pow(2, 3) - Verify basic exponentiation
            AssertEqual(Float128.Parse("8", null), Float128PreciseTranscendentals.SafePow(Float128.Parse("2", null), Float128.Parse("3", null)), "2^3");

            // Test 8: exp(+Infinity) - Should return PositiveInfinity
            AssertEqual(Float128.PositiveInfinity, Float128PreciseTranscendentals.SafeExp(Float128.PositiveInfinity), "exp(+Infinity)");

            // Test 9: exp(-Infinity) - Should return 0
            AssertEqual(Float128.Zero, Float128PreciseTranscendentals.SafeExp(Float128.NegativeInfinity), "exp(-Infinity)");

            // Test 10: exp(0) = 1
            AssertEqual(Float128.One, Float128PreciseTranscendentals.SafeExp(Float128.Zero), "exp(0)");

            // Test 11: Log of negative value should throw an exception
            try
            {
                var result = Float128PreciseTranscendentals.SafeLog(Float128.NegativeOne);
                Console.WriteLine("FAIL: Log of negative value did not throw exception");
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("PASS: Log of negative value correctly threw exception.");
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Running Float128PreciseTranscendentals Tests...\n");
            RunTests();
            RunSmallNumberTests();
            Console.WriteLine("\nAll tests completed.");

            Console.WriteLine($"4^4^4^4 result: {Arithmonym.Pow(4, Arithmonym.Pow(4, Arithmonym.Pow(4, 4)))}");
        }
    }
}