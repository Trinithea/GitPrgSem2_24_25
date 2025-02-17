using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace pisisvorky
{
    internal class Game : INotifyPropertyChanged
    {
        private int size = 30;
        private bool circleTurn;
        private TileButton[,] tileButtons;
        private Grid PiskvorkyGrid;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool CircleTurn
        {
            get { return circleTurn; }
            private set
            {
                circleTurn = value;
                CurrentPlayer = value ? "O" : "X";
                PlayerColor = value ? Brushes.Red : Brushes.Green;
            }
        }
        private string currentPlayer;

        public string CurrentPlayer
        {
            get { return currentPlayer; }
            set { currentPlayer = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentPlayer")); }
        }

        private Brush playerColor;

        public Brush PlayerColor
        {
            get { return playerColor; }
            set { playerColor = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PlayerColor")); }
        }




        public Game(Grid grid)
        {
            PiskvorkyGrid = grid;
            generateGrid();
        }
        private void generateGrid()
        {
            PiskvorkyGrid.Children.Clear();
            PiskvorkyGrid.RowDefinitions.Clear();
            PiskvorkyGrid.ColumnDefinitions.Clear();
            tileButtons = new TileButton[size, size];
            for (int i = 0; i < size; i++)
            {
                PiskvorkyGrid.RowDefinitions.Add(new RowDefinition());
                PiskvorkyGrid.ColumnDefinitions.Add(new ColumnDefinition());
                for (int j = 0; j < size; j++)
                {
                    TileButton button = new TileButton([i, j]);
                    button.ButtonClick += butonClick;
                    tileButtons[i, j] = button;

                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    PiskvorkyGrid.Children.Add(button);

                }
            }
            CircleTurn = true;
        }
        private void butonClick(object sender, RoutedEventArgs e)
        {
            if (sender is not TileButton button)
                return;

            button.button.Content = CurrentPlayer;
            button.button.Foreground = circleTurn ? Brushes.Red : Brushes.Green;
            button.button.IsEnabled = false;

            if (isGonnaWin(button.Position))
            {
                MessageBox.Show($"{button.button.Content} Won");
                generateGrid();
            }

            CircleTurn = !CircleTurn;
        }
        private bool isGonnaWin(int[] position)
        {
            int x = position[0];
            int y = position[1];;
            int numOfConsecutives = 0;

            int i = x - 4;
            for (int j = y - 4; j < y + 5; j++)
            {

                if (j < 0 || j >= size || i < 0 || i >= size)
                {
                    i++;
                    continue;
                }


                TileButton button = tileButtons[i, j];

                if (button.button.Content == CurrentPlayer)
                    numOfConsecutives++;
                else
                    numOfConsecutives = 0;

                if (numOfConsecutives >= 5)
                    return true;
                i++;
            }

            i = x + 4;
            for (int j = y - 4; j < y + 5; j++)
            {

                if (j < 0 || j >= size || i < 0 || i >= size)
                {
                    i--;
                    continue;
                }


                TileButton button = tileButtons[i, j];

                if (button.button.Content == CurrentPlayer)
                    numOfConsecutives++;
                else
                    numOfConsecutives = 0;

                if (numOfConsecutives >= 5)
                    return true;
                i--;
            }

            numOfConsecutives = 0;
            for (int j = y - 4; j < y + 5; j++)
            {
                if (j < 0 || j >= size)
                    continue;

                TileButton button = tileButtons[x, j];

                if (button.button.Content == CurrentPlayer)
                    numOfConsecutives++;
                else
                    numOfConsecutives = 0;

                if (numOfConsecutives >= 5)
                    return true;
            }

            numOfConsecutives = 0;
            for (int j = x - 4; j < x + 5; j++)
            {
                if (j < 0 || j >= size)
                    continue;

                TileButton button = tileButtons[j, y];

                if (button.button.Content == CurrentPlayer)
                    numOfConsecutives++;
                else
                    numOfConsecutives = 0;

                if (numOfConsecutives >= 5)
                    return true;
            }


            return false;
        }
    }
}
