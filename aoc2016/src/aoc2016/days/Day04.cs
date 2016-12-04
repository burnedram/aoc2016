using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aoc2016.day04
{
    public class Solution
    {
        public static void Run()
        {
            string[] input = System.IO.File.ReadAllLines("day04.txt");
            List<Room> rooms = input.Select(r => new Room(r)).ToList();
            List<Room> validRooms = rooms.Where(r => r.Checksum == r.ActualChecksum).ToList();

            // Part 1
            int part1 = validRooms.Sum(r => r.ID);
            Console.WriteLine("==== Part 1 ====");
            Console.WriteLine($"Sum of IDs: {part1}");

            // Part 2
            List<string> keywords = new List<string> { "north", "pole" };
            Console.WriteLine();
            Console.WriteLine("==== Part 2 ====");
            Console.WriteLine($"Searching for rooms containing these keywords: {string.Join(", ", keywords)}");
            bool foundAny = false;
            foreach (var r in validRooms)
            {
                if (keywords.Any(k => r.ActualName.Contains(k)))
                {
                    Console.WriteLine($"  {r.ID.ToString().PadLeft(5)}: {r.ActualName}");
                    foundAny = true;
                }
            }
            if (!foundAny)
                Console.WriteLine("No matches found");
        }

        private class Room
        {
            public string Name { get; }
            public string NameNoDashes { get; }
            private string _actualName;
            public string ActualName {
                get
                {
                    return _actualName ?? (_actualName = CrackName());
                }
            }
            public int ID { get; }
            public string Checksum { get; }
            public string ActualChecksum { get; }

            public Room(string str)
            {
                int dash = str.LastIndexOf('-');
                Name = str.Substring(0, dash);
                NameNoDashes = Name.Replace("-", "");
                str = str.Substring(dash);
                int bracket = str.IndexOf('[');
                ID = int.Parse(str.Substring(1, bracket - 1));
                str = str.Substring(bracket);
                Checksum = str.Substring(1, str.Length - 2);
                ActualChecksum = CalculateChecksum();
            }

            private string CalculateChecksum()
            {
                char[] counts = NameNoDashes.GroupBy(c => c)
                    .Select(c => Tuple.Create(c.Key, c.Count()))
                    .OrderByDescending(c => c.Item2)
                    .ThenBy(c => c.Item1)
                    .Take(Checksum.Length)
                    .Select(c => c.Item1).ToArray();
                return new string(counts);
            }

            private string CrackName()
            {
                return new string(Name.Select(c =>
                {
                    if (c == '-')
                        return ' ';
                    c = (char)('a' + (c - 'a' + ID) % ('z' - 'a' + 1));
                    return c;
                }).ToArray());
            }
        }
    }
}
