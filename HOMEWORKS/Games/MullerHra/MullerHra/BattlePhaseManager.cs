using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program.MonsterManager;
using Program.LootLibrary;
using Program;
using RM;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata.Ecma335;

namespace Program.BattlePhaseManager
{
    class BattlePhase
    {
        Program program = new Program();
        RandomMessages RandomMessage = new RandomMessages();
        public int turnNumber {  get; private set; }
        public void StartBattle(Player player, int roundCount)
        {
            turnNumber = 1;
            Console.Clear();
            Monster activeMonster = SelectActiveMonster(roundCount, player.levelNumber);
            if (activeMonster is IPassive) 
            {
                activeMonster.PassiveAbility(player);
            }
            else 
            {
                Console.Write(RandomMessage.MonsterSpawn(activeMonster) + " ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(activeMonster.Name);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine( roundCount + ". střetnutí");
                Console.ReadKey();
                BattleTurn(player, activeMonster);
            }
        }
        //PRŮBĚH BITVY
        private void BattleTurn(Player player, Monster activeMonster) 
        {
            if (player.health > 0) 
            {
                Console.Clear();
                //vypsání statistik
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n Máš " + player.health + "♥/" + (player.maxHealth + player.armor.MaxHealthIncrease) + "♥ | ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(activeMonster.Name + " má " + activeMonster.Health + "♥");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Tvé brnění : " + player.armor.Name + " --- " + player.armor.Description);

                //tah hráče
                int WeaponTimePenalty = 0;
                PlayerTurn(ref player, activeMonster, ref WeaponTimePenalty);
                if (player.BonusTurns + player.armor.BonusTurns - WeaponTimePenalty > 0)
                {
                    for (int i = 0; i < player.BonusTurns + player.armor.BonusTurns - WeaponTimePenalty; i++)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("\n Máš " + player.health + "♥ | ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(activeMonster.Name + " má " + activeMonster.Health + "♥");
                        Console.ForegroundColor = ConsoleColor.White;

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Můžeš si vybrat extra možnost!");
                        Console.ForegroundColor = ConsoleColor.White;
                        PlayerTurn(ref player, activeMonster, ref WeaponTimePenalty);
                    }
                }
                player.BonusTurns = 0;


                //tah monstra
                if (activeMonster.Health > 0)
                {
                    //monstrum zaútočí tolikrát, kolik má bonusovýh tahů nebo podle penalty na použité zbrani hráče

                    for (int i = 0; i <= WeaponTimePenalty; i++)
                    {
                        int monsterAttack = activeMonster.Attack();
                        Console.Write("Monstrum útočí silou ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(+monsterAttack + "!");
                        Console.ForegroundColor = ConsoleColor.White;
                        if (player.armor.Defence != 0 && monsterAttack > 1)
                        {
                            if (monsterAttack - player.armor.Defence <= 0)
                            {
                                player.health -= 1;
                                Console.Write("Tvé brnění útok redukovalo na ");
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("1");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else
                            {
                                int temp = monsterAttack - player.armor.Defence;
                                player.health -= monsterAttack;
                                player.health += player.armor.Defence;
                                Console.Write("Tvé brnění útok redukovalo na ");
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine(+temp);
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                        }
                        else
                        {
                            player.health -= monsterAttack;
                        }
                        activeMonster.SpecialAbility(ref player);

                        Console.ReadKey();
                        turnNumber++;
                    }

                    activeMonster.BonusTurns = 0;

                    //smrt hráče
                    if (player.health <= 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Máš " + player.health + "♥ \n" + activeMonster.Name + " " + RandomMessage.PlayerDeathMessage(activeMonster) + "\nTvá cesta zde končí");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        BattleTurn(player, activeMonster);
                    }

                }
                //smrt monstra
                else if (activeMonster.Health <= 0)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(activeMonster.Name + " " + RandomMessage.MonsterDeathMessage(activeMonster));
                    Console.ForegroundColor = ConsoleColor.White;
                    activeMonster.OnDeathAbility(player);
                    if (activeMonster.Health > 0) //Pokud monstrum vstane z mrtvých
                    {
                        BattleTurn(player, activeMonster);
                    }
                    else
                    {
                        Console.ReadKey();
                    }
                }
            }
            else //smrt hráče pro specifický příklad při smrti u Nekromancera
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Máš " + player.health + "♥ \n" + activeMonster.Name + " " + RandomMessage.PlayerDeathMessage(activeMonster) + "\nTvá cesta zde končí");
                Console.ForegroundColor = ConsoleColor.White;
            }
            
        }

        // TAH HRÁČE
        private void PlayerTurn(ref Player player, Monster activeMonster, ref int wtp) 
        {
            Console.WriteLine("Tvé možnosti:");
            Console.WriteLine("1. || " + player.weapons[0].Name + " || --- " + player.weapons[0].Description
                + "\n2. || " + player.weapons[1].Name + " || --- " + player.weapons[1].Description);
            int i = 3;
            foreach (Loot loot in player.inventory) 
            {
                Console.WriteLine(i + ". || " + loot.Name + " || --- " + loot.Description);
                i++;
            }
            int input = program.PlayerInput();
            
            if (input == 1 || input == 2)
            { //pokud si hráč vybral zaútočit zbraní
                int playerAttack = player.weapons[input - 1].Attack();
                Console.Write("Útočíš pomocí zbraně " + player.weapons[input - 1].Name + " silou ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine( + playerAttack);
                Console.ForegroundColor = ConsoleColor.White;
                if (activeMonster.DodgeChance != 0) 
                {
                    Console.ReadKey();
                    switch (activeMonster.Dodge()) 
                    {
                        case true:
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            Console.WriteLine(activeMonster.Name + " se však útoku vyhnul" + activeMonster.Suffix);
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case false:
                            Console.WriteLine("Trefa!");
                            activeMonster.Health -= playerAttack;
                            player.weapons[input - 1].SpecialAbility(ref player, activeMonster);
                            break;
                    }
                }
                else 
                {
                    Console.WriteLine("Trefa!");
                    activeMonster.Health -= playerAttack;
                    player.weapons[input - 1].SpecialAbility(ref player, activeMonster);
                }



                wtp = player.weapons[input - 1].TimePenalty; //nastaví se časová penalta podle zbraně která byla použita
            }
            else if (input - 3 < player.inventory.Count && input > 0)
            { //pokud si hráč vybral využít nějaký předmět
                player.inventory[input - 3].SpecialAbility(ref player, activeMonster);
                player.inventory.RemoveAt(input - 3);
                Console.ReadKey();
            }
            else 
            { //pokud je hráčem zadaný input moc velký nebo moc malý
                PlayerTurn(ref player, activeMonster, ref wtp);
            }

            player.armor.SpecialAbility(ref player, activeMonster);
            Console.ReadKey();
        }

        

        //Výběr aktivního monstra
        private Monster SelectActiveMonster(int roundCount, int levelNumber)
        {
            Random r = new Random();
            int[] monsterSelect = [r.Next(1,4),r.Next(1, 6),r.Next(3, 8),r.Next(5, 10),r.Next(8, 11),r.Next(1, 14), r.Next(10, 14), r.Next(14, 16)];

            int[] monsterSelect2 = [r.Next(1, 4), r.Next(1, 6), r.Next(3, 7), r.Next(5, 8), r.Next(7, 11), r.Next(1, 12), r.Next(9, 12), r.Next(12,14)];

            switch (levelNumber) 
            {
                default:
                    switch (monsterSelect[roundCount - 1]) //selekce monster se mění na základě kolikáté kolo hraješ
                    {
                        default: //nebo-li pokud to bude 1 (visual studio z nějakýho důvodu křičí když tady je "case 1")
                            return new Skřeťulátko();
                        case 2:
                            return new Trpaslík();
                        case 3:
                            return new Sliz();
                        case 4:
                            return new Sliz2();
                        case 5:
                            return new Pavouk();
                        case 6:
                            return new Skřet();
                        case 7:
                            return new MiléDěvče();
                        case 8:
                            return new Bandita();
                        case 9:
                            return new Zmije();
                        case 10:
                            return new Masožravka();
                        case 11:
                            return new TemnýStín();
                        case 12:
                            return new BandaSkřetů();
                        case 13:
                            return new ZakletáKočka();
                        case 14:
                            return new BABAJAGA();
                        case 15:
                            return new MEGASKŘET();
                    }
                case 2:
                    //monsterSelect2[roundCount - 9]
                    switch (monsterSelect2[roundCount - 9]) //selekce monster se mění na základě kolikáté kolo hraješ
                    {
                        default: //nebo-li pokud to bude 1 (visual studio z nějakýho důvodu křičí když tady je "case 1")
                            return new Krysa();
                        case 2:
                            return new Sliz3();
                        case 3:
                            return new ObrovskáTarantule();
                        case 4:
                            return new Kostlivec();
                        case 5:
                            return new Krkavci();
                        case 6:
                            return new HejnoSršní();
                        case 7:
                            return new BandaKostlivců();
                        case 8:
                            return new Mlha();
                        case 9:
                            return new Bandita2();
                        case 10:
                            return new Ghúl();
                        case 11:
                            return new Mimik();
                        case 12:
                            return new NEKROMANCER();
                        case 13:
                            return new ARACHNOFILIA();
                    }
                case 3:
                    return new PoskociBaltazara();
                    break;
                case 4:
                    return new Baltazar();
            }
            //monsterSelect[roundCount - 1]
            
        }


    }
}
