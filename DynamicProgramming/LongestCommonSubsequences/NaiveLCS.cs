namespace LongestCommonSubsequences;

public static class NaiveLCS
{
    public static string FindString(string? s1, string? s2) 
    {
        if (s1 == null || s2 == null)
        {
            return string.Empty;
        }

        var n = s1.Length;
        var m = s2.Length;

        if (m == 0 || n == 0)
        {
            return string.Empty;
        }

        if (s1[n - 1] == s2[m - 1])
        {
            return FindString(s1[..(n - 1)], s2[..(m - 1)]) + s1[n - 1];
        }

        var lcs1 = FindString(s1[..(n - 1)], s2[..m]);
        var lcs2 = FindString(s1[..n], s2[..(m - 1)]);
        return lcs1.Length > lcs2.Length ? lcs1 : lcs2;
    }
}