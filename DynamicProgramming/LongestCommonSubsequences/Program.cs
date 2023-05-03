
var s1 = "XMJYAUZ";
var s2 = "MZJAWXUDSADSAF";

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

var len = lp[n, m];
var s3 = new char[len];
for (var i = n; i > 0; i--)
{
    for (var j = m; j > 0; j--)
    {
        while (i > 0 && lp[i, j] == lp[i - 1, j])
        {
            i--;
        }

        while (j > 0 && lp[i, j] == lp[i, j - 1])
        {
            j--;
        }

        if (i > 0 && lp[i, j] > 0)
        {
            s3[--len] = s1[i - 1];

            i--;
        }
    }
}


Console.WriteLine("LCS('{0}', '{1}') = '{2}'", s1, s2, new string(s3));
Console.ReadLine();
