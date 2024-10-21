using System;
using Program.LootLibrary;
using Program.PhaseManager;
using Program.StoryPresenter;


namespace Program
{
    class Player
    {
        public int health = 5;
        public int maxHealth = 5;
        public int BonusTurns = 0;
        public char PrefferedSuffix;
        public int levelNumber = 1;
        public int regeneration = 3;
        public Weapon[] weapons = [new Pěst(), new Pěst()]; 
        public Armor armor = new Nahý();
        public List<Loot> inventory = new List<Loot>();
    }
    class Program
    {
        Player player;
        static void Main(string[] args)
        {
            Player player = new Player();
            Game game = new Game();
            Program program = new Program();
            Presenter storyPresenter = new Presenter();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("o=||===================>");
            Console.WriteLine("Cesta na Smaragodvý vrch");
            Console.WriteLine("<===================||=o");
            Console.ForegroundColor = ConsoleColor.White;

            storyPresenter.Tutorial();
            Console.WriteLine("Vítej, vyber si preferovaný rod oslovování:\n1.Mužský\n2.Ženský\n3.Nebinární");
            program.GenderSelect(player, program);

            Console.WriteLine("1. Nová hra (s příběhem)\n2. Rychlý start (bez příběhu)");
            switch (program.PlayerInput())
            { case 1: Console.Clear(); storyPresenter.Start(player); break;
              case 2: Console.Clear(); storyPresenter.QuickStart(player); break;
            }

            //player.health = 20;
            //player.maxHealth = 20;
            ////player.weapons[1] = new StehenníKost();
            //player.inventory.Add(new Příšerák());
            //player.inventory.Add(new JedovatýPrach());
            //player.weapons[0] = new Puška();
            //player.weapons[1] = new Řemdich();
            ////player.armor = new SlunečníBrýle();

            //player.armor = new DiamantováZbroj();
            //player.levelNumber = 3;
            //player.regeneration = 7;


            game.BeginGame(player);

        }

        public int PlayerInput()
        {
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("> ");
                    Console.ForegroundColor = ConsoleColor.White;
                    int input = Convert.ToInt32(Console.ReadLine());
                    return input;
                }
                catch
                { //pokud se hráčem zadaný input nedá konvertovat na int
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Toto není správně zadaný input. Zkus to znovu.");
                    Console.ForegroundColor = ConsoleColor.White;
                    return PlayerInput();
                }
            }
           
        }
        private void GenderSelect(Player player, Program program) 
        {
            switch (program.PlayerInput())
            {
                case 1:
                    break;
                case 2:
                    player.PrefferedSuffix = 'a';
                    break;
                case 3:
                    player.PrefferedSuffix = 'i';
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Takovou možnost nemáš! Zkus to znovu!");
                    Console.ForegroundColor = ConsoleColor.White;
                    GenderSelect(player, program);
                    break;
            }
        }
    }

}