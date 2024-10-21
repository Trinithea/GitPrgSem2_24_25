using Program.LootLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Program.BattlePhaseManager;
using Program.LootPhaseManager;
using Program.StoryPresenter;

namespace Program.MonsterManager
{
    interface IPassive{}
    class Monster
    {
        public string Name { get; protected set; }
        public int DodgeChance { get; protected set; } //šance na to že se hráč při útoku netrefí, v procentech (0%-100%)
        public int Health { get; set; }
        public char Suffix {  get; protected set; } //Koncovka pro správné skloňování při vypisování textu
        public int BonusTurns { get; set; }
        protected bool died;
        public Monster(int health) 
        {
               Health = health;
        }
        public virtual int Attack() 
        {
            return 1;
        }
        public virtual void SpecialAbility(ref Player player) { }
        public virtual void OnDeathAbility(Player player) { }
        public virtual void PassiveAbility(Player player) { }
        public bool Dodge()
        {
            Random r = new Random();
            if (r.Next(0, 101) <= DodgeChance) 
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
    }
    //MONSTRA - LES
    class Skřeťulátko : Monster
    {

        public Skřeťulátko() : base(3)
        {
            Name = "Skřeťulátko";
            DodgeChance = 20;
            Suffix = 'o';
        }
        public override int Attack()
        {
            return 1;
        }
    }
    class Trpaslík : Monster
    {

