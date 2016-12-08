using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aoc2016.day07
{
    public class Solution
    {
        public static void Run()
        {
            string[] input = System.IO.File.ReadAllLines("day07.txt");
            List<IP7> ips = input.Select(ip => new IP7(ip)).ToList();

            // Part 1
            int part1 = ips.Count(ip => ip.SupportsTLS());
            Console.WriteLine("==== Part 1 ====");
            Console.WriteLine($"Answer: {part1}");

            // Part 2
            int part2 = ips.Count(ip => ip.SupportsSSL());
            Console.WriteLine("==== Part 2 ====");
            Console.WriteLine($"Answer: {part2}");
        }

        private class IP7
        {

            public static bool HasABBA(string net)
            {
                for (int i = 0; i < net.Length - 3; i++)
                    if (net[i] != net[i + 1] && net[i] == net[i + 3] && net[i + 1] == net[i + 2])
                        return true;
                return false;
            }

            public static IEnumerable<string> GetABAs(string net)
            {
                for (int i = 0; i < net.Length - 2; i++)
                    if (net[i] != net[i + 1] && net[i] == net[i + 2])
                        yield return net.Substring(i, 3);
            }

            public static string AbaToBab(string aba)
            {
                return new string(new char[] { aba[1], aba[0], aba[1] });
            }

            public string Full { get; }
            public List<string> Supernets { get; }
            public List<string> Hypernets { get; }

            public IP7(string ip7)
            {
                this.Full = ip7;
                string[] split = ip7.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                List<string>[] nets = new List<string>[] { new List<string>(), new List<string>() };
                int offset = ip7.StartsWith("[") ? 1 : 0;
                for (int i = 0; i < split.Length; i++)
                    nets[(i + offset) % 2].Add(split[i]);
                Supernets = nets[0];
                Hypernets = nets[1];
            }

            public bool SupportsTLS()
            {
                return !Hypernets.Any(HasABBA) && Supernets.Any(HasABBA);
            }

            public bool SupportsSSL()
            {
                HashSet<string> abas = new HashSet<string>(Supernets.SelectMany(GetABAs));
                HashSet<string> babs = new HashSet<string>(Hypernets.SelectMany(GetABAs));
                return abas.Any(aba => babs.Contains(AbaToBab(aba)));
            }

        }

    }
}
