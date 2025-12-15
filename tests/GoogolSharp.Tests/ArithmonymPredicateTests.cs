namespace GoogolSharp.Tests;

public class ArithmonymPredicateTests
{
    [Fact]
    public void TestZero()
    {
        Assert.True(Arithmonym.IsZero(Arithmonym.Zero));
    }

    [Fact]
    public void TestPositiveInfinity()
    {
        Assert.True(Arithmonym.IsInfinity(Arithmonym.PositiveInfinity));
    }

    [Fact]
    public void TestNegativeInfinity()
    {
        Assert.True(Arithmonym.IsInfinity(Arithmonym.NegativeInfinity));
    }

    [Fact]
    public void TestNaN()
    {
        Assert.True(Arithmonym.IsNaN(Arithmonym.NaN));
    }
    [Fact]
    public void TestOneIsNotZeroOrInfinity()
    {
        Assert.False(Arithmonym.IsZero(Arithmonym.One));
        Assert.False(Arithmonym.IsInfinity(Arithmonym.One));
    }

    [Fact]
    public void TestReciprocalOfTwoIsHalf()
    {
        var half = Arithmonym.Two.Reciprocal;
        double value = half.ToDouble();
        Assert.Equal(0.5, value, precision: 10);
    }

    [Fact]
    public void TestNegationOfTwoIsNegativeTwo()
    {
        var negTwo = Arithmonym.Two.Negated;
        double value = negTwo.ToDouble();
        Assert.Equal(-2.0, value, precision: 10);
    }

    [Fact]
    public void TestLog10OfTenIsOne()
    {
        var ten = new Arithmonym(10);
        var log10Ten = Arithmonym.Log10(ten);
        double value = log10Ten.ToDouble();
        Assert.Equal(1.0, value, precision: 10);
    }

    [Fact]
    public void TestConversionsFromOne()
    {
        Assert.Equal(1UL, Arithmonym.One.ToUlong());
        Assert.Equal(1L, Arithmonym.One.ToLong());
        Assert.Equal(1U, Arithmonym.One.ToUint());
        Assert.Equal(1, Arithmonym.One.ToInt());
        Assert.Equal(1.0, Arithmonym.One.ToDouble());
    }

    [Fact]
    public void TestQNaNPredicate()
    {
        Assert.True(Arithmonym.IsQNaN(Arithmonym.NaN));
    }

    [Fact]
    public void TestPositiveInfinityReciprocalIsZero()
    {
        var reciprocal = Arithmonym.PositiveInfinity.Reciprocal;
        Assert.True(Arithmonym.IsZero(reciprocal));
    }

    [Fact]
    public void TestNegativeInfinityNegatedIsPositiveInfinity()
    {
        var negated = Arithmonym.NegativeInfinity.Negated;
        Assert.True(Arithmonym.IsInfinity(negated));
        Assert.False(Arithmonym.IsNegative(negated));
    }
}
