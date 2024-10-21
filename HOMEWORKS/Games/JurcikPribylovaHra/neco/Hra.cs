
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skbidihra
{
    internal class Hra
    {
        private char[,] pole;
        private Hrdina hrdina;
        private Zloduch zloduch;
        public Hra(int x, int y)
        {
            zloduch = new Zloduch(x, y);
            pole = new char[x, y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    pole[i, j] = '.';
                }
            }
            vyberPostavy();

        }

        private void vyberPostavy()
        {
            Console.WriteLine("Vyberte si postavu (Druid, Opilec, Podvodnik, Gambler)");
            string input = Console.ReadLine();
            switch (input.ToLower())
            {
                case "druid":
                    hrdina = new Druid();
                    break;
                case "opilec":
                    hrdina = new Opilec();
                    break;
                case "podvodnik":
                    hrdina = new Podvodnik();
                    break;
                case "gambler":
                    hrdina = new Gambler();
                    break;
                default:
                    Console.WriteLine("Spatne zadana postava");
                    vyberPostavy();
                    break;
            }
            Console.Clear();
        }

        private void VykresliHru()
        {
            Console.Clear();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pole.GetLength(0); i++)
            {
                for (int j = 0; j < pole.GetLength(1); j++)
                {
                    if (i == hrdina.Pozice[0] && j == hrdina.Pozice[1])
                    {
                        sb.Append(hrdina.obrazek);
                        continue;
                    }
                    if (i == zloduch.Pozice[0] && j == zloduch.Pozice[1])
                    {
                        sb.Append(zloduch.obrazek);
                        continue;
                    }

                    sb.Append(pole[i, j]);
                }

                Console.WriteLine(sb.ToString());
                sb.Clear();
            }

            Console.WriteLine($"zivoty: {hrdina.Zivoty}");
            Console.WriteLine($"zloduchovy zivoty: {zloduch.Zivoty}");
            Console.WriteLine($"pocet vitezstvi: {hrdina._pocetVitezstvi}");
        }

        public void Hraj()
        {
            ConsoleKey tlacitko;
            VykresliHru();
            while (true)
            {
                if (hrdina._pocetVitezstvi == 3)
                {
                    Console.WriteLine("Vyhral jsi!");
                    break;
                }
                else if (hrdina.Zivoty <= 0)
                {
                    Console.WriteLine("Prohral jsi!");
                    break;
                }


                tlacitko = Console.ReadKey(true).Key;

                if (tlacitko == ConsoleKey.D)
                {
                    hrdina.pohniSeDoprava();
                    VykresliHru();
                }
                else if (tlacitko == ConsoleKey.S)
                {
                    hrdina.pohniSeDolu();
                    VykresliHru();
                }
                else if (tlacitko == ConsoleKey.A)
                {
                    hrdina.pohniSeDoleva();
                    VykresliHru();
                }
                else if (tlacitko == ConsoleKey.W)
                {
                    hrdina.pohniSeNahoru();
                    VykresliHru();
                }
                else if (tlacitko == ConsoleKey.Spacebar)
                {
                    if (decimal.Abs(zloduch.Pozice[0] - hrdina.Pozice[0]) <= 1 && decimal.Abs(zloduch.Pozice[1] - hrdina.Pozice[1]) <= 1)
                    {
                        hrdina.Utok(zloduch);
                    }
                }
            }


        }




    }
}