using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aoc2016.day06
{
    public class Solution
    {
        public static void Run()
        {
            string[] input = System.IO.File.ReadAllLines("day06.txt");
            int nxlen = input[0].Length;
            var errchecked = Enumerable.Range(0, nxlen).Select(i => new Dictionary<char, int>()).ToList();
            Parallel.ForEach(input, () => Enumerable.Range(0, nxlen).Select(i => new Dictionary<char, int>()).ToList(),
                (nx, loopState, partialResult) =>
                {
                    for (int i = 0; i < nxlen; i++)
                    {
                        var ec = partialResult[i];
                        if (ec.ContainsKey(nx[i]))
                            ec[nx[i]] = ec[nx[i]] + 1;
                        else
                            ec[nx[i]] = 1;
                    }
                    return partialResult;
                },
                (localPartial) =>
                {
                    lock(errchecked)
                    {
                        for (int i = 0; i < nxlen; i++)
                        {
                            var ec = errchecked[i];
                            var lp = localPartial[i];
                            foreach (var kv in lp)
                            {
                                if (ec.ContainsKey(kv.Key))
                                    ec[kv.Key] = ec[kv.Key] + kv.Value;
                                else
                                    ec[kv.Key] = kv.Value;
                                }
                        }
                    }
                }
            );

            string part1 = new string(errchecked.Select(dict => dict.OrderByDescending(kv => kv.Value).First().Key).ToArray());
            Console.WriteLine("==== Part 1 ====");
            Console.WriteLine($"Error checked message: {part1}");

            string part2 = new string(errchecked.Select(dict => dict.OrderBy(kv => kv.Value).First().Key).ToArray());
            Console.WriteLine("==== Part 2 ====");
            Console.WriteLine($"Error checked message: {part2}");
        }
    }
}
