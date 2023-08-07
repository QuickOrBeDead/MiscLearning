static int[,] PrepareLCSArray(string[] s1, string[] s2)
{
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

    return lp;
}


static IList<string> GenerateDiffList(string[] s1, string[] s2)
{
    int[,] lp = PrepareLCSArray(s1, s2);

    var n = s1.Length;
    var m = s2.Length;
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

    return diff;
}


static void GenerateDiffListForTextFile(Diff diff)
{
    TextFile t1 = diff.File1;
    TextFile t2 = diff.File2;

    int[,] lp = PrepareLCSArray(t1.Lines.Select(x => x.Text).ToArray(), t2.Lines.Select(x => x.Text).ToArray());

    var n = t1.Lines.Count;
    var m = t2.Lines.Count;

    var j = m;
    var i = n;
    while (i > 0 || j > 0)
    {
        if (i > 0 && j > 0 && t1.Lines[i - 1].Text == t2.Lines[j - 1].Text)
        {
            diff.Diffs.Add(new DiffInfo { Line1 = i - 1, Line2 = j - 1 });

            i--;
            j--;
        } 
        else if (j > 0 && (i == 0 || lp[i, j - 1] >= lp[i - 1, j]))
        {
            diff.Diffs.Add(new DiffInfo { File = 2, Line1 = i - 1, Line2 = j - 1 });

            j--;
        }
        else if (i > 0 && (j == 0 || lp[i, j - 1] < lp[i - 1, j]))
        {
            diff.Diffs.Add(new DiffInfo { File = 1, Line1 = i - 1, Line2 = j - 1 });

            i--;
        }
        else
        {
            break;
        }
    }
}

var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
var file1 = new TextFile { Lines = File.ReadAllLines(Path.Combine(dataPath, "Text1.txt")).Select(x => new Line { Text = x }).ToList() };
var file2 = new TextFile { Lines = File.ReadAllLines(Path.Combine(dataPath, "Text2.txt")).Select(x => new Line { Text = x }).ToList() };
var diff = new Diff(file1, file2);

GenerateDiffListForTextFile(diff);

Console.WriteLine($"S1:{Environment.NewLine}{string.Join(Environment.NewLine, file1.Lines.Select(x => x.Text))}");
Console.WriteLine();
Console.WriteLine($"S2:{Environment.NewLine}{string.Join(Environment.NewLine, file2.Lines.Select(x => x.Text))}");
Console.WriteLine();
Console.WriteLine("Diff:");

for (int i = diff.Diffs.Count - 1; i >= 0; i--)
{
    DiffInfo d = diff.Diffs[i];
    if (d.File == 1)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(diff.File1.Lines[d.Line1].Text);
    }
    else if (d.File == 2)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(diff.File2.Lines[d.Line2].Text);
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(diff.File1.Lines[d.Line1].Text);
    }

    Console.Write(Environment.NewLine);
}

Console.WriteLine();
Console.ReadLine();

class DiffInfo
{
    public int File { get; set; }

    public int Line1 { get; set; }

    public int Line2 { get; set; }

    public IList<string> WordDiff { get; set; }
}

class Diff
{
    public TextFile File1 { get; }

    public TextFile File2 { get; }

    public IList<DiffInfo> Diffs { get; }

    public Diff(TextFile file1, TextFile file2)
    {
        File1 = file1;
        File2 = file2;
        Diffs = new List<DiffInfo>();
    }
}

class TextFile
{
    public IList<Line> Lines { get; set; }

    public TextFile()
    {
        Lines = new List<Line>();
    }
}

class Line
{
    public string Text { get; set; }
}

