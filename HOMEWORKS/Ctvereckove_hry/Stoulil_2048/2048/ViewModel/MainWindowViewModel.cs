using _2048.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace _2048.ViewModel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private Grid GameGrid { get; set; }

        private const int GridSize = 4;

        private Tile[,] Map { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        private int score;
        public int Score
        {
            get => score;
            set
            {
                if (score != value)
                {
                    score = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility paused;
        public Visibility Paused
        {
            get => paused;
            set
            {
                if (paused != value)
                {
                    paused = value;
                    OnPropertyChanged();

                    // znovu nastavení hry
                    if(paused == Visibility.Visible)
                    {
                        StartGameWithDelay();
                    }


                }
            }
        }

        private async void StartGameWithDelay()
        {
            await Task.Delay(1500);
            Score = 0;
            InitializeGame(GameGrid);
        }

        public void InitializeGame(Grid grid)
        {
            GameGrid = grid;
            GameGrid.ColumnDefinitions.Clear();
            GameGrid.RowDefinitions.Clear();
            Paused = Visibility.Hidden;

            for (int i = 0; i < GridSize; i++)
            {
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < GridSize; i++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition());
            }

            Map = new Tile[GridSize, GridSize];

            InitializeTile();

            RenderMap();
        }


        private void InitializeTile()
        {
            Random r = new Random();

            int rX = r.Next(0, GridSize);
            int rY = r.Next(0, GridSize);
            int val = (r.Next(0, 2) == 0) ? 2 : 4;

            Tile tile = new Tile(rX, rY, val);

            if (Map[tile.Coords.x, tile.Coords.y] == null)
            {
                //MessageBox.Show(rX.ToString() + ", " + rY.ToString());
                Map[tile.Coords.x, tile.Coords.y] = tile;
                return;
            }
            else
            {
                InitializeTile();
            }
        }

        public void Move(object direction)
        {
            if(Paused == Visibility.Visible) return;

            if (direction is string dir)
            {
                switch (dir)
                {
                    case "Up": MoveUp(); break;
                    case "Down": MoveDown(); break;
                    case "Left": MoveLeft(); break;
                    case "Right": MoveRight(); break;
                }
            }

            InitializeTile(); //random tile

            RenderMap();

            EndCheck(); //pokud všude v Mapě null tak end
        }

        private void MoveUp()
        {
            for (int x = 0; x < Map.GetLength(0); x++)
            {
                for (int y = 1; y <= Map.GetLength(1) - 1; y++)
                {
                    Tile tile = Map[x, y];

                    if (tile != null)
                    {

                        //MessageBox.Show(y.ToString() + "moving tile");
                        MoveTileUp(tile);

                    }
                }
            }
        }

        private void MoveTileUp(Tile tile)
        {
            for (int y = tile.Coords.y; y > 0; y--)
            {
                int nextY = y - 1;

                if (Map[tile.Coords.x, nextY] == null)
                {

                    tile.Coords.y = nextY;
                    Map[tile.Coords.x, y] = null;
                    Map[tile.Coords.x, nextY] = tile;

                }
                else
                {
                    if (Map[tile.Coords.x, y].Value == Map[tile.Coords.x, nextY].Value)
                    {
                        tile.Value += Map[tile.Coords.x, nextY].Value;
                        tile.Coords.y = nextY;
                        Map[tile.Coords.x, y] = null;
                        Map[tile.Coords.x, nextY] = tile;

                        Score += tile.Value;
                    }
                    break;
                }
            }
        }
        private void MoveDown()
        {
            for (int x = 0; x < Map.GetLength(0); x++)
            {
                for (int y = Map.GetLength(1) - 2; y >= 0; y--)
                {
                    Tile tile = Map[x, y];

                    if (tile != null)
                    {
                        //MessageBox.Show(y.ToString() + "moving tile");
                        MoveTileDown(tile);


                    }
                }
            }
        }

        private void MoveTileDown(Tile tile)
        {
            for (int y = tile.Coords.y; y < Map.GetLength(1) - 1; y++)
            {

                int nextY = y + 1;

                if (Map[tile.Coords.x, nextY] == null)
                {
                    // posun
                    tile.Coords.y = nextY;
                    Map[tile.Coords.x, y] = null;
                    Map[tile.Coords.x, nextY] = tile;

                }
                else
                {
                    if (Map[tile.Coords.x, y].Value == Map[tile.Coords.x, nextY].Value)
                    {
                        tile.Value += Map[tile.Coords.x, nextY].Value; //dvoj násobek
                        tile.Coords.y = nextY;
                        Map[tile.Coords.x, y] = null;
                        Map[tile.Coords.x, nextY] = tile;

                        Score += tile.Value;
                    }


                    break;
                }

            }
        }

        private void MoveLeft()
        {
            for (int y = 0; y < Map.GetLength(1); y++)
            {
                for (int x = 1; x <= Map.GetLength(0) - 1; x++)
                {
                    Tile tile = Map[x, y];

                    if (tile != null)
                    {

                        //MessageBox.Show(x.ToString() + "moving tile left");
                        MoveTileLeft(tile);

                    }
                }
            }

        }

        private void MoveTileLeft(Tile tile)
        {
            for (int x = tile.Coords.x; x > 0; x--)
            {
                int nextX = x - 1;

                if (Map[nextX, tile.Coords.y] == null)
                {
                    tile.Coords.x = nextX;
                    Map[x, tile.Coords.y] = null;
                    Map[nextX, tile.Coords.y] = tile;

                }
                else
                {
                    if (Map[x, tile.Coords.y].Value == Map[nextX, tile.Coords.y].Value)
                    {
                        tile.Value += Map[nextX, tile.Coords.y].Value;
                        tile.Coords.x = nextX;
                        Map[x, tile.Coords.y] = null;
                        Map[nextX, tile.Coords.y] = tile;

                        Score += tile.Value;
                    }


                    break;
                }

            }
        }


        private void MoveRight()
        {
            for (int y = 0; y < Map.GetLength(1); y++)
            {
                for (int x = Map.GetLength(0) - 2; x >= 0; x--)
                {
                    Tile tile = Map[x, y];

                    if (tile != null)
                    {
                        //MessageBox.Show(x.ToString() + "moving tile");
                        MoveTileRight(tile);

                    }
                }
            }
        }

        private void MoveTileRight(Tile tile)
        {
            for (int x = tile.Coords.x; x < Map.GetLength(0) - 1; x++)
            {
                int nextX = x + 1;

                if (Map[nextX, tile.Coords.y] == null)
                {
                    tile.Coords.x = nextX;
                    Map[x, tile.Coords.y] = null;
                    Map[nextX, tile.Coords.y] = tile;

                }
                else
                {
                    if (Map[x, tile.Coords.y].Value == Map[nextX, tile.Coords.y].Value)
                    {
                        tile.Value += Map[nextX, tile.Coords.y].Value;
                        tile.Coords.x = nextX;
                        Map[x, tile.Coords.y] = null;
                        Map[nextX, tile.Coords.y] = tile;

                        Score += tile.Value;
                    }

                    break;
                }

            }
        }

        private void RenderMap()
        {

            GameGrid.Children.Clear();

            for (int y = 0; y < Map.GetLength(0); y++)
            {
                for (int x = 0; x < Map.GetLength(1); x++)
                {
                    Tile tile = Map[x, y];

                    //MessageBox.Show(x.ToString() + ", " + y.ToString());

                    if (tile != null)
                    {
                        SolidColorBrush color = new SolidColorBrush(Colors.Red);
                        switch (tile.Value)
                        {
                            case 4:
                                color = new SolidColorBrush(Colors.Green);
                                break;
                            case 8:
                                color = new SolidColorBrush(Colors.Yellow);
                                break;
                            case 16:
                                color = new SolidColorBrush(Colors.Orange);
                                break;
                            case 32:
                                color = new SolidColorBrush(Colors.Purple);
                                break;
                        }

                        RenderTile(color, tile.Coords, tile.Value);
                        //MessageBox.Show((x.ToString() + ", " + y.ToString()) + ", tile");
                    }
                    else
                    {
                        RenderTile(new SolidColorBrush(Colors.Bisque), (x, y));
                        //MessageBox.Show((x.ToString() + ", " + y.ToString()) + ", null");
                    }
                }
            }

        }

        private void RenderTile(SolidColorBrush color, (int x, int y) Coords, int Value = 0)
        {
            Canvas tileContainer = new Canvas();

            int colWidth = (int)GameGrid.ActualWidth / GridSize;
            int rowWidth = (int)GameGrid.ActualHeight / GridSize;
            //MessageBox.Show(GameGrid.ActualHeight.ToString());

            Rectangle tileBox = new Rectangle();
            tileBox.Fill = color;
            tileBox.Width = (color.Color == Colors.Red) ? colWidth * 0.85 : colWidth * 0.8;
            tileBox.Height = (color.Color == Colors.Red) ? rowWidth * 0.85 : rowWidth * 0.8;
            tileBox.RadiusX = 10;
            tileBox.RadiusY = 10;

            TextBlock text = new TextBlock();
            text.Text = (Value != 0) ? Value.ToString() : "";
            text.FontSize = 24;
            text.Foreground = new SolidColorBrush(Colors.Black);
            text.FontWeight = FontWeights.Bold;

            tileBox.Loaded += (s, e) =>
            {
                Canvas.SetLeft(tileBox, (colWidth * 0.15) / 2);
                Canvas.SetTop(tileBox, (rowWidth * 0.15) / 2);

                Canvas.SetLeft(text, (colWidth - text.ActualWidth) / 2);
                Canvas.SetTop(text, (rowWidth - text.ActualHeight) / 2);
            };

            tileContainer.Children.Add(tileBox);
            tileContainer.Children.Add(text);

            Grid.SetColumn(tileContainer, Coords.x); // x
            Grid.SetRow(tileContainer, Coords.y); // y

            GameGrid.Children.Add(tileContainer);
        }

        private void EndCheck()
        {
            bool end = true;

            for (int x = 0; x < Map.GetLength(0); x++)
            {
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    if (Map[x, y] == null)
                    {
                        end = false;                   
                    }
                }
            }
            Paused = end ? Visibility.Visible : Visibility.Hidden;
      
        }

    }

}
