namespace LongestCommonSubsequences;

public static class DynamicProgrammingLCS
{
    public static string FindString(string s1, string s2) 
    {
        var n = s1.Length;
        var m = s2.Length;

        var dp = new int[n + 1, m + 1];

        for (var i = 1; i <= n; i++)
        {
            for (var j = 1; j <= m; j++)
            {
                if (s1[i - 1] == s2[j - 1])
                {
                    dp[i, j] = dp[i - 1, j - 1] + 1;
                }
                else
                {
                    dp[i, j] = Math.Max(dp[i, j - 1], dp[i - 1, j]);
                }
            }
        }

        var len = dp[n, m];
        var result = new char[len];
        for (var i = n; i > 0; i--)
        {
            for (var j = m; j > 0; j--)
            {
                while (i > 0 && dp[i, j] == dp[i - 1, j])
                {
                    i--;
                }

                while (j > 0 && dp[i, j] == dp[i, j - 1])
                {
                    j--;
                }

                if (i > 0 && dp[i, j] > 0)
                {
                    result[--len] = s1[i - 1];

                    i--;
                }
            }
        }

        return new string(result);
    }
}