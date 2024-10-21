using System;
using System.Collections.Generic;
using System.Linq;

namespace Hra;

public class Game
{
    private string? _hrac1;
    private string? _hrac2;
    private readonly List<Entity> _postavyHrac1 = new() {new Druid(), new Elf(), new Trpaslik()};
    private readonly List<Entity> _postavyHrac2 = new() {new Druid(), new Elf(), new Trpaslik()};

    public void Loop()
    {
        string tahne = _hrac1;
        bool run = true;
        
        while (run)
        {
            run = PlayerMove(tahne);

            if (run == false)
            {
                Console.WriteLine($"Vyhral hrac {tahne}");
            }

            if (tahne == _hrac1)
            {
                tahne = _hrac2;
            }
            else
            {
                tahne = _hrac1;
            }

            Console.WriteLine();
            
        }
    }

    private bool PlayerMove(string hracNaTahu)
    {
        List<Entity> postavy;
        List<Entity> postavyNepritel;
        if (hracNaTahu == _hrac1)
        {
            postavy = _postavyHrac1;
            postavyNepritel = _postavyHrac2;
        }
        else
        {
            postavy = _postavyHrac2;
            postavyNepritel = _postavyHrac1;
        }
        Console.WriteLine($"Hrac {hracNaTahu} je na rade.");
        Console.WriteLine("Tvoji hrdinove jsou:");
        foreach (var hrdina in postavy)
        {
            Console.WriteLine(hrdina);
        }

        Console.WriteLine();
        Console.WriteLine("Vyber si s jakym hrdinou budes hrat");
        for (int i = 0; i < postavy.Count; i++)
        {
            Console.WriteLine($"{postavy[i].Type}/{i + 1}");
        }

        Console.WriteLine();
        int postava = Convert.ToInt16(Console.ReadLine()) - 1;
        Console.WriteLine("Vyber si akci. (utok/1) (kouzlo/2)");
        int akce = Convert.ToInt16(Console.ReadLine());
        
        if (akce == 2)
        {
            postavy[postava].Kouzlo();
        }
        
        else if (akce == 1)
        {
            Console.WriteLine("Rozhodl jsi se zautocit, vyber si nepritelovu postavu, na kterou chces zautocit");
            for (int i = 0; i < postavyNepritel.Count; i++)
            {
                Console.WriteLine($"{postavyNepritel[i].Type}/{i + 1}");
            }

            int utok = Convert.ToInt16(Console.ReadLine()) - 1;
            postavy[postava].Utok(postavyNepritel[utok]);
            if (postavyNepritel[utok].Zivoty <= 0)
            {
                Console.WriteLine($"Uspesne jsi zabil nepritelova {postavyNepritel[utok].Type}");
                postavyNepritel.RemoveAt(utok);
            }
            else
            {
                Console.WriteLine($"Zautocil jsi na nepritelova {postavyNepritel[utok].Type} a zbylo mu {postavyNepritel[utok].Zivoty} zivotu");
            }
        }

        Console.WriteLine();
        Console.WriteLine("------------");
        if (postavyNepritel.Count == 0)
        {
            return false;
        }

        return true;
    }
    
    public void Initiate()
    {
        Console.WriteLine("Vitejte v hre!");
        Console.WriteLine("Cil hry je znicit vsechny hrdiny nepritele.");
        Console.WriteLine("Hrac 1 si nyni muze zvolit jmeno:");
        string s1 = Console.ReadLine();
        Console.WriteLine("Hrac 2 si nyni muze zvolit jmeno:");
        string s2 = Console.ReadLine();
        _hrac1 = s1;
        _hrac2 = s2;
        Console.WriteLine();
    }
}
abstract class Entity
{
    public int Zivoty { get;  protected set; }
    protected int Mana { get;  set; }
    protected int Sila { get;  set; }

    public string Type { get; }
    
    
    public void Utok(Entity nepritel) { nepritel.TakeDamage(Sila); }

    public abstract void Kouzlo();

    public virtual void TakeDamage(int damage) { Zivoty -= damage; }

    public override string ToString()
    {
        return $"{Type} - Pocet zivotu: {Zivoty}, Mana: {Mana}, Sila: {Sila}";
    }

    protected Entity(int z, int m, int s, string t)
    {
        Zivoty = z;
        Mana = m;
        Sila = s;
        Type = t;
    }
}

class Trpaslik : Entity
{
    public Trpaslik() : base(300, 100, 150, "Trpaslik") {}

    private bool _isShieldActive = false;

    public override void TakeDamage(int damage)
    {
        if (_isShieldActive)
        {
            Zivoty -= damage / 2;
            _isShieldActive = false;
        }
        else
        {
            Zivoty -= damage;
        }
    }
    
    public override void Kouzlo()
    {
        if (Mana >= 50)
        {
            Console.WriteLine("Prisel jsi o 50 many, ale mas zaktivovany stit.");
            _isShieldActive = true;
        }
        else
        {
            Console.WriteLine("Nemuzes udelat kouzlo, mas moc malo many");
        }
       
    }

}

class Druid : Entity
{
    public Druid() : base(100, 300, 100, "Druid") { }

    public override void Kouzlo()
    {
        if (Mana >= 50)
        {
            Console.WriteLine("Prisel jsi o 50 many, ale ziskal jsi 50 sily");
            Mana -= 50;
            Sila += 50;
        }
        else
        {
            Console.WriteLine("Nemuzes udelat kouzlo, mas moc malo many");
        }
       
    }
}

class Elf : Entity
{
    public Elf() : base(200, 200, 75, "Elf") { }

    public override void Kouzlo()
    {
        if (Mana >= 50)
        {
            Console.WriteLine("ztratil jsi 50 many, ale ziskal jsi 100 zivotu");
            Mana -= 50;
            Zivoty += 100;
        }
        else
        {
            Console.WriteLine("Nemuzes udelat kouzlo, mas moc malo many");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Game hra = new Game();
        hra.Initiate();
        hra.Loop();
    }
}