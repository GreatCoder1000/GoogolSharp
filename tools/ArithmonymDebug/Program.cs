using System;
using GoogolSharp;
using GoogolSharp.Helpers;
using QuadrupleLib;
using Float128 = QuadrupleLib.Float128<QuadrupleLib.Accelerators.DefaultAccelerator>;

Console.WriteLine("Diagnostics for Arithmonym/Float128");

void Print<T>(string name, T val) => Console.WriteLine($"{name}: {val}");

// Inline SnapToInt since it's private in Arithmonym
Float128 SnapToInt(Float128 x)
{
    Float128 n = Float128.Round(x);
    return (Float128.Abs(x - n) < Float128PreciseTranscendentals.SafeExp2(-40)) ? n : x;
}

Print("Arithmonym.Zero.ToFloat128()", Arithmonym.Zero.ToFloat128());
Print("Arithmonym.IsZero(Zero)", Arithmonym.IsZero(Arithmonym.Zero));
Print("Arithmonym.IsInfinity(PositiveInfinity)", Arithmonym.IsInfinity(Arithmonym.PositiveInfinity));
Print("Arithmonym.IsNaN(Arithmonym.NaN)", Arithmonym.IsNaN(Arithmonym.NaN));

var orig4 = (Float128)4;
Print("original float128 4", orig4);
Print("orig4 as double string", ((double)orig4).ToString("G17"));

// replicate mapping logic from constructor
Float128 temp = orig4;
if (temp < 0) temp = -temp;
if (temp < 1) temp = 1 / temp;
if (temp < 20)
{
    temp /= 2;
    temp = SnapToInt(temp);
    Print("temp after mapping", temp);
    Print("temp as double string", ((double)temp).ToString("G17"));
}

