using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace aoc2016.day09
{
    public class Solution
    {
        public static void Run()
        {
            string input = System.IO.File.ReadAllText("day09.txt");
            input = Regex.Replace(input, @"\s+", "");

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Length;)
            {
                int lb = input.IndexOf('(', i);
                if (lb < 0)
                {
                    sb.Append(input.Substring(i));
                    break;
                }
                sb.Append(input.Substring(i, lb - i));

                i = input.IndexOf(')', i);
                if (i < 0)
                {
                    sb.Append(input.Substring(lb));
                    break;
                }

                string[] xy = input.Substring(lb + 1, i - lb - 1).Split('x');
                int len = int.Parse(xy[0]);
                int n = int.Parse(xy[1]);
                i++;
                foreach (var repeat in Enumerable.Repeat(input.Substring(i, len), n))
                    sb.Append(repeat);
                i += len;
            }
            int part1 = sb.ToString().Length;
            Console.WriteLine("==== Part 1 ====");
            Console.WriteLine($"Decompressed length: {part1}");
            Console.WriteLine();

            Comp2 part2 = new Comp2(1, input);
            Console.WriteLine("==== Part 2 ====");
            Console.WriteLine($"Decompressed length: {part2.Length}");
        }

        private class Comp2
        {
            public bool IsCompressed { get; }
            public int Repeat { get; }
            public string String { get; }
            public List<Comp2> Parts { get; } = new List<Comp2>();
            public long Length { get; private set; }

            public Comp2(string str)
            {
                this.IsCompressed = false;
                this.Repeat = 1;
                this.String = str;
                this.Length = str.Length;
            }

            public Comp2(int repeat, string str)
            {
                this.IsCompressed = true;
                this.Repeat = repeat;
                this.String = str;
                Comp2 nextComp;
                do
                {
                    str = GetNextComp(str, out nextComp);
                    Parts.Add(nextComp);
                    Length += Repeat * nextComp.Length;
                }
                while (str.Length > 0);
            }

            public string GetNextComp(string str, out Comp2 comp)
            {
                int lb = str.IndexOf('(');
                if (lb < 0)
                {
                    comp = new Comp2(str);
                    return "";
                }
                else if (lb > 0)
                {
                    comp = new Comp2(str.Substring(0, lb));
                    return str.Substring(lb);
                }
                int rb = str.IndexOf(')', lb);
                if (rb < 0)
                {
                    comp = new Comp2(str);
                    return "";
                }

                string[] xy = str.Substring(lb + 1, rb - lb - 1).Split('x');
                int len = int.Parse(xy[0]);
                int repeat = int.Parse(xy[1]);
                var s = str.Substring(rb + 1, len);
                comp = new Comp2(repeat, s);
                var rest = str.Substring(rb + 1 + len);
                return rest;
            }

            public override string ToString()
            {
                return String;
            }
        }

    }
    
}
