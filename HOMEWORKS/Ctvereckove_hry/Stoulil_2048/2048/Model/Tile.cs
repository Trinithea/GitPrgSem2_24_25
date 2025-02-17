using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Model
{
    internal class Tile
    {

        public int Value;
        public (int x, int y) Coords;

        public Tile(int x, int y, int val)
        {
            this.Value = val;
            Coords = (x, y);
        }

    }
}
