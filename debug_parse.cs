using GoogolSharp;

// Debug the parsing issue
var parsed = Arithmonym.Parse("eeee2.069197e36305", null);
var tetrated = Arithmonym.Tetration(Arithmonym.Six, Arithmonym.Seven);

Console.WriteLine($"Parsed: {parsed.ToString()}");
Console.WriteLine($"Parsed ToCommon: {parsed.ToCommonString()}");
Console.WriteLine($"Parsed ToAbbreviated: {parsed.ToAbbreviatedString()}");
Console.WriteLine();
Console.WriteLine($"Tetrated: {tetrated.ToString()}");
Console.WriteLine($"Tetrated ToCommon: {tetrated.ToCommonString()}");
Console.WriteLine($"Tetrated ToAbbreviated: {tetrated.ToAbbreviatedString()}");

// Try parsing just the core value
var simpleParse = Arithmonym.Parse("2.069197e36305", null);
Console.WriteLine();
Console.WriteLine($"Simple Parse: {simpleParse.ToString()}");

// Try parsing with one 'e'
var onceE = Arithmonym.Parse("e2.069197e36305", null);
Console.WriteLine($"Once E: {onceE.ToString()}");

// Try parsing with two 'e's
var twiceE = Arithmonym.Parse("ee2.069197e36305", null);
Console.WriteLine($"Twice E: {twiceE.ToString()}");
