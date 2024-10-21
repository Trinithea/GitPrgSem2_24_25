using Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program.LootLibrary;
using Program.MonsterManager;
using RM;

namespace Program.LootPhaseManager
{
    class LootPhase
    {
        Program program = new Program();
        LootList lootLibrary = new LootList();
        RandomMessages RandomMessage = new RandomMessages();
        public void Start(Player player)
        {
            Console.Clear();
            List<Loot> LootPool = new List<Loot>();
            CreateLootPool(3, LootPool, player);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Tvé brnění : " + player.armor.Name + " --- " + player.armor.Description);
            Console.WriteLine("Tvé zbraně:");
            Console.WriteLine("1. || " + player.weapons[0].Name + " || --- " + player.weapons[0].Description
                + "\n2. || " + player.weapons[1].Name + " || --- " + player.weapons[1].Description);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(RandomMessage.LootSpawn(player.levelNumber));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Vyber si jeden předmět: ");
            PlayerLootSelection(player, LootPool);
        }
        public void StartSpecial(Player player)
        {
            Console.WriteLine("Tvé brnění : " + player.armor.Name + " --- " + player.armor.Description);
            Console.WriteLine("Tvé zbraně:");
            Console.WriteLine("1. || " + player.weapons[0].Name + " || --- " + player.weapons[0].Description
                + "\n2. || " + player.weapons[1].Name + " || --- " + player.weapons[1].Description);
            List<Loot> LootPool = new List<Loot>();
            CreateLootPool(2, LootPool, player);
            Console.WriteLine("Vyber si jeden předmět: ");
            PlayerLootSelection(player, LootPool);
        }
        private void PlayerLootSelection(Player player, List<Loot> LootPool)
        {
            int i = 1;
            foreach (Loot loot in LootPool) //Výpis Lootu
            {
                Console.WriteLine(i + ". | " + LootPool[i - 1].Name + " | --- " + LootPool[i - 1].Description);
                i++;
            }

            i = 1;
            int input = program.PlayerInput();
            if (input > LootPool.Count || input < 1)
            { //pokud je hráčem zadaný input moc velký
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Takovou možnost nemáš. Vyber si znovu");
                Console.ForegroundColor = ConsoleColor.White;
                PlayerLootSelection(player, LootPool);

            }
            else 
            {
                switch(LootPool[input - 1].Type) 
                {
                    case "Weapon":
                        Console.WriteLine("Vyber si kterou zbraň chceš nahradit:");
                        foreach (Loot weapon in player.weapons)
                        {
                            Console.WriteLine(i + ". " + weapon.Name);
                            i++;
                        }
                        int input2 = program.PlayerInput();
                        switch (input2)
                        {
                            case 1 or 2:
                                player.weapons[input2 - 1] = (Weapon)LootPool[input - 1];
                                break;
                            default://pokud je hráčem zadaný input moc velký
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Takovou možnost nemáš. Vyber si znovu");
                                Console.ForegroundColor = ConsoleColor.White;
                                PlayerLootSelection(player, LootPool);
                                break;
                        }
                        break;
                    case "Armor":
                        player.armor = (Armor)LootPool[input - 1];
                        player.health += player.armor.MaxHealthIncrease;
                        if (player.health > player.maxHealth + player.armor.MaxHealthIncrease)
                        { player.health = player.maxHealth + player.armor.MaxHealthIncrease; }
                        break;
                    case "Item":
                        player.inventory.Add((Item)LootPool[input - 1]);
                        break;
                }
            }
        }
        private void CreateLootPool(int LootOptionCount, List<Loot> LootPool, Player player)
        {
            for (int i = 0; i < LootOptionCount; i++)
            {
                LootPool.Add(AddLootToLootPool(player, player.levelNumber));
            }
        }

        private Loot AddLootToLootPool(Player player, int levelNumber)
        {
            switch (player.levelNumber)
            {
                case 1:
                    Random rLoot = new Random();
                    switch (rLoot.Next(1, 25))
                    {
                        default: //nebo-li pokud to bude 1 (visual studio z nějakýho důvodu křičí když tady je "case 1")
                            return new Dýka();
                        case 2:
                            return new Meč();
                        case 3:
                            return new ŽeleznáZbroj();
                        case 4:
                            return new SportovníLegíny();
                        case 5:
                            return new LektvarLéčení();
                        case 6:
                            return new VelkýLektvarLéčení();
                        case 7:
                            return new ObrovskýLektvarLéčení();
                        case 8:
                            return new Příšerák();
                        case 9:
                            return new Větev();
                        case 10:
                            return new RezavýNůž();
                        case 11:
                            return new Meč2();
                        case 12:
                            return new Palcát();
                        case 13:
                            return new KoženáTunika();
                        case 14:
                            return new KroužkováZbroj();
                        case 15:
                            return new SlunečníBrýle();
                        case 16:
                            return new KouzelnýHábit();
                        case 17:
                            return new ProkletáZbroj();
                        case 18:
                            return new Šutr();
                        case 19:
                            return new Balvan();
                        case 20:
                            return new Dynamit();
                        case 21:
                            return new Sušenka();
                        case 22:
                            return new PlechovkaSušenek();
                        case 23:
                            return new KouzelnáJahoda();
                        case 24:
                            return new Kroksy();
                    }
                default:
                    Random rLoot2 = new Random();
                    switch (rLoot2.Next(1, 25))
                    {
                        default: //nebo-li pokud to bude 1 (visual studio z nějakýho důvodu křičí když tady je "case 1")
                            return new Kyj();
                        case 2:
                            return new Šavle();
                        case 3:
                            return new Sekyra();
                        case 4:
                            return new Řemdich();
                        case 5:
                            return new KouzelnáHůl();
                        case 6:
                            return new KouzelnáHůl2();
                        case 7:
                            return new KouzelnáHůl3();
                        case 8:
                            return new Kuše();
                        case 9:
                            return new ProkletýHábit();
                        case 10:
                            return new ZlatáZbroj();
                        case 11:
                            return new BrněníProKoně();
                        case 12:
                            return new OstráZbroj();
                        case 13:
                            return new DiamantováZbroj();
                        case 14:
                            return new Kšandy();
                        case 15:
                            return new Exoskeleton();
                        case 16:
                            return new GigantickáZbroj();
                        case 17:
                            return new ObrovskýLektvarLéčení();
                        case 18:
                            return new VelkýLektvarLéčení();
                        case 19:
                            return new Loutna();
                        case 20:
                            return new PříšerákXXL();
                        case 21:
                            return new GigaBalvan();
                        case 22:
                            return new BalíkDynamitu();
                        case 23:
                            return new JedovatýPrach();
                        case 24:
                            return new Puška();
                    }
            }
        }
    }
}
