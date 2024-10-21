using System.Text;

partial class Program
{
    class BattleControler(List<Creature> party1, List<Creature> party2)
    {
        readonly List<Creature> p1 = party1;
        readonly List<Creature> p2 = party2;

        public bool Turn(){
            p1.ForEach(cr => cr.ResetCooldown());
            p2.ForEach(cr => cr.ResetCooldown());

            while (true)
            {
                if (p2.Count == 0){
                    Console.Clear();
                    p1.ForEach(cr => cr.Heal(20));

                    return true;
                }
                else if (p1.Count == 0){
                    Console.Clear();

                    return false;
                }

                Console.Clear();
                p1.ForEach(Console.WriteLine);
                Console.WriteLine("\n Vs \n");
                p2.ForEach(Console.WriteLine);
                Console.ReadLine();

                Console.Clear();
                p1.ForEach(cr => {
                    Console.Clear();
                    if (cr.IsOnCooldown)
                    {
                        Console.WriteLine($"{cr} is on cooldown");
                        cr.DecreaseCooldown();
                    }
                    else
                    {
                        string move = cr?.CombatMove(p2, p1) ?? throw new Exception("Něco je blbě");
                        Console.Clear();
                        Console.WriteLine($"{cr} used {move}");
                    }
                    Console.ReadLine();
                });
                p2.RemoveAll(cr => cr.Health <= 0);

                Console.Clear();
                p2.ForEach(cr => {
                    Console.Clear();
                    if (cr.IsOnCooldown)
                    {
                        Console.WriteLine($"{cr} is on cooldown");
                        cr.DecreaseCooldown();
                    }
                    else
                    {
                        string move = cr?.CombatMove(p1, p2) ?? throw new Exception("Něco mi nesedí");
                        Console.Clear();
                        Console.WriteLine($"{cr} used {move}");
                    }
                    Console.ReadLine();
                });
                p1.RemoveAll(cr => cr.Health <= 0);
            }
        }
    }

    static T MakeDecision<T>(string topMessage) where T : Enum
    {
        T[] options = (T[])Enum.GetValues(typeof(T));
        int selectedIndex = 0;
        ConsoleKey key;

        Console.CursorVisible = false;

        do
        {
            Console.Clear();
            Console.WriteLine(topMessage + "\n");
            Console.WriteLine("Use ↑/↓ arrows to select, Enter to confirm:\n");

            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine($" {options[i].ToString().Replace('_', ' ').Replace('đ', '(').Replace('Đ', ')').Replace('ß', '%')} ");

                Console.ResetColor();
            }

            key = Console.ReadKey().Key;

            if (key == ConsoleKey.UpArrow && selectedIndex > 0)
            {
                selectedIndex--;
            }
            else if (key == ConsoleKey.DownArrow && selectedIndex < options.Length - 1)
            {
                selectedIndex++;
            }

        } while (key != ConsoleKey.Enter);

        Console.CursorVisible = true;
        return options[selectedIndex];
    }

    static int MakeDecision<T>(string topMessage, List<T> options)
    {
        int selectedIndex = 0;
        ConsoleKey key;

        Console.CursorVisible = false;

        do
        {
            Console.Clear();
            Console.WriteLine(topMessage + "\n");
            Console.WriteLine("Use ↑/↓ arrows to select, Enter to confirm:\n");

            for (int i = 0; i < options.Count; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine($" {(options[i]?.ToString()?.Replace('_', ' ').Replace('đ', '(').Replace('Đ', ')').Replace('ß', '%')) ?? throw new Exception("Něco je špatně")} ");

                Console.ResetColor();
            }

            key = Console.ReadKey().Key;

            if (key == ConsoleKey.UpArrow && selectedIndex > 0)
            {
                selectedIndex--;
            }
            else if (key == ConsoleKey.DownArrow && selectedIndex < options.Count - 1)
            {
                selectedIndex++;
            }

        } while (key != ConsoleKey.Enter);

        Console.CursorVisible = true;
        return selectedIndex;
    }

    static void DrawProgressBar(int completed, int outOf)
    {
        //○---○---○---○
        //●---●---○---○

        int consoleWidth = Console.WindowWidth - outOf; // Leave 1 character for line break
        int distance = consoleWidth/(outOf-1);
        int final = consoleWidth - (distance*(outOf-2));

        StringBuilder output = new();

        // new string('-', distance)
        for (int i = 0; i < outOf; i++){
            output.Append($"{(i < completed ? '*' : 'O')}");

            if (i <= outOf-3){
                output.Append(new string('-', distance));
            }
            else if (i == outOf-2){
                output.Append(new string('-', final));
            }
        }

        Console.WriteLine(output.ToString());
    }
}