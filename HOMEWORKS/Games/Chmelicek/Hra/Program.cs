using System;
using System.Collections.Generic;

public abstract class Postava
{
    public int HP { get; set; }
    public int Penize { get; set; }
    public int FightingPower { get; set; }

    public abstract void VykonatAkci(Hrac hrac);
}

public class Hrac : Postava
{
    public Hrac()
    {
        HP = 100;
        Penize = 50;
        FightingPower = 20;
    }

    public override void VykonatAkci(Hrac hrac)
    {
        return;
    }
}

public class Cigan : Postava
{
    private Random random = new Random();

    public Cigan()
    {
        HP = 30;
        Penize = 10;
    }

    public override void VykonatAkci(Hrac hrac)
    {
        FightingPower = random.Next(10, 40);

        Console.WriteLine("Potkal jsi cigána!");

        Console.WriteLine("Chceš utéct? (ano/ne)");
        string volba = Console.ReadLine().ToLower();

        if (volba == "ano")
        {
            hrac.Penize -= 10;
            Console.WriteLine("\n Utekl jsi cigánovi, a vypadlo ti z kapsy 10 peněz.\n");
            return;
        }

        if (hrac.FightingPower >= FightingPower)
        {
            hrac.Penize += Penize;
            Console.WriteLine("\n Zabil jsi cigána a získal 10 peněz!\n");
        }
        else
        {
            hrac.HP -= 10;
            hrac.Penize -= 5;
            Console.WriteLine("\n Cigán tě sundal. Ztratil jsi 10 HP a 5 peněz.\n");
        }
    }
}

public class ZdrogovanyClovek : Postava
{
    private Random random = new Random();

    public ZdrogovanyClovek()
    {
        HP = 50;
        Penize = 20;
        FightingPower = 25;
    }

    public override void VykonatAkci(Hrac hrac)
    {
        Console.WriteLine("Potkal jsi zdrogovaného člověka!");

        Console.WriteLine("Chceš utéct? (ano/ne)");
        string volba = Console.ReadLine().ToLower();

        if (volba == "ano")
        {
            hrac.Penize -= 10;
            Console.WriteLine("\n Utekl jsi od zdrogovaného člověka a ztratil 10 peněz.\n");
            return;
        }

        Console.WriteLine("Co chceš udělat?");
        Console.WriteLine("1. Pravý hák\n2. Levý hák\n3. Jab\n4. Cross\n5. Livershot\n6. Uppercut");

        int guess = int.Parse(Console.ReadLine());

        List<int> spravneUdery = new List<int>();
        while (spravneUdery.Count < 3)
        {
            int spravny = random.Next(1, 7);
            if (!spravneUdery.Contains(spravny))
            {
                spravneUdery.Add(spravny);
            }
        }

        if (spravneUdery.Contains(guess))
        {
            hrac.Penize += Penize;
            Console.WriteLine("\n Zabil jsi Feťáka! Získal jsi 20 peněz.\n");
        }
        else
        {
            hrac.HP -= 15;
            hrac.Penize -= 5;
            Console.WriteLine("\n Feťák tě sundal. Ztratil jsi 15 HP a 5 peněz.\n");
        }
    }
}

public class Babicka : Postava
{
    public int HPToGive { get; set; } = 20;
    public int Price { get; set; } = 15;

    public Babicka()
    {
        HP = 40;
        Penize = 0;
        FightingPower = 0;
    }

    public override void VykonatAkci(Hrac hrac)
    {
        Console.WriteLine("A hele! Babička z klubu Babských rad!");
        Console.WriteLine("Můžeš si koupit lektvar za 15 peněz. Chcete ho koupit? (ano/ne)");

        string odpoved = Console.ReadLine().ToLower();

        if (odpoved == "ano")
        {
            if (hrac.Penize >= Price)
            {
                hrac.HP += HPToGive;
                hrac.Penize -= Price;
                Console.WriteLine("\n Koupil jsi lektvar a získal 20 HP.\n");
            }
            else
            {
                Console.WriteLine("\n Nemáš dostatek peněz.\n");
            }
        }
        else
        {
            Console.WriteLine("\n Rozhodl ses nekoupit lektvar.\n");
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Hrac hrac = new Hrac();
        List<Postava> ulice = new List<Postava>
        {
            new Cigan(),
            new ZdrogovanyClovek(),
            new Babicka()
        };

        while (hrac.HP > 0)
        {
            Console.WriteLine($"\nVaše HP: {hrac.HP}, Peníze: {hrac.Penize}, Síla: {hrac.FightingPower}");
            Console.WriteLine("Pohybuješ se ulicí. Stiskni klávesu (1) abys šel doprava, nebo (2) abys šel doleva.");

            string volba = Console.ReadLine();
            Console.WriteLine("");
            Postava potkanyPostava = ulice[new Random().Next(ulice.Count)];
            potkanyPostava.VykonatAkci(hrac);
        }

        Console.WriteLine("Konec hry. Zemřel jsi.");
    }
}
