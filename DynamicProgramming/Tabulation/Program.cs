const int q = 8;
Console.WriteLine($"f({8}) = {Fibonacci(q)}");

int Fibonacci(int n) 
{
    var lookup = new int[n + 1];
    lookup[0] = 0;
    lookup[1] = 1;

    for (int i = 2; i <= n; i++)
    {
        lookup[i] = lookup[i - 1] + lookup[i - 2];
    }

    return lookup[n];
}
