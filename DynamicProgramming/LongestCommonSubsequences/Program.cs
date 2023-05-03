
var s1 = "ABCDEFG";
var s2 = "BCDGK";

var n = s1.Length;
var m = s2.Length;

var lp = new int[n + 1, m + 1];

for (var i = 1; i <= n; i++)
{
    for (var j = 1; j <= m; j++)
    {
        if (s1[i - 1] == s2[j - 1])
        {
            lp[i, j] = lp[i - 1, j - 1] + 1;
        }
        else
        {
            lp[i, j] = Math.Max(lp[i, j - 1], lp[i - 1, j]);
        }
    }
}

Console.WriteLine("LCS('{0}', '{1}') = {2}", s1, s2, lp[n, m]);
Console.ReadLine();
