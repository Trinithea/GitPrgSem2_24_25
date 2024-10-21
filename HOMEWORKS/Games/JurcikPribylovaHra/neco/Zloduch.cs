using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skbidihra
{
    public class Zloduch : Postava
    {
        Random rnd = new Random();
        private int x;
        private int y;

        public Zloduch(int _x, int _y) : base(4, 1, 1)
        {
            x = _x;
            y = _y;
            obrazek = 'X';
            Pozice = new int[2] { rnd.Next(0, x + 1), rnd.Next(0, y + 1) };
        }

        public void uberZivoty(int utrzenePoskozeni)
        {
            Zivoty -= utrzenePoskozeni;
        }

        public void ZmenPozici()
        {
            Pozice[0] = random.Next(0, x + 1);
            Pozice[1] = random.Next(0, y + 1);
        }


    }
}