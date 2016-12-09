using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace aoc2016.day08
{
    public class Solution
    {
        public static void Run()
        {
            string[] input = System.IO.File.ReadAllLines("day08.txt");

            LCD<bool> lcd = new LCD<bool>(50, 6);
            lcd.Formatter = val => val ? "#" : ".";
            lcd.FormatDelimiter = null;

            AnimateLCD(lcd, input, TimeSpan.FromMilliseconds(2500));
            Console.WriteLine();
            Console.WriteLine();

            int part1 = lcd.Count(val => val);
            Console.WriteLine("==== Part 1 ====");
            Console.WriteLine($"Lit pixels: {part1}");
            Console.WriteLine();

            Console.WriteLine("==== Part 2 ====");
            Console.WriteLine("Read from the LCD above");
        }

        private static void AnimateLCD(LCD<bool> lcd, string[] cmds, TimeSpan totalTime)
        {
            int cLeft = Console.CursorLeft;
            int cTop = Console.CursorTop;
            var start = DateTime.UtcNow;
            var end = start.Add(totalTime);
            int step = (int) ((end - start).TotalMilliseconds / cmds.Length);
            for (int i = 0; i < cmds.Length - 1; i++)
            {
                lcd.RunCommand(cmds[i], true);
                Console.SetCursorPosition(cLeft, cTop);
                Console.Write(lcd);
                Thread.Sleep(Math.Max(0, (int)(start.AddMilliseconds(step * i) - DateTime.UtcNow).TotalMilliseconds));
            }
            lcd.RunCommand(cmds[cmds.Length - 1], true);
            Console.SetCursorPosition(cLeft, cTop);
            Console.Write(lcd);
        }

        private class LCD<T> : IEnumerable<T>
        {

            public int Width { get; }
            public int Height { get; }
            private T[,] _lcd;
            public Func<T, string> Formatter { get; set; } = val => val.ToString();
            public char? FormatDelimiter = ' ';

            public LCD(int w, int h)
            {
                this.Width = w;
                this.Height = h;
                this._lcd = new T[w, h];
            }

            public T this[int i, int j]
            {
                get
                {
                    return _lcd[i, j];
                }
                set
                {
                    _lcd[i, j] = value;
                }
            }

            public void RunCommand(string cmd, T rectVal)
            {
                int space = cmd.IndexOf(' ');
                string args = cmd.Substring(space + 1);
                cmd = cmd.Substring(0, space);
                switch(cmd)
                {
                    case "rect":
                        string[] xy = args.Split('x');
                        Rect(int.Parse(xy[0]), int.Parse(xy[1]), rectVal);
                        break;
                    case "rotate":
                        string pattern = @"^(column|row) .=(\d+) by (\d+)$";
                        var matches = Regex.Match(args, pattern);
                        if (!matches.Success)
                            throw new ArgumentException("wrongly formatted \"rotate\"");
                        int rowcol = int.Parse(matches.Groups[2].Value);
                        int n = int.Parse(matches.Groups[3].Value);
                        if (matches.Groups[1].Value == "column")
                            RotateColumn(rowcol, n);
                        else
                            RotateRow(rowcol, n);
                        break;
                }
            }

            public void Rect(int A, int B, T val)
            {
                for (int i = 0; i < A; i++)
                    for (int j = 0; j < B; j++)
                        this[i, j] = val;
            }

            public void RotateRow(int A, int B)
            {
                B = B % Width;
                T[] vals = new T[B];
                for (int i = 0; i < B; i++)
                    vals[i] = this[i, A];
                for (int i = 0; i < Width; i++)
                {
                    T bak = this[(i + B) % Width, A];
                    this[(i + B) % Width, A] = vals[i % B];
                    vals[i % B] = bak;
                }
            }

            public void RotateColumn(int A, int B)
            {
                B = B % Height;
                T[] vals = new T[B];
                for (int i = 0; i < B; i++)
                    vals[i] = this[A, i];
                for (int i = 0; i < Height; i++)
                {
                    T bak = this[A, (i + B) % Height];
                    this[A, (i + B) % Height] = vals[i % B];
                    vals[i % B] = bak;
                }
            }

            public override string ToString()
            {
                return ToString(Formatter);
            }

            public string ToString(Func<T, string> formatter)
            {
                int maxWidth = Enumerable.Cast<T>(_lcd).Max(val => formatter(val).Length);
                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < Height; j++)
                {
                    for (int i = 0; i < Width - 1; i++)
                    {
                        sb.Append(formatter(this[i, j]).PadLeft(maxWidth));
                        if (FormatDelimiter != null)
                            sb.Append(FormatDelimiter);
                    }
                    sb.Append(formatter(this[Width - 1, j]).PadLeft(maxWidth));
                    sb.AppendLine();
                }
                sb.Length -= Environment.NewLine.Length; // Remove last newline
                return sb.ToString();
            }

            public IEnumerator<T> GetEnumerator()
            {
                foreach (var val in _lcd)
                    yield return val;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _lcd.GetEnumerator();
            }
        }
    }
}
