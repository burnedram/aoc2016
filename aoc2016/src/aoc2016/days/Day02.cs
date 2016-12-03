using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aoc2016.day02
{
    public class Solution
    {
        public static void Run()
        {
            string[] input = System.IO.File.ReadAllLines("day02.txt");

            // Part 1
            Keypad keypad1 = new Keypad("5",
@"1 2 3
  4 5 6
  7 8 9");
            string part1 = keypad1.GetCode(input);
            Console.WriteLine("==== Part 1 ====");
            Console.WriteLine($"Code: {part1}");

            // Part 2
            Keypad keypad2 = new Keypad("5",
@"_ _ 1 _ _
  _ 2 3 4 _
  5 6 7 8 9
  _ A B C _
  _ _ D _ _");
            string part2 = keypad2.GetCode(input);
            Console.WriteLine("==== Part 2 ====");
            Console.WriteLine($"Code: {part2}");
        }

        private struct Pos
        {
            public int x, y;

            public Pos Up()
            {
                this.y--;
                return this;
            }

            public Pos Down()
            {
                this.y++;
                return this;
            }

            public Pos Right()
            {
                this.x++;
                return this;
            }

            public Pos Left()
            {
                this.x--;
                return this;
            }
        }

        private class Keypad
        {

            private int _nRows, _nCols;
            private string[,] _pad;
            public Pos start { get; }
            public Pos pos { get; private set; }

            public Keypad(string startButton, string pad)
            {
                string[] rows = pad.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                _nRows = rows.Length;
                _nCols = rows[0].Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Length;
                _pad = new string[_nCols, _nRows];
                for (int row = 0; row < rows.Length; row++)
                {
                    string[] cols = rows[row].Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                    for (int col = 0; col < cols.Length; col++)
                    {
                        if (cols[col] == "_")
                            continue;
                        _pad[col, row] = cols[col];
                        if (cols[col] == startButton)
                            start = new Pos { x = col, y = row };
                    }
                }
            }

            public string GetCode(string[] dirs)
            {
                pos = start;
                string code = "";
                foreach (var line in dirs)
                {
                    foreach (var dir in line)
                        Move(dir);
                    code += Current();
                }
                return code;
            }

            public void Move(char dir)
            {
                switch (dir)
                {
                    case 'U':
                        if (pos.y > 0)
                        {
                            pos = pos.Up();
                            if (Current() == null)
                                pos = pos.Down();
                        }
                        break;
                    case 'D':
                        if (pos.y < _nRows - 1)
                        {
                            pos = pos.Down();
                            if (Current() == null)
                                pos = pos.Up();
                        }
                        break;
                    case 'L':
                        if (pos.x > 0)
                        {
                            pos = pos.Left();
                            if (Current() == null)
                                pos = pos.Right();
                        }
                        break;
                    case 'R':
                        if (pos.x < _nCols - 1)
                        {
                            pos = pos.Right();
                            if (Current() == null)
                                pos = pos.Left();
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            public string Current()
            {
                return _pad[pos.x, pos.y];
            }
        }

    }

}
