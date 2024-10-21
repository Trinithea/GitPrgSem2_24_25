
using System;
using System.Data;
using System.Data.Common;


Console.WriteLine("How to Play (Almost)\n" +
    "Your goal in this game is to eliminate all of your opponents Heroes.\n" +
    "Each hero has several stats (SPD - which determines the action order and the number\n" +
    "of steps the Hero gets, ATK - which determines the damage the hero deals,\n" +
    "DEF - which reduces the damage taken and HP - hit points).\n\n" +
    "There are 4 types of Heroes each with their unique abilities: Dwarf, Succubus, Tree and Witch\n" +
    "The Dwarf has close-range AoE attack, the Succubus has a close-range single-target lifesteal attack,\n" +
    "the Tree has a long-range single-target attack but it can't move and the Witch has a variety of magic spells.\n" +
    "The spells are the following: Boost an ally's SPD/ATK/DEF/HP by 1 or Swap any two Heroes on the Map.\n");
Console.WriteLine("Press any key to continue");
Console.ReadKey();
Console.Clear();


Map m = new Map();


Dwarf dwarf1 = new Dwarf(4, 3, 3, 3, 6);
Dwarf dwarf2 = new Dwarf(4, 3, 3, 3, 6);
Succubus succubus1 = new Succubus(4, 3, 3, 3, 6);
Succubus succubus2 = new Succubus(4, 3, 3, 3, 6);
Tree tree1 = new Tree(0, 3, 3, 3, 6);
Tree tree2 = new Tree(0, 3, 3, 3, 6);
Witch witch1 = new Witch(3, 3, 3, 3, 4);
Witch witch2 = new Witch(3, 3, 3, 3, 4);
Team team1 = new Team(1);
Team team2 = new Team(2);
Team[] teams = { team1, team2 };
team1.opponent = team2;
team2.opponent = team1;
List<Hero> characters = new List<Hero>() { dwarf1, succubus1, tree1, witch1, dwarf2, succubus2, tree2, witch2 };
foreach (Hero character in characters)
{
    character.map = m;
}
team1.heroes = new List<Hero>() { dwarf1, succubus1, tree1, witch1 };
team2.heroes = new List<Hero>() { dwarf2, succubus2, tree2, witch2 };
witch1.characters = characters;
witch2.characters = characters;




foreach (Hero ch in team1.heroes)
{
    ch.team = team1;
    ch.SetStats();
}
foreach (Hero ch in team2.heroes)
{
    ch.team = team2;
    ch.avatar = (char)(ch.avatar + 32);
    ch.SetStats();
}
foreach (Team t in teams)
{
    IEnumerable<Hero> temp = t.heroes.OrderByDescending(h => h.spd);
    t.actionOrder = new List<Hero>();
    foreach (Hero h in temp)
    {
        t.actionOrder.Add(h);
    }
}



dwarf1.row = 10;
dwarf1.column = 1;
succubus1.row = 12;
succubus1.column = 3;
tree1.row = 10;
tree1.column = 3;
witch1.row = 12;
witch1.column = 1;
dwarf2.row = 1;
dwarf2.column = 10;
succubus2.row = 3;
succubus2.column = 12;
tree2.row = 3;
tree2.column = 10;
witch2.row = 1;
witch2.column = 12;
dwarf1.UpdateAvatar();
dwarf2.UpdateAvatar();
succubus1.UpdateAvatar();
succubus2.UpdateAvatar();
tree1.UpdateAvatar();
tree2.UpdateAvatar();
witch1.UpdateAvatar();
witch2.UpdateAvatar();


if (team1.actionOrder[0].spd < team2.actionOrder[0].spd)
{
    team2.actionOrder[0].Play();
    foreach (Hero h in team2.opponent.heroes)
    {
        if (h.hp <= 0) { h.team.actionOrder.Remove(h); }
    }
    Hero temp = team2.actionOrder[0];
    team2.actionOrder.RemoveAt(0);
    team2.actionOrder.Add(temp);
}

while (team1.actionOrder.Count() > 0 & team2.actionOrder.Count() > 0)
{
    int i = 0;
    while (i < 2 & team1.actionOrder.Count() > 0 & team2.actionOrder.Count() > 0)
    {
        teams[i].actionOrder[0].Play();
        foreach (Hero h in teams[i].opponent.heroes)
        {
            if (h.hp <= 0) { h.team.actionOrder.Remove(h); }
        }
        Hero temp = teams[i].actionOrder[0];
        teams[i].actionOrder.RemoveAt(0);
        teams[i].actionOrder.Add(temp);
        i++;

    }
}
if (team1.actionOrder.Count() <= 0) { Console.WriteLine("Player 2 won"); }
else if (team2.actionOrder.Count() <= 0) { Console.WriteLine("Player 1 won"); }


