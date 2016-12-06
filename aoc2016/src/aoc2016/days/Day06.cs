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
            var errchecked = input.Aggregate(Enumerable.Range(0, input[0].Length).Select(i => new Dictionary<char, int>()).ToList(), (err, nx) =>
            {
                for (int i = 0; i < nx.Length; i++)
                {
                    if (err[i].ContainsKey(nx[i]))
                        err[i][nx[i]] = err[i][nx[i]] + 1;
                    else
                        err[i][nx[i]] = 1;
                }
                return err;
            });

            string part1 = new string(errchecked.Select(dict => dict.OrderByDescending(kv => kv.Value).First().Key).ToArray());
            Console.WriteLine("==== Part 1 ====");
            Console.WriteLine($"Error checked message: {part1}");

            string part2 = new string(errchecked.Select(dict => dict.OrderBy(kv => kv.Value).First().Key).ToArray());
            Console.WriteLine("==== Part 2 ====");
            Console.WriteLine($"Error checked message: {part2}");
        }
    }
}
