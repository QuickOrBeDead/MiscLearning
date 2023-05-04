namespace LongestCommonSubsequences;

public static class NaiveLCS
{
    public static string FindString(string? s1, string? s2) 
    {
        if (s1 == null || s2 == null)
        {
            return string.Empty;
        }

        var seq1 = FindSubsequences(s1).OrderByDescending(x => x.Length).ToList();
        var seq2 = FindSubsequences(s2);
        
        for (int i = 0; i < seq1.Count; i++)
        {
            var s = seq1[i];
            if (seq2.Contains(s)) 
            {
                return s;
            }
        }

        return string.Empty;
    }

    private static IList<string> FindSubsequences(string s, string seq = "", int i = 0) 
    {
        var result = new List<string>();
        for (; i < s.Length; i++)
        {
            var newSeq = seq + s[i];
            result.Add(newSeq);

            if (i < s.Length - 1)
            {
                result.AddRange(FindSubsequences(s, newSeq, i + 1));
            }
        }

        return result;
    }
}