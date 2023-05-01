using System.Linq;

var sequence = new int[] { 10, 22, 9, 33, 21, 50, 41, 60 };
var lookup = Enumerable.Repeat(1, sequence.Length).ToArray();

for (int i = 0; i < sequence.Length; i++)
{
    for (int j = i + 1; j < sequence.Length; j++)
    {
        if (sequence[j] > sequence[i]) 
        {
            var s1 = lookup[i];
            if (s1 >= lookup[j]) 
            {
                lookup[j] = s1 + 1;
            }
        }
    }
}

Console.WriteLine($"Result: {lookup.Max(x => x)}");
