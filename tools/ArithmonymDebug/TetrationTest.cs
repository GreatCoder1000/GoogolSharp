using GoogolSharp;
using System;

/// <summary>
/// Debug utility for testing Arithmonym tetration operation.
/// 
/// Tetration (also called power tower or hyper-4) computes repeated exponentiation:
/// 2↑↑4 = 2^(2^(2^2)) = 2^(2^4) = 2^16 = 65536
/// 
/// This test validates the Arithmonym.Tetration method with basic inputs.
/// </summary>
class TetrationTest
{
    /// <summary>
    /// Main entry point for tetration debugging.
    /// 
    /// Tests: Arithmonym.Tetration(2, 4) which should equal 65536
    /// 
    /// This represents the mathematical operation:
    /// 2↑↑4 = 2^2^2^2 = 2^(2^(2^2)) = 2^(2^4) = 2^16 = 65536
    /// </summary>
    static void Main()
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║   Arithmonym Tetration Debugging: 2↑↑4 = 65536             ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝\n");

        TestTetration();
    }

    /// <summary>
    /// Tests Arithmonym.Tetration(2, 4) and displays detailed diagnostic information.
    /// </summary>
    static void TestTetration()
    {
        Console.WriteLine("Test: Tetration(2, 4)");
        Console.WriteLine("Mathematical operation: 2↑↑4 = 2^(2^(2^2)) = 2^16 = 65536\n");

        try
        {
            var two = Arithmonym.Two;
            var four = Arithmonym.Four;

            Console.WriteLine($"Base (2):   {two}");
            Console.WriteLine($"Height (4): {four}\n");

            Console.WriteLine("Computing result...");
            var result = Arithmonym.Tetration(two, four);

            Console.WriteLine($"\nResult:    {result}");
            Console.WriteLine($"Expected:  65536");

            var expected = new Arithmonym(65536);
            bool matches = result == expected;

            Console.WriteLine($"\nVerification:");
            Console.WriteLine($"  Equals 65536:     {matches}");

            // Try to extract numeric value
            try
            {
                var resultInt64 = (long)result;
                Console.WriteLine($"  As Int64:         {resultInt64}");
                Console.WriteLine($"  Decimal match:    {resultInt64 == 65536}");
            }
            catch (Exception castEx)
            {
                Console.WriteLine($"  Int64 conversion failed: {castEx.Message}");
            }

            if (matches)
            {
                Console.WriteLine("\n✓ TEST PASSED: Tetration(2, 4) = 65536");
            }
            else
            {
                Console.WriteLine("\n✗ TEST FAILED: Tetration(2, 4) does not equal 65536");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ ERROR: {ex.Message}");
            Console.WriteLine($"\nStack Trace:\n{ex.StackTrace}");
        }
    }
}