using GoogolSharp;

// Call Tetration - debug output goes to console
var result = Arithmonym.Tetration(Arithmonym.Six, Arithmonym.Seven);

// Write comparison to file
using (var writer = System.IO.File.CreateText("comp_result.txt"))
{
    writer.WriteLine($"Tetration(6,7) result: {result}");
    writer.WriteLine($"Letter: 0x{result.Letter:X2}");
    writer.WriteLine($"Operand: {result.Operand}");

    var expected = Arithmonym.Parse("eeee2.069197e36305", null);
    writer.WriteLine($"\nParsed expected: {expected}");
    writer.WriteLine($"Letter: 0x{expected.Letter:X2}");
    writer.WriteLine($"Operand: {expected.Operand}");

    writer.WriteLine($"\nOperand difference: {expected.Operand - result.Operand}");
    writer.WriteLine($"Abs diff: {Arithmonym.Abs(expected.Operand - result.Operand)}");
}



