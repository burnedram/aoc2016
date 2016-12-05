using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace aoc2016.day05
{
    public class Solution
    {

        private static readonly string DoorID = "abbhdwsy";

        public static void Run()
        {
            long worksize = 500000;
            int nZeros = 5;
            int passwordLength = 8;

            Console.WriteLine("==== Part 1 ====");
            Console.Write($"Cracking... ");
            string part1 = Part1(worksize, nZeros, passwordLength);
            Console.WriteLine();
            Console.WriteLine($"Password: {part1}");

            Console.WriteLine("==== Part 2 ====");
            Console.Write($"Cracking... ");
            string part2 = Part2(worksize, nZeros, passwordLength);
            Console.WriteLine();
            Console.WriteLine($"Password: {part2}");
        }

        // Display live hacking of part 1
        private static void HackingPart1(ref bool done, int passwordLength, SortedList<long, string> hashes)
        {
            int cLeft = Console.CursorLeft, cTop = Console.CursorTop;
            while (!done)
            {
                lock (hashes)
                {
                    string hacking = new string(hashes.Take(passwordLength).Select(hash => hash.Value[5]).ToArray());
                    Console.SetCursorPosition(cLeft, cTop);
                    Console.Write($"{hacking.PadRight(passwordLength, '_')}");
                    Monitor.Wait(hashes);
                }
            }
        }

        // Calculate part 1 in parallel
        private static string Part1(long worksize, int nZeros, int passwordLength)
        {
            SortedList<long, string> hashes = new SortedList<long, string>();
            int hashesFound = 0;
            long largestIndex = 0;
            string zeros = new string('0', nZeros);
            bool done = false;

            Thread hacking = new Thread(() => HackingPart1(ref done, passwordLength, hashes));
            hacking.Start();

            Parallel.ForEach(MD5Ext.YieldWhile(() => hashesFound < passwordLength, i => Tuple.Create(i * worksize, (i + 1) * worksize)), bound =>
            {
                MD5 md5 = MD5.Create();
                long lowerbound = bound.Item1, upperbound = bound.Item2;
                for (long index = lowerbound; index < upperbound && (hashesFound < passwordLength || index < Interlocked.Read(ref largestIndex)); index++)
                {
                    string hash = md5.ComputeHash(Encoding.ASCII.GetBytes(DoorID + index)).MD5String();
                    if (hash.StartsWith(zeros))
                    {
                        lock (hashes)
                        {
                            hashes.Add(index - 1, hash);
                            Monitor.Pulse(hashes);
                            if (index - 1 > largestIndex)
                                largestIndex = index - 1;
                            Interlocked.Increment(ref hashesFound);
                        }
                    }
                }
            });
            lock(hashes)
            {
                done = true;
                Monitor.Pulse(hashes);
            }
            hacking.Join();
            return new string(hashes.Take(passwordLength).Select(hash => hash.Value[5]).ToArray());
        }

        // Display live hacking of part 2
        private static void HackingPart2(ref bool done, int passwordLength, Dictionary<int, Tuple<long, string>> hashes)
        {
            int cLeft = Console.CursorLeft, cTop = Console.CursorTop;
            while (!done)
            {
                lock (hashes)
                {
                    StringBuilder hacking = new StringBuilder(new string('_', passwordLength));
                    foreach (var hash in hashes)
                        hacking[hash.Key] = hash.Value.Item2[6];
                    Console.SetCursorPosition(cLeft, cTop);
                    Console.Write($"{hacking.ToString().PadRight(passwordLength, '_')}");
                    Monitor.Wait(hashes);
                }
            }
        }

        private static string Part2(long worksize, int nZeros, int passwordLength)
        {
            Dictionary<int, Tuple<long, string>> hashes = new Dictionary<int, Tuple<long, string>>();
            int hashesFound = 0;
            long largestIndex = 0;
            string zeros = new string('0', nZeros);
            bool done = false;

            Thread hacking = new Thread(() => HackingPart2(ref done, passwordLength, hashes));
            hacking.Start();

            Parallel.ForEach(MD5Ext.YieldWhile(() => hashesFound < passwordLength, i => Tuple.Create(i * worksize, (i + 1) * worksize)), bound =>
            {
                MD5 md5 = MD5.Create();
                long lowerbound = bound.Item1, upperbound = bound.Item2;
                for (long index = lowerbound; index < upperbound && (hashesFound < passwordLength || index < Interlocked.Read(ref largestIndex)); index++)
                {
                    string hash = md5.ComputeHash(Encoding.ASCII.GetBytes(DoorID + index)).MD5String();
                    if (hash.StartsWith(zeros))
                    {
                        int key = Convert.ToInt32(hash.Substring(5, 1), 16);
                        if (key < passwordLength)
                        {
                            lock (hashes)
                            {
                                bool contains = hashes.ContainsKey(key);
                                if (!contains || hashes[key].Item1 > index - 1)
                                {
                                    hashes[key] = Tuple.Create(index - 1, hash);
                                    Monitor.Pulse(hashes);
                                    if (index - 1 > Interlocked.Read(ref largestIndex))
                                        Interlocked.Exchange(ref largestIndex, index - 1);
                                    if (!contains)
                                        Interlocked.Increment(ref hashesFound);
                                }
                            }
                        }
                    }
                }
            });
            lock(hashes)
            {
                done = true;
                Monitor.Pulse(hashes);
            }
            hacking.Join();
            return new string(hashes.Take(passwordLength).OrderBy(hash => hash.Key).Select(hash => hash.Value.Item2[6]).ToArray());
        }
    }

    public static class MD5Ext
    {

        public static IEnumerable<T> YieldWhile<T>(Func<bool> pred, Func<int, T> func)
        {
            for (int i = 0; pred(); i++)
                yield return func(i);
        }

        public static string MD5String(this byte[] md5)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var b in md5)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }
    }

}
