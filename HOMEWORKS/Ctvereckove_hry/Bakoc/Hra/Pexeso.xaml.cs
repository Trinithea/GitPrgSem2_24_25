using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
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

namespace Hra
{


    class Player
    {
        public int points = 0;
        public StackPanel takenDisplay;
        public Label label;
        public string name;
        public Player(string n)
        {
            name = n;
            takenDisplay = new StackPanel { Orientation = Orientation.Vertical };
            label = new Label
            {
                FontSize = 30,
                FontWeight = FontWeights.Regular
            };
            updateLabel();
        }
        public void updateLabel()
        {
            label.Content = $"{name}: {points}";
        }

        

        public void collectPair(BitmapImage source)
        {
            if (points % 3 == 0)
            {
                takenDisplay.Children.Add(new StackPanel { Orientation = Orientation.Horizontal });
            }
            StackPanel last = (StackPanel)takenDisplay.Children[takenDisplay.Children.Count - 1];

            last.Children.Add(new Image { Source = source, Width = 50, Height = 50 });

            points++;
            updateLabel();
        }

        public void Reset()
        {
            points = 0;
            takenDisplay.Children.Clear();
            updateLabel();
            label.FontWeight = FontWeights.Regular;
        }
    }




    public partial class Pexeso : Window
    {


        public Pexeso(int p)
        {
            
            InitializeComponent();
            PexesoViewModel vm = new PexesoViewModel(p, grid, playersDisplay);
            
        }

    }

    public class PexesoViewModel : INotifyPropertyChanged
    {

        Button[,] buttons;
        int[,] pictureID;
        bool[,] taken;
        Player[] players;

        int clicks = 0;
        (int, int) second_picked;
        (int, int) first_picked;

        int onTurn = 0;

        BitmapImage backside;
        BitmapImage[] pictures;

        Grid grid;
        StackPanel playersDisplay;

        int left_pairs;


        int player_count;

        public PexesoViewModel(int p, Grid g, StackPanel pd)
        {
            if (p <= 0 || p > 6)
            {
                throw new ArgumentException();
            }
            player_count = p;
            grid = g;
            playersDisplay = pd;


            pictures = new BitmapImage[24];
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "resources", "pexeso");
            for (int i = 0; i < 24; i++)
            {
                pictures[i] = new BitmapImage(new Uri(System.IO.Path.Combine(path, $"{i}.png")));
            }

            backside = new BitmapImage(new Uri(System.IO.Path.Combine(path, "backside.jpg")));

            buttons = new Button[8, 6];


            players = new Player[player_count];

            for (int i = 0; i < player_count; i++)
            {
                players[i] = new Player($"Player {i + 1}");
                playersDisplay.Children.Add(players[i].label);
                playersDisplay.Children.Add(players[i].takenDisplay);
            }

            players[onTurn].label.FontWeight = FontWeights.Bold;

            createButtons();

            generateGame();


        }

        private void createButtons()
        {
            for (int i = 0; i < 8; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int j = 0; j < 6; j++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    Button b = new Button { Tag = new Tuple<int, int>(i, j), Content = "", FontSize = 3 };

                    b.Width = 12;
                    b.Height = 12;
                    b.BorderThickness = new Thickness(1);
                    b.BorderBrush = Brushes.DarkGray;
                    b.Padding = new Thickness(0);
                    b.Content = new Image { Source = backside, Width = 10, Height = 10 };
                    b.Click += Button_Click;

                    Grid.SetColumn(b, i);
                    Grid.SetRow(b, j);

                    grid.Children.Add(b);
                    buttons[i, j] = b;

                }
            }
        }

        private void generateGame()
        {
            pictureID = new int[8, 6];
            taken = new bool[8, 6];
            left_pairs = 24;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    pictureID[i, j] = (int)((i * 6 + j) / 2);
                    taken[i, j] = false;
                }
            }
            Random random = new Random();
            for (int i = 0; i < 400; i++)
            {
                int x1 = random.Next(8);
                int y1 = random.Next(6);
                int x2 = random.Next(8);
                int y2 = random.Next(6);

                (pictureID[x1, y1], pictureID[x2, y2]) = (pictureID[x2, y2], pictureID[x1, y1]);
                (pictureID[x1, y2], pictureID[x2, y1]) = (pictureID[x2, y1], pictureID[x1, y2]);
            }
        }


        private void ResetGame()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    buttons[i, j].Content = new Image { Source = backside, Width = 10, Height = 10 };
                    buttons[i, j].BorderBrush = Brushes.DarkGray;

                }
            }
            generateGame();
            for (int i = 0; i < player_count; i++)
            {
                players[i].Reset();
            }
            onTurn = 0;
            players[onTurn].label.FontWeight = FontWeights.Bold;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Tuple<int, int> coordinates)
            {
                if (clicks == 2)
                {
                    buttons[first_picked.Item1, first_picked.Item2].Content = new Image { Source = backside, Width = 10, Height = 10 };
                    buttons[second_picked.Item1, second_picked.Item2].Content = new Image { Source = backside, Width = 10, Height = 10 };
                    players[onTurn].label.FontWeight = FontWeights.Regular;
                    onTurn = (onTurn + 1) % player_count;
                    players[onTurn].label.FontWeight = FontWeights.Bold;
                    clicks = 0;

                    return;
                }
                int x = coordinates.Item1;
                int y = coordinates.Item2;

                if (taken[x, y])
                {
                    return;
                }

                if (clicks == 0)
                {
                    btn.Content = new Image { Source = pictures[pictureID[x, y]], Width = 10, Height = 10 };
                    first_picked = (x, y);
                    clicks++;
                    return;
                }
                else
                {
                    btn.Content = new Image { Source = pictures[pictureID[x, y]], Width = 10, Height = 10 };
                    second_picked = (x, y);
                    if (first_picked == second_picked)
                    {
                        return;
                    }
                    if (pictureID[first_picked.Item1, first_picked.Item2] == pictureID[second_picked.Item1, second_picked.Item2])
                    {
                        players[onTurn].collectPair(pictures[pictureID[first_picked.Item1, first_picked.Item2]]);
                        taken[first_picked.Item1, first_picked.Item2] = true;
                        taken[second_picked.Item1, second_picked.Item2] = true;
                        buttons[first_picked.Item1, first_picked.Item2].BorderBrush = Brushes.DarkCyan;
                        buttons[second_picked.Item1, second_picked.Item2].BorderBrush = Brushes.DarkCyan;
                        clicks = 0;
                        left_pairs -= 1;

                        if (left_pairs == 0)
                        {
                            List<int> winners = new List<int> { };
                            int winner_points = 0;
                            for (int i = 0; i < player_count; i++)
                            {
                                if (players[i].points > winner_points)
                                {
                                    winners = new List<int> { i };
                                    winner_points = players[i].points;
                                }
                                else if (players[i].points == winner_points)
                                {
                                    winners.Add(i);
                                }
                            }
                            if (winners.Count == 1)
                            {
                                MessageBox.Show($"{players[winners[0]].name} won with {winner_points} points!");
                            }
                            else
                            {
                                string m = "";
                                for (int i = 0; i < winners.Count; i++)
                                {
                                    if (i > 0)
                                    {
                                        m += ", ";
                                    }
                                    m += $"{players[winners[i]].name}";
                                }
                                MessageBox.Show($"{m} won with {winner_points} points!");
                            }
                            ResetGame();

                        }
                    }
                    else
                    {
                        clicks++;
                    }
                }

            }
        }





        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName=null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
