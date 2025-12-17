# Getting Started with GoogolSharp

This guide will help you set up GoogolSharp and start working with **extremely large numbers** using the `Arithmonym` type.

---

## 📦 Installation

1. `GoogolSharp` is in NuGet! 🎉 Simply install using `dotnet add package GoogolSharp`.
2. Remember to put `using GoogolSharp;` at the start of your file.
3. Done!

---

## Example

```csharp
using GoogolSharp;

var six = new Arithmonym(6);
var seven = new Arithmonym(7);
Console.WriteLine(Arithmonym.Pow(six, seven)); // 326592
```