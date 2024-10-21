
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skbidihra
{
    public class Hrdina : Postava
    {
        public int _pocetVitezstvi { get; private set; }
        public int Penize { get; private set; }
        public Hrdina(int zivoty, int hody, int poskozeni) : base(zivoty, hody, poskozeni)
        {
            obrazek = 'P';
            Penize = 0;
            Pozice = new int[2] { 0, 0 };
            _pocetVitezstvi = 0;

        }


        public void Utok(Zloduch protivnik)
        {
            int hrdinuvHod = HodKostkou();
            int zloduchuvHod = protivnik.HodKostkou();
            Console.WriteLine($"hrdina hodil {hrdinuvHod}, zloduch hodil {zloduchuvHod}");
            if (zloduchuvHod < hrdinuvHod)
            {
                VyhralSouboj(protivnik);
            }
            else
            {
                prohralSouboj(protivnik.Poskozeni, protivnik);
            }
        }

        protected void VyhralSouboj(Zloduch protivnik)
        {
            
            
            protivnik.uberZivoty(Poskozeni);
            Console.WriteLine("Vyhral jsi souboj");
            if (protivnik.Zivoty <= 0)
            {
                Console.WriteLine("protivnik zabit");
                protivnik.ZmenPozici();
                _pocetVitezstvi++;
            }

        }
        protected void prohralSouboj(int Poskozeni, Zloduch protivnik)
        {
            Zivoty -= Poskozeni;
            Console.WriteLine("Prohral jsi souboj");
        }
        public void pohniSeDoprava()
        {
            Pozice[1]++;
        }
        public void pohniSeDoleva()
        {
            Pozice[1]--;
        }
        public void pohniSeNahoru()
        {
            Pozice[0]--;
        }
        public void pohniSeDolu()
        {
            Pozice[0]++;
        }
    }
}