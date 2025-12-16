using GoogolSharp;

class Program
{
    static void Main()
    {
        var zero = Arithmonym.Zero;
        var big = new Arithmonym(123456);
        Console.WriteLine(Arithmonym.IsZero(zero*big));
    }
}