# Factorials & Combinatorics

In statistics, factorials is a must. And, Arithmonym is the perfect type to approximate the number of possible combinations in a 1000x1000 rubik's cube. No limit, why not a googolplex by googolplex rubik's cube?

For example, 1000 factorial is calculated with `Arithmonym.Factorial((Arithmonym)1000)` (NEW IN `GoogolSharp 0.2`!)

The input can be `Arithmonym` or `double` (thanks to the implicit cast, see [Arithmonym Casts](arithmonym-casts.md))

---

## Example

For the sake of it let's make a program that calculates number of possible combinations in a N&times;N&times;N rubik's cube to test this out.

```csharp
using GoogolSharp;

class Program
{
    static Arithmonym RubiksCC(int n)
    {
        Arithmonym numerator = ((n % 2) == 0) ? (Arithmonym)1 : (Arithmonym)11771943321600;
        numerator *= 3674160;

        Arithmonym factorial24 = Arithmonym.Factorial((Arithmonym)24);
        numerator *= Arithmonym.Pow(factorial24, (Arithmonym)((n - 2) * (long)n / 4L));

        Arithmonym exponent = (Arithmonym)(6L * ((n - 2) * (long)(n - 2) / 4L));

        Arithmonym denominator = Arithmonym.Pow(24, exponent);

        return numerator / denominator;
    }

    static void Main()
    {
        Console.WriteLine(RubiksCC(1000));
    }
}
```