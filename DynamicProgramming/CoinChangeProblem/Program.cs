using System.Collections.Generic;

Console.WriteLine(getWays(4, new List<long> { 1, 2, 3 }));


long getWays(int n, List<long> c) 
{
    c.Sort((a, b) => a.CompareTo(b));

    var dp = new long[n + 1];
    dp[0] = 1;

    var len = c.Count;
    for (var i = 0; i < len; i++) 
    {
        long v = c[i];

        for (var j = v; j <= n; j++) 
        {
            dp[j] += dp[j - v];
        }
    }

    return dp[n];
}

/*
long getWays(long n, List<long> c, int j, Dictionary<long, long> lookup)
{
    if (lookup.TryGetValue(n, out var b)) {
        return b;
    }

    long result = 0;
    var len = c.Count;
    for (var i = j; i < len; i++) 
    {
        long v = c[i];
        if (v > n) 
        {
            continue;
        }
        
        if (v == n) 
        {
            result++;
            break;
        } 
        else 
        {
            result += getWays(n - v, c, i, lookup);
        }
    }
    
    return  (lookup[n] = result);
}
*/