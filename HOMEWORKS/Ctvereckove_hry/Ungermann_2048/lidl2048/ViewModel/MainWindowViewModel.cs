using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using lidl2048.Model;

namespace lidl2048.ViewModel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public enum Direction
        {
            LEFT = 0,
            RIGHT = 1,
            UP = 0,
            DOWN = 1
        }

        private static int _size { get; set; }
        private bool _win {  get; set; }
        private static List<List<Tile>> _tileMap {  get; set; }
        private static List<int[]> _freeSpaces { get; set; }

        private static Grid _gameGrid { get; set; }
        private static Random _random = new Random();

        private static MyColors _colors {  get; set; }

        private int _score;
        public int Score
        {
            get => _score;
            set
            {
                if (_score != value)
                {
                    _score = value;
                    OnPropertyChanged();
                }
            }
        }

        // možná úprava jména hry (pouze základní čílo, ne násobek) :)
        private int _baseNumber;
        private int _nameOfGame;
        public int NameOfGame
        {
            get => _nameOfGame;
            set
            {
                if (_nameOfGame != value)
                {
                    _nameOfGame = value;
                    OnPropertyChanged();
                }
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void LoseFunction()
        {
            MessageBox.Show($"Bohužel jsi prohrál/a. Tvé finální skóre je {Score}.", $"{NameOfGame}", MessageBoxButton.OK, MessageBoxImage.Information);
            Score = 0;
            ClearGrid();
        }

        private void WinFunction()
        {
            MessageBox.Show($"Sláva, jsi mistr {NameOfGame}. Tvé skóre je {Score}. Dokážeš doáhnout {NameOfGame * 2}?", $"{Math.Pow(NameOfGame, 11)}", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void RestartGame()
        {
            MessageBoxResult result = MessageBox.Show("Chceš začít odznova?", "2048", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes) 
            {
                Score = 0;
                ClearGrid();
            }

        } 
     
        private void ClearGrid()
        {
            for (int i = 0; i < _size; i++)
            {
                _tileMap[i].Clear();
                for (int j = 0; j < _size; j++)
                {
                    _tileMap[i].Add(new Tile(0));
                }
            }
            GenerateNewTile(); 
            RenderTiles();
        }

        private List<Tile> Move(List<Tile> row, Direction d)
        {
            List<Tile> movedRow = new List<Tile>();

            if (d == Direction.RIGHT)
            {
                movedRow.Reverse();
            }

            for (int i = 0; i < row.Count; i++)
            {
                if (row[i].Number != 0)
                {
                    movedRow.Add(row[i]);
                }
            }
            int fillCount = row.Count - movedRow.Count;
            for (int i = 0; i < fillCount; i++)
            {
                movedRow.Add(new Tile(0));
            }

            if (d == Direction.RIGHT)
            {
                movedRow.Reverse();
            }

            return movedRow;
        }

        private List<Tile> Merge(List<Tile> row, Direction d)
        {
            List<Tile> mergedRow = new List<Tile>();

            int start = 0;
            int end = row.Count;
            int difference = 1;

            if (d == Direction.RIGHT)
            {
                row.Reverse();
            }
            for (int i = start; i < end; i += difference)
            {
                int x = row[i].Number;
                int y;
                if (i + 1 == end)
                {
                    y = -1;
                }
                else
                {
                    y = row[i + 1].Number;
                }

                if (x != y )
                {
                    mergedRow.Add(row[i]);
                }
                else if (x != 0)
                {
                    mergedRow.Add(new Tile(x + y));
                    mergedRow.Add(new Tile(0));
                    Score += x + y;
                    i++;
                }
                if (x + y == _nameOfGame)
                {
                    _win = true;
                }
            }


            int fillCount = row.Count - mergedRow.Count;
            for (int i = 0; i < fillCount; i++)
            {
                mergedRow.Add(new Tile(0));
            }

            if (d == Direction.RIGHT)
            {
                mergedRow.Reverse();
            }

            return mergedRow;
        }

        private void WinCheck()
        {
            if (_win)
            {
                WinFunction();
                _win = false;
            }
        }
        public void HandleHorizontal(Direction d)
        {
            for (int i = 0; i < _tileMap.Count; i++)
            {
                _tileMap[i] = Move(Merge(Move(_tileMap[i], d), d), d);
            }
            GenerateNewTile();
            RenderTiles();
            WinCheck();

        }

        public void HandleVertical(Direction d)
        {
            List<List<Tile>> cols = new List<List<Tile>>();

            for (int i = 0; i < _tileMap.Count; i++)
            {
                cols.Add(new List<Tile>());
                for (int j = 0; j < _tileMap.Count; j++)
                {
                    cols[i].Add(_tileMap[j][i]);
                }
            }

            for (int i = 0; i < cols.Count; i++)
            {
                cols[i] = Move(Merge(Move(cols[i], d), d), d);
            }

            for (int i = 0; i < _tileMap.Count; i++)
            {
                for (int j = 0; j < _tileMap.Count; j++)
                {
                    _tileMap[j][i] = cols[i][j];
                }
            }
            GenerateNewTile();
            RenderTiles();
            WinCheck();
        }

        private SolidColorBrush TextColorHelper(int n)
        {
            string color;
            if (n <= _baseNumber * 2)
            {
                color = "#776E65";
            }
            else
            {
                color = "#F9F6F2";
            }

            return (SolidColorBrush)new BrushConverter().ConvertFromString(color);
        }
        private void RenderTiles()
        {
            _gameGrid.Children.Clear();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {

                    TextBlock t = new TextBlock
                    {
                        Text = _tileMap[i][j].Number.ToString(),
                        Foreground = TextColorHelper(_tileMap[i][j].Number),
                        FontSize = 40,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextAlignment = TextAlignment.Center
                    };

                    Rectangle rect = new Rectangle
                    {
                        Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(_colors.ColorList[_tileMap[i][j].Number]),
                        Margin = new Thickness(10),
                        RadiusX = 10,
                        RadiusY = 10
                    };



                    Grid tileInCell = new Grid();

                    if (_tileMap[i][j].Number == 0)
                    {
                        tileInCell.Children.Add(rect);
                    }
                    else
                    {
                        tileInCell.Children.Add(rect);
                        tileInCell.Children.Add(t);
                    }

                    Grid.SetRow(tileInCell, i);
                    Grid.SetColumn(tileInCell, j);
                    _gameGrid.Children.Add(tileInCell);
                }
            }
        }

        private void GenerateNewTile() 
        {
            _freeSpaces.Clear();
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    if (_tileMap[i][j].Number == 0)
                    {
                        _freeSpaces.Add([ i, j ]);
                    }
                }
            }

            int chanceFour = _random.Next(0, 10);
            int spawPlace = _random.Next(0, _freeSpaces.Count);

            if (_freeSpaces.Count == 0)
            {
                LoseFunction();
            }
            else
            {
                int[] coords = [_freeSpaces[spawPlace][0], _freeSpaces[spawPlace][1]];
                if (chanceFour == 10)
                {
                    _tileMap[coords[0]][coords[1]] = new Tile(_baseNumber * 2);
                }
                else
                {
                    _tileMap[coords[0]][coords[1]] = new Tile(_baseNumber);
                }
            }
        }
        public MainWindowViewModel(Grid grid, int size, int n) 
        {
            int r = grid.RowDefinitions.Count;
            int c = grid.ColumnDefinitions.Count;

            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#97989c"),
                        Margin = new Thickness(10),
                        RadiusX = 10,
                        RadiusY = 10
                    };



                    Grid.SetRow(rect, i);
                    Grid.SetColumn(rect, j);

                   
                    grid.Children.Add(rect);
                }
            }


            _baseNumber = n;
            _nameOfGame = (int)Math.Pow(n, 11);
            _tileMap = new List<List<Tile>>();
            _size = size;
            _gameGrid = grid;
            _freeSpaces = new List<int[]>();
            _colors = new MyColors(n);
            _win = false;
            Score = 0;
            for (int i = 0; i < size; i++)
            {
                _tileMap.Add(new List<Tile>());
                for (int j = 0; j < size; j++)
                {
                    _tileMap[i].Add(new Tile(0));
                }
            }

            GenerateNewTile();
            RenderTiles();
        }
    }
}
