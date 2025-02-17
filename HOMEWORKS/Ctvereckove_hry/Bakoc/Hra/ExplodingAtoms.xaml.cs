using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Xaml;

namespace Hra
{

    public partial class ExplodingAtoms : Window
    {
        public ExplodingAtoms()
        {
            InitializeComponent();


            ExplodingAtomsViewModel vm = new ExplodingAtomsViewModel(grid, (Style)FindResource("ButtonStyle"));
            DataContext = vm;
            vm.Start();
        }
    }



    public class ExplodingAtomsViewModel : INotifyPropertyChanged
    {
        bool onTurn = true;
        int[,] Atoms = new int[8, 6];
        char Atom = '◯';
        Button[,] buttons = new Button[8, 6];
        bool roundOne = true;
        Grid grid;
        Style style;

        private Brush background;
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

        public ExplodingAtomsViewModel(Grid g, Style s)
        {
            grid = g;
            style = s;
            

        }

        public void Start()
        {
            generateUI();
            Background = Brushes.LightBlue;
        }

        private void generateUI()
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
                    Button b = new Button { Tag = new Tuple<int, int>(i, j), Content = "", FontSize = 3, FontWeight = FontWeights.UltraBold };

                    b.Width = 10;
                    b.Height = 10;
                    b.BorderThickness = new Thickness(1, 1, 1, 1);
                    b.Style = style;
                    b.Click += Button_Click;

                    Grid.SetColumn(b, i);
                    Grid.SetRow(b, j);

                    grid.Children.Add(b);
                    buttons[i, j] = b;

                    Atoms[i, j] = 0;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Tuple<int, int> coordinates)
            {
                int x = coordinates.Item1;
                int y = coordinates.Item2;

                Play(x, y);
            }
        }

        private void Play(int x, int y)
        {
            int s = Math.Sign(Atoms[x, y]);

            if (((s == -1) && onTurn) || ((s == 1) && !onTurn))
            {
                return;
            }

            AddAtom(x, y);

            bool win = CheckWin();
            if (win && !roundOne)
            {
                UpdateUI();
                if (onTurn)
                {
                    MessageBox.Show("Blue has won!");
                }
                else
                {
                    MessageBox.Show("Red has won!");
                }
                onTurn = true;
                roundOne = true;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        Atoms[i, j] = 0;
                    }
                }

            }
            else
            {
                onTurn = !onTurn;
                if (onTurn)
                {
                    roundOne = false;
                }
            }


            UpdateUI();
        }

        private void AddAtom(int x, int y)
        {
            int add = (onTurn ? 1 : -1);

            Atoms[x, y] += add;
            if (Math.Abs(Atoms[x, y]) < 4)
            {
                return;
            }

            Atoms[x, y] = 0;


            (int, int)[] smery = { (1, 0), (0, 1), (-1, 0), (0, -1) };
            foreach ((int, int) t in smery)
            {
                int xx = x + t.Item1;
                int yy = y + t.Item2;

                if (xx >= 0 && yy >= 0 && xx < 8 && yy < 6)
                {
                    Atoms[xx, yy] = Math.Abs(Atoms[xx, yy]) * add;
                    AddAtom(xx, yy);
                }
            }
        }

        private void UpdateUI()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    int electrons = Atoms[i, j];
                    string s = "";
                    if (Math.Abs(electrons) == 1)
                    {
                        s = $"{Atom}";
                    }
                    if (Math.Abs(electrons) == 2)
                    {
                        s = $"{Atom} {Atom}";
                    }
                    if (Math.Abs(electrons) == 3)
                    {
                        s = $"{Atom} {Atom}\n{Atom}";
                    }

                    buttons[i, j].Content = s;
                    buttons[i, j].Foreground = (Math.Sign(Atoms[i, j]) == 1 ? Brushes.Blue : Brushes.Red);
                }
            }
            Background = (onTurn ? Brushes.LightBlue : Brushes.LightCoral);
        }

        private bool CheckWin()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (onTurn && Math.Sign(Atoms[i, j]) == -1 || !onTurn && Math.Sign(Atoms[i, j]) == 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
