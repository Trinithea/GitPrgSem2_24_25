using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skbidihra;

public class Podvodnik : Hrdina
{
    public Podvodnik() : base(6, 1, 1) { }

    public override int HodKostkou()
    {
        int soucet = 0;

        for (int i = 0; i < Hody; i++)
        {
            soucet += random.Next(5, 7);
        }
        return soucet;
    }
}
