# Arithmonym Casts

There's lots of casts that `Arithmonym` provides, both `implicit` and `explicit`.

Here is a list of all the `implicit` casts...

* `double` &rightarrow; `Arithmonym`
* `Arithmonym` &rightarrow; `Float128` (may lose precision, beware)

And the explicit casts are to and from all built-in numerical types, except `decimal`, which simply cannot be supported due to the need of exponential functions in the conversions.

---

## Example

```csharp
using GoogolSharp;
using FakeStatisticsAPI;

float factorialOf10 = FakeFactorial(10);
float factorialOf20 = FakeFactorial(20);

Arithmonym product = Arithmonym.Pow((Arithmonym)factorialOf10, (Arithmonym)factorialOf20);
```

Thanks to the casts we can safely raise the two numbers, without overflowing.

A third-party type `Float128` used in this library is also supported.