class Team
{
    public int teamId { get; set; }
    public Team(int i)
    {
        this.teamId = i;
    }
    public Team opponent { get; set; }
    public List<Hero> heroes { get; set; }
    public List<Hero> actionOrder { get; set; }
}
abstract class Hero
{
    public Team team { get; set; }
    public Map map { get; set; }
    public string type { get; protected set; }
    public int spd { get; set; }
    public int atk { get; set; }
    public int hp { get; set; }
    public int def { get; set; }

    public int skillPoints { get; protected set; }

    public char avatar { get; set; }
    public int column { get; set; }
    public int row { get; set; }
    public int prevrow { get; set; }
    public int prevcolumn { get; set; }

    public Hero(int speed, int attack, int hp, int defense, int skillPoints)
    {
        this.hp = hp;
        this.spd = speed;
        this.def = defense;
        this.atk = attack;
        this.skillPoints = skillPoints;
    }
    public void SetStats()
    {
        while (this.skillPoints > 0)
        {
            Console.Clear();
            Console.WriteLine("Player " + this.team.teamId.ToString() + ": Modify your " + this.type + " stats (f.e. s/ads/ssahdd )");
            Console.WriteLine("SPD: " + this.spd.ToString() + "   ATK: " + this.atk.ToString() + "   HP: " + this.hp.ToString() + "   DEF: " + this.def.ToString());
            Console.WriteLine("Skill Points remaining: " + this.skillPoints.ToString());
            string statChanges = Console.ReadLine();
            foreach (char c in statChanges)
            {
                if (this.skillPoints == 0) { break; }
                switch (c)
                {
                    case 's':
                        this.spd++;
                        this.skillPoints--;
                        break;

                    case 'a':
                        this.atk++;
                        this.skillPoints--;
                        break;

                    case 'h':
                        this.hp++;
                        this.skillPoints--;
                        break;

                    case 'd':
                        this.def++;
                        this.skillPoints--;
                        break;
                }
            }
        }
        Console.Clear();

    }

    public virtual void Move()
    {
        ConsoleKeyInfo key1;
        for (int i = 0; i < this.spd; i++)
        {
            Console.WriteLine("Current turn: Player " + this.team.teamId.ToString() + ", " + this.type + "(" + this.avatar.ToString() + ")");
            Console.WriteLine("Steps left: " + (this.spd - i).ToString());
            key1 = Console.ReadKey(true);
            if (key1.Key == ConsoleKey.X) { break; }
            else if (key1.Key == ConsoleKey.W || key1.Key == ConsoleKey.A || key1.Key == ConsoleKey.S || key1.Key == ConsoleKey.D)
            {
                this.MakeMove(key1);
                this.UpdateAvatar();
                Console.Clear();
                map.PrintMap();

            }
            else { Console.Clear(); map.PrintMap(); i--; }
        }
    }
    public virtual void Action()
    {
        Console.WriteLine("Choose enemy to atttack");
        char target = Console.ReadLine().ToCharArray()[0];
        foreach (Hero h in this.team.opponent.actionOrder)
        {
            if (target == h.avatar)
            {
                int damageDealt = this.atk - h.def;
                if (damageDealt <= 0)
                {
                    Console.WriteLine("Enemy defense too high");
                    return;
                }
                else
                {
                    h.hp -= damageDealt;
                    if (h.hp <= 0)
                    {
                        map.field[h.row, h.column] = '.';
                        Console.Clear();
                        map.PrintMap();
                        Console.WriteLine(h.type + " was killed.");
                    }
                    else
                    {
                        Console.WriteLine("Damage dealt to " + h.type + ": " + damageDealt.ToString());
                    }
                }
            }
            break;
        }

    }
    public void Play()
    {
        map.PrintMap();
        this.Move();
        this.Action();
        Console.WriteLine("Your turn is over. Press any key to continue");
        Console.ReadKey();
        Console.Clear();
    }
    public void UpdateAvatar()
    {
        map.field[prevrow, prevcolumn] = '.';
        map.field[row, column] = avatar;

    }
    public bool CanMove(int Row, int Column)
    {
        return !(map.field[Row, Column] == 'x' || map.field[Row, Column] == 'd' || map.field[Row, Column] == 'D' || map.field[Row, Column] == 's' || map.field[Row, Column] == 'S' || map.field[Row, Column] == 'T' || map.field[Row, Column] == 't' || map.field[Row, Column] == 'w' || map.field[Row, Column] == 'W');
    }
    public void MakeMove(ConsoleKeyInfo k)
    {
        prevrow = row;
        prevcolumn = column;
        switch (k.Key)
        {
            case ConsoleKey.W:
                if (CanMove(row - 1, column))
                {
                    row -= 1;
                }
                break;

            case ConsoleKey.A:
                if (CanMove(row, column - 1))
                {
                    column -= 1;
                }
                break;
            case ConsoleKey.S:
                if (CanMove(row + 1, column))
                {
                    row += 1;
                }
                break;
            case ConsoleKey.D:
                if (CanMove(row, column + 1))
                {
                    column += 1;
                }
                break;
            case ConsoleKey.X:
                break;
        }
    }


}
class Dwarf : Hero
{
    public Dwarf(int speed, int attack, int hp, int defense, int skillPoints) : base(speed, attack, hp, defense, skillPoints)
    {
        type = "Dwarf";
        avatar = 'D';

    }
    public override void Action()
    {
        foreach (Hero h in this.team.opponent.actionOrder)
        {
            if ((h.column == this.column || h.column == this.column - 1 || h.column == this.column + 1) & (h.row == this.row || h.row == this.row - 1 || h.row == this.row + 1))
            {
                int damageDealt = this.atk - h.def;
                if (damageDealt <= 0)
                {
                    Console.WriteLine("Enemy defense too high");
                    return;
                }
                else
                {
                    h.hp -= damageDealt;
                    if (h.hp <= 0)
                    {
                        Console.Clear();
                        map.field[h.row, h.column] = '.';
                        map.PrintMap();
                        Console.WriteLine(h.type + " was killed.");
                    }
                    else
                    {
                        Console.WriteLine("Damage dealt to " + h.type + ": " + damageDealt.ToString());
                    }
                }
            }
        }

    }
}
class Succubus : Hero
{
    public Succubus(int speed, int attack, int hp, int defense, int skillPoints) : base(speed, attack, hp, defense, skillPoints)
    {
        type = "Succubus";
        avatar = 'S';
    }

