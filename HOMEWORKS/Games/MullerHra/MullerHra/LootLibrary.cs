using Program.MonsterManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.LootLibrary
{
    class LootList 
    {
        public Loot[] Elements = [new Dýka(), new Meč(), new ŽeleznáZbroj()];
    }
    abstract class Loot
    {
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public string Type { get; protected set; }
        public virtual void SpecialAbility(ref Player player, Monster monster) { }
    }
    abstract class Weapon : Loot 
    {
        public int TimePenalty = 0;
        public Weapon() 
        {
            Type = "Weapon";
        }
        public virtual int Attack() { return 1; }
        

    }
    abstract class Armor : Loot 
    {
        public int Defence { get; protected set; }
        public int MaxHealthIncrease { get; protected set; }
        public int HealPhaseBonus { get; protected set; }
        public int BonusTurns { get; protected set; }
        public Armor() 
        {
            Type = "Armor";
        }
    }
    abstract class Item : Loot 
    {
        public Item()
        {
            Type = "Item";
        }
    }
    //WEAPONS
    class Pěst : Weapon 
    {
        public Pěst()
        {
            Name = "Pěst";
            Description = "(Síla 1) Moc monster pěstí neumlátíš";
        }
        public override int Attack()
        {
            return 1;
        }
    }
    //Les
    class Větev : Weapon
    {
        public Větev()
        {
            Name = "Utržená Větev";
            Description = "(Síla 1-2?) Málem ti spadla na hlavu";
        }
        public override int Attack()
        {
            Random r = new Random();
            return r.Next(1, 3);
        }

    }
    class Dýka: Weapon 
    {
        public Dýka() 
        {
            Name = "Dýka";
            Description = "(Síla 2) Lehká a rychlá";
        }
        public override int Attack()
        {
            return 2;
        }
    }
    class RezavýNůž : Weapon
    {
        public RezavýNůž()
        {
            Name = "Rezavý nůž";
            Description = "(Síla 0-3) Rezavý a nepředvídatelný";
        }
        public override int Attack() 
        {
            Random r = new Random();
            return r.Next(0,4);
        }

    }
    class Meč: Weapon
    {
        public Meč()
        {
            Name = "Rezavý Meč";
            Description = "(Síla 3, -1 akce) Pomalejší zbraň z kdysi nejkvalitnější oceli";
            TimePenalty = 1;
        }
        public override int Attack()
        {
            return 3;
        }
    }
    class Meč2 : Weapon
    {
        public Meč2()
        {
            Name = "Meč";
            Description = "(Síla 1-3) Těžko ovladatelná zbraň z nejkvalitnější oceli";
        }
        public override int Attack()
        {
            Random r = new Random();
            return r.Next(1, 4);
        }

    }
    class Palcát : Weapon
    {
        public Palcát()
        {
            Name = "Palcát";
            Description = "(Síla 5, -2 akce) Pomalá, tupá, ale je z kvalitního dřeva";
            TimePenalty = 2;
        }
        public override int Attack()
        {
            return 5;
        }
    }
    //Roklina
    class Kyj : Weapon
    {
        public Kyj()
        {
            Name = "Kyj";
            Description = "(Síla 6-8, -2 akce) Jsou na něm hřebíky";
            TimePenalty = 2;
        }
        public override int Attack()
        {
            Random r = new Random();
            return r.Next(6, 9);
        }
    }
    class Šavle : Weapon 
    {
        public Šavle() 
        {
            Name = "Šavle";
            Description = "(Síla 4) Vžum!";
        }
        public override int Attack()
        {
            return 4;
        }
    }
    class Sekyra : Weapon
    {
        public Sekyra()
        {
            Name = "Válečná sekyra";
            Description = "(Síla 11! -2 akce, nemotornost) Její váha rozdrtí všechny... i tebe";
            TimePenalty = 2;
        }
        public override int Attack()
        {
            return 10;
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Válečná sekyra je těžko zvladatelná zbraň");
            Console.WriteLine("Ztrácíš 2 životy!");
            Console.ForegroundColor = ConsoleColor.White;
            player.health -= 2;
        }

    }
    class Řemdich : Weapon
    {
        public Řemdich()
        {
            Name = "Řemdich";
            Description = "(Síla 1-7) Jak se tohle vůbec používá?";
        }
        public override int Attack()
        {
            Random r = new Random();
            return r.Next(1,8);
        }

    }
    class KouzelnáHůl : Weapon
    {
        public KouzelnáHůl() 
        {
            Name = "Kouzelná hůl";
            Description = "(+5 životů pro tebe, +3 pro monstrum) Voní po jahodách.";
        }
        public override int Attack()
        {
            return 0;
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Kolem tebe se vytvořila mystická aura");
            Console.WriteLine("Získáváš 5 životů");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Kolem monstra se vytvořila mystická aura");
            Console.WriteLine("Získává 3 životy");
            Console.ForegroundColor = ConsoleColor.White;
            if(player.health + 5 > player.maxHealth + player.armor.MaxHealthIncrease) 
            {
                player.health = player.maxHealth + player.armor.MaxHealthIncrease;
            }
            else 
            {
                player.health += 5;
            }

            monster.Health += 3;
        }
    }
    class KouzelnáHůl2 : Weapon
    {
        public KouzelnáHůl2()
        {
            Name = "Kouzelná hůl";
            Description = "(-4 životy pro tebe, -7 pro monstrum) Páchne po kouři";
        }
        public override int Attack()
        {
            return 0;
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Kolem monstra se vytvořil kruh z ohně");
            Console.WriteLine("Ztrácí 7 životů!");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Kolem tebe se vytvořil kruh z ohně");
            Console.WriteLine("Ztrácíš 4 životy");
            Console.ForegroundColor = ConsoleColor.White;

            player.health -= 4;
            monster.Health -= 7;
        }
    }
    class KouzelnáHůl3 : Weapon
    {
        public KouzelnáHůl3()
        {
            Name = "Kouzelná hůl";
            Description = "(síla 3) Saje život...";
        }
        public override int Attack()
        {
            return 3;
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Cítíš jak jak do tebe vstoupila energie");
            Console.WriteLine("Získáváš 2 životy");
            Console.ForegroundColor = ConsoleColor.White;

            player.health += 2;
        }
    }
    class Kuše : Weapon
    {
        public Kuše()
        {
            Name = "Kuše";
            Description = "(síla 1 + ?) Čím větší monstrum, tím víc to bolí";
        }
        public override int Attack()
        {
            return 1;
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.WriteLine("Monstrum ztrácí další " + (monster.Health/4) + " životy!");
            monster.Health -= monster.Health/4;
        }
    }

    //ARMOR
    class Nahý : Armor
    {
        public Nahý()
        {
            Name = "Holá hruď";
            Description = "Ukazuješ se světu";
            Defence = 0;
        }
    }
    class RezaváZbroj : Armor
    {
        public RezaváZbroj()
        {
            Name = "Rezavá zbroj";
            Description = "(Obrana 1, +1 život) Skoro se rozpadá, ale alespoň něco";
            MaxHealthIncrease = 1;
            Defence = 1;
        }
    }
    class ŽeleznáZbroj : Armor
    {
        public ŽeleznáZbroj() 
        {
            Name = "Železná zbroj";
            Description = "(Obrana 2, +3 životy) V tomto brnění tě to nebude tolik bolet";
            MaxHealthIncrease = 3;
            Defence = 2;
        }
    }
    class SportovníLegíny : Armor 
    {
        public SportovníLegíny() 
        {
            Name = "Sportovní Legíny";
            Description = "(+1 akce) Žádná obrana, ale budeš dvakrát rychlejší";
            Defence = 0;
            BonusTurns = 1;
            
        }
    }
    class KoženáTunika : Armor
    {
        public KoženáTunika()
        {
            Name = "Kožená Tunika";
            Description = "(+3 regenerace, 1 Obrana, +3 max životy) Teplá a příjemná";
            Defence = 1;
            MaxHealthIncrease = 3;
            HealPhaseBonus = 3;
        }
    }
    class Kroksy : Armor
    {
        public Kroksy()
        {
            Name = "Kroksy na sportovní mód";
            Description = "(+2 akce, -3 max životy) Nedá se v nich zastavit";
            BonusTurns = 2;
            MaxHealthIncrease = -3;
        }
    }
    class KroužkováZbroj : Armor
    {
        public KroužkováZbroj()
        {
            Name = "Kroužková Zbroj";
            Description = "(Obrana 3, +2 max životy) Těžká, ale odolná";
            Defence = 3;
            MaxHealthIncrease = 2;
        }
    }
    class SlunečníBrýle : Armor
    {
        public SlunečníBrýle()
        {
            Name = "Sluneční Brýle";
            Description = "(+1 Drip)";
            MaxHealthIncrease = 1;
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nVypadáš dobře!");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
    class KouzelnýHábit : Armor
    {
        public KouzelnýHábit()
        {
            Name = "KOUZELNÝ HÁBIT";
            Description = "(+10 max životů) Záhadný hřejivý hábit";
            MaxHealthIncrease = 10;
        }
    }
    class ProkletáZbroj : Armor
    {
        public ProkletáZbroj()
        {
            Name = "Prokletá zbroj";
            Description = "(???) Je těžká";
            MaxHealthIncrease = -2;
            Defence = 5;
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Random r = new Random();
            int chance = r.Next(1, 101);
            if (chance <= 10)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Okolo monstra se vytvořila záhadná temná aura");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Monstrum ztrácí 2 životy");
                monster.Health -= 2;
            }
            else if (chance > 10 && chance <= 20)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Vytvořila se okolo tebe záhadná temná aura");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Ztrácíš 1 život!");
                player.health -= 1;
            }



        }
    }
    //Roklina
    class ProkletýHábit : Armor
    {
        public ProkletýHábit() 
        {
            Name = "Prokletý hábit";
            Description = "(???) Příjemně hřeje";
            MaxHealthIncrease = 20;
            HealPhaseBonus = -4;
        }
    }
    class ZlatáZbroj : Armor
    {
        public ZlatáZbroj()
        {
            Name = "Zlatá zbroj";
            Description = "(Obrana 3, +5 max životů) Hezky se blýská";
            MaxHealthIncrease = 5;
            Defence = 3;
        }
    }
    class BrněníProKoně : Armor
    {
        public BrněníProKoně()
        {
            Name = "Brnění pro koně";
            Description = "(Obrana 3, +7 max životů, -2 regenerace) Očividně je na tebe moc velké";
            MaxHealthIncrease = 7;
            Defence = 3;
            HealPhaseBonus = -2;
        }
    }
    class OstráZbroj : Armor 
    {
        public OstráZbroj() 
        {
            Name = "Ostrá zbroj";
            Description = "(+2 síla, Obrana 2, -5 max životů) Má na sobě bodáky";
            MaxHealthIncrease = -5;
            Defence = 2;
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.WriteLine("Monstrum se nabodlo na tvoje brnění! Monstrum ztrácí 2 životy!");
            monster.Health -= 2;
        }
    }
    class DiamantováZbroj : Armor 
    {
        public DiamantováZbroj() 
        {
            Name = "Diamantová zbroj";
            Description = "(Obrana 4) DIAMANTY!!";
            Defence = 4;
        }
    }
    class Kšandy : Armor
    {
        public Kšandy() 
        {
            Name = "Barevné kšandy";
            Description = "(-5 drip, +2 akce, -5 max životů, Obrana 2) Tak ošklivé, že doslova paralyzují každého kdo se na ně podívá";
            MaxHealthIncrease = -5;
            Defence = 2;
            BonusTurns = 2;
        }
    }
    class Exoskeleton : Armor
    {
        public Exoskeleton() 
        {
            Name = "Exoskeleton";
            Description = "(Obrana 2, zastrašování) Vytvořen z opradových kostlivců";
            Defence = 2;
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Random r = new Random();
            switch(r.Next(1,9))
            {
                case 1:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Monstrum bylo tak vystrašeno z tvého vzhledu, že můžeš hrát znovu!");
                    Console.ForegroundColor = ConsoleColor.White;
                    player.BonusTurns += 1;
                    break;
            };
        }
    }
    class GigantickáZbroj : Armor
    {
        public GigantickáZbroj() 
        {
            Name = "Gigantická zbroj";
            Description = "(Obrana 5, -10 max životů) Tu neuneseš";
            Defence = 5;
            MaxHealthIncrease= -5;
        }
    }
    //ITEMS
    class LektvarLéčení : Item 
    {
        public LektvarLéčení() 
        {
            Name = "Lektvar léčení";
            Description = "(vyléčí ti 3 životy) Chutná hnusně.";
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.WriteLine("Jsi vyléčen" + player.PrefferedSuffix + " o 3♥");
            if (player.health + 3 >= player.maxHealth + player.armor.MaxHealthIncrease) 
            {
                Console.WriteLine("Máš plný počet životů!");
                player.health = player.maxHealth + player.armor.MaxHealthIncrease;
            }
            else 
            {
                player.health += 3;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(player.health + "♥");
            Console.ForegroundColor= ConsoleColor.White;
        }
    }
    class VelkýLektvarLéčení : Item
    {
        public VelkýLektvarLéčení()
        {
            Name = "Velký lektvar léčení";
            Description = "(vyléčí ti 6 životů) Chutná absolutně nechutně";
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.WriteLine("Jsi vyléčen" + player.PrefferedSuffix +  " o 6♥");
            if (player.health + 6 >= player.maxHealth + player.armor.MaxHealthIncrease)
            {
                Console.WriteLine("Máš plný počet životů!");
                player.health = player.maxHealth + player.armor.MaxHealthIncrease;
            }
            else
            {
                player.health += 6;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(player.health + "♥");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
    class Příšerák : Item 
    {
        public Příšerák() 
        {
            Name = "Příšerák (Monster energy drink)";
            Description = "(+3 akce) Nabudí tě energií";
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            switch (player.PrefferedSuffix) 
            {
                default:
                    Console.WriteLine("JSI PLNÝ ENERGIE!!!");
                    break;
                case 'a':
                    Console.WriteLine("JSI PLNÁ ENERGIE!!!");
                    break;
                case 'i':
                    Console.WriteLine("JSI PLNÍ ENERGIE!!!");
                    break;
            }
            
            Console.ForegroundColor = ConsoleColor.White;
            player.BonusTurns = 3;
        }

    }
    class Šutr : Item//
    {
        public Šutr() 
        {
            Name = "Šutr";
            Description = "Můžeš ho po někom hodit";
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.WriteLine("Šutr byl vržen na nepřítele!");
            Console.ReadKey();
            Console.WriteLine("Uděluje nepříteli drtivou ránu a ztrácí 4 životy!");
            monster.Health -= 4;
        }
    }
    class Balvan : Item//
    {
        public Balvan()
        {
            Name = "Balvan";
            Description = "Můžeš zkusit ho po někom hodit";
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.WriteLine("Balvan byl vržen na nepřítele!");
            Console.ReadKey();
            Console.WriteLine("Balvan je moc těžký a spadl nepříteli pouze na palec!");
            Console.ReadKey();
            Console.WriteLine("Moc silný útok to nebyl, avšak je to extrémně bolestivé!\n Nepřítel ztrácí 2 životy, a můžeš hrát znovu!");
            monster.Health -= 2;
            player.BonusTurns += 1;
        }
    }
    class Dynamit : Item
    {
        public Dynamit()
        {
            Name = "Dynamit";
            Description = "Dávej si pozor, může ublížit i tobě";
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.WriteLine("Dynamit byl vržen na nepřítele!");
            Console.ReadKey();
            Console.WriteLine("Dynamit explodoval, nepřítel hoří, avšak spálil i tebe");
            Console.ReadKey();
            Console.WriteLine("Ztrácíš 3 životy, nepřítel ztrácí 7 životů");
            monster.Health -= 7;
            player.health -= 3;
        }
    }
    class Sušenka : Item
    {
        public Sušenka() 
        {
            Name = "Sušenka";
            Description = "Čokoládová, moc dobrá!";
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Mňam! Byl" + player.PrefferedSuffix + " jsi vyléčen" + player.PrefferedSuffix + " o 1 život!");
            Console.ReadKey();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("S nově nabitými silami, můžeš provést další akci");
            player.health += 1;
            player.BonusTurns += 1;
        }
    }
    class PlechovkaSušenek : Item
    {
        public PlechovkaSušenek()
        {
            Name = "PLECHOVKA SUŠENEK!";
            Description = "Čokoládový, moc dobrý!";
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Mňam! Byl" + player.PrefferedSuffix + " jsi vyléčen" + player.PrefferedSuffix + " o 3 životy!");
            Console.ReadKey();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("S nově nabitými silami, můžeš provést další akci");
            player.health += 3;
            player.BonusTurns += 1;
        }
    }
    class KouzelnáJahoda : Item
    {
        public KouzelnáJahoda() 
        {
            Name = "Kouzelná jahoda";
            Description = "(+ 3 max životy) Hezky voní?";
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Jahodu spolkneš");
            Console.ReadKey();
            Console.WriteLine("Zvýšil se ti maximálnní počet životů o 3 a vyléčily se ti 3 životy!)");
            Console.ForegroundColor = ConsoleColor.White;
            player.health += 3;
            player.maxHealth += 3;
        }

    }
    //Roklina
    class ObrovskýLektvarLéčení : Item
    {
        public ObrovskýLektvarLéčení()
        {
            Name = "OBROVSKÝ LEKTVAR LÉČENÍ";
            Description = "(vyléčí ti 11 životů) HNUS! Vedlejší účinky obsahují spontánní zvracení a průjem.";
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.WriteLine("Jsi vyléčen" + player.PrefferedSuffix + " o 11♥");
            if (player.health + 11 >= player.maxHealth + player.armor.MaxHealthIncrease)
            {
                Console.WriteLine("Máš plný počet životů!");
                player.health = player.maxHealth + player.armor.MaxHealthIncrease;
            }
            else
            {
                player.health += 11;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(player.health + "♥");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
    class Loutna : Item
    {
        public Loutna() 
        {
            Name = "Loutna";
            Description = "Dokáže vydat andělské tóny";
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Zahraješ jemnou melodii...");
            Console.ReadKey();
            Random r = new Random();
            switch (r.Next(1, 4)) 
            {
                default:
                    Console.WriteLine("Monstrum na tvou píseň nijak nereaguje.");
                    Console.ReadKey();
                    Console.WriteLine("Tak loutnou flákneš monstrum do ksichtu!");
                    Console.ReadKey();
                    Console.WriteLine("Loutna se rozbila a monstrum ztrácí 5 životů!");
                    monster.Health -= 5;
                    break;
                case 2:
                    Console.WriteLine("Monstrum na tvou píseň nijak nereaguje.");
                    Console.ReadKey();
                    Console.WriteLine("Pak ti náhle bohužel praskla struna");
                    Console.ReadKey();
                    Console.WriteLine("Ale andělské tóny loutny tě přivedly do stavu euforie. Máš o 7 životů navíc!");
                    player.health += 7;
                    break;
                case 3:
                    Console.WriteLine("Tvá loutna vydává krásné nebeské tóny.");
                    Console.ReadKey();
                    Console.WriteLine("Pak ti náhle bohužel praskla struna");
                    Console.ReadKey();
                    Console.WriteLine("Monstrum je tvou písní uklidněno. Máš 2 akce navíc!");
                    Console.ReadKey();
                    player.BonusTurns += 2;
                    break;
            }
        }
    }
    class PříšerákXXL : Item
    {
        public PříšerákXXL()
        {
            Name = "Příšerák XXL! (Monster energy drink)";
            Description = "(+4 akce) Nabudí tě energií a další";
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            switch (player.PrefferedSuffix)
            {
                default:
                    Console.WriteLine("JSI PLNÝ ENERGIE!!!");
                    break;
                case 'a':
                    Console.WriteLine("JSI PLNÁ ENERGIE!!!");
                    break;
                case 'i':
                    Console.WriteLine("JSI PLNÍ ENERGIE!!!");
                    break;
            }

            Console.ForegroundColor = ConsoleColor.White;
            player.BonusTurns = 4;
        }

    }
    class GigaBalvan : Item//
    {
        public GigaBalvan()
        {
            Name = "Giga Balvan";
            Description = "Skoro ho neuneseš, ale stále ho můžeš zkusit po někom hodit";
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.WriteLine("Giga balvan byl vržen na nepřítele!");
            Console.ReadKey();
            Console.WriteLine("Giga Balvan je ale extrémně těžký a spadl před nepřítele!");
            Console.ReadKey();
            Console.WriteLine("Vypadá to, že to vůbec nepomohlo, když najednou nepřítel o balvan zakopl a spadl na obličej!\n Nepřítel ztrácí 4 životy, a můžeš hrát znovu!");
            monster.Health -= 4;
            player.BonusTurns += 1;
        }
    }
    class BalíkDynamitu : Item
    {
        public BalíkDynamitu()
        {
            Name = "Balík dynamitu";
            Description = "Rozbombí všechny v okolí";
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.WriteLine("Balík dynamitu byl vržen na nepřítele!");
            Console.ReadKey();
            Console.WriteLine("BUM!, nepřítel hoří, avšak spálil i tebe");
            Console.ReadKey();
            Console.WriteLine("Ztrácíš 5 životů, nepřítel ztrácí 10 životů");
            monster.Health -= 10;
            player.health -= 5;
        }
    }
    class JedovatýPrach : Item 
    {
        public JedovatýPrach() 
        {
            Name = "Jedovatý prach";
            Description = "Otráví nepřítele. Účinější proti zdravějším monstrům";
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.WriteLine("Jedovatý prach byl vržen na nepřítele!");
            Console.ReadKey();
            Console.WriteLine("Nepřítel lape po dechu, dusí se.");
            Console.ReadKey();
            Console.WriteLine("Nepřítel ztrácí " + monster.Health/3 + "životů!");
            monster.Health -= monster.Health/3;
        }
    }
    class KrabiceSušenek : Item
    {
        public KrabiceSušenek()
        {
            Name = "KRABICE SUŠENEK!";
            Description = "Pořád čokoládový, pořád moc dobrý!";
        }
        public override void SpecialAbility(ref Player player, Monster monster)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Mňam! Byl" + player.PrefferedSuffix + " jsi vyléčen" + player.PrefferedSuffix + " o 5 životů!");
            Console.ReadKey();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("S nově nabitými silami, můžeš provést další akci");
            player.health += 5;
            player.BonusTurns += 1;
        }
    }
    //SPECIÁLNÍ
    class KřesadlováPistole : Weapon 
    {
        public KřesadlováPistole() 
        {
            Name = "Křesadlová pistole";
            Description = "(síla 7, -2 akce) Silná, ale velmi pomalá pistol";
            TimePenalty = 2;
        }
        public override int Attack()
        {
            return 7;
        }
    }
    class Žebro : Weapon
    {
        public Žebro() 
        {
            Name = "Kostlivcovo žebro";
            Description = "(síla 3-5) Rychlé, nechutné, avšak překvapivě účinné";
        }
        public override int Attack()
        {
            Random r = new Random();
            return r.Next(3, 6);
        }
    }
    class StehenníKost : Weapon
    {
        public StehenníKost() 
        {
            Name = "Stehenní kost";
            Description = "(síla 5-7, -1 akce) Pomalý, nechutný, avšak překvapivě účinný";
            TimePenalty= 1;
        }
        public override int Attack()
        {
            Random r = new Random();
            return r.Next(5, 8);
        }
    }
    class Puška : Weapon 
    {
        public Puška() 
        {
            Name = "Puška";
            Description = "(síla 10, -2 akce) Velmi siilná, ale velmi pomalá";
            TimePenalty = 2;
        }
        public override int Attack()
        {
            return 10;
        }
    }
}
