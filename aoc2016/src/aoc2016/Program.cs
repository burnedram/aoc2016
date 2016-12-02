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
            day01.Solution.Run();
#else
            switch (args[0].ToLower())
            {
                case "day01":
                    day01.Solution.Run();
                    break;
            }
#endif
        }
    }
}
