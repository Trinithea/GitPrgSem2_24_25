
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skbidihra
{
    public abstract class Postava
    {
        public int[] Pozice { get; protected set; }
        protected static Random random = new Random();
        public int Zivoty { get; protected set; }
        public int Hody { get; }
        public int Poskozeni { get; }
        public char obrazek { get; protected set; }

        public Postava(int zivoty, int hody, int poskozeni)
        {
            this.Zivoty = zivoty;
            this.Hody = hody;
            this.Poskozeni = poskozeni;

        }


        public virtual int HodKostkou()
        {
            int soucet = 0;

            for (int i = 0; i < Hody; i++)
            {
                soucet += random.Next(1, 7);
            }
            return soucet;
        }


    }
}