        public Trpaslík() : base(3)
        {
            Name = "Trpaslík";
            DodgeChance = 13;
        }
        public override int Attack()
        {
            return 2;
        }
    }
    class Skřet : Monster 
    {
        public Skřet() : base(9) 
        {
            Name = "Skřet";
        }
        public override int Attack()
        {
            return 1;
        }
    }
    class Pavouk : Monster 
    {
        public Pavouk() : base(6)
        {
            Name = "Pavouk";
        }
        public override int Attack()
        {
            return 2;
        }
        public override void SpecialAbility(ref Player player)
        {
            Random r = new Random();
            switch (r.Next(1, 7)) 
            {
                case 1:
                    Console.WriteLine("Pavouk tě zamotal do své pavučiny!");
                    Console.WriteLine("Pavuk útočí 2x po sobě!");
                    Console.WriteLine("Monstrum útočí silou " + this.Attack() + "!");

                    if (player.armor.Defence != 0 && this.Attack() > 1)
                    {
                        if (this.Attack() - player.armor.Defence <= 0)
                        {
                            player.health -= 1;
                            Console.WriteLine("Tvé brnění útok redukovalo na 1");
                        }
                        else
                        {
                            int temp = this.Attack() - player.armor.Defence;
                            player.health -= this.Attack();
                            player.health += player.armor.Defence;
                            Console.WriteLine("Tvé brnění útok redukovalo na " + temp);
                        }
                    }
                    else
                    {
                        player.health -= this.Attack();
                    }
                    break;
                default: break;
            }
        }
    }
    class Zmije : Monster 
    {
        public Zmije() : base(8)
        {
            Name = "Veliká Zmije";
            Suffix = 'a';
        }
        public override int Attack()
        {
            Random r = new Random();
            return r.Next(1,3);
        }
    }
    class Sliz : Monster
    {
        public Sliz() : base(5)
        {
            Name = "Zelený sliz";
        }
        public override int Attack()
        {
            return 1;
        }
        public override void SpecialAbility(ref Player player)
        {
            Random r = new Random();
            if (r.Next(1, 101 ) <= 15)
            {
                Random r2 = new Random();
                int temp = r2.Next(0, 2);
                if (player.weapons[temp].Name != "Pěst")
                {
                    WeaponDissolution(temp, ref player);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Sliz vcucl vaši ruku, ale hbitě jste ji vytáhli");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
        private void WeaponDissolution(int index, ref Player player)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Sliz do sebe vtáhl tvojí zbraň " + player.weapons[index].Name);
            Console.ReadKey();
            Console.WriteLine("S vystrašeným výrazem sleduješ sliz jak tvou zbraň v sobě rozpustil na atomy");
            Console.ForegroundColor = ConsoleColor.White;
            player.weapons[index] = new Pěst();
        }
    }
    class Sliz2 : Monster
    {
        public Sliz2() : base(5)
        {
            Name = "Modrý sliz";
        }
        public override int Attack()
        {
            return 2;
        }
        public override void SpecialAbility(ref Player player)
        {
            Random r = new Random();
            if (r.Next(1, 101) <= 15)
            {
                Random r2 = new Random();
                int temp = r2.Next(0, 2);
                if (player.armor.Name != "Holá hruď")
                {
                    ArmorDissolution(ref player);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Sliz se přicucl na tvoji holou hruď, ale neudržel se moc dlouho a spadl na zem.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
        private void ArmorDissolution(ref Player player)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Sliz na tebe skočil a přicucl se na tvé brnění " + player.armor.Name);
            Console.ReadKey();
            Console.WriteLine("S hrůzou sleduješ jak sliz v sobě rozpustil tvé brnění na atomy a pak z tebe spadl na zem");
            Console.ForegroundColor = ConsoleColor.White;
            player.armor = new Nahý();
        }
    }
    class Masožravka : Monster
    {
        public Masožravka() : base(12)
        {
            Name = "Masožravka";
            Suffix = 'a';
        }
        public override int Attack() 
        {
            return 2;
        }
        public override void SpecialAbility(ref Player player)
        {
            Console.WriteLine("Masožravka tě schramstla do svého chřtánu");
            if (player.inventory.Exists(x => x.Name == "Lektvar léčení") || player.inventory.Exists(x => x.Name == "Velký lektvar léčení") || player.inventory.Exists(x => x.Name == "OBROVSKÝ LEKTVAR LÉČENÍ"))
            {
                Console.WriteLine("Ale masožravka ucítila zápach nechutného lektvaru léčení ve tvém inventáři a vyplivla tě ven");
            }
            else 
            {
                Console.WriteLine("Zub masožravky ti uděluje úder silou 2!");
                if (player.armor.Defence != 0)
                {
                    if (2 - player.armor.Defence <= 0)
                    {
                        player.health -= 1;
                        Console.WriteLine("Tvé brnění útok redukovalo na 1");
                    }
                }
                else
                {
                    player.health -= 2;
                }
            }

            
        }
    }
    class BandaSkřetů : Monster 
    {
        public BandaSkřetů() : base(12)
        {
            Name = "banda skřetů";
            Suffix = 'a';
        }
        public override int Attack()
        {
            return (this.Health / 4) + 1;
        }
        public override void OnDeathAbility(Player player)
        {
            LootPhase lp = new LootPhase();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Banda skřetů měla u sebe něco užitečného");
            Console.ForegroundColor = ConsoleColor.White;
            lp.StartSpecial(player);

        }
    }
    class Bandita : Monster 
    {
        public Bandita() : base(10)
        {
            Name = "Bandita";
        }
        public override int Attack()
        {
            Random r = new Random();
            return r.Next(1, 5);
        }
        public override void OnDeathAbility(Player player)
        {
            Random r = new Random();
            if(r.Next(1,5)== 1) 
            {
                Program program = new Program();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Bandita u sebe něco měl");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("| Křesadlová pistole | - (síla 7, -2 akce) Silná, ale velmi pomalá pistol");
                Console.WriteLine("Chceš si ji vzít? \n1. Ano\n2. Ne");
                WeaponSelection(program, player);
            }
        }
        private void WeaponSelection(Program program, Player player) 
        {
            int input = program.PlayerInput();
            switch (input)
            {
                case 1:
                    Console.WriteLine("Vyber si kterou zbraň chceš nahradit:");
                    int i = 1;
                    foreach (Loot weapon in player.weapons)
                    {
                        Console.WriteLine(i + ". " + weapon.Name);
                        i++;
                    }
                    int input2 = program.PlayerInput();

                    switch (input2)
                    {
                        case 1 or 2:
                            player.weapons[input2 - 1] = new KřesadlováPistole();
                            break;
                        default://pokud je hráčem zadaný input moc velký
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Takovou možnost nemáš. Vyber si znovu");
                            Console.ForegroundColor = ConsoleColor.White;
                            WeaponSelection(program, player);
                            break;
                    }
                    break;
                case 2:
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Takovou možnost nemáš. Vyber si znovu");
                    Console.ForegroundColor = ConsoleColor.White;
                    WeaponSelection(program, player);
                    break;
            }
        }
    }
    class ZakletáKočka : Monster 
    {
        public ZakletáKočka() : base(1) 
        {
            Name = "Zakletá kočka";
            Suffix = 'a';
            DodgeChance = 70;
        }
        public override void SpecialAbility(ref Player player)
        {
            Console.WriteLine("ZAKLETÁ KOČKA: Mňahahau!");
        }
    }
    class MiléDěvče : Monster, IPassive
    {
        public MiléDěvče() : base(1)
        {
            Name = "Milé děvče";
        }
        public override void PassiveAbility(Player player) 
        {
            Console.Write("Spatřil" + player.PrefferedSuffix + " jsi ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("milé děvče.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
            if (player.armor.Name == "Holá hruď")
            {
                Console.WriteLine("Děvče tě spatřilo též.");
                Console.ReadKey();
                Console.WriteLine("Jelikož na sobě nemáš žádné oblečení, tak pouze zakřičelo a uteklo");
                Console.ReadKey();
            }
            else 
            {
                Console.WriteLine("Děvče k tobě přišlo");
                Console.ReadKey();
                if (player.armor.Name == "Sluneční Brýle") { Console.WriteLine("Pochválilo ti sluneční brýle"); Console.ReadKey(); }
                Console.WriteLine("Beze slova děvče vytáhlo z košíku záhadný hezky vonící lektvar a dalo ti ho do ruky");
                Console.ReadKey();
                Console.WriteLine("Poté se jak přízrak opět vytratilo.");
                Console.ReadKey();
                Console.WriteLine("Letvar vypiješ. Chutná po jahodách");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Lektvar tě vyléčil na maximální počet životů. Maximální počet životů se ti zvýšil o 5!");
                Console.ForegroundColor = ConsoleColor.White;
                player.maxHealth += 5;
                player.health = player.maxHealth + player.armor.MaxHealthIncrease;
                Console.ReadKey();
            }
        }
    }
    class TemnýStín : Monster, IPassive
    {
        public TemnýStín() : base(1)
        {
            Name = "Temný stín";
        }
        public override void PassiveAbility(Player player)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Cestu ti zablokoval temný stín...");
            Console.ReadKey();
            Console.WriteLine("...");
            Console.ReadKey();
            Console.WriteLine("...");
            Console.ReadKey();
            Console.WriteLine("A je pryč...");
            Console.ReadKey();
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
    class BABAJAGA : Monster
    {
        public BABAJAGA() : base(15) 
        {
            Name = "BABA JAGA!";
            Suffix = 'a';
        }
        public override int Attack()
        {
            Random r = new Random();
            return r.Next(2, 6);
        }
        public override void SpecialAbility(ref Player player)
        {
            Random r = new Random();
            int temp = r.Next(1, 101);
            if (temp <= 15) 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Kolem Baby Jagy se objevila magická aura");
                Console.ReadKey();
                Console.WriteLine("Baba Jaga si vyléčila 3 životy!");
                Console.ReadKey();
                Console.WriteLine("BABA JAGA: " + VoiceLines(0, 4));
                Console.ReadKey();
                this.Health += 3;
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if(temp <= 25 && temp > 15) 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Baba Jaga vyčarovala ohnivou kouli a vrhla ji na tebe");
                Console.ReadKey();
                Console.WriteLine("Ztrácíš další 2 životy");
                Console.ReadKey();
                Console.WriteLine("BABA JAGA: " + VoiceLines(0, 4));
                Console.ReadKey();
                player.health -= 2;
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (temp <= 35 && temp > 25)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Baba Jaga pokazila zaklínadlo!");
                Console.ReadKey();
                Console.WriteLine("Baba Jaga ti vyléčila 3 životy!");
                Console.ReadKey();
                Console.WriteLine("BABA JAGA: " + VoiceLines(5, 7));
                Console.ReadKey();
                player.health += 3;
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        private string VoiceLines(int min = 0, int max = 7)
        {
            Random r = new Random();
            string[] voicelines = ["CHACHACHACHÁ!", "Kdopak mi to loupe perníček?", "CHICHICHICHI!", "Já ti ukážu Jeníčku!", "Já ti ukážu Mařenko!", "AUCHICHOUVEJ!","AJAJAJ!", "NEEEEE!"];
            return voicelines[r.Next(min, max+1)];
        }
        public override void OnDeathAbility(Player player)
        {
            Presenter storyPresnter = new Presenter();
            storyPresnter.Progress(player);
        }
    }
    class MEGASKŘET : Monster
    {
        public MEGASKŘET() : base(22)
        {
            Name = "MEGA SKŘET!";
        }
        public override int Attack()
        {
            Random r = new Random();
            return r.Next(3, 5);
        }
        public override void SpecialAbility(ref Player player)
        {
            Random r = new Random();
            int temp = r.Next(1, 101);
            if (temp <= 15)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Nemotorný skřet si ublížil svým vlastním kyjem");
                Console.ReadKey();
                Console.WriteLine("Mega skřet přišel o 1 život");
                Console.ReadKey();
                Console.WriteLine("MEGA SKŘET: " + VoiceLines(2, 3));
                Console.ReadKey();
                this.Health -= 1;
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if(temp <= 40 && temp > 15) 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("MEGA SKŘET: " + VoiceLines(0, 4));
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        private string VoiceLines(int min = 0, int max = 7)
        {
            Random r = new Random();
            string[] voicelines = ["Grrrghr...", "Grugnrhglrgrgrpgrlm...","GRUAAAAGH!", "GRAAAAAA!", "GUGUGAGA!"];
            return voicelines[r.Next(min, max + 1)];
        }
        public override void OnDeathAbility(Player player)
        {
            Presenter storyPresnter = new Presenter();
            storyPresnter.Progress(player);
        }
    }
    //MONSTRA - ROKLINA
    class Krysa : Monster
    {
        public Krysa() : base(7)
        {
            Name = "Nemrtvá krysa";
            Suffix = 'a';
            DodgeChance = 20;
            died = false;
        }
        public override int Attack()
        {
            if (died == false)
            {
                return 4;
            }
            else 
            {
                return 3;
            }
            
        }
        public override void OnDeathAbility(Player player)
        {
            
            switch (died)
            {
                case true:
                    break;
                case false:
                    Random r = new Random();
                    switch (r.Next(1,5))
                    {
                        case 1:
                            this.died = true;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.ReadKey();
                            Console.WriteLine("Krysa chvíli nehybně ležela na štěrku, ale poté se začala opět hýbat");
                            Console.ReadKey();
                            Console.WriteLine("Krysa vstala z mrtvých!");
                            Console.ReadKey();
                            Console.ForegroundColor = ConsoleColor.White;
                            this.Health = 3;
                            break;
                    }
                    break;
            }
            
        }
    }
    class ObrovskáTarantule : Monster
    {
        public ObrovskáTarantule() : base(10)
        {
            Name = "Obrovská tarantule";
            Suffix = 'a';
        }
        public override int Attack()
        {
            return 5;
        }
        public override void SpecialAbility(ref Player player)
        {
            Random r = new Random();
            switch (r.Next(1, 7))
            {
                case 1:
                    Console.WriteLine("Obrovská tarantule tě zamotala do své pavučiny!");
                    Console.WriteLine("Obrovská tarantule útočí 2x po sobě!");
                    Console.WriteLine("Monstrum útočí silou " + this.Attack() + "!");
                    if (player.armor.Defence != 0 && this.Attack() > 1)
                    {
                        if (this.Attack() - player.armor.Defence <= 0)
                        {
                            player.health -= 1;
                            Console.WriteLine("Tvé brnění útok redukovalo na 1");
                        }
                        else
                        {
                            int temp = this.Attack() - player.armor.Defence;
                            player.health -= this.Attack();
                            player.health += player.armor.Defence;
                            Console.WriteLine("Tvé brnění útok redukovalo na " + temp);
                        }
                    }
                    else
                    {
                        player.health -= this.Attack();
                    }
                    break;
                default: break;
            }
        }
    }
    class Sliz3 : Monster
    {
        public Sliz3() : base(9)
        {
            Name = "Červený sliz";
        }
        public override int Attack()
        {
            return 4;
        }
        public override void SpecialAbility(ref Player player)
        {
            Random r = new Random();
            if (r.Next(1, 101) <= 20)
            {
                Random r2 = new Random();
                int temp = r2.Next(0, player.inventory.Count);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Červený otevřel svou pusu a začal sát jako vysavač");
                Console.ReadKey();
                Console.WriteLine("Přidržel" + player.PrefferedSuffix + " nejbližší skály, ale do slizova chřtánu vletěl předmět " + player.inventory[temp] );
                Console.ReadKey();
                Console.WriteLine("S panickým strachem sleduješ jak sliz v sobě rozpustil tvůj předmět na atomy");
                Console.ForegroundColor = ConsoleColor.White;
                player.inventory.RemoveAt(temp);
            }
        }
    }
    class Kostlivec : Monster
    {
        public Kostlivec() : base(15)
        {
            Name = "Kostlivec";
        }
        public override int Attack()
        {
            Random r = new Random();
            return r.Next(3, 6);
        }
        public override void OnDeathAbility(Player player)
        {
            Random r = new Random();
            if (r.Next(1, 3) == 1)
            {
                Program program = new Program();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Kostlivec u sebe něco měl");
                Console.ForegroundColor = ConsoleColor.White;
                switch (r.Next(1, 3)) 
                {
                    default:
                        Console.WriteLine("| Stehenní kost | - (síla 5-7, -1 akce) Pomalý, nechutný, avšak překvapivě účinný");
                        Console.WriteLine("Chceš si ho vzít? \n1. Ano\n2. Ne");
                        WeaponSelection(program, player, new StehenníKost());
                        break;
                    case 2:
                        Console.WriteLine("| Žebro | - (síla 3-5) Rychlé, nechutné, avšak překvapivě účinné");
                        Console.WriteLine("Chceš si ho vzít? \n1. Ano\n2. Ne");
                        WeaponSelection(program, player, new Žebro());
                        break;
                }
                
            }
        }
        private void WeaponSelection(Program program, Player player, Weapon weapon)
        {
            int input = program.PlayerInput();
            switch (input)
            {
                case 1:
                    Console.WriteLine("Vyber si kterou zbraň chceš nahradit:");
                    int i = 1;
                    foreach (Loot w in player.weapons)
                    {
                        Console.WriteLine(i + ". " + w.Name);
                        i++;
                    }
                    int input2 = program.PlayerInput();

                    switch (input2)
                    {
                        case 1 or 2:
                            player.weapons[input2 - 1] = weapon;
                            break;
                        default://pokud je hráčem zadaný input moc velký
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Takovou možnost nemáš. Vyber si znovu");
                            Console.ForegroundColor = ConsoleColor.White;
                            WeaponSelection(program, player, weapon);
                            break;
                    }
                    break;
                case 2:
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Takovou možnost nemáš. Vyber si znovu");
                    Console.ForegroundColor = ConsoleColor.White;
                    WeaponSelection(program, player, weapon);
                    break;
            }
        }
    }
    class Krkavci : Monster
    {
        public Krkavci() : base(7) 
        {
            Name = "Sedmero začarovaných krkavců";
            Suffix = 'o';
            DodgeChance = 20;

        }
        public override int Attack()
        {
            return this.Health;
        }
        public override void SpecialAbility(ref Player player)
        {
            if(this.Health < 6) 
            {
                Console.ForegroundColor= ConsoleColor.Red;
                Console.WriteLine("Krkavci se náhle zduplikovali!");
                Console.ReadKey();
                Console.WriteLine("Objevili se dva další krkavci! (+2 životy)");
                Console.ForegroundColor = ConsoleColor.White;
                this.Health += 2;

            }
            
        }
    }
    class HejnoSršní : Monster
    {
        public HejnoSršní() : base(12)
        {
            Name = "Hejno sršní";
            Suffix = 'o';
            DodgeChance = 30;
        }
        public override int Attack()
        {
            return 1;
        }
        public override void SpecialAbility(ref Player player)
        {
            Console.ForegroundColor=ConsoleColor.Red;
            Console.WriteLine("Pronikl do tebe sršní jed!");
            Console.WriteLine("Přicházíš o " + player.health/5 );
            Console.ForegroundColor = ConsoleColor.White;
            player.health -= player.health / 5;
        }

    }
    class BandaKostlivců : Monster
    {
        public BandaKostlivců() : base(18)
        {
            Name = "Banda kostlivců";
            Suffix = 'a';
        }
        public override int Attack()
        {
            return (this.Health / 4) + 1;
        }
        public override void OnDeathAbility(Player player)
        {
            LootPhase lp = new LootPhase();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Banda skřetů měla u sebe něco užitečného");
            Console.ForegroundColor = ConsoleColor.White;
            lp.StartSpecial(player);

        }
    }
    class Bandita2 : Monster
    {
        public Bandita2() : base(15)
        {
            Name = "Nemrtvý bandita";
            DodgeChance = 5;
            died = false;
        }
        public override int Attack()
        {
            if (died == false)
            {
                Random r = new Random();
                return r.Next(6, 8);
            }
            else
            {
                return 5;
            }

        }
        public override void OnDeathAbility(Player player)
        {

            switch (died)
            {
                case true:
                    Program program = new Program();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Bandita u sebe něco měl");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("| Puška | - (síla 10, -2 akce) Velmi siilná, ale velmi pomalá");
                    Console.WriteLine("Chceš si ji vzít? \n1. Ano\n2. Ne");
                    WeaponSelection(program, player);

                    break;
                case false:
                    Random r = new Random();
                    switch (r.Next(1,4))
                    {
                        case 1:
                            this.died = true;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.ReadKey();
                            Console.WriteLine("Bandita chvíli nehybně ležel na štěrku, ale poté se začal opět hýbat");
                            Console.ReadKey();
                            Console.WriteLine("Bandita vstal z mrtvých!");
                            Console.ReadKey();
                            Console.ForegroundColor = ConsoleColor.White;
                            this.Health = 9;
                            break;
                    }
                    break;
            }

        }
        private void WeaponSelection(Program program, Player player)
        {
            int input = program.PlayerInput();
            switch (input)
            {
                case 1:
                    Console.WriteLine("Vyber si kterou zbraň chceš nahradit:");
                    int i = 1;
                    foreach (Loot weapon in player.weapons)
                    {
                        Console.WriteLine(i + ". " + weapon.Name);
                        i++;
                    }
                    int input2 = program.PlayerInput();

                    switch (input2)
                    {
                        case 1 or 2:
                            player.weapons[input2 - 1] = new Puška();
                            break;
                        default://pokud je hráčem zadaný input moc velký
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Takovou možnost nemáš. Vyber si znovu");
                            Console.ForegroundColor = ConsoleColor.White;
                            WeaponSelection(program, player);
                            break;
                    }
                    break;
                case 2:
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Takovou možnost nemáš. Vyber si znovu");
                    Console.ForegroundColor = ConsoleColor.White;
                    WeaponSelection(program, player);
                    break;
            }
        }
    }
    class Mlha : Monster, IPassive
    {
        public Mlha() : base(1)
        {
            Name = "Šeptající mlha";
        }
        public override void PassiveAbility(Player player)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Najednou se okolo tebe zamlžilo...");
            Console.ReadKey();
            Console.WriteLine("Nic nevidíš...");
            Console.ReadKey();
            Console.WriteLine("Slyšíš šepot...");
            Console.ReadKey();
            Console.WriteLine("A je pryč...");
            Console.ReadKey();
            Console.WriteLine("A je pryč...");
            Console.ReadKey();
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
    class Ghúl : Monster
    {
        public Ghúl() : base(24)
        {
            Name = "Ghúl";
        }
        public override int Attack()
        {
            return 6;
        }

    }
    class Mimik : Monster
    {
        public Mimik() : base(12)
        {
            Name = "Mimik";
        }
        public override int Attack()
        {
            return 4;
        }
        public override void SpecialAbility(ref Player player)
        {
            if (player.inventory.Count > 3) 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Mimik cítí všechny ty šťavňaťoučké předměty ve tvém inventáři...");
                Console.WriteLine("Mimik získal odhodlání sebrat ti předměty a zaútočil znovu!");
                Console.WriteLine("Ztrácíš 3 životy!");
                player.health -= 3;
            }
        }
        public override void OnDeathAbility(Player player)
        {
            LootPhase lp = new LootPhase();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Mimik v sobě měl něco užitečného");
            Console.ForegroundColor = ConsoleColor.White;
            lp.StartSpecial(player);

        }
    }
    class NEKROMANCER : Monster
    {
        public int deathCount;
        public NEKROMANCER() : base(6)
        {
            Name = "NEKROMANCER!";
            deathCount = 0;
        }
        public override int Attack()
        {
            Random r = new Random();
            return r.Next(2, 5) + deathCount;
        }
        public override void SpecialAbility(ref Player player)
        {
            Random r = new Random();
            int temp = r.Next(1, 101);
            if (temp <= 50)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Z půdy se vynořila ruka kostlivce a chytla tě za nohu!");
                Console.ReadKey();
                Console.WriteLine("Padáš na zem a ztrácíš 2 životy!");
                Console.ReadKey();
                Console.WriteLine("NEKROMANCER: " + VoiceLines());
                Console.ReadKey();
                player.health -= 2;
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (temp <= 75 && temp > 50)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("NEKROMANCER: " + VoiceLines());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        private string VoiceLines(int min = 0, int max = 6)
        {
            Random r = new Random();
            string[] voicelines = ["Smrt mne nepřelstí jen tak!", "Já jsem pánem smrti!", "Zemřeš!", "Je tady nějak mrtvo...", "Celý svět se podrobí mé moci", "(vydává zvláštní suchý smích)", "(zachrčí)"];
            return voicelines[r.Next(min, max + 1)];
        }
        public override void OnDeathAbility(Player player)
        {
            deathCount++;
            if (deathCount < 6) 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Nekromancerovo kosti se začaly k sobě samy od sebe skládat!");
                Console.ReadKey();
                Console.WriteLine("Nekromancer vstal z mrtvých! Je unavenější, ale naštvanější!");
                this.Health = 6 - deathCount;
                Console.ReadKey();
                int temp = this.Attack();
                Console.WriteLine("Nekromancer útočí silou " + (temp) + "!");
                if (player.armor.Defence != 0 && temp > 1)
                {
                    if (temp - player.armor.Defence <= 0)
                    {
                        player.health -= 1;
                        Console.WriteLine("Tvé brnění útok redukovalo na 1");
                    }
                    else
                    {
                        int temp2 = temp - player.armor.Defence;
                        player.health -= temp;
                        player.health += player.armor.Defence;
                        Console.WriteLine("Tvé brnění útok redukovalo na " + temp2);
                    }
                }
                else
                {
                    player.health -= temp;
                }
                Console.WriteLine("NEKROMANCER: " + VoiceLines());
                Console.ReadKey();
                Console.ForegroundColor = ConsoleColor.White;


            }
            else 
            {
                Presenter storyPresnter = new Presenter();
                storyPresnter.EndGame1(player);
            }
        }
    }
    class ARACHNOFILIA : Monster
    {
        public ARACHNOFILIA() : base(40)
        {
            Name = "ARACHNOFILIA!";
            DodgeChance = 10;
            Suffix = 'a';
        }
        public override int Attack()
        {
            return 7;
        }
        public override void SpecialAbility(ref Player player)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Random r = new Random();
            switch (r.Next(1, 7))
            {
                case 1:
                    Console.WriteLine("Tento gigantický pavouk tě zamotal do své pavučiny!");
                    Console.WriteLine("Arachnofilia útočí 2x po sobě!");
                    Console.WriteLine("Monstrum útočí silou " + this.Attack() + "!");
                    if (player.armor.Defence != 0 && this.Attack() > 1)
                    {
                        if (this.Attack() - player.armor.Defence <= 0)
                        {
                            player.health -= 1;
                            Console.WriteLine("Tvé brnění útok redukovalo na 1");
                        }
                        else
                        {
                            int temp = this.Attack() - player.armor.Defence;
                            player.health -= this.Attack();
                            player.health += player.armor.Defence;
                            Console.WriteLine("Tvé brnění útok redukovalo na " + temp);
                        }
                    }
                    else
                    {
                        player.health -= this.Attack();
                    }
                    Console.WriteLine("ARHACNOFILIA: " + VoiceLines());
                    break;
                case 2:
                    Console.WriteLine("ARACHNOFILIA: " + VoiceLines());
                    break;
                default: break;
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        private string VoiceLines(int min = 0, int max = 6)
        {
            Random r = new Random();
            string[] voicelines = ["chssss...", "(Zastrašuje tě svými kusadly)", "sSSsssSsss...", "(Zvedá nohy do vzduchu)", "(zachrčí)", "(vyleze na skálu)"];
            return voicelines[r.Next(min, max + 1)];
        }
        public override void OnDeathAbility(Player player)
        {
            Presenter storyPresnter = new Presenter();
            storyPresnter.EndGame1(player);
        }
    }
    class PoskociBaltazara : Monster
    {
        public PoskociBaltazara() : base(45) 
        {
            Name = "poskoci Baltazara";
            Suffix = 'i';
        }
        public override int Attack()
        {
            return this.Health/5;
        }
        public override void SpecialAbility(ref Player player)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Random r = new Random();
            switch (r.Next(1, 7))
            {
                case 1:
                    Console.WriteLine("Jeden z poskoků Baltazara po tobě hodil půl litr s pivem!");
                    Console.WriteLine("Rozbil se ti o hlavu!");
                    Console.WriteLine("Ztrácíš 2 životy!");
                    player.health -= 2;
                    Console.WriteLine("JEDEN Z POSKOKŮ: " + VoiceLines(2,3));

                    break;
                case 2:
                    Console.WriteLine("JEDEN Z POSKOKŮ: " + VoiceLines());
                    break;
                case 3:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Hospodský si nenechá líbit takovýhle bordel ve své hospodě!");
                    Console.WriteLine("Hospodský vytáhl loveckou brokovnici a vystřelil po poskocích Baltazara!");
                    Console.WriteLine("Poskoci Baltazara ztrácí 4 životy!");
                    Console.WriteLine("HOSPODSKÝ: Zabíjíte mi zákazníky vy magoři!");
                    Console.ForegroundColor = ConsoleColor.White;
                    this.Health -= 4;
                    break;
                case 4:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Hospodský si nenechá líbit takovýhle bordel ve své hospodě!");
                    Console.WriteLine("Hospodský vytáhl loveckou brokovnici a vystřelil po tobě!");
                    Console.WriteLine("Ztrácíš 4 životy!");
                    Console.WriteLine("HOSPODSKÝ: Zabíjíš mi zákazníky ty magore!");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                default: break;
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        private string VoiceLines(int min = 0, int max = 6)
        {
            Random r = new Random();
            string[] voicelines = ["Ty se odvažuješ se tu ještě ukázat?", "Tohle bude zábava", "Chutná ti?" ,"Kvůli tobě se mi vylilo pivo! >:(", "Dostaneš do držky!", "Copak. V lese se ti nelíbilo?", "(plivne na zem)"];
            return voicelines[r.Next(min, max + 1)];
        }
        public override void OnDeathAbility(Player player)
        {
            Presenter storyPresenter = new Presenter();
            storyPresenter.EndGame2(player);
        }
    }
    class Baltazar : Monster
    {
        public Baltazar() : base(50)
        {
            Name = "BALTAZAR GOSSE!";
        }
        public override int Attack()
        {
            return 6;
        }
        public override void SpecialAbility(ref Player player)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Random r = new Random();
            switch (r.Next(1, 7))
            {
                case 1:
                    Console.WriteLine("Baltazar vypil lektvar léčení");
                    Console.WriteLine("Baltazar získal 4 životy!");
                    this.Health += 4;
                    Console.WriteLine("BALTAZAR: " + VoiceLines());

                    break;
                case 2:
                    Console.WriteLine("BALTAZAR: " + VoiceLines());
                    break;
                case 3:

                    Console.WriteLine("Baltazar na tebe zaútočil malou ostrou sekyrou, která se prodrala skrz tvou zbroj!");
                    Console.WriteLine("Ztrácíš 3 životy!");
                    Console.WriteLine("BALTAZAR: " + VoiceLines());
                    player.health -= 3;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Tento útok byl však pomalý. Příští tah máš akci navíc!");
                    Console.ForegroundColor = ConsoleColor.White;
                    player.BonusTurns += 1;
                    break;
                case 4:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Ve svém domě se vyznáš a vytáhneš skrytý lektvar léčení!");
                    player.inventory.Add(new ObrovskýLektvarLéčení());
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                default: break;
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        public override void OnDeathAbility(Player player)
        {
            Presenter storyPresenter = new Presenter();
            storyPresenter.Victory(player);
        }
        private string VoiceLines(int min = 0, int max = 6)
        {
            Random r = new Random();
            string[] voicelines = ["Tady teď bydlím já!", "Já vlastním tohle město!", "Můj otec potrestá celou tvoji rodinu!", "Ha! Teď to začíná být zábavný!", "Ani se nemusíš snažit!", "Rovnou se vzdej!", "Jsi pod mojí úroveň!"];
            return voicelines[r.Next(min, max + 1)];
        }
    }

}
