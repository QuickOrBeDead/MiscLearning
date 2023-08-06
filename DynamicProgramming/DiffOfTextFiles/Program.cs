using System.IO;

var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
var s1 = File.ReadAllText(Path.Combine(dataPath, "Text1.txt")).Split(' ');
var s2 = File.ReadAllText(Path.Combine(dataPath, "Text2.txt")).Split(' ');

var n = s1.Length;
var m = s2.Length;

var lp = new int[n + 1, m + 1];

for (var i1 = 1; i1 <= n; i1++)
{
    for (var j1 = 1; j1 <= m; j1++)
    {
        if (s1[i1 - 1] == s2[j1 - 1])
        {
            lp[i1, j1] = lp[i1 - 1, j1 - 1] + 1;
        }
        else
        {
            lp[i1, j1] = Math.Max(lp[i1, j1 - 1], lp[i1 - 1, j1]);
        }
    }
}

var diff = new List<string>();
var j = m;
var i = n;
while (i > 0 || j > 0)
{
    if (i > 0 && j > 0 && s1[i - 1] == s2[j - 1])
    {
        diff.Add(s1[i - 1]);

        i--;
        j--;
    } 
    else if (j > 0 && (i == 0 || lp[i, j - 1] >= lp[i - 1, j]))
    {
        diff.Add("+" + s2[j - 1]);
        j--;
    }
    else if (i > 0 && (j == 0 || lp[i, j - 1] < lp[i - 1, j]))
    {
        diff.Add("-" + s1[i - 1]);
        i--;
    }
    else
    {
        break;
    }
}

diff.Reverse();
Console.WriteLine("S1\t = {0}", string.Join(" ", s1));
Console.WriteLine("S2\t = {0}", string.Join(" ", s2));
Console.Write("Diff\t = ");

foreach (var s in diff)
{
    if (s.StartsWith("-", StringComparison.OrdinalIgnoreCase))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(s[1..]);
    }
    else if (s.StartsWith("+", StringComparison.OrdinalIgnoreCase))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(s[1..]);
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(s);
    }

    Console.Write(' ');
}

Console.WriteLine();
Console.ReadLine();
