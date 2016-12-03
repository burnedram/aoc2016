using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aoc2016.day03
{
    public class Solution
    {
        public static void Run()
        {
            string[] input = System.IO.File.ReadAllLines("day03.txt");
            List<int[]> rows = input.Select(line => line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()).ToList();

            // Part 1
            var orderedRows = rows.Select(line => line.OrderBy(i => i));
            int part1 = orderedRows.Count(tri => tri.Take(2).Sum() > tri.Last());
            Console.WriteLine("==== Part 1 ====");
            Console.WriteLine($"Valid triangles: {part1}");

            // Part 2
            List<int[]> cols = new List<int[]>(rows.Count);
            for (int i = 0; i < rows.Count; i += 3)
                for (int j = 0; j < 3; j++)
                    cols.Add(new int[] { rows[i][j], rows[i + 1][j], rows[i + 2][j] });
            var orderedCols = cols.Select(line => line.OrderBy(i => i));
            int part2 = orderedCols.Count(tri => tri.Take(2).Sum() > tri.Last());
            Console.WriteLine("==== Part 2 ====");
            Console.WriteLine($"Valid triangles: {part2}");
        }
    }

    public static class IEnumerableEx
    {
        public static IEnumerable<T> TakeEvery<T>(this IEnumerable<T> ienum, int n)
        {
            while (ienum.Any())
            {
                int c = ienum.Count();
                yield return ienum.First();
                ienum = ienum.Skip(n);
            }
        }

        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> ienum, int n)
        {
            while(ienum.Any())
            {
                yield return ienum.Take(n);
                ienum = ienum.Skip(3);
            }
        }
    }
}
