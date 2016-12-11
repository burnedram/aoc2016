using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aoc2016
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            day11.Solution.Run();
#else
            switch (args.FirstOrDefault()?.ToLower() ?? "")
            {
                case "day01":
                    day01.Solution.Run();
                    break;
                case "day02":
                    day02.Solution.Run();
                    break;
                case "day03":
                    day03.Solution.Run();
                    break;
                case "day04":
                    day04.Solution.Run();
                    break;
                case "day05":
                    day05.Solution.Run();
                    break;
                case "day06":
                    day06.Solution.Run();
                    break;
                case "day07":
                    day07.Solution.Run();
                    break;
                case "day08":
                    day08.Solution.Run();
                    break;
                case "day09":
                    day09.Solution.Run();
                    break;
                case "day10":
                    day10.Solution.Run();
                    break;
                case "day11":
                    day11.Solution.Run();
                    break;
                default:
                    Console.WriteLine("Usage: aoc2016 <day>");
                    break;
            }
#endif
        }
    }
}
