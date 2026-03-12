using QuadrupleLib;
using QuadrupleLib.Accelerators;
using GoogolSharp.Helpers;
using GoogolSharp;
using Float128 = QuadrupleLib.Float128<QuadrupleLib.Accelerators.DefaultAccelerator>;
using System.Reflection;

Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║         EXHAUSTIVE EXPONENTIAL FUNCTION DEBUGGING          ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝\n");

// Test 1: Basic Exp10 convergence
Console.WriteLine("\n▶ TEST 1: Exp10 Convergence Analysis");
Console.WriteLine("═══════════════════════════════════════════════════════════\n");

TestExp10Convergence(1);
TestExp10Convergence(0);
TestExp10Convergence(2);
TestExp10Convergence(-1);

// Test 2: Exp2 convergence
Console.WriteLine("\n▶ TEST 2: Exp2 Convergence Analysis");
Console.WriteLine("═══════════════════════════════════════════════════════════\n");

TestExp2Convergence(1);
TestExp2Convergence(0.5);
TestExp2Convergence(Math.Log(10) / Math.Log(2)); // Should give 10

// Test 3: Exp convergence
Console.WriteLine("\n▶ TEST 3: Exp Convergence Analysis");
Console.WriteLine("═══════════════════════════════════════════════════════════\n");

TestExpConvergence(1); // Should be e
TestExpConvergence(Math.Log(10)); // Should be 10
TestExpConvergence(0);
TestExpConvergence(2);

// Test 4: Log consistency checks
Console.WriteLine("\n▶ TEST 4: Log Consistency Checks");
Console.WriteLine("═══════════════════════════════════════════════════════════\n");

TestLogConsistency(10);
TestLogConsistency(2);
TestLogConsistency(100);

// Test 5: Roundtrip tests
Console.WriteLine("\n▶ TEST 5: Roundtrip Consistency");
Console.WriteLine("═══════════════════════════════════════════════════════════\n");

TestRoundtrips(10);
TestRoundtrips(20);
TestRoundtrips(5);

// Test 6: Tetration
Console.WriteLine("\n▶ TEST 6: Tetration Test");
Console.WriteLine("═══════════════════════════════════════════════════════════\n");

try
{
    var baseV = Arithmonym.Two;
    var heightV = Arithmonym.Four;
    Console.WriteLine($"Computing Tetration(2, 4)...");
    Console.WriteLine($"  Base: {baseV}");
    Console.WriteLine($"  Height: {heightV}");

    var result = Arithmonym.Tetration(baseV, heightV);
    Console.WriteLine($"Result (ToString): {result}");
    //Console.WriteLine($"Expected: 65536");

    // Try to get the underlying operand
    var operprop = result.GetType().GetProperty("Operand", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreCase);
    if (operprop != null)
    {
        var operand = operprop.GetValue(result);
        Console.WriteLine($"Operand: {operand}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Stack: {ex.StackTrace}");
}

void TestExp10Convergence(double input)
{
    Console.WriteLine($"Exp10({input}):");
    var inputF = (Float128)input;

    // Reference value
    var expectedRef = System.Math.Pow(10, input);

    // Test SafeExp10
    var result = Float128PreciseTranscendentals.SafeExp10(inputF);
    Console.WriteLine($"  SafeExp10 result:  {(double)result}");
    Console.WriteLine($"  Expected (double): {expectedRef}");
    Console.WriteLine($"  Difference:        {(double)(result - (Float128)expectedRef):E15}");
    Console.WriteLine($"  Relative error:    {(double)(Float128.Abs((result - (Float128)expectedRef) / (Float128)expectedRef)):E15}");

    // Trace via Exp2(y * Log2(10))
    var log2_10 = Float128PreciseTranscendentals.SafeLog2((Float128)10);
    var product = inputF * log2_10;
    Console.WriteLine($"  Log2(10):          {(double)log2_10}");
    Console.WriteLine($"  y * Log2(10):      {(double)product}");

    var exp2_result = Float128PreciseTranscendentals.SafeExp2(product);
    Console.WriteLine($"  Exp2(y*Log2(10)): {(double)exp2_result}");
    Console.WriteLine();
}

void TestExpConvergence(double input)
{
    Console.WriteLine($"Exp({input}):");
    var inputF = (Float128)input;

    var expectedRef = System.Math.Exp(input);
    var result = Float128PreciseTranscendentals.SafeExp(inputF);

    Console.WriteLine($"  SafeExp result:    {(double)result}");
    Console.WriteLine($"  Expected (double): {expectedRef}");
    Console.WriteLine($"  Difference:        {(double)(result - (Float128)expectedRef):E15}");
    Console.WriteLine($"  Relative error:    {(double)(Float128.Abs((result - (Float128)expectedRef) / (Float128)expectedRef)):E15}");
    Console.WriteLine();
}

void TestLogConsistency(double value)
{
    Console.WriteLine($"Log consistency for {value}:");
    var valueF = (Float128)value;

    var log_e = Float128PreciseTranscendentals.SafeLog(valueF);
    var log2 = Float128PreciseTranscendentals.SafeLog2(valueF);
    var log10 = Float128PreciseTranscendentals.SafeLog10(valueF);

    Console.WriteLine($"  Log({value}):   {(double)log_e}");
    Console.WriteLine($"  Log2({value}):  {(double)log2}");
    Console.WriteLine($"  Log10({value}): {(double)log10}");

    // Check relationship: Log10(x) * Ln(10) should equal Ln(x)
    var check1 = log10 * Float128PreciseTranscendentals.Ln10;
    Console.WriteLine($"  Log10*Ln10:       {(double)check1} (should be {(double)log_e})");
    Console.WriteLine($"  Error:            {(double)(check1 - log_e):E15}");

    // Check relationship: Log2(x) * Ln(2) should equal Ln(x)
    var check2 = log2 * Float128PreciseTranscendentals.Ln2;
    Console.WriteLine($"  Log2*Ln2:         {(double)check2} (should be {(double)log_e})");
    Console.WriteLine($"  Error:            {(double)(check2 - log_e):E15}");
    Console.WriteLine();
}

void TestRoundtrips(double value)
{
    Console.WriteLine($"Roundtrip tests for {value}:");
    var valueF = (Float128)value;

    // Exp(Log(x)) = x
    var log_result = Float128PreciseTranscendentals.SafeLog(valueF);
    var exp_log = Float128PreciseTranscendentals.SafeExp(log_result);
    Console.WriteLine($"  Exp(Log({value})):     {(double)exp_log} (should be {value})");
    Console.WriteLine($"  Error:               {(double)(exp_log - valueF):E15}");

    // Exp10(Log10(x)) = x
    var log10_result = Float128PreciseTranscendentals.SafeLog10(valueF);
    var exp10_log10 = Float128PreciseTranscendentals.SafeExp10(log10_result);
    Console.WriteLine($"  Exp10(Log10({value})):  {(double)exp10_log10} (should be {value})");
    Console.WriteLine($"  Error:               {(double)(exp10_log10 - valueF):E15}");

    // Exp2(Log2(x)) = x
    var log2_result = Float128PreciseTranscendentals.SafeLog2(valueF);
    var exp2_log2 = Float128PreciseTranscendentals.SafeExp2(log2_result);
    Console.WriteLine($"  Exp2(Log2({value})):   {(double)exp2_log2} (should be {value})");
    Console.WriteLine($"  Error:               {(double)(exp2_log2 - valueF):E15}");
    Console.WriteLine();
}
