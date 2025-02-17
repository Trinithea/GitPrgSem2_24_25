using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Minesweeper
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public double MineProbability = 20;


        private Tile[,] Tiles { get; set; }
        private Image[,] Buttons { get; set; }
        private enum Size {Tiny, Small, Medium, Large }
        private Size BoardSize { get; set; }

        private UniformGrid GridU { get; set; }
        private Image faceImg;

        private bool FirstMovePlayed { get; set; }

        private bool IsFirstGame = true;
        public int MineCount { get; set; }
        private int TilesFlagged { get; set; }
        private int TilesRevealed { get; set; }


        public Dictionary<string, BitmapImage> tileTextures = new Dictionary<string, BitmapImage>();





        private Dictionary<Size, int> tilesPerSide = new Dictionary<Size, int>()
            {
                {Size.Tiny, 6},
                {Size.Small, 9},
                {Size.Medium, 12},
                {Size.Large, 15}
            };
        public int SideLength
        {
            get => tilesPerSide[BoardSize];
            set
            {
                if (tilesPerSide[BoardSize] != value)
                {
                    tilesPerSide[BoardSize] = value;
                    OnPropertyChanged();
                }
            }

        }


        

       private int _minesRemaining;
        public int MinesRemaining
        {
            get => _minesRemaining;
            set
            {
                if (_minesRemaining != value)
                {
                    _minesRemaining = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _gameIsRunning;
        public bool GameIsRunning
        {
            get { return !_gameIsRunning; }
            set
            {
                if (_gameIsRunning != value)
                {
                    _gameIsRunning = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



        private Dictionary<string, string> texturesTemp = new Dictionary<string, string>()
            {
                {"cell0", @"/resources/cells/cell0.png"},
                {"cell1", @"/resources/cells/cell1.png" },
                {"cell2", @"/resources/cells/cell2.png" },
                {"cell3", @"/resources/cells/cell3.png" },
                {"cell4", @"/resources/cells/cell4.png" },
                {"cell5", @"/resources/cells/cell5.png" },
                {"cell6", @"/resources/cells/cell6.png" },
                {"cell7", @"/resources/cells/cell7.png" },
                {"cell8", @"/resources/cells/cell8.png" },
                {"cellflag", @"/resources/cells/cellflag.png" },
                {"cellmine", @"/resources/cells/cellmine.png" },
                {"cellup", @"/resources/cells/cellup.png" },
                {"cellfalsemine", @"/resources/cells/falsemine.png" },
                {"cellblast", @"/resources/cells/blast.png" },

                {"facesmile", @"/resources/faces/smileface.png" },
                {"facesmiledown", @"/resources/faces/smilefacedown.png" },
                {"faceclick", @"/resources/faces/clickface.png" },
                {"facewin", @"/resources/faces/winface.png" },
                {"facelost", @"/resources/faces/lostface.png" },


        };

        public void LoadTexturesVM()
        {
            foreach (KeyValuePair<string, string> entry in texturesTemp)
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(entry.Value, UriKind.Relative);
                bitmapImage.EndInit();
                tileTextures.Add(entry.Key, bitmapImage);
            }
        }
        public void StartGame(string mode, UniformGrid grid, Image faceImage)
        {
            if (!IsFirstGame)
            {
                for (int i = 0; i < SideLength; i++)
                {
                    for (int  j = 0; j < SideLength; j++)
                    {
                        GridU.Children.Remove(Buttons[i, j]);
                        Buttons[i, j] = null;   //nevím jestli se to doopravdy smaže, snad jo
                        Tiles[i, j] = null;

                    }
                }
                Buttons = null;
                Tiles = null;
            }
            
            faceImg = faceImage;
            BoardSize = (Size)Enum.Parse(typeof(Size), mode);
            GridU = grid;
            MineCount = 0;
            TilesRevealed = 0;
            TilesFlagged = 0;
            CreateTiles();
            CreateGrid();
            PlaceMines();
            GameIsRunning = true;            
            FirstMovePlayed = false;
            IsFirstGame = false;
        }
        public void RestartGame()
        {
            foreach (Image b in Buttons)
            {
                GridU.Children.Remove(b);

            }
            Buttons = null;
            Tiles = null;
        }
        private void CreateTiles()
        {
            Tiles = new Tile[SideLength, SideLength];
            for (int i = 0; i < SideLength; i++)
            {
                for (int j = 0; j < SideLength; j++)
                {
                    Tile tile = new Tile();
                    Tiles[i, j] = tile;

                }
            }
            for (int i = 0; i < SideLength; i++)
            {
                for (int j = 0; j < SideLength; j++)
                {
                    FindNeighbours(i, j);
                }
            }
        }
        private void FindNeighbours(int a, int b)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 & j == 0) { continue; }
                    try
                    {
                        if (Tiles[a + i, b + j] != null) { Tiles[a, b].Neighbours.Add(Tiles[a + i, b + j]); }
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
            }
        }
        private void CreateGrid()
        {
            Buttons = new Image[this.SideLength, this.SideLength];
            for (int i = 0; i < SideLength; i++)
            {
                for (int j = 0; j < SideLength; j++)
                {
                     
                    Image button = new Image();
                    button.MouseLeftButtonUp += TileClicked;
                    button.MouseRightButtonUp += TileFlagged;
                    Buttons[i, j] = button;
                    button.Source = tileTextures["cellup"];
                    GridU.Children.Add(button);
                }
            }
        }
        private void PlaceMines()
        {
            Random random = new Random();
            for (int i = 0; i < SideLength; i++)
            {
                for (int j = 0; j < SideLength; j++)
                {
                    if (random.Next(100) < MineProbability)
                    {
                        Tiles[i, j].IsMine = true;
                        foreach (Tile tile in Tiles[i, j].Neighbours) { tile.AdjacentMines++; }
                        MineCount++;
                    }
                }
            }
            MinesRemaining = MineCount;
        }
        private void TileFlagged(object sender, RoutedEventArgs e)
        {
            Tuple<int, int> coords = ButtonCoordinates(sender);
            Tile tile = Tiles[coords.Item1, coords.Item2];
            if (!tile.IsRevealed)
            {
                tile.IsFlagged = !tile.IsFlagged;
                if (tile.IsFlagged) 
                { 
                    MinesRemaining--;
                    TilesFlagged++;
                    ((Image)sender).Source = tileTextures["cellflag"];
                    foreach (Tile t in tile.Neighbours)
                    {
                        t.AdjacentFlags++;
                    }
                }
                else 
                {
                    MinesRemaining++;
                    TilesFlagged--;
                    ((Image)sender).Source = tileTextures["cellup"];
                    foreach (Tile t in tile.Neighbours)
                    {
                        t.AdjacentFlags--;
                    }
                }
            }
            
        }
        private void TileClicked(object sender, RoutedEventArgs e)
        {
            faceImg.Source = tileTextures["facesmiledown"];
            Tuple<int, int> coords = ButtonCoordinates(sender);
            Tile tile = Tiles[coords.Item1, coords.Item2];
            if (!tile.IsFlagged)
            {
                if (!FirstMovePlayed)
                {
                    FirstTileClicked(sender, e);
                }
                else if (tile.IsRevealed & tile.AdjacentFlags == tile.AdjacentMines)
                {
                    foreach (Tile t in tile.Neighbours)
                    {
                        if (!t.IsFlagged & !t.IsRevealed)
                        {
                            Tuple<int, int> coords1 = TileCoordinates(t);
                            Image b = Buttons[coords1.Item1, coords1.Item2];
                            RevealTile(b, t);
                            t.IsRevealed = true;
                        }
                    }
                }
                else
                {
                        RevealTile(sender, tile);                   
                }
            }
            faceImg.Source = tileTextures["facesmile"];

        }
        //zařizuje aby první pole nemělo kolem sebe minu
        private void FirstTileClicked(object sender, RoutedEventArgs e) 
        {
            Tuple<int, int> coords = ButtonCoordinates(sender);
            Tile tile = Tiles[coords.Item1, coords.Item2];
            if (tile.IsMine)
            {
                tile.IsMine = false;
                MinesRemaining--;
                MineCount--;
                foreach (Tile t in tile.Neighbours)
                {
                    if (t.AdjacentMines != 0)
                    {
                        t.AdjacentMines--;
                    }
                }                
            }
            foreach (Tile t in tile.Neighbours)
            {
                if (t.IsMine)
                {
                    t.IsMine = false;
                    MinesRemaining--;
                    MineCount--;
                    foreach (Tile t2 in t.Neighbours)
                    {
                        if (t2.AdjacentMines != 0)
                        {
                            t2.AdjacentMines--;
                        }
                    }
                }
            }


            RevealTile(sender, tile);
            FirstMovePlayed = true;
        }
        private Tuple<int, int> ButtonCoordinates(object sender)
        {
            for (int i = 0; i < SideLength; i++)
            {
                for (int j = 0; j < SideLength; j++)
                {
                    if (Buttons[i, j].Equals(sender))
                        return Tuple.Create(i, j);
                }
            }
            return Tuple.Create(-1, -1);
        }
        private Tuple<int, int> TileCoordinates(Tile tile) //chtělo by to nahradit
        {
            for (int i = 0; i < SideLength; i++)
            {
                for (int j = 0; j < SideLength; j++)
                {
                    if (Tiles[i, j].Equals(tile))
                        return Tuple.Create(i, j);
                }
            }
            return Tuple.Create(-1, -1);
        }

        private void RevealTile(object sender, Tile tile)
        {

            if (tile.IsMine)
            {
                GameLost();
                ((Image)sender).Source = tileTextures["cellblast"];
            }
            else
            {
                string textureID = "cell" + tile.AdjacentMines.ToString();
                ((Image)sender).Source = tileTextures[textureID];
                tile.IsRevealed = true;
                TilesRevealed++;
                if ((TilesRevealed + MineCount == SideLength * SideLength)& (MinesRemaining + TilesFlagged == MineCount))
                {
                    GameWon();
                }
                if (tile.AdjacentMines == 0)
                {
                    foreach (Tile t in tile.Neighbours)
                    {
                        if (!t.IsRevealed)
                        {
                            Tuple<int, int> coords = TileCoordinates(t);
                            Image button = Buttons[coords.Item1, coords.Item2];
                            RevealTile(button, t);

                        }
                    }
                }
                
            }
        }
        private void GameLost()
        {
            foreach (Image b in Buttons)
            {
                b.IsEnabled = false;
            }
            GameIsRunning = false;
            foreach (Tile tile in Tiles)
            {
                if (tile.IsMine)
                {
                    Tuple<int, int> coords = TileCoordinates(tile);
                    Image button = Buttons[coords.Item1, coords.Item2];
                    ((Image)button).Source = tileTextures["cellmine"];
                }
                else if (!tile.IsMine & tile.IsFlagged)
                {
                    Tuple<int, int> coords = TileCoordinates(tile);
                    Image button = Buttons[coords.Item1, coords.Item2];
                    ((Image)button).Source = tileTextures["cellfalsemine"];
                }                              
            }
            faceImg.Source = tileTextures["facelost"];
            MessageBox.Show("You lost.");

        }
        private void GameWon()
        {
            foreach (Image b in Buttons)
            {
                b.IsEnabled = false;
            }
            GameIsRunning = false;
            foreach (Tile tile in Tiles)
            {
                if (tile.IsMine & !tile.IsFlagged)
                {
                    Tuple<int, int> coords = TileCoordinates(tile);
                    Image button = Buttons[coords.Item1, coords.Item2];
                    ((Image)button).Source = tileTextures["cellflag"];
                }
            }
            faceImg.Source = tileTextures["facewin"];
            MessageBox.Show("You won!");

        }

        
    }
}
