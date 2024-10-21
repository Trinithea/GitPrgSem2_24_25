using System;
using System.Collections.Generic;
using System.Media;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Xml.Linq;

namespace fantasy_game
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Begin();
        }
    }

    class Game
    {
        private GoodGuy player;
        private List<BadGuy> baddies = new List<BadGuy>
        {
            new LoganusPaul(25, null),
            new KSI(15, null),
            new MasterBeast(40, "beast lair")
        };

        List<string> places = new List<string> { "magical forest", "castle", "old tavern", "mansion", "the circle", "travelling cart", "beast lair" };
        private List<string> visited = new List<string>();
        private Merchant merchant;
        private List<string> taken = new List<string>();

        public Game()
        {
            Console.WriteLine("Choose your hero (Knight, Monkey, Fairy): ");
            string choice = Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine($"Name your {choice}:");
            string name = Console.ReadLine();

            switch (choice)
            {
                case "Knight":
                    player = new Knight(name);
                    break;
                case "Monkey":
                    player = new Monkey(name);
                    break;
                case "Fairy":
                    player = new Fairy(name);
                    break;
                default:
                    Console.WriteLine("Invalid hero choice. Defaulting to Knight.");
                    player = new Knight(name);
                    break;
            }
            Console.WriteLine("---===---===---");
            Console.WriteLine();
            Console.WriteLine("Welcome to BeastLandia! Your goal is to defeat the looming monster that sits in the Beast Lair... Be careful, traveller!");
            Console.WriteLine();

            Random rand = new Random();
            merchant = new Merchant();

            foreach (BadGuy badguy in baddies)
            {
                if (badguy is MasterBeast) { continue; }
                string randPlace = places[rand.Next(places.Count - 2)]; // all places except Beast Lair and Travelling Cart
                while (taken.Contains(randPlace))
                {
                    randPlace = places[rand.Next(places.Count - 2)];
                }
                taken.Add(randPlace);
                badguy.Place = randPlace;
            }
        }

        public void Begin()
        {

            bool ongoing = true;
            Console.WriteLine($"{player.Name}: BeastLandia.... What a strange but beautiful place. I suspect there may be more obstacles before I get to the Lair... I should prepare.");
            Console.WriteLine();
            while (ongoing)
            {
                if (player.Health <= 0)
                {
                    ongoing = false;
                    break;
                }
                if (player.Experience == 420)
                {
                    Console.WriteLine("You won!");
                    ongoing = false;
                    break;
                }

                Console.WriteLine();
                Console.WriteLine(player.ToString());
                Console.WriteLine();
                Console.WriteLine("Where would you like to go? Your options are - " + string.Join(", ", places));
                string chosenPlace = Console.ReadLine().ToLower();
                while (!places.Contains(chosenPlace))
                {
                    Console.WriteLine("Invalid town input. Please choose from the options.");
                    chosenPlace = Console.ReadLine();
                }
                Console.WriteLine();
                Console.WriteLine("---===---===---");
                Console.WriteLine($"{player.Name} is headed to {chosenPlace} ...");
                Console.WriteLine("---===---===---");
                BadGuy baddieLocated = baddies.Find(b => b.Place == chosenPlace);

                if (baddieLocated != null)
                {
                    Console.WriteLine($"Oh no! It seems like there is company in {chosenPlace}...");
                    Console.WriteLine();
                    Console.WriteLine("Do you want to (1) continue to fight or (2) run away?");
                    int actionChoice = int.Parse(Console.ReadLine());
                    Console.WriteLine();

                    if (actionChoice == 1)
                    {
                        Console.WriteLine();
                        Console.WriteLine(baddieLocated.ToString());
                        Console.WriteLine();
                        player.Fight(baddieLocated);
                    }
                    else
                    {
                        Console.WriteLine($"{player.Name} ran away like a little kid!");
                    }
                }
                else if (chosenPlace == "travelling cart")
                {
                    Console.WriteLine("You find a merchant selling various items.");
                    merchant.OpenShop(player);
                }
                else
                {
                    Console.WriteLine("The place is quiet and peaceful.");
                    if (visited.Contains(chosenPlace))
                    {
                        Console.WriteLine();
                        Console.WriteLine("You've already been here. There is nothing left.");
                    }
                    else
                    {
                        Random mony = new Random();
                        int found = mony.Next(1, 11);
                        Console.WriteLine();
                        Console.WriteLine($"You found {found} Beast Coins!");
                        visited.Add(chosenPlace);
                        player.Money += found;
                    }
                }
            }
        }
    }

    class Merchant
    {
        private List<Item> inventory = new List<Item>
        {
            new Item("Throwing Knife", 10, "A knife that deals 3 damage to your enemy."),
            new Item("Health Potion", 15, "Heals 5 health points."),
            new Item("MrBeast Bar", 30, "Permanently applies double damage to all your attacks.")
        };

        public void OpenShop(GoodGuy player)
        {
            Console.WriteLine("---===---===---");
            Console.WriteLine("Welcome to my shop! Here's what I have for sale:");
            Console.WriteLine();
            for (int i = 0; i < inventory.Count; i++)
            {
                Item item = inventory[i];
                Console.WriteLine($"{i + 1}. {item.Name} - {item.Price} Beast Coins: {item.Description}");
            }
            Console.WriteLine();
            Console.WriteLine($"You have {player.Money} Beast Coins.");
            Console.WriteLine("---===---===---");
            while (true)
            {
                Console.WriteLine("Enter the number of the item you want to buy, or type 0 to leave:");
                int choice = int.Parse(Console.ReadLine());
                Console.WriteLine("---===---===---");

                if (choice > 0 && choice <= inventory.Count)
                {
                    Item selectedItem = inventory[choice - 1];
                    if (player.Money >= selectedItem.Price)
                    {
                        player.Money -= selectedItem.Price;
                        player.AddToInventory(selectedItem);
                        Console.WriteLine($"You purchased {selectedItem.Name}. You now have {player.Money} Beast Coins left.");
                        Console.WriteLine("---===---===---");
                    }
                    else
                    {
                        Console.WriteLine("You don't have enough money for this item.");
                        Console.WriteLine("---===---===---");
                    }
                }
                else
                {
                    Console.WriteLine("Leaving the shop.");
                    Console.WriteLine("---===---===---");
                    break;
                }
            }
        }
    }

    class Item
    {
        public string Name { get; }
        public int Price { get; }
        public string Description { get; }

        public Item(string name, int price, string description)
        {
            Name = name;
            Price = price;
            Description = description;
        }

        public void ApplyEffect(GoodGuy player, BadGuy badguy)
        {
            switch (Name)
            {
                case "Throwing Knife":
                    Console.WriteLine("---===---===---");
                    Console.WriteLine($"You throw a knife, dealing 3 damage to {badguy.Race}!");
                    Console.WriteLine("---===---===---");
                    badguy.Health -= 3;
                    break;
                case "Health Potion":
                    Console.WriteLine("---===---===---");
                    Console.WriteLine("You drink a health potion and restore 5 health!");
                    Console.WriteLine("---===---===---");
                    player.Health += 5;
                    break;
                case "MrBeast Bar":
                    Console.WriteLine("---===---===---");
                    Console.WriteLine("You eat the MrBeast bar... You feel... stronger?");
                    Console.WriteLine("---===---===---");
                    player.BeastEffect = true;
                    break;
            }
        }
    }

    abstract class Character
    {
        public Character(string race)
        {
            Race = race;
        }

        public string Race { get; }
        public int Health { get; internal set; }
        public int Experience { get; internal set; }
    }

    abstract class GoodGuy : Character
    {
        public GoodGuy(string name, int experience, string race) : base(race)
        {
            Name = name;
            Experience = experience;
            Health = 25;
            Inventory = new List<Item>();
        }

        public string Name { get; }
        public int Money { get; set; } = 25;
        public bool BeastEffect { get; set; } = false;
        public List<Item> Inventory { get; }

        public void AddToInventory(Item item)
        {
            Inventory.Add(item);
        }

        public void Fight(BadGuy badguy)
        {
            bool usedEffect = false;
            bool battleOver = false;
            Random rand = new Random();

            while (!battleOver)
            {
                if (badguy.Health <= 4 && !usedEffect)
                {
                    Console.WriteLine("!!!!!!!!!");
                    badguy.LowHealthEffect(this);
                    usedEffect = true;
                    Console.WriteLine("!!!!!!!!!");
                    Console.WriteLine();

                }
                Console.WriteLine("---===---===---");
                Console.WriteLine($"Your health: {Health}");
                Console.WriteLine($"{badguy.Race} health: {badguy.Health}");
                Console.WriteLine("---===---===---");
                Console.WriteLine("Choose your action: (1) Attack, (2) Flee, (3) Use Item");
                if (this is Fairy)
                {
                    Console.WriteLine("(4) Use Spell");
                }

                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        Attack(badguy);
                        if (badguy.Health <= 0)
                        {
                            Console.WriteLine("---===---===---");
                            Console.WriteLine($"{badguy.Race} has been defeated!");
                            Console.WriteLine();
                            Console.WriteLine($"{badguy}:");
                            badguy.FightWon(this);
                            Console.WriteLine("---===---===---");
                            battleOver = true;
                        }
                        if (Health <= 0)
                        {
                            Console.WriteLine("---===---===---");
                            badguy.FightLost(this);
                            Console.WriteLine();
                            Console.WriteLine($"You have been defeated!");
                            battleOver = true;
                        }
                        break;

                    case 2:
                        Console.WriteLine($"{Name} ran away!");
                        Console.WriteLine($"{badguy}:");
                        badguy.PlayerRan(this);
                        Console.WriteLine($"{Name} lost 1 health and 5 money!");
                        Health -= 1;
                        Money -= 5;
                        battleOver = true;
                        break;

                    case 3:
                        if (Inventory.Count > 0)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Choose an item to use (0 if you want to return):");
                            for (int i = 0; i < Inventory.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {Inventory[i].Name}");
                            }
                            
                            int itemChoice = int.Parse(Console.ReadLine()) - 1;
                            if (itemChoice < 0)
                            {
                                break;
                            }
                            while (Inventory[itemChoice] == null)
                            {
                                Console.WriteLine("Invalid input. Try again.");
                                itemChoice = int.Parse(Console.ReadLine()) - 1;
                            }
                            
                            Console.WriteLine();
                            Inventory[itemChoice].ApplyEffect(this, badguy);
                            Inventory.Remove(Inventory[itemChoice]);
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("You have no items to use.");
                        }
                        Console.WriteLine();
                        break;

                    case 4:
                        if (this is Fairy fairy)
                        {
                            fairy.Spell(badguy, this);
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice.");
                        }
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        public void Attack(BadGuy badguy)
        {
            Random rand = new Random();

            int pDamage = rand.Next(1, 3);
            if (BeastEffect)
            {
                pDamage *= 2;
            }


            if (pDamage == 2 || pDamage == 4)
            {
                Console.WriteLine($"Critical hit on {badguy.Race}!");
            }

            badguy.Health -= pDamage;
            Console.WriteLine("---===---===---");
            Console.WriteLine($"{Name} attacks {badguy.Race} for {pDamage} damage!");
            Console.WriteLine("---===---===---");

            int bDamage = rand.Next(1, 3);

            if (badguy.Experience > Experience)
            {
                bDamage *= 2;
            }

            if (bDamage == 2 || bDamage == 4)
            {
                Console.WriteLine($"Critical hit on {Name}!");
            }

            Health -= bDamage;
            Console.WriteLine("---===---===---");
            Console.WriteLine($"{badguy.Race} attacks {Name} for {bDamage} damage!");
            Console.WriteLine("---===---===---");
        }

        public override string ToString()
        {
            return string.Format("---===---===--- \n" +
                "-----Character info-----\n" +
                $"Class: {Race}\n" +
                $"Name: {Name}\n" +
                $"Health: {Health}\n" +
                $"Money: {Money}\n" +
                $"Experience: {Experience} \n" +
                "---===---===---");
        }
    }

    abstract class BadGuy : Character
    {
        public BadGuy(string race, int experience, string place) : base(race)
        {
            Experience = experience;
            Place = place;
            Health = 10;
        }
        public string Place { get; set; }

        abstract public void LowHealthEffect(GoodGuy goodguy);
        abstract public void PlayerRan(GoodGuy goodguy);
        abstract public void FightWon(GoodGuy goodguy);
        abstract public void FightLost(GoodGuy goodguy);
        public override string ToString()
        {
            return string.Format("---===---===--- \n" +
                "Your enemy:\n" +
                $"Class: {Race}\n" +
                $"Health: {Health}\n" +
                $"Experience: {Experience}\n" +
                "---===---===---");
        }
    }


    interface IMagical
    {
        void Spell(BadGuy badguy, GoodGuy goodguy);
    }

    class Knight : GoodGuy
    {
        public Knight(string name) : base(name, 25, "Knight")
        {
            Health = 35;
        }
    }

    class Monkey : GoodGuy
    {
        public Monkey(string name) : base(name, 20, "Monkey")
        {
            Inventory.Add(new Item("Throwing Knife", 0, "A knife that deals 3 damage to your enemy."));
            Inventory.Add(new Item("Throwing Knife", 0, "A knife that deals 3 damage to your enemy."));
            Inventory.Add(new Item("Throwing Knife", 0, "A knife that deals 3 damage to your enemy."));
        }
    }

    class Fairy : GoodGuy, IMagical
    {
        public Fairy(string name) : base(name, 30, "Fairy") { }

        public void Spell(BadGuy badguy, GoodGuy goodguy)
        {
            Random rand = new Random();
            int roll = rand.Next(1, 6);
            Console.WriteLine("---===---===---");
            Console.WriteLine($"{Name} thinks about what to cast...");
            Console.WriteLine();
            switch (roll)
            {
                case 1:
                    Console.WriteLine($"{Name} magically takes away 5 damage from the opponent!");
                    badguy.Health -= 5;
                    break;
                case 2:
                    Console.WriteLine($"{Name} hurts themselves due to them being bad at magic! You lost 3 hearts.");
                    Health -= 3;
                    break;
                case 3:
                    Console.WriteLine($"{Name} creates money from thin air! You earned 10 Beast Coins!");
                    Money += 10;
                    break;
                case 4:
                    Console.WriteLine($"{Name} healed themselves! You gained 3 hearts.");
                    Health += 3;
                    break;
                case 5:
                    Console.WriteLine($"{Name} does absolutely nothing!");
                    break;
                default:
                    break;
            }
            Console.WriteLine("---===---===---");
        }
    }

    class LoganusPaul : BadGuy
    {
        public LoganusPaul(int experience, string place) : base("Loganus Paul", experience, place)
        {
            Health = 15;
        }

        public override void LowHealthEffect(GoodGuy goodguy)
        {
            Console.WriteLine("Guys... I think there's someone hanging right there...");
            Console.WriteLine("Loganus Paul used 'Cringe' on you. It was very effective. You lost 3 hearts!.");
            goodguy.Health -= 3;
        }

        public override void PlayerRan(GoodGuy goodguy)
        {
            Console.WriteLine("Ha! Run little boy! You're no match for big Paul!");
        }

        public override void FightWon(GoodGuy goodguy)
        {
            Console.WriteLine("My dying wish *cough*... Please buy *cough* Prime *cough* for only 100 Kč...");
            Console.WriteLine($"{goodguy.Name} gained 35 Beast Coins and 30 Experience!");
            Place = null;
            goodguy.Money += 35;
            goodguy.Experience += 30;
        }

        public override void FightLost(GoodGuy goodguy)
        {
            Console.WriteLine("Loserrrrrrrr!!!! Not hitting 1 million on youtube anyway.");
        }
    }

    class KSI : BadGuy
    {
        public KSI(int experience, string place) : base("KSI", experience, place) { }

        public override void LowHealthEffect(GoodGuy goodguy) 
        {
            Console.WriteLine("I can do this... I can go through the thick of it...");
            Console.WriteLine("KSI used his last wish! He gained 1 heart!");
            Health += 1;
        }
        public override void PlayerRan(GoodGuy goodguy) 
        {
            Console.WriteLine("You p*ssy.");
        }
        public override void FightWon(GoodGuy goodguy) 
        {
            Console.WriteLine($"My fight... is not over yet... {goodguy.Name}...");
            Console.WriteLine($"{goodguy.Name} gained 20 Beast Coins and 15 Experience!");
            Place = null;
            goodguy.Money += 20;
            goodguy.Experience += 15;
        }
        public override void FightLost(GoodGuy goodguy) 
        {
            Console.WriteLine("From the screen to the ring, to the pen, to the king. Where's my crown? That's my bling, always drama when I ring.");
            Console.WriteLine("*he's going into the thick of it*");
        }
    }

    class MasterBeast : BadGuy
    {
        public MasterBeast(int experience, string place) : base("Master Beast", experience, place)
        {
            Health = 20;
        }

        public override void LowHealthEffect(GoodGuy goodguy)
        {
            Console.WriteLine("Elon Musk... give me your power!");
            Console.WriteLine("What??? Master Beast magically took away 3 of your hearts!");
            goodguy.Health -= 3;
            Health += 3;
        }
        public override void PlayerRan(GoodGuy goodguy)
        {
            Console.WriteLine($"You will never defeat me! I am MASTER BEAST! FEAR ME!");
        }
        public override void FightWon(GoodGuy goodguy)
        {
            Console.WriteLine($"NOOOOOOOOOOOOOOOOOOO MY MRBEAST BAR CHOCOLATE.....!!!!!");
            Console.WriteLine();
            Console.WriteLine($"You did it! You finally got rid of him! Good job {goodguy.Name}.");
            goodguy.Experience = 420;
        }
        public override void FightLost(GoodGuy goodguy)
        {
            Console.WriteLine($"Lmao nerd");
        }
    }
}
