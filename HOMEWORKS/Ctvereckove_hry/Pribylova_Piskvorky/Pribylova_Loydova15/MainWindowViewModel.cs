using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace KackaPiskvorky
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        int numberOfSquares = 225;
        private Grid PlayGrid;
        private char tah;
        private Button[,] squares = new Button[15, 15];
        Dictionary<Button, (int, int)> squarePositions = new Dictionary<Button, (int, int)>();

        public char Tah
        {
            get { return tah; }
            set { tah = value; OnPropertyChanged(); }
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private Brush stetec;

        public Brush ColorOfTah
        {
            get { return stetec; }
            set { stetec = value; OnPropertyChanged(); }
        }


        public void StartGame(Grid grid)
        {
            PlayGrid = grid;
            CreateGameGrid();

            Tah = 'x';
            ColorOfTah = Brushes.Pink;

        }
        private void CreateGameGrid()
        {
            int sqrt = Convert.ToInt32(Math.Sqrt(numberOfSquares));
            PlayGrid.RowDefinitions.Clear();
            PlayGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < sqrt; i++)
                PlayGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            for (int i = 0; i < sqrt; i++)
                PlayGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            for (int i = 0; i < sqrt; i++)
            {
                for (int j = 0; j < sqrt; j++)
                {
                    Button button = new Button();
                    //button.Content = Items[sqrt*i + j].Id;
                    button.Tag = sqrt * i + j;
                    button.Click += SquareClicked;
                    button.Background = new SolidColorBrush(Colors.AliceBlue);

                    // Nastavení pozice v Gridu
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);

                    // Přidání tlačítka do Gridu
                    PlayGrid.Children.Add(button);
                    squares[i, j] = button;
                    squarePositions[button] = (i, j);
                }
            }
        }
        private void SquareClicked(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
            {
                return;
            }
            button.Content = Tah;
            if (CheckIfWinning(button))
            {
                MessageBox.Show($"Vyhral {Tah}!");
            }
            ChangeTah();
        }
        private void ChangeTah()
        {
            if (Tah == 'x')
            {
                Tah = 'o';
                ColorOfTah = Brushes.Blue;
            }
            else if (Tah == 'o')
            {
                Tah = 'x';
                ColorOfTah = Brushes.Pink;
            }

        }
        private bool CheckIfWinning(Button button)
        {
            int x = squarePositions[button].Item1;
            int y = squarePositions[button].Item2;
            int pocetZnaku = 0;
            for (int i = x - 4; i < x + 5; i++)
            {
                if (i < 0 || i >= 15)
                {
                    continue;
                }
                if (pocetZnaku == 5)
                {
                    return true;
                }
                if ((char?)squares[i, y].Content == Tah)
                {
                    pocetZnaku++;
                }
                else
                {
                    pocetZnaku = 0;
                }
            }
            pocetZnaku = 0;
            for (int i = y - 4; i < y + 5; i++)
            {
                if (i < 0 || i >= 15)
                {
                    continue;
                }
                if (pocetZnaku == 5)
                {
                    return true;
                }
                if ((char?)squares[x, i].Content == Tah)
                {
                    pocetZnaku++;
                }
                else
                {
                    pocetZnaku = 0;
                }
            }
            pocetZnaku = 0;
            int j = y - 4;
            for (int i = x - 4; i < x + 5; i++)
            {
                if (i < 0 || i >= 15 || j < 0 || j >= 15)
                {
                    j++;
                    continue;
                }
                if (pocetZnaku == 5)
                {
                    return true;
                }
                if ((char?)squares[i, j].Content == Tah)
                {
                    pocetZnaku++;
                }
                else
                {
                    pocetZnaku = 0;
                }
                j++;
            }
            pocetZnaku = 0;
            j = y + 4;
            for (int i = x - 4; i < x + 5; i++)
            {
                if (i < 0 || i >= 15 || j < 0 || j >= 15)
                {
                    j--;
                    continue;
                }
                if (pocetZnaku == 5)
                {
                    return true;
                }
                if ((char?)squares[i, j].Content == Tah)
                {
                    pocetZnaku++;
                }
                else
                {
                    pocetZnaku = 0;
                }
                j--;
            }

            return false;
        }

    }
}
