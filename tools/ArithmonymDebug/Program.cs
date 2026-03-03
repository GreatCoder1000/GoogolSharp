using System;
using GoogolSharp;
using QuadrupleLib;

Console.WriteLine("Diagnostics for Arithmonym/Float128");

void Print<T>(string name, T val) => Console.WriteLine($"{name}: {val}");

Print("Arithmonym.Zero.ToFloat128()", Arithmonym.Zero.ToFloat128());
Print("Arithmonym.IsZero(Zero)", Arithmonym.IsZero(Arithmonym.Zero));
Print("Arithmonym.IsInfinity(PositiveInfinity)", Arithmonym.IsInfinity(Arithmonym.PositiveInfinity));
Print("Arithmonym.IsNaN(Arithmonym.NaN)", Arithmonym.IsNaN(Arithmonym.NaN));

var a4 = new Arithmonym((Float128)4);
var a5 = new Arithmonym((Float128)5);
Print("new Arithmonym(4).ToFloat128()", a4.ToFloat128());
Print("new Arithmonym(5).ToFloat128()", a5.ToFloat128());
Print("4*5 via Arithmonym", (a4 * a5).ToFloat128());

Print("Reciprocal(4)", a4.Reciprocal.ToFloat128());
Print("Reciprocal(2)", new Arithmonym((Float128)2).Reciprocal.ToFloat128());

// Show internal packing via reflection
var t = typeof(Arithmonym);
var zero = Arithmonym.Zero;
var lo = t.GetField("squishedLo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(zero);
var mid = t.GetField("squishedMid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(zero);
var hi = t.GetField("squishedHi", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(zero);
Print("Zero.squishedLo", lo);
Print("Zero.squishedMid", mid);
Print("Zero.squishedHi", hi);

// Show the packed UInt128 value used to build Zero
UInt128 packedZero = ((UInt128)0x3f << (Arithmonym.FRACTION_BITS + 3)) | ((UInt128)1 << (Arithmonym.FRACTION_BITS + 9));
Print("packedZero.Lo", (ulong)packedZero);
Print("packedZero.Hi", (ulong)(packedZero >> 64));

// Print representation for new Arithmonym(0.0)
var z2 = new Arithmonym((Float128)0);
lo = t.GetField("squishedLo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(z2);
mid = t.GetField("squishedMid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(z2);
hi = t.GetField("squishedHi", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(z2);
Print("new Arithmonym(0).squishedLo", lo);
Print("new Arithmonym(0).squishedMid", mid);
Print("new Arithmonym(0).squishedHi", hi);
