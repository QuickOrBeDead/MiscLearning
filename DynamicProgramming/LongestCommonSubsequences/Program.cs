using LongestCommonSubsequences;

var s1 = "XMJYAUZ";
var s2 = "MZJAWXUDSADSAF";

Console.WriteLine("LCS('{0}', '{1}') = '{2}'", s1, s2, DynamicProgrammingLCS.FindString(s1, s2));
Console.ReadLine();
