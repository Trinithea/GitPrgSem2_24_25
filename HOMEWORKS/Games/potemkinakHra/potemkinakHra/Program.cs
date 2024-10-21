using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        // Příběh (lore) hry....nemám už ani žádnou fantazii, help
        Console.WriteLine("Příběh:");
        Console.WriteLine("---------------------------------------");
        Console.WriteLine("Jednoho dne sukuba unesla tvého kamaráda, trpaslíka/elfa.");
        Console.WriteLine("Tvým úkolem je bojovat za jeho svobodu.");
        Console.WriteLine("Musíš se postavit sukubám, ale buď opatrný,");
        Console.WriteLine("každý krok a pohyb mečem tě vysiluje.");
        Console.WriteLine("---------------------------------------");
        Console.WriteLine();

        // Vytvoření herního pole
        char[,] pole = new char[5, 5];

        // Zadání jména hráče
        Console.WriteLine("Zadej své jméno:");
        string jmenoHrace = Console.ReadLine();

        // Výběr postavy s vysvětlením rozdílů
        Postava hrac;
        Console.WriteLine("Vyber si postavu:");
        Console.WriteLine("1 = Trpaslík (schopnost: +1 síla za 1 minci)");
        Console.WriteLine("2 = Elf (schopnost: +1 život za 1 minci)");
        int volba = int.Parse(Console.ReadLine());

        if (volba == 1)
            hrac = new Trpaslik(jmenoHrace, 0, 0);
        else
            hrac = new Elf(jmenoHrace, 0, 0);

        // Inicializace nepřátel
        Random rand = new Random();
        int pocetNepratel = rand.Next(1, 6); // Generování náhodného počtu nepřátel (mezi 1 a 5)

        List<Nepritel> nepratele = new List<Nepritel>();

        // Vytváření sukub s náhodnými pozicemi
        for (int i = 0; i < pocetNepratel; i++)
        {
            int nepritelX, nepritelY;

            do
            {
                nepritelX = rand.Next(0, 5); // Náhodná pozice X
                nepritelY = rand.Next(0, 5); // Náhodná pozice Y
            } while (nepritelX == hrac.PoziceX && nepritelY == hrac.PoziceY); // Nepřítel nesmí být na stejné pozici jako hráč

            nepratele.Add(new Sukuba(nepritelX, nepritelY));
        }

        // Hlavní herní smyčka
        while (hrac.Zivoty > 0 && hrac.Sila > 0 && nepratele.Count > 0)
        {
            // Vykreslení herního pole
            VykresliHerniPole(pole, hrac, nepratele);

            Console.WriteLine($"\n{hrac.Jmeno} má {hrac.Zivoty} životů, {hrac.Mince} mincí a {hrac.Sila} síly.");
            Console.WriteLine("Co chceš dělat? w = nahoru, s = dolů, a = vlevo, d = vpravo, b = boj, c = cesta (nákup), p = použít speciální schopnost");

            char akce = Console.ReadKey().KeyChar;
            Console.WriteLine();

            if (akce == 'w' || akce == 's' || akce == 'a' || akce == 'd')
            {
                hrac.Pohyb(akce);
            }
            else if (akce == 'b' && nepratele.Count > 0)
            {
                Nepritel nepritel = najdiNepriteleNaPozici(hrac, nepratele);
                if (nepritel != null)
                {
                    hrac.Bojuj(nepritel);
                    if (nepritel.Zivoty > 0)
                    {
                        nepritel.Bojuj(hrac);
                    }
                    else
                    {
                        nepratele.Remove(nepritel);
                        hrac.Mince += 10; // Odměna za výhru
                        Console.WriteLine($"{hrac.Jmeno} zabil {nepritel.GetType().Name} a získal 10 mincí!");
                    }
                }
                else
                {
                    Console.WriteLine("Na této pozici není žádný nepřítel.");
                }
            }
            else if (akce == 'c') // Přejít na Cestu (nákup)
            {
                PrejitNaCestu(hrac);
            }
            else if (akce == 'p') // Použití speciální schopnosti
            {
                hrac.PouzitSchopnost();
            }
        }

        if (hrac.Zivoty <= 0)
            Console.WriteLine($"{hrac.Jmeno} zemřel. Hra skončila.");
        else if (nepratele.Count == 0)
            Console.WriteLine($"{hrac.Jmeno} zabil všechny nepřátele! Vyhrál jsi hru!");
    }

    // Vykreslení herního pole
    static void VykresliHerniPole(char[,] pole, Postava hrac, List<Nepritel> nepratele)
    {
        // Vymazání pole
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                pole[i, j] = '.';
            }
        }

        // Umístění hráče na pole
        pole[hrac.PoziceY, hrac.PoziceX] = 'H';

        // Umístění nepřátel na pole
        foreach (var nepritel in nepratele)
        {
            pole[nepritel.PoziceY, nepritel.PoziceX] = 'N';
        }

        // Vykreslení pole
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Console.Write(pole[i, j] + " ");
            }
            Console.WriteLine();
        }
    }

    // Najdi nepřítele na pozici hráče
    static Nepritel najdiNepriteleNaPozici(Postava hrac, List<Nepritel> nepratele)
    {
        foreach (var nepritel in nepratele)
        {
            if (nepritel.PoziceX == hrac.PoziceX && nepritel.PoziceY == hrac.PoziceY)
            {
                return nepritel;
            }
        }
        return null;
    }

    // Funkce pro přechod na Cestu
    static void PrejitNaCestu(Postava hrac)
    {
        Console.WriteLine("\nVítej na Cestě! Co chceš koupit?");
        Console.WriteLine("1 = Elixír (+2 životy za 20 mincí), 2 = Voda (+5 síly za 5 mincí), 0 = Zpět");

        int vyber = int.Parse(Console.ReadLine());

        if (vyber == 1)
        {
            if (hrac.Mince >= 20)
            {
                hrac.Mince -= 20;
                hrac.Zivoty += 2;
                Console.WriteLine("Koupil jsi Elixír a získal +2 životy.");
            }
            else
            {
                Console.WriteLine("Nemáš dost mincí.");
            }
        }
        else if (vyber == 2)
        {
            if (hrac.Mince >= 5)
            {
                hrac.Mince -= 5;
                hrac.Sila += 5;
                Console.WriteLine("Koupil jsi Vodu a získal +5 síly.");
            }
            else
            {
                Console.WriteLine("Nemáš dost mincí.");
            }
        }
    }
}

