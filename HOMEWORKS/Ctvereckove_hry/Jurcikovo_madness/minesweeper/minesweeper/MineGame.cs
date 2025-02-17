using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows;

namespace minesweeper
{
    internal class MineGame : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private MineButton[,] MineGrid;
        private Grid GameDisplayGrid;
        private int gridSize;
        private int numOfMines;
        private int numOfFlags;
        public ICommand FlagButtonClicked { get; }


        public int FlagsLeft
        {
            get { return numOfFlags; }
            set { numOfFlags = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FlagsLeft")); }
        }

        private bool flagSelected;
        private bool FlagSelected { get => flagSelected; set { flagSelected = value; FlagBorderColor = value ? Brushes.Red : Brushes.Green; } }
        private int unclickedTiles;
        BitmapImage flagImageBitmap = new();
        BitmapImage mineImageBitmap = new();
        ImageBrush flagImage = new();
        ImageBrush mineImage = new();
        private (int, int)[] MinePositions;
        private Brush[] numberColors = new Brush[]
        {
            Brushes.White,
            Brushes.LightGreen,
            Brushes.Blue,
            Brushes.Orange,
            Brushes.Orchid,
            Brushes.Purple,
            Brushes.DarkOrange,
            Brushes.DarkRed,
        };

        private Visibility blockerVisibility;

        public Visibility BlockerVisibility
        {
            get { return blockerVisibility; }
            set { blockerVisibility = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BlockerVisibility")); }
        }

        private Brush flagBrush;

        public Brush FlagBorderColor
        {
            get { return flagBrush; }
            set { flagBrush = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FlagBorderColor")); }
        }



        public MineGame(int _gridSize, int _numOfMines, Grid grid)
        {
            GameDisplayGrid = grid;
            gridSize = _gridSize;
            numOfMines = _numOfMines;
            FlagsLeft = numOfMines;
            unclickedTiles = gridSize * gridSize;
            FlagButtonClicked = new RelayCommand(Button_Click);
            FlagSelected = false;

            MineGrid = new MineButton[gridSize, gridSize];

            MinePositions = GenerateMineCoordinates(_gridSize, numOfMines);

            for (int i = 0; i < gridSize; i++)
            {
                GameDisplayGrid.RowDefinitions.Add(new RowDefinition());
                GameDisplayGrid.ColumnDefinitions.Add(new ColumnDefinition());
                for (int j = 0; j < gridSize; j++)
                {
                    MineButton mineButton = new(MinePositions.Contains((i, j)) ? true : false, [i, j]);
                    mineButton.ButtonClick += TileClicked;
                    Grid.SetRow(mineButton, i);
                    Grid.SetColumn(mineButton, j);

                    GameDisplayGrid.Children.Add(mineButton);
                    MineGrid[i, j] = mineButton;
                }
            }

            flagImageBitmap.BeginInit();
            mineImageBitmap.BeginInit();
            flagImageBitmap.UriSource = new Uri(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Images\flag.png"));
            mineImageBitmap.UriSource = new Uri(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Images\Mine.png"));
            flagImageBitmap.EndInit();
            mineImageBitmap.EndInit();

            flagImage.ImageSource = flagImageBitmap;
            mineImage.ImageSource = mineImageBitmap;
            GameDisplayGrid.Loaded += Game_Loaded;
            GameDisplayGrid.PreviewKeyDown += PreviewKeyDown;
            blockerVisibility = Visibility.Hidden;
        }

        private (int, int)[] GenerateMineCoordinates(int size, int ammount)
        {
            List<(int, int)> allCoords = new();
            Random rng = new();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    allCoords.Add((i, j));
                }
            }

            for (int i = 0; i < ammount; i++)
            {
                int randomnum = rng.Next(size * size);
                (allCoords[i], allCoords[randomnum]) = (allCoords[randomnum], allCoords[i]);
            }
            (int, int)[] mineLocations = new (int, int)[ammount];
            Array.Copy(allCoords.ToArray(), mineLocations, ammount);
            return mineLocations;
        }

        private void TileClicked(object sender, EventArgs e)
        {
            if (sender is not MineButton button)
                return;

            HandleTile(button);
        }

        private void HandleTile(MineButton button)
        {
            if (FlagSelected)
            {
                if (button.Flagged)
                {
                    FlagsLeft++;
                    button.Flagged = false;
                    button.rect.Fill = Brushes.White;
                }
                else
                {
                    if (FlagsLeft <= 0)
                        return;
                    FlagsLeft--;
                    button.Flagged = true;
                    button.rect.Fill = flagImage;
                }
                return;
            }

            if (button.Flagged)
                return;

            if (button.IsMine)
            {
                GameOver();
                return;
            }

            unclickedTiles--;


            int[] position = button.Position;
            int a = position[0];
            int b = position[1];
            int numOfAdjacentMines = GetNumOfAdjacentMines(a, b);

            button.rect.Fill = Brushes.LightSlateGray;
            button.text.Foreground = numberColors[numOfAdjacentMines];
            button.text.Content = numOfAdjacentMines.ToString();
            button.IsClickable = false;

            if (numOfAdjacentMines == 0)
            {
                int[][] positions = new int[][]
                {
                    new int[] {a, b - 1 },
                    new int[] {a - 1, b - 1 },
                    new int[] {a - 1, b },
                    new int[] {a - 1, b + 1 },
                    new int[] {a, b + 1 },
                    new int[] {a + 1, b + 1 },
                    new int[] {a + 1, b },
                    new int[] {a + 1, b - 1 },
                };

                foreach (int[] newPosition in positions)
                {
                    if (newPosition[0] < 0 || newPosition[0] >= gridSize || newPosition[1] < 0 || newPosition[1] >= gridSize)
                        continue;

                    MineButton newButton = MineGrid[newPosition[0], newPosition[1]];
                    if (newButton.IsClickable)
                        HandleTile(newButton);
                }
            }
            if (unclickedTiles == numOfMines)
                GameWon();
        }

        private void GameOver()
        {
            foreach ((int, int) minePosition in MinePositions)
            {
                MineGrid[minePosition.Item1, minePosition.Item2].rect.Fill = Brushes.Red;
            }
            BlockerVisibility = Visibility.Visible;
            MessageBox.Show("You Lost");
        }

        private void GameWon()
        {
            MessageBox.Show("you won");
        }
        private int GetNumOfAdjacentMines(int a, int b)
        {
            int count = 0;
            int[][] positions = new int[][]
            {
                new int[] {a, b - 1 },
                new int[] {a - 1, b - 1 },
                new int[] {a - 1, b },
                new int[] {a - 1, b + 1 },
                new int[] {a, b + 1 },
                new int[] {a + 1, b + 1 },
                new int[] {a + 1, b },
                new int[] {a + 1, b - 1 },
            };

            foreach (int[] position in positions)
            {
                if (position[0] < 0 || position[0] >= gridSize || position[1] < 0 || position[1] >= gridSize)
                    continue;

                MineButton button = MineGrid[position[0], position[1]];
                if (button.IsMine)
                    count++;

            }
            return count;
        }

        private void Button_Click(object? param)
        {
            FlagSelected = !FlagSelected;
        }



        private void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key[] switchKeys = new Key[]
            {
                Key.Space, Key.Enter
            };
            if (switchKeys.Contains(e.Key))
            {
                FlagSelected = !FlagSelected;
            }
        }

        private void Game_Loaded(object sender, RoutedEventArgs e)
        {
            GameDisplayGrid.Focusable = true;
            GameDisplayGrid.Focus();
            Keyboard.Focus(GameDisplayGrid);
        }
    }
}