    public override void Action()
    {
        Console.WriteLine("Choose enemy to atttack");
        char target = Console.ReadLine().ToCharArray()[0];
        foreach (Hero h in this.team.opponent.actionOrder)
        {
            if (target == h.avatar)
            {
                if ((h.column == this.column || h.column == this.column - 1 || h.column == this.column + 1) & (h.row == this.row || h.row == this.row - 1 || h.row == this.row + 1))
                {
                    int damageDealt = this.atk - h.def;
                    if (damageDealt <= 0)
                    {
                        Console.WriteLine("Enemy defense too high");
                        return;
                    }
                    else
                    {
                        h.hp -= damageDealt;
                        this.hp += damageDealt;
                        if (h.hp <= 0)
                        {
                            Console.Clear();
                            map.field[h.row, h.column] = '.';
                            map.PrintMap();
                            Console.WriteLine(h.type + " was killed.");
                        }
                        else
                        {
                            Console.WriteLine("Damage dealt to " + h.type + ": " + damageDealt.ToString());
                        }
                        Console.WriteLine("Succubus healed " + damageDealt.ToString() + " HP");
                    }
                }
                else { Console.WriteLine("Enemy is out of range"); }
                break;
            }
        }

    }
}
class Tree : Hero
{
    public Tree(int speed, int attack, int hp, int defense, int skillPoints) : base(speed, attack, hp, defense, skillPoints)
    {
        type = "Tree";
        avatar = 'T';
    }

    public override void Action()
    {
        Console.WriteLine("Choose enemy to atttack");
        char target = Console.ReadLine().ToCharArray()[0];
        foreach (Hero h in this.team.opponent.actionOrder)
        {
            if (target == h.avatar)
            {
                int damageDealt = this.atk - h.def;
                if (damageDealt <= 0)
                {
                    Console.WriteLine("Enemy defense too high");
                    return;
                }
                else
                {
                    h.hp -= damageDealt;
                    if (h.hp <= 0)
                    {
                        Console.Clear();
                        map.field[h.row, h.column] = '.';
                        map.PrintMap();
                        Console.WriteLine(h.type + " was killed.");
                    }
                    else
                    {
                        Console.WriteLine("Damage dealt to " + h.type + ": " + damageDealt.ToString());
                    }
                }
                break;
            }
        }

    }
    public override void Move()
    {
        Console.WriteLine("Current turn: Player " + this.team.teamId.ToString() + ", " + this.type + "(" + this.avatar.ToString() + ")");
        Console.WriteLine("Trees can't move");
    }
}
class Witch : Hero
{
    public Witch(int speed, int attack, int hp, int defense, int skillPoints) : base(speed, attack, hp, defense, skillPoints)
    {
        type = "Witch";
        avatar = 'W';
    }
    public List<Hero> characters;

