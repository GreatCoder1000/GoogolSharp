# Getting Started with GoogolSharp

This guide will help you set up GoogolSharp and start working with **extremely large numbers** using the `Arithmonym` type.

---

## 📦 Installation

1. `GoogolSharp` is in NuGet! 🎉 Simply install using `dotnet add package GoogolSharp`.
2. Remember to put `using GoogolSharp;` at the start of your file.
3. Make sure your `.csproj` looks something like:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <!-- Target the same frameworks as GoogolSharp -->
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <!-- Reference GoogolSharp from NuGet -->
    <PackageReference Include="GoogolSharp" Version="0.2.9" />
  </ItemGroup>

</Project>
```

---

## Example

```csharp
using GoogolSharp;

var six = (Arithmonym)6;
var seven = (Arithmonym)7;
Console.WriteLine(Arithmonym.Pow(six, seven)); // 326592
```