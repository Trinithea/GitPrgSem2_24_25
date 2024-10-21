partial class Program
{
    class GameMaster{
        readonly List<Creature> heroParty = [];
        int progress = 0;
        readonly int maxProgress = 7;

        readonly Random rng = new();

        enum Hero{
            Sldr,
            Hlr,
            Gmblr,
            Arcnst,
            Stnnr,
        }

        public void StartGame(){
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Heroes");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Soldier\n\tLight Attack: Deals 10 damage\n\tHeavy Attack: Deals 30 damage, but stuns self for 2 turns\n\tDefend: Reduces incoming damage by 50% for 1 turn\nHealer\n\tLight Attack: Deals 5 damage\n\tHeal: Restores 30 health to a single ally\n\tHeal Party: Restores 10 health to all allies, but stuns self for 1 turn\nGambler\n\tSmall Bet: Deals 5-10 random damage\n\tAll On Red: Fully heals a random creature (ally or enemy)\n\tAll On Black: 50% chance to instantly defeat target enemy or defeat self\n\tNot Today: If health is 1 or less, instantly defeats an enemy\nArcanist\n\tAdd Defence: Reduces an ally's incoming damage by 20% for 3 turns\n\tAdd Strength: Increases an ally's damage by 20% for 5 turns\n\tKick: Deals 3 damage\nStunner\n\tStun: 66% chance to stun an enemy for 2 turns, 33% chance to stun self\n\tLight Attack: Deals 20 damage\n\tAttack All: Deals 10 damage to all enemies\n(press enter to continue)");
            Console.ReadLine();
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Enemies");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Goblin\n\tBite: Deals 15 damage to the enemy with the lowest health\nSpider\n\tWeb: Deals 9 damage, 25% chance to stun the target for 2 turns\nSuccubus\n\tKiss: Reduces an ally's incoming damage by 30% for 2 turns\n\tSuck: Increases an ally's damage by 50% for 3 turns\n\tSometimes does nothing\nOrc\n\tSlash: Deals 30 damage to a random enemy\n(press enter to continue)");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("You start the game with a soldier");
            AddHero(Hero.Sldr);
            Console.Clear();


            while (progress < maxProgress){
                Console.Clear();
                Console.WriteLine("Your progress so far: ");
                DrawProgressBar(progress, maxProgress);
                Console.ReadLine();

                List<Creature> enemyParty = GenerateEnemies();
                BattleControler bt = new(heroParty, enemyParty);

                if (!bt.Turn()){
                    Console.WriteLine("You lost :(");
                    return;
                }

                Console.Clear();
                AddHero(GenerateHero());

                progress++;
            }

            Console.Clear();
            Console.WriteLine("You won, well done!");
            return;
        }

        readonly string[] firstNames = [
            "Tonda",
            "Antonín",
            "Toník",
            "Toníček",
            "Anton",
            "Antoin",
            "Tony",
            "Anthony",
            "Tuone",
            "Tóňa",
            "Aintiwan",
            "Antal",
            "Onoy",
            "Anakoni"
        ];

        readonly string[] surnames = [
            "z Ústí",
            "z Brna",
            "z Hradce",
            "z Chaty",
            "z Nučič",
            "z Přílep",
            "z Květnice",
            "ze Slovenska",
            "z Prahy",
            "z Přecechtělovi 2240/5",
            "z Budějovic",
            "z Bratislavy",
            "Homeless",
            "z Kumratic",
            "z Jiřího z Poděbrad"
        ];

        string GenerateName(){
            return $"{firstNames[rng.Next(firstNames.Length)]} {surnames[rng.Next(surnames.Length)]}";
        }

        List<Creature> GenerateEnemies(){
            if (progress == 0) return [new Goblin(GenerateName()), new Goblin(GenerateName())];
            if (progress == 1) return [new Orc(GenerateName()), new Succubus(GenerateName())];
            if (progress == 2) return [new Orc(GenerateName()), new Spider(GenerateName()),];
            if (progress == 3) return [new Orc (GenerateName()), new Orc(GenerateName()), new Spider(GenerateName())];
            if (progress == 4) return [new Goblin(GenerateName()), new Goblin(GenerateName()), new Goblin(GenerateName()), new Goblin(GenerateName()), new Goblin(GenerateName()), new Goblin(GenerateName()), new Goblin(GenerateName()), new Goblin(GenerateName()), new Goblin(GenerateName()), new Goblin(GenerateName()), new Goblin(GenerateName())];
            if (progress == 5) return [new Orc(GenerateName()), new Spider(GenerateName()), new Spider(GenerateName()), new Spider(GenerateName()), new Orc(GenerateName()) ];

            return [new Beholder("Pepa z Depa"), new Succubus("Slovenka")];
        }

        Hero GenerateHero(){
            List<Hero> Heroes = [.. (Hero[])Enum.GetValues(typeof(Hero))];
            Heroes = [.. Heroes.OrderBy(x => rng.Next())];
            
            List<string> HeroChoices = [heroNames[Heroes[0]], heroNames[Heroes[1]], heroNames[Heroes[2]]];
            
            int decision = MakeDecision("Choose your new party member: ", HeroChoices);

            Console.Clear();
            return Heroes[decision];
        }

        readonly Dictionary<Hero, string> heroNames = new() {
            [Hero.Sldr] = "Soldier",
            [Hero.Hlr] = "Healer",
            [Hero.Gmblr] = "Gambler",
            [Hero.Stnnr] = "Stunner",
            [Hero.Arcnst] = "Arcanist",
        };

        void AddHero(Hero h){
            Console.WriteLine($"Name your {heroNames[h]}:");

            string? name = Console.ReadLine() ?? throw new Exception("Cannot read the console!");

            Dictionary<Hero, Func<Creature>> heroClasses = new() {
                [Hero.Sldr] = () => new Soldier(name),
                [Hero.Hlr] = () => new Healer(name),
                [Hero.Gmblr] = () => new Gambler(name),
                [Hero.Stnnr] = () => new Stunner(name),
                [Hero.Arcnst] = () => new Arcanist(name),
            };

            heroParty.Add(heroClasses[h]());
        }
    }
}