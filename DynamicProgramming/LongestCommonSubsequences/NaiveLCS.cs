namespace LongestCommonSubsequences;

public static class NaiveLCS
{
    public static string FindString(string s1, string s2) 
    {
        var seq1 = FindSubsequences(s1);
        var seq2 = FindSubsequences(s2);
        
        var maxLen = Math.Min(s1.Length, s2.Length);

        for (int i = 0; i < seq1.Count; i++)
        {
            if (seq1.Count > maxLen) 
            {
                continue;
            }

            var s = seq1[i];
            if (seq2.Contains(s)) 
            {
                return s;
            }
        }

        return string.Empty;
    }

    private static IList<string> FindSubsequences(string s) 
    {
        if (string.IsNullOrEmpty(s)) 
        {
            return new List<string>(0);
        }

        var result = new List<string>();
        for (int n = 1; n < s.Length; n++)
        {
            var seq = string.Empty;
            for (int i = 0; i < s.Length; i++)
            {
                seq += s[i].ToString();
                n--;

                if (n == 0)
                {
                    result.Add(seq);
                    seq = string.Empty;
                    n++;
                    continue;
                }

                for (int j = n; j < s.Length; j++)
                {
                    seq += s[j].ToString();
                    n--;

                    if (n == 0)
                    {
                        result.Add(seq);
                        seq = string.Empty;
                        n++;
                        continue;
                    }
                }
            }
        }

        result.Add(s);
        return result;
    }
}