// Abstraktní třída Postava
abstract class Postava
{
    public string Jmeno { get; set; }
    public int Zivoty { get; set; }
    public int Mince { get; set; }
    public int Sila { get; set; }
    public int PoziceX { get; set; }
    public int PoziceY { get; set; }

    public Postava(string jmeno, int poziceX, int poziceY)
    {
        Jmeno = jmeno;
        Zivoty = 10;
        Mince = 10;
        Sila = 4;
        PoziceX = poziceX;
        PoziceY = poziceY;
    }

    public void Pohyb(char smer)
    {
        switch (smer)
        {
            case 'w':
                if (PoziceY > 0)
                {
                    PoziceY--;
                    Sila--; // Snížení síly při pohybu nahoru
                }
                break;
            case 's':
                if (PoziceY < 4)
                {
                    PoziceY++;
                    Sila--; // Snížení síly při pohybu dolů
                }
                break;
            case 'a':
                if (PoziceX > 0)
                {
                    PoziceX--;
                    Sila--; // Snížení síly při pohybu vlevo
                }
                break;
            case 'd':
                if (PoziceX < 4)
                {
                    PoziceX++;
                    Sila--; // Snížení síly při pohybu vpravo
                }
                break;
        }
    }

    public abstract void PouzitSchopnost();
    public abstract void Bojuj(Nepritel nepritel);
}

// Třída Trpaslík
class Trpaslik : Postava
{
    public Trpaslik(string jmeno, int poziceX, int poziceY) : base(jmeno, poziceX, poziceY) { }

    public override void PouzitSchopnost()
    {
        if (Mince > 0)
        {
            Mince--;
            Sila++;
            Console.WriteLine($"{Jmeno} (Trpaslík) si zvýšil sílu o 1 za 1 minci.");
        }
        else
        {
            Console.WriteLine("Nemáš dost mincí.");
        }
    }

    public override void Bojuj(Nepritel nepritel)
    {
        Console.WriteLine($"{Jmeno} (Trpaslík) bojuje s {nepritel.GetType().Name}!");
        nepritel.Zivoty -= 1; // Trpaslík útočí za 1 poškození
        Sila--; // Trpaslík ztrácí 1 sílu za útok
    }
}

// Třída Elf
class Elf : Postava
{
    public Elf(string jmeno, int poziceX, int poziceY) : base(jmeno, poziceX, poziceY) { }

    public override void PouzitSchopnost()
    {
        if (Mince > 0)
        {
            Mince--;
            Zivoty++;
            Console.WriteLine($"{Jmeno} (Elf) si obnovil 1 život za 1 minci.");
        }
        else
        {
            Console.WriteLine("Nemáš dost mincí.");
        }
    }

    public override void Bojuj(Nepritel nepritel)
    {
        Console.WriteLine($"{Jmeno} (Elf) bojuje s {nepritel.GetType().Name}!");
        nepritel.Zivoty -= 2; // Elf útočí za 2 poškození
        Sila--; // Elf ztrácí 1 sílu za útok
    }
}

// Abstraktní třída Nepřítel
abstract class Nepritel
{
    public int Zivoty { get; set; }
    public int PoziceX { get; set; }
    public int PoziceY { get; set; }

    public Nepritel(int poziceX, int poziceY)
    {
        Zivoty = 5;
        PoziceX = poziceX;
        PoziceY = poziceY;
    }

    public abstract void Bojuj(Postava hrac);
}

// Konkrétní nepřítel: Sukuba
class Sukuba : Nepritel
{
    public Sukuba(int poziceX, int poziceY) : base(poziceX, poziceY) { }

    public override void Bojuj(Postava hrac)
    {
        Console.WriteLine($"Sukuba útočí na {hrac.Jmeno}!");
        hrac.Zivoty -= 2; // Sukuba útočí za 2 poškození
    }
}
