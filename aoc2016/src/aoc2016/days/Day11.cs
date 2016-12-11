using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace aoc2016.day11
{
    public class Solution
    {
        public static void Run()
        {
            string[] input = System.IO.File.ReadAllLines("day11.txt");
            Facility fac = new Facility(input);

            Console.WriteLine(fac.UserManual());
            Console.WriteLine();

            int cLeft = Console.CursorLeft;
            int cTop = Console.CursorTop;
            while (true)
            {
                Console.SetCursorPosition(cLeft, cTop);
                fac.UserInteract();
            }
        }

        private class Facility
        {
            public SortedSet<Element> Elements { get; } = new SortedSet<Element>();
            public List<Floor> Floors { get; } = new List<Floor>();
            public Elevator Elevator { get; }
            public int HandLevelPosition { get; private set; } = 0;
            public int HandElevatorPosition { get; private set; } = 0;
            public bool HandInElevator { get; private set; } = false;
            public string HandStatus { get; private set; }

            public int MaxSymbolLength { get; }
            public int MaxLevelLength { get; }

            public Facility(string[] floors)
            {
                Elevator = new Elevator(this, 2);

                // Assume that the floors are in order, low to high
                foreach (var floorStr in floors)
                {
                    if(floorStr.EndsWith("nothing relevant."))
                    {
                        Floors.Add(new Floor(this, Floors.Count));
                        continue;
                    }
                    string pattern = @"an? (.+?)( generator|-compatible microchip)";
                    var matches = Regex.Matches(floorStr, pattern);
                    if (matches.Count == 0)
                        throw new ArgumentException("cant parse floor");
                    Floor floor = new Floor(this, Floors.Count);
                    foreach (var match in matches.Cast<Match>())
                    {
                        Element ele = Element.FromName(match.Groups[1].Value);
                        Elements.Add(ele);
                        ItemType iType = match.Groups[2].Value == " generator" ? ItemType.GENERATOR : ItemType.MICROCHIP;
                        floor.Items[iType].Add(ele);
                    }
                    Floors.Add(floor);
                }

                MaxSymbolLength = Elements.Max(ele => ele.Symbol.Length);
                MaxLevelLength = Floors.Max(floor => floor.Level.ToString().Length);
            }

            public bool CheckState()
            {
                foreach (var floor in Floors)
                {
                    HashSet<Element> rtgs = new HashSet<Element>(floor.Items[ItemType.GENERATOR]);
                    if (floor.HasElevator())
                        rtgs.UnionWith(Elevator.Items[ItemType.GENERATOR]);
                    if (!rtgs.Any())
                        continue;

                    HashSet<Element> chips = new HashSet<Element>(floor.Items[ItemType.MICROCHIP]);
                    if (floor.HasElevator())
                        chips.UnionWith(Elevator.Items[ItemType.MICROCHIP]);

                    chips.ExceptWith(rtgs);
                    if (chips.Any())
                        return false;
                }
                return true;
            }

            public string UserManual()
            {
                return 
@"Move the hand with <LEFT> and <RIGHT> arrow keys.
Pickup an item with <SPACE>.
Switch between the current floor and the elevator with <ENTER>.
Move the elevator with the <UP> and <DOWN> arrow keys.
Your goal is to move all RTGs (G) and microchips (M) to the top floor,
  without frying any components.";
            }

            public void UserInteract()
            {
                Console.WriteLine(DrawFacility());
                int cTop = Console.CursorTop;
                Console.SetCursorPosition(0, cTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, cTop + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, cTop + 2);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, cTop + 3);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, cTop);
                Console.WriteLine(DrawHand());
                Console.WriteLine(GetStatus());
                if (HandStatus != null)
                {
                    Console.WriteLine(HandStatus);
                    HandStatus = null;
                }
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (HandInElevator)
                            HandElevatorPosition = Math.Max(0, HandElevatorPosition - 1);
                        else
                            HandLevelPosition = Math.Max(0, HandLevelPosition - 1);
                        break;
                    case ConsoleKey.RightArrow:
                        if (HandInElevator)
                            HandElevatorPosition = Math.Min(Elevator.GetItems().Count() - 1, HandElevatorPosition + 1);
                        else
                            HandLevelPosition = Math.Min(Elements.Count * 2 - 1, HandLevelPosition + 1);
                        break;
                    case ConsoleKey.UpArrow:
                        if (Elevator.IsOnTopFloor())
                            break;
                        if (Elevator.IsEmpty())
                        {
                            HandStatus = "The elevator can't move if it is empty";
                            break;
                        }
                        Elevator.MoveUp();
                        if (!CheckState())
                        {
                            HandStatus = "That would have fried a microchip! Be more careful next time...";
                            Elevator.MoveDown();
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (Elevator.IsOnBottomFloor())
                            break;
                        if (Elevator.IsEmpty())
                        {
                            HandStatus = "The elevator can't move if it is empty";
                            break;
                        }
                        Elevator.MoveDown();
                        if (!CheckState())
                        {
                            HandStatus = "That would have fried a microchip! Be more careful next time...";
                            Elevator.MoveUp();
                        }
                        break;
                    case ConsoleKey.Spacebar:
                        if (HandInElevator)
                        {
                            var item = Elevator.GetItem(HandElevatorPosition);
                            Elevator.UnloadIntemOntoFloor(item.Item1, item.Item2);
                            if (Elevator.IsEmpty())
                            {
                                HandElevatorPosition = 0;
                                HandInElevator = false;
                            }
                            else
                                HandElevatorPosition = Math.Min(Elevator.GetItems().Count() - 1, HandElevatorPosition);
                        }
                        else
                        {
                            var item = Elevator.CurrentFloor.GetItem(HandLevelPosition);
                            if (item == null)
                                break;
                            if (Elevator.IsFull())
                            {
                                HandStatus = "The elevator is already full";
                                break;
                            }
                            Elevator.CurrentFloor.LoadItemIntoElevator(item.Item1, item.Item2);
                        }
                        break;
                    case ConsoleKey.Enter:
                        if (!HandInElevator && Elevator.IsEmpty())
                            break;
                        HandInElevator = !HandInElevator;
                        break;
                }
            }
            
            public string DrawFacility()
            {
                return $"{string.Join("\n", Floors.Reverse<Floor>())}";
            }

            public string DrawHand()
            {
                if (HandInElevator)
                    return $"{new string(' ', "Elevator contains: ".Length)}{new string(' ', (MaxSymbolLength + 1 + 1) * HandElevatorPosition)}{new string('v', MaxSymbolLength + 1)}";
                else
                    return $"{new string(' ', MaxLevelLength + 1 + 3)}{new string(' ', (MaxSymbolLength + 1 + 1) * HandLevelPosition)}{new string('^', MaxSymbolLength + 1)}";
            }

            public string GetStatus()
            {
                string elevatorString = Elevator.IsEmpty() ? "Elevator is empty" : $"Elevator contains: {Elevator}";
                string stateCheck = CheckState() ? "" : "\n!!!Invalid state!!!";
                return $"{elevatorString}{stateCheck}";
            }

            public override string ToString()
            {
                return $"{DrawFacility()}\n\n{GetStatus()}";
            }

        }

        private class Container
        {
            public Facility Fac { get; }
            public SortedDictionary<ItemType, HashSet<Element>> Items { get; } = new SortedDictionary<ItemType, HashSet<Element>>
            {
                { ItemType.GENERATOR, new HashSet<Element>() },
                { ItemType.MICROCHIP, new HashSet<Element>() }
            };
            public int MaxItems { get; }

            public Container(Facility fac) : this(fac, 0)
            {
            }

            public Container(Facility fac, int maxItems)
            {
                this.Fac = fac;
                this.MaxItems = maxItems;
            }

            public bool IsEmpty()
            {
                return Items.Sum(items => items.Value.Count) <= 0;
            }

            public bool IsFull()
            {
                return MaxItems > 0 && Items.Sum(items => items.Value.Count) >= MaxItems;
            }

            public virtual IEnumerable<Tuple<ItemType, Element>> GetItems()
            {
                return Items.Select(kv => kv.Value.Select(ele => Tuple.Create(kv.Key, ele))).Interleave();
            }

            public Tuple<ItemType, Element> GetItem(int index)
            {
                return GetItems().Skip(index).First();
            }

            protected void MoveItemTo(Container that, ItemType iType, Element ele)
            {
                if (!Items[iType].Contains(ele))
                    throw new InvalidOperationException("no such item");
                if (that.IsFull())
                    throw new InvalidOperationException("full");
                Items[iType].Remove(ele);
                that.Items[iType].Add(ele);
            }

            public override string ToString()
            {
                var allItems = GetItems().Select(item => $"{item.Item2.Symbol}{item.Item1.Suffix()}");
                return string.Join(" ", allItems.Select(ele => ele.PadCenter(Fac.MaxSymbolLength + 1)));
            }
        }

        private class Elevator : Container
        {
            public int Level { get; private set; } = 0;
            public Floor CurrentFloor
            {
                get
                {
                    return Fac.Floors[Level];
                }
            }

            public Elevator(Facility fac, int maxItems) : base(fac, maxItems)
            {
            }

            public bool IsOnTopFloor()
            {
                return Level >= Fac.Floors.Count - 1;
            }

            public bool IsOnBottomFloor()
            {
                return Level <= 0;
            }

            public void MoveUp()
            {
                if (IsEmpty())
                    throw new InvalidOperationException("elevator empty");
                if (IsOnTopFloor())
                    throw new InvalidOperationException("already on top floor");
                Level++;
            }
            
            public void MoveDown()
            {
                if (IsEmpty())
                    throw new InvalidOperationException("elevator empty");
                if (IsOnBottomFloor())
                    throw new InvalidOperationException("already on bottom floor");
                Level--;
            }

            public void UnloadIntemOntoFloor(ItemType iType, Element ele)
            {
                base.MoveItemTo(CurrentFloor, iType, ele);
            }

        }

        private class Floor : Container
        {
            public int Level { get; }

            public Floor(Facility fac, int level) : base(fac)
            {
                this.Level = level;
            }

            public bool HasElevator()
            {
                return Fac.Elevator.Level == Level;
            }

            public void LoadItemIntoElevator(ItemType iType, Element ele)
            {
                if (!HasElevator())
                    throw new InvalidOperationException("elevator is on another floor");
                base.MoveItemTo(Fac.Elevator, iType, ele);
            }

            public override IEnumerable<Tuple<ItemType, Element>> GetItems()
            {
                return Items.Select(kv => Fac.Elements.Select(ele => kv.Value.Contains(ele) ? Tuple.Create(kv.Key, ele) : null)).Interleave();
            }

            public override string ToString()
            {
                var allItems = GetItems().Select(item => item != null ? $"{item.Item2.Symbol}{item.Item1.Suffix()}" : new string('.', Fac.MaxSymbolLength + 1));
                string items = string.Join(" ", allItems.Select(ele => ele.PadCenter(Fac.MaxSymbolLength + 1)));
                return $"F{(Level + 1).ToString().PadLeft(Fac.MaxLevelLength)} {(HasElevator() ? 'E' : '.')} {items}";
            }

        }

        private struct Element : IComparable<Element>
        {
            private static ImmutableDictionary<string, Element> ParseElements()
            {
                string[] input = System.IO.File.ReadAllLines("elementlist.csv");
                Dictionary<string, Element> dict = new Dictionary<string, Element>();
                foreach (var ele in input)
                {
                    string[] split = ele.Split(',');
                    dict[split[2].ToLowerInvariant()] = new Element(int.Parse(split[0]), split[2], split[1]);
                }
                return dict.ToImmutableDictionary();
            }

            public static Element FromName(string name)
            {
                return ELEMENTS[name.ToLowerInvariant()];
            }

            private static ImmutableDictionary<string, Element> ELEMENTS = ParseElements();

            public int AtomicNumber { get; }
            public string Name { get; }
            public string Symbol { get; }

            private Element(int number, string name, string symbol)
            {
                this.AtomicNumber = number;
                this.Name = name;
                this.Symbol = symbol;
            }

            public static bool operator ==(Element e1, Element e2)
            {
                if (object.ReferenceEquals(e1, null))
                    return object.ReferenceEquals(e1, e2);
                return e1.AtomicNumber == e2.AtomicNumber;
            }

            public static bool operator !=(Element e1, Element e2)
            {
                return !(e1 == e2);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(obj, null))
                    return false;
                if (obj is Element)
                    return ((Element)obj).AtomicNumber == this.AtomicNumber;
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return AtomicNumber;
            }

            public override string ToString()
            {
                return Name;
            }

            public int CompareTo(Element that)
            {
                return this.AtomicNumber - that.AtomicNumber;
            }
        }

    }

    public enum ItemType
    {
        GENERATOR, MICROCHIP
    }

    public static class Ext
    {

        public static string Suffix(this ItemType iType)
        {
            switch (iType)
            {
                case ItemType.GENERATOR:
                    return "G";
                case ItemType.MICROCHIP:
                    return "M";
                default:
                    throw new Exception("bork");
            }
        }

        public static IEnumerable<T> Interleave<T>(this IEnumerable<IEnumerable<T>> ienums)
        {
            var rats = ienums.Select(ie => ie.GetEnumerator()).ToArray();
            bool[] bs = new bool[rats.Length];
            for (int i = 0; i < rats.Length; i++)
                bs[i] = true;
            while(true)
            {
                bool cont = false;
                for (int i = 0; i < rats.Length; i++)
                    if (bs[i] && (bs[i] = rats[i].MoveNext()))
                    {
                        cont = true;
                        yield return rats[i].Current;
                    }
                if (!cont)
                    break;
            }
        }

        public static string PadCenter(this string str, int padLen)
        {
            return str.PadCenter(padLen, ' ');
        }

        public static string PadCenter(this string str, int padLen, char padChar)
        {
            int off = padLen % 2; // Right justifies
            return padLen <= str.Length ? str : str.PadLeft(str.Length + (padLen - str.Length) / 2 + off, padChar).PadRight(padLen - off, padChar);
        }
    }
}
