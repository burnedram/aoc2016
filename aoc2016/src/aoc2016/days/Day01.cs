using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aoc2016.day01
{

    public class Solution
    {

        private static readonly string INPUT =
        #region 
            "R5, R4, R2, L3, R1, R1, L4, L5, R3, L1, L1, R4, L2, R1, R4, R4, L2, L2, R4, L4, R1, R3, L3, L1, L2, R1, R5, L5, L1, L1, R3, R5, L1, R4, L5, R5, R1, L185, R4, L1, R51, R3, L2, R78, R1, L4, R188, R1, L5, R5, R2, R3, L5, R3, R4, L1, R2, R2, L4, L4, L5, R5, R4, L4, R2, L5, R2, L1, L4, R4, L4, R2, L3, L4, R2, L3, R3, R2, L2, L3, R4, R3, R1, L4, L2, L5, R4, R4, L1, R1, L5, L1, R3, R1, L2, R1, R1, R3, L4, L1, L3, R2, R4, R2, L2, R1, L5, R3, L3, R3, L1, R4, L3, L3, R4, L2, L1, L3, R2, R3, L2, L1, R4, L3, L5, L2, L4, R1, L4, L4, R3, R5, L4, L1, L1, R4, L2, R5, R1, R1, R2, R1, R5, L1, L3, L5, R2";
            #endregion

        public static void Run()
        {
            string[] strs = INPUT.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            List<Walk> walks = strs.Aggregate(new List<Walk>(strs.Length), (ws, str) =>
            {
                ws.Add(new Walk(ws.LastOrDefault(), str));
                return ws;
            });

            // Part 1
            Dist2 part1 = walks.Aggregate(new Dist2 { x = 0, y = 0 }, (d2, w) => d2.Move(w));
            Console.WriteLine("==== Part 1 ====");
            Console.WriteLine($"X: {part1.x}");
            Console.WriteLine($"Y: {part1.y}");
            Console.WriteLine($"Total distance: {Math.Abs(part1.x) + Math.Abs(part1.y)}");

            // Part 2
            Dist2 part2 = Part2(walks);
            Console.WriteLine("==== Part 2 ====");
            Console.WriteLine($"X: {part2.x}");
            Console.WriteLine($"Y: {part2.y}");
            Console.WriteLine($"Total distance: {Math.Abs(part2.x) + Math.Abs(part2.y)}");
        }

        private static Dist2 Part2(List<Walk> walks)
        {
            HashSet<Dist2> visited = new HashSet<Dist2>();
            Dist2 part2 = new Dist2 { x = 0, y = 0 };
            foreach (var walk in walks)
            {
                foreach (var i in Enumerable.Range(0, walk.steps))
                if (!visited.Add(part2 = part2 + walk.dir))
                    return part2;
            }
            return part2;
        }

        private struct Dist2
        {
            public int x, y;

            public static Dist2 operator +(Dist2 d2, Directions dir)
            {
                Dist2 d22 = new Dist2 { x = d2.x, y = d2.y };
                switch (dir)
                {
                    case Directions.North:
                        d22.y--;
                        return d22;
                    case Directions.East:
                        d22.x++;
                        return d22;
                    case Directions.South:
                        d22.y++;
                        return d22;
                    case Directions.West:
                        d22.x--;
                        return d22;
                    default:
                        throw new NotImplementedException();
                }
            }

            public Dist2 Move(Walk w)
            {
                Dist2 d2 = new Dist2 { x = x, y = y };
                switch (w.dir)
                {
                    case Directions.North:
                        d2.y -= w.steps;
                        return d2;
                    case Directions.East:
                        d2.x += w.steps;
                        return d2;
                    case Directions.South:
                        d2.y += w.steps;
                        return d2;
                    case Directions.West:
                        d2.x -= w.steps;
                        return d2;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private class Walk {

            public Directions dir { get; }
            public int steps { get; }

            public Walk(Walk prev, string walk)
            {
                switch (walk[0])
                {
                    case 'R':
                        this.dir = prev?.dir.Right() ?? Directions.East; // We begin facing north
                        break;
                    case 'L':
                        this.dir = prev?.dir.Left() ?? Directions.West; // We begin facing north
                        break;
                    default:
                        throw new NotImplementedException();
                }
                this.steps = int.Parse(walk.Substring(1));
            }

        }

    }

    enum Directions
    {
        North, East, South, West
    }

    static class DirectionsExt
    {
        public static Directions Left(this Directions dir)
        {
            switch (dir)
            {
                case Directions.North:
                    return Directions.West;
                case Directions.West:
                    return Directions.South;
                case Directions.South:
                    return Directions.East;
                case Directions.East:
                    return Directions.North;
                default:
                    throw new NotImplementedException();
            }
        }

        public static Directions Right(this Directions dir)
        {
            switch (dir)
            {
                case Directions.North:
                    return Directions.East;
                case Directions.East:
                    return Directions.South;
                case Directions.South:
                    return Directions.West;
                case Directions.West:
                    return Directions.North;
                default:
                    throw new NotImplementedException();
            }
        }

    }

}
