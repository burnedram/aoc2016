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
            day03.Solution.Run();
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
                default:
                    Console.WriteLine("Usage: aoc2016 <day>");
                    break;
            }
#endif
        }
    }
}
