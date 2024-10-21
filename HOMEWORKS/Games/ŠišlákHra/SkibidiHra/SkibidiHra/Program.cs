using System;
using System.Buffers;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Numerics;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MojeHra
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(20, 20, true); // posledni argument false pokud nefugnují barvy v konkrétní konzoli
            game.Start();
        }
    }
    class Game
    {
        private Player Player;
        private int EnemyCount;
        private Character[,] TileMap;
        private int MapHeight, MapWidth;
        private bool UseColors;
        private int EnemyCountCopy;
        public Game(int h, int w, bool useColors)
        {
            UseColors = useColors;
            MapHeight = h; MapWidth = w;
            TileMap = new Character[MapHeight, MapWidth];
            EnemyCount = 0;



            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;
            Console.Title = "PETIKOVA HRA";

            WarningDisplay();
            MainMenuDisplay();
            int numOfEnemies = OptionInput();

            Player.Position.X = MapWidth / 2;
            Player.Position.Y = MapHeight / 2;
            TileMap[Player.Position.X, Player.Position.Y] = Player;

            Random rnd = new Random();

            for (int i = 0; i < numOfEnemies; i++) // num of enemies
            {
                int x, y;
                do
                {
                    x = rnd.Next(0, MapWidth);
                    y = rnd.Next(0, MapHeight);
                } while (TileMap[x, y] != null);
                Enemy newEnemy;
                switch (rnd.Next(1, 7))
                {
                    case 1:
                        newEnemy = new Succubus(rnd.Next(1, 10));
                        break;
                    case 2:
                        newEnemy = new Goblin(rnd.Next(1, 10));
                        break;
                    case 3:
                        newEnemy = new Giant(rnd.Next(1, 10));
                        break;
                    case 4:
                        newEnemy = new LightningWizard(rnd.Next(1, 10));
                        break;
                    case 5:
                        newEnemy = new FireDragon(rnd.Next(1, 10));
                        break;
                    case 6:
                        newEnemy = new IceWraith(rnd.Next(1, 10));
                        break;
                    default: // jen aby nebrecel error
                        newEnemy = new Goblin(1);
                        break;
                }
                newEnemy.Position.X = x;
                newEnemy.Position.Y = y;
                EnemyCount++;
                TileMap[x, y] = newEnemy;
                MainDisplay(); // pro krasnou animaci .;,,;.
            }
            EnemyCountCopy = EnemyCount;

        }
        public void Start()
        {
            Console.CursorVisible = false;

            [DllImport("user32.dll")]
            static extern short GetAsyncKeyState(int vKey);

            Dictionary<int, bool> keyStates = new Dictionary<int, bool>
            {
                { 0x57, false }, // W
                { 0x41, false }, // A
                { 0x53, false }, // S
                { 0x44, false }, // D
                { 0x59, false }, // Y
                { 0x4E, false }, // N
                { 0x49, false }, // I
                { 0x4F, false }, // O
                { 0x50, false }  // P
            };

            while (EnemyCount > 0)
            {
                foreach (KeyValuePair<int, bool> kvp in keyStates)
                {
                    bool isKeyPressed = (GetAsyncKeyState(kvp.Key) & 0x8000) != 0;
                    if (isKeyPressed && !kvp.Value)
                    {
                        HandleOneStep((char)kvp.Key); // zde main loop
                        keyStates[kvp.Key] = true;
                    }
                    else if (!isKeyPressed && kvp.Value)
                    {
                        keyStates[kvp.Key] = false;
                    }
                }
            }

            GameWonDisplay();

        }
        private void HandleOneStep(char Key)
        {
            switch (Key)
            {
                case 'W':
                    CheckForInteraction(Player, -1, 0);
                    break;
                case 'S':
                    CheckForInteraction(Player, 1, 0);
                    break;
                case 'A':
                    CheckForInteraction(Player, 0, -1);
                    break;
                case 'D':
                    CheckForInteraction(Player, 0, 1);
                    break;
                case 'P':
                    SkillSystem();
                    break;
                case 'O':
                    ShopSystem();
                    break;


            }
        }
        private void CheckForInteraction(Player player, int dx, int dy)
        {
            MainDisplay();
            if (!IsNotOutOfBounds(player, dx, dy)) return;
            Character destination = TileMap[player.Position.X + dx, player.Position.Y + dy];
            if (destination is Enemy)
            {
                Enemy enemy = (Enemy)destination;
                EnemySideDisplay(enemy);
                if (PromptFightDecision())
                {
                    bool? fightResult = player.Fight(enemy);
                    if (fightResult == null) // nerozhodne
                    {
                        MainDisplay();
                        if (FightIsADraw(enemy))
                        {
                            if (RNG(2))
                            {
                                MainDisplay();
                                LuckyWinDisplay(enemy);
                                MoveCharacter(player, dx, dy);
                                player.StealMoney(enemy, 200);
                                MoneyDisplay();
                                EnemyCount--;
                            }
                            else
                            {
                                GameOverDisplay();
                            }
                        }
                        else
                        {
                            enemy.StealMoney(player, 50);
                            MainDisplay();
                        }
                        //screen zkusit stesti(50/50 nebo chcipnout) / vzdat se 50% penez
                    }
                    else if (fightResult == true)
                    {
                        // ziska penize a stoupi na policko
                        MoveCharacter(player, dx, dy);
                        MainDisplay();
                        FightWonDisplay(enemy);
                        if (enemy is IDeathCry dc)
                        {
                            dc.Cry(MapWidth);
                        }
                        Player.StealMoney(enemy, 100);
                        MoneyDisplay();
                        EnemyCount--;
                        //fightwon display

                    }
                    else
                    {
                        MainDisplay();
                        if (FightLostDisplay(enemy))
                        {
                            enemy.StealMoney(player, 50);
                            MainDisplay();
                            if (enemy is IPlayerMocker pm)
                            {
                                pm.MockPlayerMessage(player, MapWidth);
                            }
                        }
                        else
                        {
                            GameOverDisplay();
                        }

                        //prijmout smrt (hra me nebavi) / vzdat se 50% penez
                    }
                }
                else
                {
                    MainDisplay();
                    if (enemy is IExitEffect eEnemy)
                    {
                        eEnemy.LeaveEffect(player, MapWidth * 2 + 10);
                        MoneyDisplay();
                    }
                    //interface

                }
            }
            else
            {
                MoveCharacter(Player, dx, dy);
                MainDisplay();
            }
        }
        private void MoveCharacter(Character character, int dx, int dy) // character a ne player kdybych nekdy chtel pohybovat i enemy
        {
            TileMap[character.Position.X, character.Position.Y] = null;
            character.Move(dx, dy);
            TileMap[character.Position.X, character.Position.Y] = character;
        }
        private bool IsNotOutOfBounds(Character character, int dx, int dy)
        {
            return (character.Position.X + dx >= 0 && character.Position.X + dx <= (MapHeight - 1)
            && character.Position.Y + dy >= 0 && character.Position.Y + dy <= (MapWidth - 1));
        }
        private void MainDisplay()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{YELLOW} ┌-");
            sb.Append(new string('-', MapHeight - 1));
            sb.Append('N');
            sb.Append(new string('-', MapHeight) + '┐' + new string(' ', 150));
            sb.Append($"\n{NORMAL}");

            for (int i = 0; i < MapWidth; i++)
            {
                if (i != MapWidth / 2)
                {
                    sb.Append($"{YELLOW} | {NORMAL}");
                }
                else
                {
                    sb.Append($"{YELLOW} W {NORMAL}");
                }
                for (int j = 0; j < MapHeight; j++)
                {
                    if (TileMap[i, j] == null)
                    {
                        sb.Append('.');
                    }
                    else
                    {
                        sb.Append(RED + TileMap[i, j].VisualChar + NORMAL);
                    }
                    sb.Append(' ');
                }
                if (i != MapWidth / 2)
                {
                    sb.Append($"{YELLOW}|{NORMAL}");
                }
                else
                {
                    sb.Append($"{YELLOW}E{NORMAL}");
                }
                sb.Append(new string(' ', 150) + '\n');
            }
            sb.Append($" {YELLOW}└-");
            sb.Append(new string('-', MapHeight - 1));
            sb.Append('S');
            sb.Append(new string('-', MapHeight) + '┘' + new string(' ', 150));
            sb.Append('\n');
            sb.Append($"\n{NORMAL}");
            sb.Append("  [W] Pohyb nahoru" + new string(' ', 6) + "[Y] Možnost ano" + new string(' ', 2 * MapWidth - 29) + $"Statistiky hráče {Player.Name}:\n");
            sb.Append("  [S] Pohyb dolu" + new string(' ', 8) + "[N] Možnost ne \n");
            sb.Append("  [A] Pohyb doleva" + new string(' ', 6) + "[O] Obchod" + new string(' ', 2 * MapWidth - 24) + $"HP: {Player.Health}                \n");
            sb.Append("  [D] Pohyb doprava" + new string(' ', 5) + "[P] Vylepšení" + new string(' ', 2 * MapWidth - 27) + $"Odolnost: \n");
            sb.Append(new string(' ', 2 * MapWidth + 10) + $"> r: {Player.Resistance.Raw}%, o: {Player.Resistance.Fire}%, l: {Player.Resistance.Ice}%," +
            $" j: {Player.Resistance.Poison}%, b: {Player.Resistance.Lightning}%                  \n");
            sb.Append(new string(' ', 2 * MapWidth + 10) + $"Poškození: \n");
            sb.Append(new string(' ', 2 * MapWidth + 10) + $"> r: {Player.Damage.Raw}, o: {Player.Damage.Fire}, l: {Player.Damage.Ice}," +
            $" j: {Player.Damage.Poison}, b: {Player.Damage.Lightning}              \n");


            Console.SetCursorPosition(0, 0);
            Console.WriteLine(sb.ToString());
            MoneyDisplay();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.SetCursorPosition(2 * Player.Position.Y + 3, Player.Position.X + 1);
            Console.Write(Player.VisualChar);
            Console.ForegroundColor = ConsoleColor.White;

        }
        private void EnemySideDisplay(Enemy enemy)
        {
            Console.SetCursorPosition(MapWidth * 2 + 9, 1);
            Console.WriteLine('┌' + new string('-', enemy.GetType().Name.Length + 5 + enemy.Level.ToString().Length) + '┐');
            Console.SetCursorPosition(MapWidth * 2 + 9, 2);
            Console.Write("| " + enemy.GetType().Name + $" [{enemy.Level}] |");
            Console.SetCursorPosition(MapWidth * 2 + 9, 3);
            Console.WriteLine('└' + new string('-', enemy.GetType().Name.Length + 5 + enemy.Level.ToString().Length) + '┘');
            Console.SetCursorPosition(MapWidth * 2 + 10, 4);
            Console.Write(enemy.VoiceLine);
            Console.SetCursorPosition(MapWidth * 2 + 10, 6);
            Console.Write($"HP: {enemy.Health}");
            Console.SetCursorPosition(MapWidth * 2 + 10, 8);
            Console.Write("Odolnost: ");
            Console.SetCursorPosition(MapWidth * 2 + 10, 9);
            Console.Write($"> r: {enemy.Resistance.Raw}%, o: {enemy.Resistance.Fire}%, l: {enemy.Resistance.Ice}%," +
                $" j: {enemy.Resistance.Poison}%, b: {enemy.Resistance.Lightning}%");
            Console.SetCursorPosition(MapWidth * 2 + 10, 10);
            Console.Write("Infliktuje poškození: ");
            Console.SetCursorPosition(MapWidth * 2 + 10, 11);
            Console.Write($"> r: {(int)Math.Round(enemy.Damage.Raw * Math.Pow(1.05, enemy.Level) * 1.2)}," +
                $" o: {(int)Math.Round(enemy.Damage.Fire * Math.Pow(1.05, enemy.Level) * 1.2)}," +
                $" l: {(int)Math.Round(enemy.Damage.Ice * Math.Pow(1.05, enemy.Level) * 1.2)}," +
                $" j: {(int)Math.Round(enemy.Damage.Poison * Math.Pow(1.05, enemy.Level) * 1.2)}," +
                $" b: {(int)Math.Round(enemy.Damage.Lightning * Math.Pow(1.05, enemy.Level) * 1.2)}");
            Console.SetCursorPosition(MapWidth * 2 + 10, 13);
            Console.Write($"Kouká směrem: {enemy.FacingDirection}");
        }
        private void MoneyDisplay()
        {
            Console.SetCursorPosition(2, MapHeight + 9);
            Console.Write($"{YELLOW}POČET MINCÍ: {Player.Money}                     {NORMAL}\n");
        }
        private void SkillDisplay()
        {
            MainDisplay();
            Console.SetCursorPosition(MapWidth * 2 + 9, 1);
            Console.WriteLine('┌' + new string('-', 10) + '┐');
            Console.SetCursorPosition(MapWidth * 2 + 9, 2);
            Console.Write($"| Zlepšení |");
            Console.SetCursorPosition(MapWidth * 2 + 9, 3);
            Console.WriteLine('└' + new string('-', 10) + '┘');

            Console.SetCursorPosition(MapWidth * 2 + 10, 5);
            if (Player.Skill["HP"] != 10)
                Console.WriteLine("[W] Životy" + new string(' ', 5) + new string('|', Player.Skill["HP"]) + new string(' ', 10 - Player.Skill["HP"]) + $"  (+{25}) cena: {(Player.Skill["HP"] + 1) * 50}");
            else
                Console.WriteLine("[W] Životy" + new string(' ', 5) + new string('|', Player.Skill["HP"]) + new string(' ', 10 - Player.Skill["HP"]) + $"  Maxxed");
            Console.SetCursorPosition(MapWidth * 2 + 10, 7);
            if (Player.Skill["Resistance"] != 10)
                Console.WriteLine("[A] Odolnost" + new string(' ', 3) + new string('|', Player.Skill["Resistance"]) + new string(' ', 10 - Player.Skill["Resistance"]) + $"  (+{5} každé) cena: {(Player.Skill["Resistance"] + 1) * 50}");
            else
                Console.WriteLine("[A] Odolnost" + new string(' ', 3) + new string('|', Player.Skill["Resistance"]) + new string(' ', 10 - Player.Skill["Resistance"]) + $"  Maxxed");
            Console.SetCursorPosition(MapWidth * 2 + 10, 9);
            if (Player.Skill["Damage"] != 10)
                Console.WriteLine("[S] Poškození" + new string(' ', 2) + new string('|', Player.Skill["Damage"]) + new string(' ', 10 - Player.Skill["Damage"]) + $"  (+{5} každé) cena: {(Player.Skill["Damage"] + 1) * 50}");
            else
                Console.WriteLine("[S] Poškození" + new string(' ', 2) + new string('|', Player.Skill["Damage"]) + new string(' ', 10 - Player.Skill["Damage"]) + $"  Maxxed");
            Console.SetCursorPosition(MapWidth * 2 + 10, 11);
            if (Player.Skill["Style"] != 10)
                Console.WriteLine("[D] Styl" + new string(' ', 7) + new string('|', Player.Skill["Style"]) + new string(' ', 10 - Player.Skill["Style"]) + $"  (+{1}) cena: {(Player.Skill["Style"] + 1) * 50}");
            else
                Console.WriteLine("[D] Styl" + new string(' ', 7) + new string('|', Player.Skill["Style"]) + new string(' ', 10 - Player.Skill["Style"]) + $"  Maxxed");
            Console.SetCursorPosition(MapWidth * 2 + 10, 14);
            Console.WriteLine("[P] Zpět do hry");
        }
        private void SkillSystem()
        {
            SkillDisplay();
            [DllImport("user32.dll")]
            static extern short GetAsyncKeyState(int vKey);

            Dictionary<int, bool> keyStates = new Dictionary<int, bool>
            {
                { 0x57, false }, // W
                { 0x41, false }, // A
                { 0x53, false }, // S
                { 0x44, false }, // D
                { 0x50, false }  // P
            };

            bool leave = false;
            while (!leave) // break kdyz P
            {
                foreach (KeyValuePair<int, bool> kvp in keyStates)
                {
                    bool isKeyPressed = (GetAsyncKeyState(kvp.Key) & 0x0001) != 0;
                    if (isKeyPressed && !kvp.Value)
                    {
                        if ((char)kvp.Key != 'P')
                            HandleSkillMenu((char)kvp.Key); // zde main loop
                        else
                            leave = true;
                        keyStates[kvp.Key] = true;
                    }
                    else if (!isKeyPressed && kvp.Value)
                    {
                        keyStates[kvp.Key] = false;
                    }
                }
            }
            MainDisplay();
        }
        private void ShopDisplay() // mohl jsem to udelat automaticky, ale nebyl cas
        {
            MainDisplay();
            Console.SetCursorPosition(MapWidth * 2 + 10, 0);
            Console.Write('┌' + new string('-', 61) + '┐');
            Console.SetCursorPosition(MapWidth * 2 + 10, 1);
            Console.Write('|' + new string(' ', 27) + "OBCHOD " + new string(' ', 27) + '|');
            Console.SetCursorPosition(MapWidth * 2 + 10, 2);
            Console.Write('├' + new string('-', 30) + '┬' + new string('-', 30) + '┤');
            Console.SetCursorPosition(MapWidth * 2 + 10, 3);
            Console.Write("| [W] Zapalovač" + new string(' ', 16) + "| [A] Podezřelá injekce" + new string(' ', 8) + "|");
            Console.SetCursorPosition(MapWidth * 2 + 10, 4);
            Console.Write("|  + 5 ohnivý útok" + new string(' ', 13) + "|  + 250 Hp" + new string(' ', 20) + "|");
            Console.SetCursorPosition(MapWidth * 2 + 10, 5);
            Console.Write('|' + new string(' ', 30) + '|' + new string(' ', 30) + '|');
            Console.SetCursorPosition(MapWidth * 2 + 10, 6);
            Console.Write('|' + new string(' ', 30) + '|' + new string(' ', 30) + '|');
            Console.SetCursorPosition(MapWidth * 2 + 10, 7);

            if (!ShopItems[1] && !ShopItems[2])
                Console.Write("|  Cena: 500 mincí" + new string(' ', 13) + "|  Cena: 420 mincí" + new string(' ', 13) + "|");
            else if (!ShopItems[1] && ShopItems[2])
                Console.Write("|  Cena: 500 mincí" + new string(' ', 13) + $"|  {BLUE}Zakoupeno{NORMAL}" + new string(' ', 19) + "|");
            else if (ShopItems[1] && !ShopItems[2])
                Console.Write($"|  {BLUE}Zakoupeno{NORMAL}" + new string(' ', 19) + "|  Cena: 420 mincí" + new string(' ', 13) + "|");
            else
                Console.Write($"|  {BLUE}Zakoupeno{NORMAL}" + new string(' ', 19) + $"|  {BLUE}Zakoupeno{NORMAL}" + new string(' ', 19) + "|");
            Console.SetCursorPosition(MapWidth * 2 + 10, 8);
            Console.Write('├' + new string('-', 30) + '┼' + new string('-', 30) + '┤');
            Console.SetCursorPosition(MapWidth * 2 + 10, 9);
            Console.Write("| [S] Kevlarová vesta" + new string(' ', 10) + "| [D] Steroidy" + new string(' ', 17) + "|");
            Console.SetCursorPosition(MapWidth * 2 + 10, 10);
            Console.Write("|  + 25 rázová odolnost" + new string(' ', 8) + "|  + 8 rázový útok" + new string(' ', 13) + "|");
            Console.SetCursorPosition(MapWidth * 2 + 10, 11);
            Console.Write('|' + new string(' ', 30) + '|' + new string(' ', 30) + '|');
            Console.SetCursorPosition(MapWidth * 2 + 10, 12);
            Console.Write('|' + new string(' ', 30) + '|' + new string(' ', 30) + '|');
            Console.SetCursorPosition(MapWidth * 2 + 10, 13);
            if (!ShopItems[3] && !ShopItems[4])
                Console.Write("|  Cena: 650 mincí" + new string(' ', 13) + "|  Cena: 300 mincí" + new string(' ', 13) + "|");
            else if (!ShopItems[3] && ShopItems[4])
                Console.Write("|  Cena: 650 mincí" + new string(' ', 13) + $"|  {BLUE}Zakoupeno{NORMAL}" + new string(' ', 19) + "|");
            else if (ShopItems[3] && !ShopItems[4])
                Console.Write($"|  {BLUE}Zakoupeno{NORMAL}" + new string(' ', 19) + "|  Cena: 300 mincí" + new string(' ', 13) + "|");
            else
                Console.Write($"|  {BLUE}Zakoupeno{NORMAL}" + new string(' ', 19) + $"|  {BLUE}Zakoupeno{NORMAL}" + new string(' ', 19) + "|");
            Console.SetCursorPosition(MapWidth * 2 + 10, 14);
            Console.Write('├' + new string('-', 30) + '┼' + new string('-', 30) + '┤');
            Console.SetCursorPosition(MapWidth * 2 + 10, 15);
            Console.Write("| [Y] Kniha zaklínadel" + new string(' ', 9) + "| [N] Nádherný peníz" + new string(' ', 11) + "|");
            Console.SetCursorPosition(MapWidth * 2 + 10, 16);
            Console.Write("|  + 10 odolností" + new string(' ', 14) + "|  + 500 mincí" + new string(' ', 17) + "|");
            Console.SetCursorPosition(MapWidth * 2 + 10, 17);
            Console.Write('|' + new string(' ', 30) + '|' + new string(' ', 30) + '|');
            Console.SetCursorPosition(MapWidth * 2 + 10, 18);
            Console.Write('|' + new string(' ', 30) + '|' + new string(' ', 30) + '|');
            Console.SetCursorPosition(MapWidth * 2 + 10, 19);
            if (!ShopItems[5] && !ShopItems[6])
                Console.Write("|  Cena: 750 mincí" + new string(' ', 13) + "|  Cena: 1000 mincí" + new string(' ', 12) + "|");
            else if (!ShopItems[5] && ShopItems[6])
                Console.Write("|  Cena: 750 mincí" + new string(' ', 13) + $"|  {BLUE}Zakoupeno{NORMAL}" + new string(' ', 19) + "|");
            else if (ShopItems[5] && !ShopItems[6])
                Console.Write($"|  {BLUE}Zakoupeno{NORMAL}" + new string(' ', 19) + "|  Cena: 1000 mincí" + new string(' ', 12) + "|");
            else
                Console.Write($"|  {BLUE}Zakoupeno{NORMAL}" + new string(' ', 19) + $"|  {BLUE}Zakoupeno{NORMAL}" + new string(' ', 19) + "|");
            Console.SetCursorPosition(MapWidth * 2 + 10, 20);
            Console.Write('└' + new string('-', 30) + '┴' + new string('-', 30) + '┘');
            Console.SetCursorPosition(MapWidth * 2 + 10, 21);
            Console.Write("[O] Zpět");

        }
        private void ShopSystem()
        {
            ShopDisplay();
            [DllImport("user32.dll")]
            static extern short GetAsyncKeyState(int vKey);

            Dictionary<int, bool> keyStates = new Dictionary<int, bool>
            {
                { 0x57, false }, // W
                { 0x41, false }, // A
                { 0x53, false }, // S
                { 0x44, false }, // D
                { 0x59, false }, // Y
                { 0x4E, false }, // M
                { 0x4F, false } // O
            };

            bool leave = false;
            while (!leave) // break kdyz P
            {
                foreach (KeyValuePair<int, bool> kvp in keyStates)
                {
                    bool isKeyPressed = (GetAsyncKeyState(kvp.Key) & 0x0001) != 0;
                    if (isKeyPressed && !kvp.Value)
                    {
                        if ((char)kvp.Key != 'O')
                            HandleShopMenu((char)kvp.Key); // zde main loop
                        else
                            leave = true;
                        keyStates[kvp.Key] = true;
                    }
                    else if (!isKeyPressed && kvp.Value)
                    {
                        keyStates[kvp.Key] = false;
                    }
                }
            }
            MainDisplay();
        }
        private void HandleSkillMenu(char key)
        {
            switch (key)
            {
                case 'W':
                    Player.LevelUpSkill("HP");
                    break;
                case 'A':
                    Player.LevelUpSkill("Resistance");
                    break;
                case 'S':
                    Player.LevelUpSkill("Damage");
                    break;
                case 'D':
                    Player.LevelUpSkill("Style");
                    break;
            }
            SkillDisplay();
        }
        private void HandleShopMenu(char key)
        {
            switch (key)
            {
                case 'W':
                    if (!ShopItems[1] && Player.Money >= 500)
                    {
                        Player.Money -= 500;
                        ShopItems[1] = true;
                        Player.Damage.Fire += 5;
                    }
                    break;
                case 'A':
                    if (!ShopItems[2] && Player.Money >= 420)
                    {
                        Player.Money -= 420;
                        ShopItems[2] = true;
                        Player.Health += 250;
                    }
                    break;
                case 'S':
                    if (!ShopItems[3] && Player.Money >= 650)
                    {
                        Player.Money -= 650;
                        ShopItems[3] = true;
                        Player.Resistance.Raw = Math.Min(100, Player.Resistance.Raw + 25);
                    }
                    break;
                case 'D':
                    if (!ShopItems[4] && Player.Money >= 300)
                    {
                        Player.Money -= 300;
                        ShopItems[4] = true;
                        Player.Damage.Raw += 8;
                    }
                    break;
                case 'Y':
                    if (!ShopItems[5] && Player.Money >= 750) // odecist money
                    {
                        Player.Money -= 750;
                        ShopItems[5] = true;
                        Player.Resistance.Raw = Math.Min(100, Player.Resistance.Raw + 10);
                        Player.Resistance.Fire = Math.Min(100, Player.Resistance.Fire + 10);
                        Player.Resistance.Ice = Math.Min(100, Player.Resistance.Ice + 10);
                        Player.Resistance.Poison = Math.Min(100, Player.Resistance.Poison + 10);
                        Player.Resistance.Lightning = Math.Min(100, Player.Resistance.Lightning + 10);
                    }
                    break;
                case 'N':
                    if (!ShopItems[6] && Player.Money >= 1000) // - 500 coins
                    {
                        Player.Money -= 500;
                        ShopItems[6] = true;
                    }
                    break;
            }
            ShopDisplay();
        }
        private bool PromptFightDecision()
        {
            Console.SetCursorPosition(MapWidth * 2 + 10, 17);
            Console.Write("[Y] BOJOVAT!   [N] ODEJÍT");
            return YesNoInput();
        }
        private void FightWonDisplay(Enemy enemy)
        {
            Console.SetCursorPosition(MapWidth * 2 + 10, 5);
            Console.Write($"Zabils {enemy.GetType().Name} [{enemy.Level}]!");
            Console.SetCursorPosition(MapWidth * 2 + 10, 6);
            Console.Write($"+{enemy.Money} mincí");
        }
        private bool FightLostDisplay(Enemy enemy)
        {
            Console.SetCursorPosition(MapWidth * 2 + 10, 5);
            Console.Write($"{enemy.GetType().Name} [{enemy.Level}] ti dal zabrat, dostals bídu.");
            Console.SetCursorPosition(MapWidth * 2 + 10, 7);
            Console.Write("[Y] UTÉCT (-50% mincí)   [N] ZEMŘÍT HANBOU (-100% mincí a konec hry)");
            return YesNoInput();
        }
        private bool FightIsADraw(Enemy enemy)
        {
            Console.SetCursorPosition(MapWidth * 2 + 10, 5);
            Console.Write($"{enemy.GetType().Name} [{enemy.Level}] je stejně slabý jako ty.");
            Console.SetCursorPosition(MapWidth * 2 + 10, 6);
            Console.Write($"Souboj byl nerozhodný.");
            Console.SetCursorPosition(MapWidth * 2 + 10, 8);
            Console.Write("[Y] ZKUSIT ŠTĚSTÍ (50% šance na výhru, pokud vyhraješ získáš dokonce dvojnásobek mincí z nepřítele)");
            Console.SetCursorPosition(MapWidth * 2 + 10, 9);
            Console.Write("[N] VZDÁT SE (-50% mincí)");
            return YesNoInput();
        }
        private void GameOverDisplay()
        {
            Console.Clear();
            StringBuilder sb = new StringBuilder();
            sb.Append(new string(' ', 30) + $"{RED}Měls na víc srabe!\n");//stred 39
            sb.Append(new string(' ', 39 - (32 + Player.Name.Length) / 2) + $"Toto je konec příběhu slabocha {Player.Name}.\n"); //pridat staty (pocet kills, celkovy pocet ziskanych minci)
            sb.Append($"\n\n{BLUE}Celkový počet zabitých nepřátel:\n{EnemyCountCopy - EnemyCount}\n ");
            sb.Append($"\nCelkový počet získaných mincí:\n{Player.MoneyStolen}\n ");
            sb.Append($"\n{YELLOW}{SwagLevel[Player.Skill["Style"]]}{BLUE}\n(Styl: {Player.Skill["Style"]})\n ");
            sb.Append($"\nVšechny tvé stisklé klávesy za celou hru:\n");
            Console.Write(sb.ToString());
            Console.ReadLine();
            Environment.Exit(0);
        }
        private void GameWonDisplay()
        {
            Console.Clear();
            StringBuilder sb = new StringBuilder();
            sb.Append(new string(' ', 30) + $"{GREEN}Jsi ten nejsilnější ze všech!\n");//stred 44
            sb.Append(new string(' ', 44 - (40 + Player.Name.Length) / 2) + $"Toto je konec krásného příběhu siláka {Player.Name}.\n"); //pridat staty (pocet kills, celkovy pocet ziskanych minci)
            sb.Append($"\n\n{BLUE}Celkový počet zabitých nepřátel:\n{EnemyCountCopy - EnemyCount}\n ");
            sb.Append($"\nCelkový počet získaných mincí:\n{Player.MoneyStolen}\n ");
            sb.Append($"\n{YELLOW}{SwagLevel[Player.Skill["Style"]]}\n(Styl: {Player.Skill["Style"]}){BLUE}\n ");
            sb.Append($"\nVšechny tvé stisklé klávesy za celou hru:\n");
            Console.Write(sb);
            Console.ReadLine();
            Environment.Exit(0);
        }
        private void LuckyWinDisplay(Enemy enemy)
        {
            Console.SetCursorPosition(MapWidth * 2 + 10, 5);
            Console.Write($"Měl jsi štěstí bratře!");
            Console.SetCursorPosition(MapWidth * 2 + 10, 7);
            Console.Write($"Zabils {enemy.GetType().Name} [{enemy.Level}].");
            Console.SetCursorPosition(MapWidth * 2 + 10, 8);
            Console.Write($"+{2 * enemy.Money} mincí");
        }
        private void MainMenuDisplay()
        {
            Console.Clear();
            StringBuilder sb = new StringBuilder();
            sb.Append("   Vyber si postavu, kterou chceš hrát:\n     (zmáčki 1 až 4, aby sis vybral)\n\n");
            sb.Append('┌' + new string('-', 20) + '┬' + new string('-', 20) + '┐');
            sb.Append("\n| [1] Knight         | [2] Assassin       |" +
                      "\n|  - síla            |  - 2x útok ze zadu |" +
                      "\n|  - vysoká odolnost |  - žádná odolnost  |" +
                      "\n|                    |                    |" +
                      "\n|   Obtížnost: 1/10  |   Obtížnost: 5/10  |\n");
            sb.Append('├' + new string('-', 20) + '┼' + new string('-', 20) + '┤');
            sb.Append("\n| [3] Druid          | [4] Ranger         |" +
                      "\n|  - elementální     |  - slabý útok      |" +
                      "\n|    útok            |  - nízká odolnost  |" +
                      "\n|                    |                    |" +
                      "\n|   Obtížnost: 3/10  |   Obtížnost: 7/10  |\n");
            sb.Append('└' + new string('-', 20) + '┴' + new string('-', 20) + "┘\n");
            Console.Write(sb.ToString());

            ConsoleKeyInfo keyinfo = Console.ReadKey(true); //jen proto, aby se input nezobrazoval pozdeji v name inputu
            [DllImport("user32.dll")]
            static extern short GetAsyncKeyState(int vKey);
            while (true)
            {
                if ((GetAsyncKeyState(0x31) & 0x0001) != 0)
                {
                    Player = new Knight(NameInput());
                    break;
                }
                else if ((GetAsyncKeyState(0x32) & 0x0001) != 0)
                {
                    Player = new Assassin(NameInput());
                    break;
                }
                else if ((GetAsyncKeyState(0x33) & 0x0001) != 0)
                {
                    Player = new Druid(NameInput());
                    break;
                }
                else if ((GetAsyncKeyState(0x34) & 0x0001) != 0)
                {
                    Player = new Ranger(NameInput());
                    break;
                }
            }
            Console.CursorVisible = false;
        }
        private string NameInput()
        {
            Console.CursorVisible = true;
            Console.Clear();
            Console.WriteLine("Pojmenuj si svou postavu: \n");

            return Console.ReadLine();
        }
        private int OptionInput()
        {
            Console.Clear();
            StringBuilder sb = new StringBuilder();
            sb.Append($"Vyber si počet nepřátel na poli (od 1 do {MapWidth * MapWidth - 1})");
            Console.Write(sb.ToString());
            int l;
            Console.CursorVisible = true;
            string x;
            do
            {
                Console.SetCursorPosition(0, 3);
                Console.Write(new string(' ', 150));
                Console.SetCursorPosition(0, 3);
                x = Console.ReadLine();

            } while (!int.TryParse(x, out l) || l > (MapHeight * MapWidth - 1) || l <= 0);
            //
            Console.CursorVisible = false;
            return l;
        }
        private void WarningDisplay()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Maximalizuj konzolové okno (fullscreen mode), jinak se to posere.");
            sb.Append("\n\n\nMaximalizoval jsem okno: ");
            sb.Append("\n\n[Y] Ano          [N] Ne");
            Console.Write(sb.ToString());
            while (!YesNoInput()) ;


        }
        private bool YesNoInput()
        {
            ConsoleKeyInfo keyinfo = Console.ReadKey(true); //jen proto, aby se input nezobrazoval pozdeji v name inputu
            [DllImport("user32.dll")]
            static extern short GetAsyncKeyState(int vKey);
            while (true)
            {
                bool YIsPressed = (GetAsyncKeyState(0x59) & 0x0001) != 0;
                bool NIsPressed = (GetAsyncKeyState(0x4E) & 0x0001) != 0;

                if (YIsPressed && NIsPressed)
                {
                    continue;
                }
                else if (YIsPressed)
                {
                    return true;
                }
                else if (NIsPressed)
                {
                    return false;
                }
            }
        }
        private bool RNG(int oneInHowMany)
        {
            Random random = new Random();
            if (random.Next(1, oneInHowMany + 1) == 1)
            {
                return true;
            }
            return false;
        }

        private string GetColorCode(string colorCode)
        {
            return UseColors ? colorCode : "";
        }
        private string NORMAL => GetColorCode("\x1b[39m");
        private string RED => GetColorCode("\x1b[91m");
        private string GREEN => GetColorCode("\x1b[92m");
        private string YELLOW => GetColorCode("\x1b[93m");
        private string BLUE => GetColorCode("\x1b[94m");
        private string MAGENTA => GetColorCode("\x1b[95m");
        private string CYAN => GetColorCode("\x1b[96m");
        private string GREY => GetColorCode("\x1b[97m");
        private string BOLD => GetColorCode("\x1b[1m");
        private string NOBOLD => GetColorCode("\x1b[22m");
        private string UNDERLINE => GetColorCode("\x1b[4m");
        private string NOUNDERLINE => GetColorCode("\x1b[24m");
        private string REVERSE => GetColorCode("\x1b[7m");
        private string NOREVERSE => GetColorCode("\x1b[27m");

        Dictionary<int, string> SwagLevel = new Dictionary<int, string>()
        {
            {0, "ŠPÍNO BEZDOMOVECKÁ, FUJ!" },
            {1, "Vypadáš, jako by tě špatně nakreslili a pak vygumovali." },
            {2, "Ty jsi tak hnusnej, že když tě matka poprvé viděla, utekla do lesa." },
            {3, "Ty jsi tak šerednej, že když jdeš kolem hřbitova, tak si mrtví zakrývají oči." },
            {4, "Ty máš takovej ksicht, že kdyby byla soutěž o největšího strašáka, vyhrál bys první tři místa." },
            {5, "Vypadáš, jako by ses pral s kombajnem a prohrál." },
            {6, "Jsi jako ten film, co si jednou pustíš a řekneš si: \"Jo, dobrý, ale podruhý to vidět nemusím.\"" },
            {7, "Ty jsi tak krásnej, že když se usměješ, i hrníčky na poličce se srovnají." },
            {8, "Kdyby krása byla zločin, tak už bys měl doživotí." },
            {9, "Kdybys byl smartphone, tak máš jen jeden problém – nikdo tě nechce dát z ruky." },
            {10, "Vypadáš skoro líp než Matyáš." }
        };

        Dictionary<int, bool> ShopItems = new Dictionary<int, bool>()
        {
            { 1, false },
            { 2, false },
            { 3, false },
            { 4, false },
            { 5, false },
            { 6, false }
        };
    }
    //interface s leave efektem a leave zpravou
    interface IExitEffect
    {
        public void LeaveEffect(Player character, int offset);
        public void LeaveMessage(int offset);
    }
    //interface s kill zpravou
    interface IDeathCry
    {
        public void Cry(int offset);
    }
    //interface s player death zpravou
    interface IPlayerMocker
    {
        public void MockPlayerMessage(Player player, int offset);
    }
    struct Position
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Position(int x, int y)
        {
            X = x; Y = y;
        }

        public override string? ToString()
        {
            return $"X: {X}, Y: {Y}";
        }
    } //tady by se taky hodila class
    struct Resistance //tady by se spis hodila class, jelikoz to menim pri kazdem nakupu skillu, takze se to casto kopiruje zbytecne
    {
        public int Raw { get; set; }
        public int Fire { get; set; }
        public int Ice { get; set; }
        public int Poison { get; set; }
        public int Lightning { get; set; }

        public Resistance(int raw, int fire, int ice, int poison, int lightning)
        {
            Raw = raw; Fire = fire; Ice = ice;
            Poison = poison; Lightning = lightning;
        }
    }
    struct Damage //tady by se taky hodila class
    {
        public int Raw { get; set; }
        public int Fire { get; set; }
        public int Ice { get; set; }
        public int Poison { get; set; }
        public int Lightning { get; set; }

        public Damage(int raw, int fire, int ice, int poison, int lightning)
        {
            Raw = raw; Fire = fire; Ice = ice;
            Poison = poison; Lightning = lightning;
        }
    }
    enum Direction { North, South, West, East }
    abstract class Character
    {
        protected Character(int money, char visualChar, int health, Resistance resistance, Damage damage)
        {
            Money = money; VisualChar = visualChar; Health = health;
            Resistance = resistance; Damage = damage;
        }
        public int Health { get; set; }
        public int Money { get; set; }
        public char VisualChar { get; }
        public Resistance Resistance;
        public Damage Damage;
        public Position Position;
        virtual public void Move(int x, int y) // kdybych nahodou chtel nekdy pohybovat i enemy
        {
            Position.X += x;
            Position.Y += y;
        }
        virtual public void StealMoney(Character character, double percentage) // this krade od character
        {
            this.Money += (int)((double)character.Money * (percentage / 100));
            character.Money -= (int)((double)character.Money * (percentage / 100));
        }
    }
    abstract class Player : Character
    {
        public string Name { get; }
        public int Mana { get; set; }
        public Damage ElementalDamage { get; set; }
        public Dictionary<string, int> Skill { get; private set; }
        public int MoneyStolen { get; private set; }
        protected Player(char visualChar, string name, int mana, int health, Resistance resistance, Damage damage) : base(550, visualChar, health, resistance, damage)
        {
            Name = name; Mana = mana; MoneyStolen = 0;
            Skill = new Dictionary<string, int>()
            {
                {"HP", 0},
                {"Resistance", 0},
                {"Damage", 0},
                {"Style", 0}
            };
        }

        //true = player won
        virtual public bool? Fight(Enemy enemy) // virual -> assasin ma 2x dmg kdyz jde do zad
        {
            double playerDmg =
                CalculateDamage((double)this.Damage.Raw, (double)enemy.Resistance.Raw) +
                CalculateDamage((double)this.Damage.Fire, (double)enemy.Resistance.Fire) +
                CalculateDamage((double)this.Damage.Ice, (double)enemy.Resistance.Ice) +
                CalculateDamage((double)this.Damage.Poison, (double)enemy.Resistance.Poison) +
                CalculateDamage((double)this.Damage.Lightning, (double)enemy.Resistance.Lightning);
            double enemyDmg =
                CalculateDamage((double)enemy.Damage.Raw, (double)this.Resistance.Raw, enemy.Level) +
                CalculateDamage((double)enemy.Damage.Fire, (double)this.Resistance.Fire, enemy.Level) +
                CalculateDamage((double)enemy.Damage.Ice, (double)this.Resistance.Ice, enemy.Level) +
                CalculateDamage((double)enemy.Damage.Poison, (double)this.Resistance.Poison, enemy.Level) +
                CalculateDamage((double)enemy.Damage.Lightning, (double)this.Resistance.Lightning, enemy.Level);

            int numOfPlayerHitsRequired = (int)Math.Ceiling((double)enemy.Health / playerDmg);
            int numOfEnemyHitsRequired = (int)Math.Ceiling((double)this.Health / enemyDmg);

            if (numOfPlayerHitsRequired == numOfEnemyHitsRequired)
                return null;
            if (numOfEnemyHitsRequired > numOfPlayerHitsRequired)
                return true;
            else
                return false;
        }
        public void LevelUpSkill(string skillName)
        {
            if (Skill[skillName] == 10)
            {
                return;
            }
            else if (this.Money - 50 * (Skill[skillName] + 1) >= 0)
            {
                this.Money -= 50 * (Skill[skillName] + 1);
                Skill[skillName]++;
                switch (skillName)
                {
                    case "HP":
                        this.Health += 25;
                        break;
                    case "Resistance":
                        this.Resistance.Raw = Math.Min(100, this.Resistance.Raw + 5);
                        this.Resistance.Fire = Math.Min(100, this.Resistance.Fire + 5);
                        this.Resistance.Ice = Math.Min(100, this.Resistance.Ice + 5);
                        this.Resistance.Poison = Math.Min(100, this.Resistance.Poison + 5);
                        this.Resistance.Lightning = Math.Min(100, this.Resistance.Lightning + 5);
                        break;
                    case "Damage":
                        this.Damage.Raw = Math.Min(100, this.Damage.Raw + 5);
                        this.Damage.Fire = Math.Min(100, this.Damage.Fire + 5);
                        this.Damage.Ice = Math.Min(100, this.Damage.Ice + 5);
                        this.Damage.Poison = Math.Min(100, this.Damage.Poison + 5);
                        this.Damage.Lightning = Math.Min(100, this.Damage.Lightning + 5);
                        break;
                }
            }
            else
            {
                return;
            }

        }

        protected double CalculateDamage(double damage, double resistance)
        {
            return (damage * (1 - resistance / 100));
        }
        protected double CalculateDamage(double damage, double resistance, int level)
        {
            return (damage * Math.Pow(1.05, level) * 1.2 * (1 - resistance / 100));
        }

        public override void StealMoney(Character character, double percentage) // jeste si to pocita do statu celkem
        {
            this.Money += (int)((double)character.Money * (percentage / 100));
            MoneyStolen += (int)((double)character.Money * (percentage / 100));
            character.Money -= (int)((double)character.Money * (percentage / 100));
        }
    }
    abstract class Enemy : Character
    {
        public int Level { get; private set; }
        public Direction FacingDirection { get; private set; }
        public string VoiceLine { get; private set; }
        protected Enemy(int money, char visualChar, string voiceLine, int level, int health, Resistance resistance, Damage damage)
            : base(money, visualChar, health, resistance, damage)
        {
            Level = level; FacingDirection = RandomDirection(); VoiceLine = voiceLine;
        }
        private Direction RandomDirection()
        {
            Random rnd = new Random();
            switch (rnd.Next(1, 5))
            {
                case 1:
                    return Direction.North;
                case 2:
                    return Direction.South;
                case 3:
                    return Direction.West;
                case 4:
                    return Direction.East;
            }
            return Direction.North;
        }
    }
    class Knight : Player // neni specialni, hotovo
    {
        public Knight(string name) : base('K', name, 50, 200, new Resistance(50, 20, 20, 0, 0), new Damage(5, 0, 0, 0, 0))
        {
        }
    }
    class Ranger : Player //nemá range, je to slabý kus
    {
        public Ranger(string name) : base('R', name, 100, 100, new Resistance(0, 10, 10, 0, 0), new Damage(1, 0, 0, 0, 0))
        {
        }
    }
    class Assassin : Player // dava 2x tolik do zad, hotovo
    {
        public Assassin(string name) : base('A', name, 50, 75, new Resistance(0, 0, 0, 0, 0), new Damage(4, 0, 0, 0, 0))
        {
        }
        public override bool? Fight(Enemy enemy)
        {
            double playerDmg =
                CalculateDamage((double)this.Damage.Raw, (double)enemy.Resistance.Raw) +
                CalculateDamage((double)this.Damage.Fire, (double)enemy.Resistance.Fire) +
                CalculateDamage((double)this.Damage.Ice, (double)enemy.Resistance.Ice) +
                CalculateDamage((double)this.Damage.Poison, (double)enemy.Resistance.Poison) +
                CalculateDamage((double)this.Damage.Lightning, (double)enemy.Resistance.Lightning);
            double enemyDmg =
                CalculateDamage((double)enemy.Damage.Raw, (double)this.Resistance.Raw, enemy.Level) +
                CalculateDamage((double)enemy.Damage.Fire, (double)this.Resistance.Fire, enemy.Level) +
                CalculateDamage((double)enemy.Damage.Ice, (double)this.Resistance.Ice, enemy.Level) +
                CalculateDamage((double)enemy.Damage.Poison, (double)this.Resistance.Poison, enemy.Level) +
                CalculateDamage((double)enemy.Damage.Lightning, (double)this.Resistance.Lightning, enemy.Level);


            switch (enemy.FacingDirection) // rana do zad dava 2x tolik dmg takze se obcas vyplati leavnout fight a obejit ho, coz pridava rng k enemies s nebezpecnym leave effectem
            {
                case Direction.North:
                    if (this.Position.X < enemy.Position.X)
                    {
                        playerDmg *= 2;
                    }
                    break;
                case Direction.South:
                    if (this.Position.X > enemy.Position.X)
                    {
                        playerDmg *= 2;
                    }
                    break;
                case Direction.West:
                    if (this.Position.Y > enemy.Position.Y)
                    {
                        playerDmg *= 2;
                    }
                    break;
                case Direction.East:
                    if (this.Position.Y < enemy.Position.Y)
                    {
                        playerDmg *= 2;
                    }
                    break;
            }

            int numOfPlayerHitsNeeded = (int)Math.Ceiling((double)enemy.Health / playerDmg);
            int numOfEnemyHitsNeeded = (int)Math.Ceiling((double)this.Health / enemyDmg);

            if (numOfPlayerHitsNeeded == numOfEnemyHitsNeeded)
                return null;
            if (numOfEnemyHitsNeeded > numOfPlayerHitsNeeded)
                return true;
            else
                return false;
        }
    }
    class Druid : Player // spawnuje summons, nebo taky ne, nezbyl cas
    {
        public Druid(string name) : base('D', name, 150, 100, new Resistance(0, 0, 0, 50, 50), new Damage(1, 1, 1, 1, 1))
        {
        }
    }
    class Goblin : Enemy, IExitEffect, IPlayerMocker // minus money pokud nechces fight ted
    {
        public Goblin(int level)
            : base(1 * (int)Math.Pow(level + 2, 1.5), 'g', "*cítím penízky mmmmm*",
                  level, 20, new Resistance(0, 0, 0, 50, 0), new Damage(10, 0, 0, 5, 0))
        {
        }
        public override void StealMoney(Character character, double percentage)
        {
            base.StealMoney(character, 100);
        }
        public void LeaveEffect(Player character, int offset)
        {
            this.Money += character.Money / 3;
            character.Money -= character.Money / 3;
            LeaveMessage(offset);
        }
        public void LeaveMessage(int offset)
        {
            Console.SetCursorPosition(offset, 6);
            Console.Write("Odešels.");
            Console.SetCursorPosition(offset, 8);
            Console.Write("\"díky za penísky milášku >:)\"");
            Console.SetCursorPosition(offset, 9);
            Console.Write("(byls okraden o 30% mincí)");
        }
        public void MockPlayerMessage(Player player, int offset)
        {
            Console.SetCursorPosition(offset * 2 + 10, 10);
            Console.Write($"\"Nedokážeš zabít ani toho nejmenšího tvora?\"");
            Console.SetCursorPosition(offset * 2 + 10, 12);
            Console.Write($"\"AHAHHAHHaaah\"");
        }
    }
    class Giant : Enemy, IPlayerMocker, IDeathCry //nic 
    {
        public Giant(int level)
            : base(3 * (int)Math.Pow(level + 2, 1.5), 'G', "*grrrhhh waaaaaaarggh*",
                  level, 100, new Resistance(60, 0, 0, 0, 0), new Damage(15, 0, 0, 0, 0))
        {
        }
        public void MockPlayerMessage(Player player, int offset)
        {
            Console.SetCursorPosition(offset * 2 + 10, 10);
            Console.Write($"\"GRAHARgRHAHARHARGARHA\"");
        }
        public void Cry(int offset)
        {
            Console.SetCursorPosition(offset * 2 + 10, 10);
            Console.Write($"*aaaaeeeeeeeeeeeeee*");
        }
    }
    class Succubus : Enemy, IExitEffect, IPlayerMocker, IDeathCry //moznost byt znasilnen
    {
        public Succubus(int level)
            : base(4 * (int)Math.Pow(level + 2, 1.6), 'S', "*oooooooh aaaaahhhhhh*",
                  level, 40, new Resistance(40, 10, 0, 0, 0), new Damage(100, 10, 0, 0, 0))
        {
        }
        public void LeaveEffect(Player character, int offset)
        {
            Console.SetCursorPosition(offset, 5);
            Console.Write("Pokusil ses odejít.");
            Console.SetCursorPosition(offset, 7);
            Console.Write("\"Kam si myslíš že jdeš zlatíčko moje?\"");
            Console.SetCursorPosition(offset, 8);
            Console.Write("*romanticky se na tebe podívá*");
            Console.SetCursorPosition(offset, 10);
            Console.Write("*podlehls její kráse*");
            Console.SetCursorPosition(offset, 12);
            Console.Write("[Y] KOPULOVAT   [N] KOPULOVAT");
            [DllImport("user32.dll")]
            static extern short GetAsyncKeyState(int vKey);
            while (true)
            {
                bool YIsPressed = (GetAsyncKeyState(0x59) & 0x0001) != 0;
                bool NIsPressed = (GetAsyncKeyState(0x4E) & 0x0001) != 0;
                if (YIsPressed)
                {
                    break;
                }
                else if (NIsPressed)
                {
                    break;
                }
            }
            character.Health /= 2;
            LeaveMessage(offset);
        }
        public void LeaveMessage(int offset)
        {
            Console.SetCursorPosition(offset, 5);
            Console.Write("Neměls sis s ní ani povídat.");
            Console.SetCursorPosition(offset, 7);
            Console.Write("Po sexuálních hrátkách ti sukuba zlomila srdce, ");
            Console.SetCursorPosition(offset, 8);
            Console.Write("půlku z něj si dokonce i nechala.");
            Console.SetCursorPosition(offset, 10);
            Console.Write("(přišels o 50% maximálních HP na vždy)");
            Console.SetCursorPosition(offset, 12);
            Console.Write("                                                      ");
        }
        public void MockPlayerMessage(Player player, int offset)
        {
            Console.SetCursorPosition(offset * 2 + 10, 10);
            Console.Write($"\"To sis myslel, že mě porazíš?\"");
            Console.SetCursorPosition(offset * 2 + 10, 12);
            Console.Write($"\"{player.Name} - největší slaboch na světě\"");
        }
        public void Cry(int offset)
        {
            Console.SetCursorPosition(offset * 2 + 10, 10);
            Console.Write($"\"I teď, v mých posledních momentech,");
            Console.SetCursorPosition(offset * 2 + 10, 11);
            Console.Write($"bych chtěla pocítit tvůj dotek\"");
            Console.SetCursorPosition(offset * 2 + 10, 13);
            Console.Write($"*se slzama v očích zemře*");
        }

    }
    class FireDragon : Enemy, IPlayerMocker // nikdy nebere penize, jen je znici
    {
        public FireDragon(int level)
            : base(3 * (int)Math.Pow(level + 2, 1.3), 'F', "*rooooaaaar boom*",
                  level, 100, new Resistance(30, 100, 0, 20, 20), new Damage(5, 40, 0, 0, 0))
        {
        }
        public override void StealMoney(Character character, double percentage)
        {
            character.Money -= (int)((double)character.Money * (percentage / 100));
        }
        public void MockPlayerMessage(Player player, int offset)
        {
            Console.SetCursorPosition(offset * 2 + 10, 10);
            Console.Write($"*ozývá se hlasité dunění*");
            Console.SetCursorPosition(offset * 2 + 10, 13);
            Console.Write($"*uvědomil sis, že se ti drak asi směje*");
        }
    }
    class LightningWizard : Enemy, IExitEffect, IDeathCry //snizuje resistenci pokud odejdu
    {
        public LightningWizard(int level)
            : base(2 * (int)Math.Pow(level + 2, 1.5), 'L', "*zaaaap*",
                  level, 30, new Resistance(0, 20, 20, 20, 100), new Damage(5, 0, 0, 0, 30))
        {
        }
        public void LeaveEffect(Player character, int offset)
        {
            character.Resistance.Raw = Math.Max(character.Resistance.Raw - 1, 0);
            character.Resistance.Fire = Math.Max(character.Resistance.Fire - 1, 0);
            character.Resistance.Ice = Math.Max(character.Resistance.Ice - 1, 0);
            character.Resistance.Poison = Math.Max(character.Resistance.Poison - 1, 0);
            character.Resistance.Lightning = Math.Max(character.Resistance.Lightning - 1, 0);
            LeaveMessage(offset);
        }
        public void LeaveMessage(int offset)
        {
            Console.SetCursorPosition(offset, 5);
            Console.Write("Odešels.");
            Console.SetCursorPosition(offset, 6);
            Console.Write("Zlý čaroděj tě proklel.");
            Console.SetCursorPosition(offset, 7);
            Console.Write("(přišels o 1 odolnosti v každé kategorii)");
        }
        public void Cry(int offset)
        {
            Console.SetCursorPosition(offset * 2 + 10, 10);
            Console.Write($"\"Tolik let strávených v Bradavicích zbytečně\"");
            Console.SetCursorPosition(offset * 2 + 10, 11);
            Console.Write($"\"neeeeeee\"");
            Console.SetCursorPosition(offset * 2 + 10, 12);
            Console.Write($"*nepokojně zemře*");
        }
    }
    class IceWraith : Enemy, IPlayerMocker //snizi dmg 0.8x
    {
        public IceWraith(int level)
            : base(2 * (int)Math.Pow(level + 2, 1.4), 'I', "*whooosh*",
                  level, 15, new Resistance(10, 0, 100, 10, 10), new Damage(5, 0, 30, 0, 0))
        {
        }
        public void MockPlayerMessage(Player player, int offset)
        {
            Console.SetCursorPosition(offset * 2 + 10, 10);
            Console.Write($"\"Budu tě strašit ve snech\"");
            Console.SetCursorPosition(offset * 2 + 10, 12);
            Console.Write($"*wooo ooo*");
        }
    }

}
// mel jsem to rozdelit na dva string buildry prava a leva, prava prvni pak leva misto pouzivani set cursor a pak console.write
// (uvedomil jsem si to pozde)
// dale jsem mel vytvorit nejakou enumeracni funkci pro resistance a damage a zmenit to na classy, taky moc pozde
// nestihl jsem udelat veci s manou - druid ktery by spawnoval summony a ostatni s ostatnimy kouzly


//  pro testy se penize meni v ctor playera

// TODO:
// fixnout input buffer bug :( (nikdo si toho nevsimne)
// lepsi hratelnost (enemy scaling, pocatecni dmg atd)
// summary funkci