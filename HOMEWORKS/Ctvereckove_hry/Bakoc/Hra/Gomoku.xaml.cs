using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Hra
{
    /// <summary>
    /// Interakční logika pro Gomoku.xaml
    /// </summary>
    public partial class Gomoku : Window
    {
        public Gomoku()
        {
            InitializeComponent();

            GomokuViewModel vm = new GomokuViewModel(grid);
            DataContext = vm;

        }

        
    }



    public class GomokuViewModel : INotifyPropertyChanged
    {

        int onTurn = 1;
        int[,] GameBoard = new int[15, 15];
        Button[,] buttons = new Button[15, 15];
        BitmapImage X, O;


        Grid grid;

        private Brush background = Brushes.LightCoral;

        public Brush Background
        {
            get { return background; }
            set
            {
                if (background != value)
                {
                    background = value;
                    OnPropertyChanged();
                }
            }
        }


        public GomokuViewModel(Grid g)
        {

            grid = g;

            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "resources", "gomoku");


            X = new BitmapImage(new Uri(System.IO.Path.Combine(path, "X.png")));
            O = new BitmapImage(new Uri(System.IO.Path.Combine(path, "O.png")));

            Background = Brushes.LightCoral;


            generateUI();
        }


        private void generateUI()
        {
            for (int i = 0; i < 15; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int j = 0; j < 15; j++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    Button b = new Button { Tag = new Tuple<int, int>(i, j), Content = "", Padding = new Thickness(0) };

                    b.Width = 20;
                    b.Height = 20;
                    b.BorderThickness = new Thickness(1, 1, 1, 1);
                    b.Click += Button_Click;
                    b.BorderBrush = new SolidColorBrush(Color.FromRgb(48, 48, 48));

                    Grid.SetColumn(b, i);
                    Grid.SetRow(b, j);

                    grid.Children.Add(b);
                    buttons[i, j] = b;

                    GameBoard[i, j] = 0;
                }
            }
        }



        private void Reset()
        {
            onTurn = 1;
            this.Background = Brushes.LightCoral;
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    buttons[i, j].Content = "";
                    GameBoard[i, j] = 0;
                }
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Tuple<int, int> coordinates)
            {
                int x = coordinates.Item1;
                int y = coordinates.Item2;

                if (GameBoard[x, y] != 0)
                {
                    return;
                }

                GameBoard[x, y] = onTurn;


                if (onTurn == 1)
                {
                    buttons[x, y].Content = new Image { Source = X, Width = 18, Height = 18 };
                }
                else
                {
                    buttons[x, y].Content = new Image { Source = O, Width = 18, Height = 18 };
                }

                if (CheckWin(x, y))
                {
                    MessageBox.Show($"{(onTurn == 1 ? 'X' : '◯')} has won!");
                    Reset();
                    return;
                }

                onTurn = -onTurn;

                Background = (onTurn == 1 ? Brushes.LightCoral : Brushes.LightBlue);
            }
        }



        private bool CheckWin(int x, int y)
        {
            int value = GameBoard[x, y];
            if (value == 0)
            {
                return false;
            }

            (int, int)[] directions = new (int, int)[]
            {
                (1,0),(0,1),(1,1),(1,-1)
            };


            foreach ((int, int) d in directions)
            {
                int forward = 0, backward = 0;
                bool f = true, b = true;


                for (int i = 1; i < 6; i++)
                {
                    int x1 = x + i * d.Item1;
                    int y1 = y + i * d.Item2;
                    int x2 = x - i * d.Item1;
                    int y2 = y - i * d.Item2;

                    if (x1 >= 0 && x1 < 15 && y1 >= 0 && y1 < 15 && f)
                    {
                        if (GameBoard[x1, y1] == value)
                        {
                            forward++;

                        }
                        else
                        {
                            f = false;
                        }
                    }
                    if (x2 >= 0 && x2 < 15 && y2 >= 0 && y2 < 15 && b)
                    {
                        if (GameBoard[x2, y2] == value)
                        {
                            backward++;
                        }
                        else
                        {
                            b = false;
                        }
                    }
                }

                if (forward + backward == 4)
                {
                    return true;
                }
            }
            return false;
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
