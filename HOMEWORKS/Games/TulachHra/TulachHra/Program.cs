using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Hra;


namespace Hra
{
    class Program
    {
        static void Main(string[] args)
        {
            Postava[,] Pole = new Postava[5, 5];

            Console.WriteLine("Chceš hrát za Mužskou(M) nebo Ženskou(Z) postavu? Kolik Nepřátel chceš?");
            string[] input = Console.ReadLine().Split();
            string Gender = input[0];
            int Pocet = Convert.ToInt32(input[1]);
            Hrac Hrac;

            if (Gender == "M")
            {
                Hrac = new Muz();
            }
            else
            {
                Hrac = new Zena();
            }

            Random rnd = new Random();
            for (int i = 0; i < Pocet; i++)
            {
                int Level = rnd.Next(1, 10);
                int Rasa = rnd.Next(0, 2);

                int a = 0;
                while (a == 0)
                {
                    int xx = rnd.Next(0, 4);
                    int yy = rnd.Next(0, 4);
                    if (Pole[xx, yy] == null && (xx != 0 || yy != 0))
                    {
                        if (Rasa == 1)
                        {
                            Pole[xx, yy] = new Goblin(Level);
                        }
                        else
                        {
                            Pole[xx, yy] = new Trol(Level);
                        }
                        a = 1;
                    }
                }
            }
            while (Pocet > 0)
            {
                Console.WriteLine("Kam chceš jít? (W, S, A, D)");
                string inp = Console.ReadLine().ToUpper();
                int docasneX = 0;
                int docasneY = 0;
                if (inp == "W")
                {
                    docasneY = -1;
                }
                if (inp == "S")
                {
                    docasneY = 1;
                }
                if (inp == "A")
                {
                    docasneX = -1;
                }
                if (inp == "D")
                {
                    docasneX = 1;
                }
                Hrac.Posun(docasneX, docasneY, 4);
                if (Pole[Hrac.x, Hrac.y] == null)
                {
                    Console.WriteLine("Nikoho jsi nepotkal. Získáváš 20 zlaťáků");
                }
                else
                {
                    Console.WriteLine("Někdo tu je!");
                    Postava Nepritel = Pole[Hrac.x, Hrac.y];
                    Nepritel.Info();
                    Console.WriteLine("Chceš bojovat (B), nebo utéct(U)?");

                    string inp2 = Console.ReadLine().ToUpper();

                    if (inp2 == "U")
                    {
                        Hrac.Utek();
                    }
                    else
                    {
                        List<int> Vysledek = Nepritel.Utok();
                        int kolikZ = Vysledek[0];
                        int kolikP = Vysledek[1];
                        Hrac.AddPenize(kolikP);
                        Hrac.AddZivoty(kolikZ);
                    }
                    Hrac.Info();
                    if (Nepritel.Zivot <= 0)
                    {
                        Pocet -= 0;
                        Console.WriteLine("Nepřítel Zemřel.");
                        Pole[Hrac.x, Hrac.y] = null;
                    }
                    if (Hrac.Zivot <= 0 || Hrac.Penize <= 0)
                    {
                        Console.WriteLine("Prohrál jsi! Zbylo " + Pocet + " nepřátel.");
                        break;
                    }
                }

            }
        }
    }
}

abstract class Postava
{
    public Postava(int zivot)
    {
        Zivot = zivot;
    }

    public string Typ { get; set; }
    public int Zivot { get; protected set; }
    public abstract List<int> Utok();

    public void Info()
    {
        Console.WriteLine("Typ: " + Typ + ", Životy: " + Zivot);
    }
}

abstract class Hrac : Postava
{
    public Hrac(int zivot, int X, int Y) : base(zivot)

    {
        x = X;
        y = Y;
    }
    public int x { get; protected set; }
    public int y { get; protected set; }

    public int Penize { get; protected set; }

    virtual public void Pohyb() { }
    public void Posun(int doprava, int dolu, int hranice)
    {
        x += doprava;
        y += dolu;
        if (x > hranice)
        {
            x = 0;
        }
        if (y > hranice)
        {
            y = 0;
        }
        if (x < 0)
        {
            x = hranice;
        }
        if (y < 0)
        {
            y = hranice;
        }
    }
    virtual public void Utek() { }
    public void AddPenize(int mnozstvi)
    {
        Penize += mnozstvi;
    }
    public void AddZivoty(int mnozstvi)
    {
        Zivot += mnozstvi;
    }
}

class Muz : Hrac
{
    public Muz() : base(100, 0, 0)
    {
        Typ = "Muž";
        Penize = 1000;
    }
    public override List<int> Utok()
    {
        return new List<int>();
    }
    override public void Pohyb()
    {
        Penize += 20;
    }
    override public void Utek()
    {
        Penize -= 250;
        Console.WriteLine("Přišel jsi o 250 zlaťáků");
    }
}
class Zena : Hrac
{
    public Zena() : base(110, 0, 0)
    {
        Typ = "Žena";
        Penize = 850;
    }
    override public void Pohyb()
    {
        Zivot += 2;
    }
    public override List<int> Utok()
    {
        return new List<int>();
    }
    override public void Utek()
    {
        Penize -= 50;
        Zivot -= 10;
        Console.WriteLine("Přišla jsi o 10 životů a 50 zlaťáků");
    }
}

class Goblin : Postava
{
    public Goblin(int level) : base(30 + 5 * level)
    {
        Level = level;
    }
    public int Level { get; protected set; }
    Random rnd = new Random();
    override public List<int> Utok()
    {
        Console.WriteLine("Typni číslo od jedné do 3");
        int Goblinovo = rnd.Next(1, 4);
        int Hracovo = Convert.ToInt32(Console.ReadLine());

        if (Goblinovo == Hracovo)
        {
            Console.WriteLine("Ach, trefil jsi se. Získáváš " + 30 * Level + " zlaťáků");
            Zivot -= 30;
            return new List<int>(new int[] { 0, 30 * Level });
        }
        else
        {
            Console.WriteLine("Netrefil ses! Ztrácíš " + 30 * Level + " zlaťáků");
            return new List<int>(new int[] { 0, -30 * Level });
        }
    }
}
class Trol : Postava
{
    public Trol(int level) : base(200 + 20 * level)
    {
        Level = level;
    }
    public int Level { get; protected set; }
    Random rnd = new Random();
    override public List<int> Utok()
    {
        Console.WriteLine("Typni číslo od jedné do 2");
        int Trolovo = rnd.Next(1, 3);
        int Hracovo = Convert.ToInt32(Console.ReadLine());

        if (Trolovo == Hracovo)
        {
            Console.WriteLine("Ach, trefil jsi se. Tady máš lektvar, který ti přidá " + 2 * Level + " životů");
            Zivot -= 50;
            return new List<int>(new int[] { 2 * Level, 0 });
        }
        else
        {
            Console.WriteLine("Netrefil ses! Dostal jsi ránu za " + 5 * Level + " životů. Vypadlo ti " + 25 * Level + " zlaťáků");
            return new List<int>(new int[] { -5 * Level, 0 });
        }
    }
}