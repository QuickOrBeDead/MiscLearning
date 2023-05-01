const int n = 8;

int[] lookup = new int[n + 1];
int Fibonacci(int n) 
{
    var v = lookup[n];
    if (v != 0) {
        return v;
    }
   
    if (n < 2) 
    {
        return (lookup[n] = n);
    }
    else
    {
        return (lookup[n] = Fibonacci(n -1) + Fibonacci(n - 2));
    }
}

Console.WriteLine($"f({n}) = {Fibonacci(n)}");


