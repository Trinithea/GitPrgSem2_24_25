using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public class Tile
    {
        public Tile() 
        {
            AdjacentMines = 0;
            AdjacentFlags = 0;
            IsMine = false;
            IsRevealed = false;
            IsFlagged = false;
            Neighbours = new List<Tile>();
        }
        public int AdjacentMines { get; set; }
        public int AdjacentFlags { get; set; }
        public bool IsMine { get; set; }
        public bool IsRevealed { get; set; }
        public bool IsFlagged { get; set; }
        public List<Tile> Neighbours { get; set; }
    }
}
