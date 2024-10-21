using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    // Na začátku hry si vyberete postavu s daným jménem a hodnotou zdraví.
    // Následně si můžete vybrat z třech možností
    // První možnost je zaútočit na jednoho z nepřátel, přičemž 
    // každý z nepřátel má jinou hodnotu zdraví a poškození a zároveň je odolný proti jednomu unikátnímu kouzlu.
    // Při útoku si vyberete jaké kouzlo chcete použít a pokud je účinné, nepříteli se ubere hodnota zdraví podle hodnoty poškození vámi použitého kouzla.
    // Jestli jste nepřítele neporazili tak na vás zaútočí a ubere vám tolik zdraví podle toho jakou má hodnotu poškození. Jeho hodnota poškození se po každým kole zvedá o 2.
    // Druhou možností je si nakoupit kouzla. Podle toho, kolik máte zlata si můžete zakoupit kouzlo.
    // Kouzla, které si můžete koupit mají vyšší poškození, než vaše dvě základní kouzla a nepřátelé proti nim nejsou odolní. 
    // Třetí možnost je pouze vypsání jméno a hodnota poškození vašich kouzel, které váš hráč smí použít. 
    // Každé kolo si musíte vybrat jednu z možností, přičemž hra končí pokud máte 0 zdraví (hráč je mrtvý) nebo jste porazili všechny nepřátelé (zbývá 0 nepřátel).

    interface ICharacter
    {
        string Jmeno { get; }
        int Zdravi { get; set; }
        bool JeNazivu();
        void VypisStatus();
    }


    abstract class Postava : ICharacter
    {
        public string Jmeno { get; protected set; }
        public int Zdravi { get; set; }

        public Postava(string jmeno, int zdravi)
        {
            Jmeno = jmeno;
            Zdravi = zdravi;
        }

        public bool JeNazivu()
        {
            return Zdravi > 0;
        }
        public virtual void VypisStatus()
        {

        }
    }

    class Hrac : Postava
    {
        public int Zlato { get; private set; }
        public Dictionary<string, int> Kouzla { get; private set; } // Kouzla a jejich poškození

        public Hrac(string jmeno, int zdravi)
            : base(jmeno, zdravi)
        {
            Zlato = 50;
            Kouzla = new Dictionary<string, int>()
        {
            { "Ohňová koule", 30 },
            { "Ledová střela", 20 }
        };
        }

        public override void VypisStatus()
        {
            Console.WriteLine("\n{0} má {1} zdraví a {2} zlata", Jmeno, Zdravi, Zlato);
        }

        // Útok hráče na nepřítele
        public void Utok(Nepritel cil, string kouzlo)
        {
            if (!Kouzla.ContainsKey(kouzlo))
            {
                Console.WriteLine("Kouzlo neexistuje.");
                return;
            }

            if (cil.Odolnosti == kouzlo)
            {
                Console.WriteLine("\n{0} je odolný proti {1}! Měl bys použít jiné kouzlo.", cil.Jmeno, kouzlo);
            }
            else
            {
                cil.Zdravi -= Kouzla[kouzlo];
                if (cil.Zdravi < 0)
                {
                    cil.Zdravi = 0;
                }
                Console.WriteLine("\nÚtočíš na {0} pomocí {1}. Způsobils {2} poškození.", cil.Jmeno, kouzlo, Kouzla[kouzlo]);
            }

            if (!cil.JeNazivu())
            {
                Console.WriteLine("{0} byl poražen!", cil.Jmeno);
                Zdravi += 50;
                Zlato += 50;
                Console.WriteLine("{0} si obnovil 50 zdraví. Nyní máš {1} zdraví.", Jmeno, Zdravi);
                Console.WriteLine("{0} si obnovil 50 zlata. Nyní máš {1} zlata.", Jmeno, Zlato);
            }
        }

        public void NakupVylepseni(string kouzlo, int cena, int poskozeni)
        {
            if (Zlato >= cena)
            {
                Kouzla.Add(kouzlo, poskozeni);
                Zlato -= cena;
                Console.WriteLine("\nKoupil sis kouzlo {0} za {1} zlata. Zůstává ti {2} zlata.", kouzlo, cena, Zlato);
            }
            else
            {
                Console.WriteLine("\nNemáš dostatek zlata. Máš pouze {0}, ale potřebuješ {1}.", Zlato, cena);
            }
        }
        public void VypisKouzla()
        {
            Console.WriteLine("\nKouzla, která máš k dispozici:");
            foreach (var kouzlo in Kouzla)
            {
                Console.WriteLine("- {0}, poškození: {1}", kouzlo.Key, kouzlo.Value);
            }
        }
    }

    class Nepritel : Postava
    {
        public string Odolnosti { get; private set; }
        public int Poskozeni { get; set; }

        public static int pocetNepratel; //static, aby to nepatřilo každému nepříteli (objektu), ale aby to patřilo třídě Nepřítel

        public Nepritel(string jmeno, int zdravi, string odolnosti, int poskozeni)
            : base(jmeno, zdravi)
        {
            Odolnosti = odolnosti;
            Poskozeni = poskozeni;
            pocetNepratel++;
        }

        public override void VypisStatus()
        {
            Console.WriteLine("\nNepřítel: {0} má {1} zdraví", Jmeno, Zdravi);
        }

        // Útok nepřítele na hráče
        public void Utok(Hrac hrac, int poskozeni)
        {

            hrac.Zdravi -= poskozeni;
            if (hrac.Zdravi < 0)
            {
                hrac.Zdravi = 0;
            }
            Console.WriteLine("\n{0} útočí na {1} a způsobuje {2} poškození.", Jmeno, hrac.Jmeno, poskozeni);
        }
    }

    // Hlavní třída hry
    class Hra
    {
        private Hrac hrac;
        private List<Nepritel> nepratele;

        public Hra()
        {
            nepratele = new List<Nepritel>()
        {
            new Nepritel("Gnom XL", 70, "Ohňová koule", 13 ),
            new Nepritel("Gnom medium", 100, "Ledová střela" , 3 )
        };
        }

        private void VytvorHrace()
        {
            Console.WriteLine("Vítej! Vyber si svou postavu:");
            Console.WriteLine("1. Klb (zdraví: 120)");
            Console.WriteLine("2. Žena (zdraví: 75)");

            int volba = int.Parse(Console.ReadLine());

            switch (volba)
            {
                case 1:
                    hrac = new Hrac("Klb", 120);
                    break;
                case 2:
                    hrac = new Hrac("Žena", 75);
                    break;
                default:
                    Console.WriteLine("Neplatná volba, budeš hrát jako Gandalf.");
                    hrac = new Hrac("Gandalf", 100);
                    break;
            }

            Console.WriteLine("Vybral sis postavu: {0}", hrac.Jmeno);
        }

        public void Start()
        {
            VytvorHrace();
            Console.WriteLine("\nHra začala!");

            while (hrac.JeNazivu() && nepratele.Count > 0)
            {
                hrac.VypisStatus();
                Console.WriteLine("\nZ {0} nepřátel zbývájí {1} neprátelé:", Nepritel.pocetNepratel, nepratele.Count);
                foreach (Nepritel nepritel in nepratele)
                {
                    nepritel.VypisStatus();
                }

                Console.WriteLine("\nCo chceš udělat?");
                Console.WriteLine("1. Bojovat");
                Console.WriteLine("2. Nakoupit kouzla");
                Console.WriteLine("3. Vypsat seznam svých kouzel");



                int volba = int.Parse(Console.ReadLine());

                switch (volba)
                {
                    case 1:
                        ZautocNaNepritele();
                        break;
                    case 2:
                        NakupKouzlo();
                        break;
                    case 3:
                        hrac.VypisKouzla();
                        break;
                    default:
                        Console.WriteLine("Neplatná volba.");
                        break;
                }

                if (!hrac.JeNazivu())
                {
                    Console.WriteLine("Hráč byl poražen. Konec hry.");

                }

                if (nepratele.Count == 0)
                {
                    Console.WriteLine("Vyhrál jsi! Porazil jsi všechny nepřátele.");
                }
            }
            Console.ReadLine();
        }

        private void ZautocNaNepritele()
        {
            Console.WriteLine("\nVyber si protivníka:");
            for (int i = 0; i < nepratele.Count; i++)
            {
                Console.WriteLine("{0}. {1} (zdraví: {2})", i + 1, nepratele[i].Jmeno, nepratele[i].Zdravi);
            }

            int volba = int.Parse(Console.ReadLine()) - 1;

            if (volba < 0 || volba >= nepratele.Count)
            {
                Console.WriteLine("Neplatná volba.");
                return;
            }

            Nepritel cil = nepratele[volba];

            Console.WriteLine("Vyber kouzlo k útoku:");

            List<string> kouzlaList = new List<string>(hrac.Kouzla.Keys);
            for (int i = 0; i < kouzlaList.Count; i++)
            {
                Console.WriteLine("{0}. {1} ", i + 1, kouzlaList[i]);
            }

            int zvoleneKouzloIndex = int.Parse(Console.ReadLine()) - 1;
            if (zvoleneKouzloIndex < 0 || zvoleneKouzloIndex >= hrac.Kouzla.Count)
            {
                Console.WriteLine("Neplatná volba.");
                return;
            }

            hrac.Utok(cil, kouzlaList[zvoleneKouzloIndex]);

            if (!cil.JeNazivu())
            {
                nepratele.Remove(cil);
            }

            // Necháme nepřítele zaútočit na hráče, pokud je naživu
            if (cil.JeNazivu())
            {
                cil.Poskozeni = cil.Poskozeni + 2; //přidávání hodnoty poškození
                cil.Utok(hrac, cil.Poskozeni);
            }
        }

        private void NakupKouzlo()
        {
            Console.WriteLine("\nKup si kouzlo, na který nejsou nepřátelé odolni.");
            Console.WriteLine("Dostupná kouzla k nákupu:");
            Console.WriteLine("1. Bleskový prd (cena: 30 zlata, poškození: 30)");
            Console.WriteLine("2. Neviditelný prd (cena: 60 zlata, poškození: 45)");

            int volba = int.Parse(Console.ReadLine());

            switch (volba)
            {
                case 1:
                    hrac.NakupVylepseni("Bleskový prd", 30, 30);
                    break;
                case 2:
                    hrac.NakupVylepseni("Neviditelný prd", 60, 45);
                    break;
                default:
                    Console.WriteLine("Neplatná volba.");
                    break;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Hra hra = new Hra();
            hra.Start();
        }
    }
}