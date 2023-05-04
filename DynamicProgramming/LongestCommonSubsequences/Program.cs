using LongestCommonSubsequences;

var s1 = "XMJYAUZ";
var s2 = "MZJAWXU";

Console.WriteLine("DynamicProgrammingLCS('{0}', '{1}') = '{2}'", s1, s2, DynamicProgrammingLCS.FindString(s1, s2));
Console.WriteLine("NaiveLCS('{0}', '{1}') = '{2}'", s1, s2, NaiveLCS.FindString(s1, s2));
Console.ReadLine();