// probe the static EncodeOperand method
var encode = typeof(Arithmonym).GetMethod("EncodeOperand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
if (encode != null)
{
    var opBits = (UInt128)encode.Invoke(null, new object[] { temp })!;
    Print("opBits result", opBits);

    // Extract floored and fraction manually  
    byte flooredByte = (byte)(opBits >> 85);
    Print("opBits floored byte", flooredByte);
}

var a3 = new Arithmonym((Float128)3);
Print("new Arithmonym(3).ToFloat128()", a3.ToFloat128());
{
    var opProp = typeof(Arithmonym).GetProperty("Operand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    var flooredProp = typeof(Arithmonym).GetProperty("OperandFloored", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    if (opProp != null && flooredProp != null)
    {
        var op = opProp.GetValue(a3);
        Print("  a3.Operand", op);
        Print("  a3.Operand as double", ((double)(Float128)op!).ToString("G17"));
        Print("  a3.OperandFloored", flooredProp.GetValue(a3));
    }
}

var a4 = new Arithmonym(orig4);
Print("new Arithmonym(4).ToFloat128()", a4.ToFloat128());
{
    var opProp = typeof(Arithmonym).GetProperty("Operand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    var flooredProp = typeof(Arithmonym).GetProperty("OperandFloored", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    if (opProp != null && flooredProp != null)
    {
        var op = opProp.GetValue(a4);
        Print("  a4.Operand", op);
        Print("  a4.Operand as double", ((double)(Float128)op!).ToString("G17"));
        Print("  a4.OperandFloored", flooredProp.GetValue(a4));
    }
}

var a5 = new Arithmonym((Float128)5);
var a20 = new Arithmonym((Float128)20);
Print("new Arithmonym(20).ToFloat128()", a20.ToFloat128());

Print("new Arithmonym(5).ToFloat128()", a5.ToFloat128());
{
    var opProp = typeof(Arithmonym).GetProperty("Operand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    var flooredProp = typeof(Arithmonym).GetProperty("OperandFloored", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    if (opProp != null && flooredProp != null)
    {
        var op = opProp.GetValue(a5);
        Print("  a5.Operand", op);
        Print("  a5.Operand as double", ((double)(Float128)op!).ToString("G17"));
        Print("  a5.OperandFloored", flooredProp.GetValue(a5));
    }
}

Print("3 + 5 via Arithmonym", (a3 + a5).ToFloat128());
Print("5 - 2 via Arithmonym", (a5 - new Arithmonym((Float128)2)).ToFloat128());
Print("4*5 via Arithmonym", (a4 * a5).ToFloat128());
Print("Reciprocal(4)", a4.Reciprocal.ToFloat128());

var a10 = new Arithmonym((Float128)10);
Print("\nnew Arithmonym(10).ToFloat128()", a10.ToFloat128());
{
    var letterProp = typeof(Arithmonym).GetProperty("Letter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    var opProp = typeof(Arithmonym).GetProperty("Operand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    if (letterProp != null && opProp != null)
    {
        var letter = (byte)letterProp.GetValue(a10)!;
        var op = (Float128)opProp.GetValue(a10)!;
        Print($"  a10.Letter: {letter:X2}", letter);
        Print($"  a10.Operand: {op}", op);
    }
}
var log10v = Arithmonym.Log10(a10);
Print("Log10(10)", log10v.ToFloat128());
{
    var letterProp = typeof(Arithmonym).GetProperty("Letter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    var opProp = typeof(Arithmonym).GetProperty("Operand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    if (letterProp != null && opProp != null)
    {
        var letter = (byte)letterProp.GetValue(log10v)!;
        var op = (Float128)opProp.GetValue(log10v)!;
        Print($"  log10v.Letter: {letter:X2}", letter);
        Print($"  log10v.Operand: {op}", op);
    }
}

var a100 = new Arithmonym((Float128)100);
Print("\nnew Arithmonym(100).ToFloat128()before Log10", a100.ToFloat128());
{
    var val100Float = (Float128)100;
    Print("  Raw Float128(100)", val100Float);

    var log100direct = Float128PreciseTranscendentals.SafeLog10(val100Float);
    Print("  SafeLog10(100) direct", log100direct);
}

Print("new Arithmonym(100).ToFloat128()", a100.ToFloat128());
{
    var letterProp = typeof(Arithmonym).GetProperty("Letter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    var opProp = typeof(Arithmonym).GetProperty("Operand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    if (letterProp != null && opProp != null)
    {
        var letter = (byte)letterProp.GetValue(a100)!;
        var op = (Float128)opProp.GetValue(a100)!;
        Print($"  a100.Letter: {letter:X2}", letter);
        Print($"  a100.Operand: {op}", op);
    }
}

var log100 = Arithmonym.Log10(a100);
Print("Log10(100)", log100.ToFloat128());

// test Exp10
var exp10 = Arithmonym.Exp10(new Arithmonym((Float128)10));
Print("Exp10(10)", exp10.ToFloat128());
{
    var letterProp = typeof(Arithmonym).GetProperty("Letter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    var opProp = typeof(Arithmonym).GetProperty("Operand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    if (letterProp != null && opProp != null)
    {
        var letter = (byte)letterProp.GetValue(exp10)!;
        var op = (Float128)opProp.GetValue(exp10)!;
        Print($"  exp10.Letter: {letter:X2}", letter);
        Print($"  exp10.Operand: {op}", op);
    }
}
{
    var letterProp = typeof(Arithmonym).GetProperty("Letter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    var opProp = typeof(Arithmonym).GetProperty("Operand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    if (letterProp != null && opProp != null)
    {
        var letter = (byte)letterProp.GetValue(log100)!;
        var op = (Float128)opProp.GetValue(log100)!;
        Print($"  log100.Letter: {letter:X2}", letter);
        Print($"  log100.Operand: {op}", op);
        Print($"  log100 is infinity? {Arithmonym.IsInfinity(log100)}", Arithmonym.IsInfinity(log100));
    }
}
