using System;
using GoogolSharp;
using GoogolSharp.Helpers;
using QuadrupleLib;
using Float128 = QuadrupleLib.Float128<QuadrupleLib.Accelerators.DefaultAccelerator>;

Console.WriteLine("=== SafeLog Implementation Test ===\n");

Console.WriteLine("Testing Math.Log values:");
Console.WriteLine($"Math.Log2(100) = {Math.Log2(100)}");
Console.WriteLine($"Math.Log(100) = {Math.Log(100)}");
Console.WriteLine($"Math.Log10(100) = {Math.Log10(100)}");

Float128 log2_100 = Float128PreciseTranscendentals.SafeLog2((Float128)100);
Float128 ln_100 = Float128PreciseTranscendentals.SafeLog((Float128)100);
Float128 log10_100 = Float128PreciseTranscendentals.SafeLog10((Float128)100);

Console.WriteLine($"\nSafeLog2(100) = {log2_100}");
Console.WriteLine($"SafeLog(100) = {ln_100}");
Console.WriteLine($"SafeLog10(100) = {log10_100}");

Console.WriteLine($"\nManual: SafeLog2(100) / Log2_10 = {log2_100} / {Float128PreciseTranscendentals.Log2_10} = {log2_100 / Float128PreciseTranscendentals.Log2_10}");

Console.WriteLine($"\n==== Test Log10 Precision ====\n");

int[] testVals = { 10, 100, 1000, 10000 };
foreach (int val in testVals)
{
    Float128 result = Float128PreciseTranscendentals.SafeLog10((Float128)val);
    double expected = Math.Log10(val);
    Float128 error = result - (Float128)expected;

    Console.WriteLine($"log10({val,5}): {result} (err={error})");
}

Console.WriteLine($"\n==== Combinatorics ====\n");

Arithmonym p = Arithmonym.Permutations(52, 4);
Arithmonym c = Arithmonym.Combinations(30, 5);

Console.WriteLine($"P(52,4) = {p}");
Console.WriteLine($"C(30,5) = {c}");
