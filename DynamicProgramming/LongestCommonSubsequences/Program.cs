using System.Diagnostics;

using LongestCommonSubsequences;

var s1 = "XMJYAUZ";
var s2 = "MZJAWXU";

Calculate(s1, s2, nameof(DynamicProgrammingLCS), () => DynamicProgrammingLCS.FindString(s1, s2));
Calculate(s1, s2, nameof(NaiveLCS), () => NaiveLCS.FindString(s1, s2));
Console.ReadLine();

static void Calculate(string s1, string s2, string name, Func<string> findStringFunc)
{
    var sw = Stopwatch.StartNew();
    var r = findStringFunc();
    sw.Stop();

    Console.WriteLine("{0}('{1}', '{2}') = '{3}' in {4} ms", name, s1, s2, r, sw.ElapsedMilliseconds);
}
