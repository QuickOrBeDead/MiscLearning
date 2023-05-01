Console.WriteLine(GetWays(4, new List<long> { 1, 2, 3 }, 0, new long?[3, 5]));
Console.ReadLine();

static long GetWays(long n, IReadOnlyList<long> c, int j, long?[,] lookup)
{
    var r = lookup[j, n];
    if (r.HasValue)
    {
        return r.Value;
    }

    if (n == 0)
    {
        return 1;
    }

    long result = 0;
    for (var i = j; i < c.Count; i++)
    {
        var v = c[i];
        if (v > n)
        {
            continue;
        }

        result += GetWays(n - v, c, i, lookup);
    }

    lookup[j, n] = result;

    return result;
}