    public override void Action()
    {
        Console.WriteLine("Choose the spell you want to use (s/a/d/h/c)");
        char spell = Console.ReadLine().ToCharArray()[0];
        switch (spell)
        {
            case 's':
                {
                    Console.WriteLine("Which ally's SPD do you want to boost");
                    char target = Console.ReadLine().ToCharArray()[0];
                    foreach (Hero h in this.team.actionOrder)
                    {
                        if (target == h.avatar)
                        {
                            h.spd += 1;
                            Console.WriteLine("SPD of " + h.type + " has been increased ");
                            break;
                        }
                    }
                    break;
                }
            case 'a':
                {
                    Console.WriteLine("Which ally's ATK do you want to boost");
                    char target = Console.ReadLine().ToCharArray()[0];
                    foreach (Hero h in this.team.actionOrder)
                    {
                        if (target == h.avatar)
                        {
                            h.atk += 1;
                            Console.WriteLine("ATK of " + h.type + " has been increased ");
                            break;
                        }
                    }
                    break;
                }
            case 'd':
                {
                    Console.WriteLine("Which ally's DEF do you want to boost");
                    char target = Console.ReadLine().ToCharArray()[0];
                    foreach (Hero h in this.team.actionOrder)
                    {
                        if (target == h.avatar)
                        {
                            h.def += 1;
                            Console.WriteLine("DEF of " + h.type + " has been increased ");
                            break;
                        }
                    }
                    break;
                }
            case 'h':
                {
                    Console.WriteLine("Which ally's HP do you want to boost");
                    char target = Console.ReadLine().ToCharArray()[0];
                    foreach (Hero h in this.team.actionOrder)
                    {
                        if (target == h.avatar)
                        {
                            h.hp += 1;
                            Console.WriteLine("HP of " + h.type + " has been increased ");
                            break;
                        }
                    }
                    break;
                }
            case 'c':
                {
                    Console.WriteLine("Which two Heroes do you want to swap");
                    char[] targets = Console.ReadLine().ToCharArray();
                    char target1 = targets[0];
                    char target2 = targets[1];
                    foreach (Hero h in characters)
                    {
                        if (target1 == h.avatar)
                        {
                            int temprow = h.row;
                            int tempcol = h.column;
                            Hero temphero = h;
                            foreach (Hero h1 in characters)
                            {
                                if (target2 == h1.avatar)
                                {
                                    temphero.row = h1.row;
                                    temphero.column = h1.column;
                                    h1.row = temprow;
                                    h1.column = tempcol;
                                    map.field[h1.row, h1.column] = h1.avatar;
                                    map.field[h.row, h.column] = h.avatar;
                                    Console.Clear();
                                    map.PrintMap();
                                    Console.WriteLine(h.type + " and " + h1.type + " have been swapped");

                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }

        }
    }




}
public class Map
{
    public char[,] field =     { { 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x' },
                                 { 'x', '.', '.', 'x', '.', '.', '.', '.', '.', '.', '.', '.', '.', 'x' },
                                 { 'x', '.', '.', 'x', '.', '.', '.', '.', '.', 'x', '.', '.', '.', 'x' },
                                 { 'x', '.', '.', '.', '.', '.', '.', '.', '.', 'x', '.', '.', '.', 'x' },
                                 { 'x', '.', '.', 'x', 'x', '.', '.', '.', '.', '.', '.', '.', '.', 'x' },
                                 { 'x', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', 'x' },
                                 { 'x', '.', '.', '.', '.', '.', 'x', '.', '.', '.', '.', '.', '.', 'x' },
                                 { 'x', '.', '.', '.', '.', '.', 'x', '.', '.', '.', 'x', 'x', '.', 'x' },
                                 { 'x', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', 'x' },
                                 { 'x', '.', '.', '.', 'x', '.', '.', '.', '.', '.', '.', '.', '.', 'x' },
                                 { 'x', '.', '.', '.', 'x', '.', '.', '.', '.', 'x', 'x', '.', '.', 'x' },
                                 { 'x', '.', '.', '.', 'x', '.', '.', '.', '.', 'x', '.', '.', '.', 'x' },
                                 { 'x', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', 'x' },
                                 { 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x' } };

    public bool Ended = false;


    public void PrintMap()
    {
        for (int i = 0; i < field.GetLength(0); i++)
        {
            for (int j = 0; j < field.GetLength(1); j++)
            {
                Console.Write(field[i, j]);
            }
            Console.WriteLine();
        }
    }

}