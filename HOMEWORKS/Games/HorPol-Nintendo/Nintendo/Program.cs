using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nintendo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Jak se jmenuješ ?");
            Console.ReadLine();
            Console.WriteLine("Díky blbečku");
            Console.WriteLine("Jaká je tvoje oblíbená rasa?");
            string r = Console.ReadLine();
            Postava user;
            Random random = new Random();
            if (r == "Sukuba")
            {
                user = new Sukuba();
                Console.WriteLine("Jsi Sukuba :3", Console.ForegroundColor = ConsoleColor.Magenta);
                Console.WriteLine("\n\n_________________________________________\n\n", Console.ForegroundColor = ConsoleColor.Blue);
                user.Hra(user, random);
            }
            else if (r == "Kameňák")
            {
                user = new Kameňák();
                Console.WriteLine("Jsi Kamňák");
                Console.WriteLine("\n\n_________________________________________\n\n", Console.ForegroundColor = ConsoleColor.Blue);

                user.Hra(user, random);
            }
            else
            {
                user = new Gobber();
                Console.WriteLine("Fuj, Gobber");
                Console.WriteLine("\n\n_________________________________________\n\n", Console.ForegroundColor = ConsoleColor.Blue);
                user.Hra(user, random);
            }
        }
    }
    abstract class Postava
    {
        public Postava(string rasa, int zivot, int penize, int maxzivot, int luck)
        {
            Rasa = rasa;
            Zivot = zivot;
            Penize = penize;
            MaxZivot = maxzivot;
            Luck = luck;
        }
        public string Rasa { get; protected set; }
        public int Zivot { get; protected set; }
        public int Penize { get; protected set; }
        public int MaxZivot { get; protected set; }
        public int Luck { get; protected set; }
        public void Info()
        {
            Console.WriteLine("Rasa: " + Rasa + ", Životy: " + Zivot + ", Prachy: " + Penize, Console.ForegroundColor = ConsoleColor.Yellow);
        }
        public void Hra(Postava hrdina, Random rnd)
        {
            while (hrdina.Zivot > 0)
            {
                Random s = new Random();
                int z = s.Next(3);
                switch (z)
                {
                    case 1:
                        Gobber gobber = new Gobber();
                        Console.WriteLine("ÚTOČÍ NA TEBE GOBBER, FUJ\n\n");
                        hrdina.Setkání(hrdina, gobber, rnd);
                        break;
                    case 2:
                        Sukuba sukuba = new Sukuba();
                        Console.WriteLine("ÚTOČÍ NA TEBE SUKUBA >:3\n\n");
                        hrdina.Setkání(hrdina, sukuba, rnd);
                        break;
                    case 3:
                        Kameňák kameňák = new Kameňák();
                        Console.WriteLine("UTOČÍ NA TEBE KAMEŇÁK :)\n\n");
                        hrdina.Setkání(hrdina, kameňák, rnd);
                        break;
                }
            }
        }
        public void Setkání(Postava hrdina, Postava nepr, Random rnd)
        {
            while (true)
            {
                hrdina.Info();
                Console.WriteLine("||||||||||||||||||||||||||||||||||||||||||", Console.ForegroundColor = ConsoleColor.Red);
                nepr.Info();
                Console.WriteLine("\n\n_________________________________________\n\n", Console.ForegroundColor = ConsoleColor.Blue);
                Console.WriteLine("Co chceš dělat blbečku?", Console.ForegroundColor = ConsoleColor.Magenta);
                Console.WriteLine("1. Útok[U]   2. Obrana[O]   3. vzdát Se[S]");
                Console.WriteLine("_________________________________________\n\n", Console.ForegroundColor = ConsoleColor.Blue);
                string UserInput = Console.ReadLine();
                Console.Clear();
                switch (UserInput)
                {
                    case "U":
                        bool my = false;
                        bool oni = false;
                        nepr.BereZivot(nepr, rnd, ref oni); //útok hrdiny na nepřítele
                        hrdina.BereZivot(hrdina, rnd, ref my); //nějaá akce metoda
                        if (my && oni)
                        {
                            Console.WriteLine("\n\noba jste byli trefeni\n");
                        }
                        else if (my && oni == false)
                        {
                            Console.WriteLine("\n\nByla do tebe zaseta rána\n");
                        }
                        else if (my == false && oni)
                        {
                            Console.WriteLine("\n\nDo tvého nepřítele byla zaseta rána\n");
                        }
                        else
                        {
                            Console.WriteLine("\n\nOba jste levý\n");
                        }
                        break;
                    case "O":
                        bool sakra = false;
                        Console.WriteLine("Nic  jsi neudělal, blbečku");
                        hrdina.BereZivot(hrdina, rnd, ref sakra);
                        if (sakra)
                        {
                            Console.WriteLine("\n\nA ještě k tomu do tebe byla zaseta rána\n");
                        }
                        Console.WriteLine("\n_________________________________________\n\n", Console.ForegroundColor = ConsoleColor.Blue);
                        break;
                    case "S":
                        int smrt = rnd.Next(10);
                        if (smrt == 1) { Console.WriteLine("\n\nVyhrál jsi hru\n\n", Console.ForegroundColor = ConsoleColor.Magenta); Thread.Sleep(5000); Environment.Exit(0); }
                        else { hrdina.Zivot = -1; }
                        break;
                }
                if (nepr.Zivot < nepr.MaxZivot)
                {
                    Console.WriteLine("_________________________________________\n\n", Console.ForegroundColor = ConsoleColor.Blue);
                    Console.WriteLine("trefil jsi nepřítele, nepřítel zdrhnul", Console.ForegroundColor = ConsoleColor.Green);
                    Console.WriteLine("_________________________________________\n\n", Console.ForegroundColor = ConsoleColor.Blue);
                    break;
                }
                if (hrdina.Zivot < 1) { Console.WriteLine("\n\nsmrt, prohra\n\n", Console.ForegroundColor = ConsoleColor.White); Thread.Sleep(5000); break; }
            }
        }
        public void BereZivot(Postava blbec, Random rnd, ref bool cože)
        {
            int zraneni = rnd.Next(12);
            if (zraneni <= blbec.Luck)
            {
                blbec.Zivot--;
                cože = true;
            }
        }
    }
    class Gobber : Postava
    {
        public Gobber() : base("Gobber", 8, 20, 8, 1)
        {

        }
    }
    class Sukuba : Postava
    {
        public Sukuba() : base("Sukuba", 10, 10, 10, 2)
        {
        }
    }
    class Kameňák : Postava
    {
        public Kameňák() : base("Kameňák", 12, 8, 10, 3)
        {
        }
    }
}