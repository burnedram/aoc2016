using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace aoc2016.day10
{
    public class Solution
    {
        public static void Run()
        {
            string[] input = System.IO.File.ReadAllLines("day10.txt");

            Simulation sim = new Simulation();

            foreach (var inp in input)
                sim.AddInstruction(inp);

            sim.Init();
            sim.Run();

            Console.WriteLine("==== Part 1 ====");
            Console.WriteLine($"17/61 Bot: {sim.Part1.Name}");
            Console.WriteLine();

            Console.WriteLine("==== Part 2 ====");
            Console.WriteLine($"Output product: {sim.Part2.Cast<int>().Aggregate(1, (a, b) => a * b)}");
        }

        private class Simulation
        {

            public List<GoesTo> GoesTos { get; } = new List<GoesTo>();

            public Dictionary<string, SimObject> Objects { get; } = new Dictionary<string, SimObject>();

            public List<Bot> Bots { get; } = new List<Bot>();
            public List<Output> Outputs { get; } = new List<Output>();

            public Bot Part1 { get; set; }
            public int?[] Part2 { get; } = new int?[3];
            private bool run = false;

            public void Init()
            {
                foreach (var gt in GoesTos)
                    gt.Object.Chips.Add(gt.Chip);
            }

            public void RunOneTick()
            {
                foreach (var bot in Bots)
                    bot.Think();
                foreach (var bot in Bots)
                {
                    bot.Chips.UnionWith(bot.NextChips);
                    bot.NextChips.Clear();
                }
            }

            public void Run()
            {
                run = true;
                while (run)
                    RunOneTick();
            }

            public void TryStop()
            {
                if (Part1 != null && Part2.All(chip => chip != null))
                    run = false;
            }

            public void AddInstruction(string cmd)
            {
                if (cmd.StartsWith("value"))
                {
                    string pattern = @"^value (\d+) goes to (.+)$";
                    var match = Regex.Match(cmd, pattern);
                    if (!match.Success)
                        throw new ArgumentException("unable to parse GoesTo");
                    int chip = int.Parse(match.Groups[1].Value);
                    string name = match.Groups[2].Value;
                    GoesTos.Add(new GoesTo(chip, AddOrGetObject(name)));
                }
                else if (cmd.StartsWith("bot"))
                {
                    string pattern = @"^(bot \d+) gives low to (.+) and high to (.+)$";
                    var match = Regex.Match(cmd, pattern);
                    if (!match.Success)
                        throw new ArgumentException("unable to parse GivesLowHigh");
                    string fromName = match.Groups[1].Value;
                    string lowName = match.Groups[2].Value;
                    string highName = match.Groups[3].Value;
                    Bot from = AddOrGetObject(fromName) as Bot;
                    from.Instructions.Add(new GivesLowHigh(from, AddOrGetObject(lowName), AddOrGetObject(highName)));
                }
                else
                    throw new ArgumentException("unknown cmd");
            }

            public SimObject AddOrGetObject(string name)
            {
                SimObject obj;
                if (Objects.ContainsKey(name))
                    obj = Objects[name];
                else
                {
                    if (name.StartsWith("bot"))
                    {
                        obj = new Bot(this, name);
                        Bots.Add(obj as Bot);
                    }
                    else if (name.StartsWith("output"))
                    {
                        obj = new Output(this, name);
                        Outputs.Add(obj as Output);
                    }
                    else
                        throw new ArgumentException("unknown name");
                    Objects[name] = obj;
                }
                return obj;
            }

        }

        private class SimObject
        {
            public Simulation Sim { get; }
            public HashSet<int> Chips { get; } = new HashSet<int>();
            public HashSet<int> NextChips { get; } = new HashSet<int>();
            public string Name { get; }

            public SimObject(Simulation sim, string name)
            {
                this.Sim = sim;
                this.Name = name;
            }

            public virtual void AddChip(int chip)
            {
                NextChips.Add(chip);
            }

            public virtual void Think()
            {
            }

            public override string ToString()
            {
                return $"{Name} [{Chips.Count}]";
            }
        }

        private class Output : SimObject
        {
            public Output(Simulation sim, string name) : base(sim, name)
            {
            }

            public override void AddChip(int chip)
            {
                base.AddChip(chip);
                if (Name == "output 0" || Name == "output 1" || Name == "output 2")
                {
                    int nr = Name.Last() - '0';
                    if (Sim.Part2[nr] == null)
                    {
                        Sim.Part2[nr] = chip;
                        Sim.TryStop();
                    }
                }
            }
        }

        private class Bot : SimObject
        {
            public List<BotInstruction> Instructions { get; } = new List<BotInstruction>();

            public Bot(Simulation sim, string name) : base(sim, name)
            {
            }
            
            public override void Think()
            {
                foreach (var ins in Instructions)
                    if (ins.ShouldExecute())
                        ins.Execute();
            }
        }

        private class GoesTo
        {
            public int Chip { get; }
            public SimObject Object { get; }

            public GoesTo(int chip, SimObject obj)
            {
                this.Chip = chip;
                this.Object = obj;
            }
        }

        private interface BotInstruction
        {
            Bot Parent { get; }
            bool ShouldExecute();
            void Execute();
        }

        private class GivesLowHigh : BotInstruction
        {
            public Bot Parent { get; }
            public SimObject Low { get; }
            public SimObject High { get; }

            public GivesLowHigh(Bot from, SimObject low, SimObject high)
            {
                this.Parent = from;
                this.Low = low;
                this.High = high;
            }

            public bool ShouldExecute()
            {
                return Parent.Chips.Count >= 2;
            }

            public void Execute()
            {
                int low = Parent.Chips.Min();
                Parent.Chips.Remove(low);
                Low.AddChip(low);

                int high = Parent.Chips.Max();
                Parent.Chips.Remove(high);
                High.AddChip(high);

                if (low == 17 && high == 61)
                {
                    Parent.Sim.Part1 = Parent;
                    Parent.Sim.TryStop();
                }
            }
        }

    }

    public static class EnumerableExt {
        public static IEnumerable<T> From<T>(T val)
        {
            yield return val;
        }
    }

}
