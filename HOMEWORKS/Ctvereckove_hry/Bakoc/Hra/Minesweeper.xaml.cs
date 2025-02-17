using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;
using NAudio;
using NAudio.Wave;

namespace Hra
{
     
    public partial class Minesweeper : Window
    {


        BitmapImage[] display = new BitmapImage[10];

        BitmapImage border_v;
        BitmapImage border_h;
        BitmapImage border_t_l;
        BitmapImage border_t_r;
        BitmapImage border_b_l;
        BitmapImage border_b_r;
        BitmapImage border_i_l;
        BitmapImage border_i_r;

        BitmapImage reset_button;

        Image[] mineDisplay = new Image[3];

        Image[] timeDisplay = new Image[3];

        

        public Minesweeper(int w, int h, int m)
        {

            loadImages();

            InitializeComponent();

            for (int i = 0; i < w+2; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int j = 0; j < h+5; j++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }

            Grid g = new Grid();
            Grid.SetColumn(g, 1);
            Grid.SetRow(g, 4);
            Grid.SetColumnSpan(g, w);
            Grid.SetRowSpan(g, h);

            grid.Children.Add(g);


            MinesweeperViewModel vm = new MinesweeperViewModel(w, h, m, g);

            vm.timeCounterChanged += timeCounterUpdate;
            vm.mineCounterChanged += mineCounterUpdate;

            

            generateUI(w,h);

            

            Button resetButton = new Button
            {
                Content = new Image
                {
                    Source = reset_button,
                    Width = 26,
                    Height = 26
                },
                Width = 26,
                Height = 26,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0),
                Padding = new Thickness(0),
                BorderThickness = new Thickness(0)
            };
            resetButton.Click += vm.ResetButton_Click;
            Grid.SetRow(resetButton, 1);
            Grid.SetRowSpan(resetButton, 2);
            Grid.SetColumn(resetButton, 1);
            Grid.SetColumnSpan(resetButton, w);
            grid.Children.Add(resetButton);

            vm.Start();

        }

        private void generateUI(int w, int h)
        {
            for (int i = 1; i < w + 1; i++)
            {
                Image i1 = new Image
                {
                    Source = border_h,
                    Width = 16,
                    Height = 10
                };
                Grid.SetColumn(i1, i);
                Grid.SetRow(i1, 0);
                grid.Children.Add(i1);
                Image i2 = new Image
                {
                    Source = border_h,
                    Width = 16,
                    Height = 10
                };
                Grid.SetColumn(i2, i);
                Grid.SetRow(i2, 3);
                grid.Children.Add(i2);
                Image i3 = new Image
                {
                    Source = border_h,
                    Width = 16,
                    Height = 10
                };
                Grid.SetColumn(i3, i);
                Grid.SetRow(i3, h + 4);
                grid.Children.Add(i3);
            }
            for (int i = 1; i < h + 4; i++)
            {
                if (i == 3)
                {
                    continue;
                }
                Image i1 = new Image
                {
                    Source = border_v,
                    Width = 10,
                    Height = 16
                };
                Grid.SetColumn(i1, 0);
                Grid.SetRow(i1, i);
                grid.Children.Add(i1);
                Image i2 = new Image
                {
                    Source = border_v,
                    Width = 10,
                    Height = 16
                };
                Grid.SetColumn(i2, w + 1);
                Grid.SetRow(i2, i);
                grid.Children.Add(i2);
            }

            Image tlcorner = new Image
            {
                Source = border_t_l,
                Width = 10,
                Height = 10
            };
            Image trcorner = new Image
            {
                Source = border_t_r,
                Width = 10,
                Height = 10
            };
            Image blcorner = new Image
            {
                Source = border_b_l,
                Width = 10,
                Height = 10
            };
            Image brcorner = new Image
            {
                Source = border_b_r,
                Width = 10,
                Height = 10
            };
            Image lintersection = new Image
            {
                Source = border_i_l,
                Width = 10,
                Height = 10
            };
            Image rintersection = new Image
            {
                Source = border_i_r,
                Width = 10,
                Height = 10
            };

            Grid.SetColumn(tlcorner, 0);
            Grid.SetColumn(trcorner, w + 1);
            Grid.SetColumn(blcorner, 0);
            Grid.SetColumn(brcorner, w + 1);
            Grid.SetColumn(lintersection, 0);
            Grid.SetColumn(rintersection, w + 1);

            Grid.SetRow(tlcorner, 0);
            Grid.SetRow(trcorner, 0);
            Grid.SetRow(blcorner, h + 4);
            Grid.SetRow(brcorner, h + 4);
            Grid.SetRow(lintersection, 3);
            Grid.SetRow(rintersection, 3);

            grid.Children.Add(tlcorner);
            grid.Children.Add(trcorner);
            grid.Children.Add(blcorner);
            grid.Children.Add(brcorner);
            grid.Children.Add(lintersection);
            grid.Children.Add(rintersection);


            StackPanel mineCounters = new StackPanel
            {
                Margin = new Thickness(6, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Orientation = Orientation.Horizontal
            };
            mineDisplay[0] = new Image
            {
                Source = display[9],
                Width = 13,
                Height = 23
            };
            mineDisplay[1] = new Image
            {
                Source = display[9],
                Width = 13,
                Height = 23
            };
            mineDisplay[2] = new Image
            {
                Source = display[9],
                Width = 13,
                Height = 23
            };
            mineCounters.Children.Add(mineDisplay[0]);
            mineCounters.Children.Add(mineDisplay[1]);
            mineCounters.Children.Add(mineDisplay[2]);
            Grid.SetRow(mineCounters, 1);
            Grid.SetRowSpan(mineCounters, 2);
            Grid.SetColumn(mineCounters, 1);
            Grid.SetColumnSpan(mineCounters, w);
            grid.Children.Add(mineCounters);



            StackPanel timeCounters = new StackPanel
            {
                Margin = new Thickness(0, 0, 6, 0),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Orientation = Orientation.Horizontal
            };
            timeDisplay[0] = new Image
            {
                Source = display[0],
                Width = 13,
                Height = 23
            };
            timeDisplay[1] = new Image
            {
                Source = display[0],
                Width = 13,
                Height = 23
            };
            timeDisplay[2] = new Image
            {
                Source = display[0],
                Width = 13,
                Height = 23
            };
            timeCounters.Children.Add(timeDisplay[0]);
            timeCounters.Children.Add(timeDisplay[1]);
            timeCounters.Children.Add(timeDisplay[2]);
            Grid.SetRow(timeCounters, 1);
            Grid.SetRowSpan(timeCounters, 2);
            Grid.SetColumn(timeCounters, 1);
            Grid.SetColumnSpan(timeCounters, w);
            grid.Children.Add(timeCounters);

            
        }

        private void loadImages()
        {
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "resources", "minesweeper");


            for (int i = 0; i < 10; i++)
            {
                display[i] = new BitmapImage(new Uri(System.IO.Path.Combine(path, $"{i}_display.png")));
            }

            border_v = new BitmapImage(new Uri(System.IO.Path.Combine(path, "border_vertical.png")));
            border_h = new BitmapImage(new Uri(System.IO.Path.Combine(path, "border_horizontal.png")));
            border_t_l = new BitmapImage(new Uri(System.IO.Path.Combine(path, "border_top_left.png")));
            border_t_r = new BitmapImage(new Uri(System.IO.Path.Combine(path, "border_top_right.png")));
            border_b_l = new BitmapImage(new Uri(System.IO.Path.Combine(path, "border_bottom_left.png")));
            border_b_r = new BitmapImage(new Uri(System.IO.Path.Combine(path, "border_bottom_right.png")));
            border_i_l = new BitmapImage(new Uri(System.IO.Path.Combine(path, "border_intersection_left.png")));
            border_i_r = new BitmapImage(new Uri(System.IO.Path.Combine(path, "border_intersection_right.png")));
            reset_button = new BitmapImage(new Uri(System.IO.Path.Combine(path, "reset_button.png")));
        }


        private void mineCounterUpdate(int newValue)
        {
            int v = Math.Min(Math.Max(0, newValue), 999);
            string n = v.ToString();

            while (n.Length < 3)
            {
                n = "0" + n;
            }
            for (int i = 0; i < 3; i++)
            {
                int digit = n[i] - '0';
                mineDisplay[i].Source = display[digit];
            }
        }

        public void timeCounterUpdate(int newValue)
        {
            int v = Math.Max(Math.Min(999, newValue),0);

            string n = v.ToString();
            while (n.Length < 3)
            {
                n = "0" + n;
            }
            for (int i = 0; i < 3; i++)
            {
                int digit = n[i] - '0';
                timeDisplay[i].Source = display[digit];
            }
        }

    }




    public  class MinesweeperViewModel : INotifyPropertyChanged
    {

        int width, height, mines;

        Button[,] buttons;

        bool[,] mine;
        bool[,] revealed;
        bool[,] flagged;
        int[,] neighbourMines;

        int left_cells;

        BitmapImage cell;
        BitmapImage mine_cell;
        BitmapImage not_mine_cell;
        BitmapImage exploded_cell;
        BitmapImage flag_cell;

        BitmapImage[] number_cells = new BitmapImage[9];

        Grid grid;


        private int mineCounter=-1;
        private int timeCounter=-1;

        public event Action<int> mineCounterChanged;
        public event Action<int> timeCounterChanged;



        SoundPlayer ClickSound;
        SoundPlayer WinSound;
        SoundPlayer ExplosionSound;
        SoundPlayer StartSound;
        SoundPlayer FlagSound;

        public int MineCounter
        {
            get { return mineCounter; }
            set
            {
                if (mineCounter != value)
                {
                    mineCounter = value;
                    mineCounterChanged?.Invoke(mineCounter);
                }
            }
        }

        public int TimeCounter
        {
            get { return timeCounter; }
            set
            {
                if (timeCounter != value)
                {
                    timeCounter = value;
                    timeCounterChanged?.Invoke(timeCounter);
                }
            }
        }


        DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1),
        };
        bool gameStarted = false;

        bool gameEnded = false;


        private CancellationTokenSource stopAnimation = new CancellationTokenSource();


        public MinesweeperViewModel(int w, int h, int m, Grid g)
        {
            width = w;
            height = h;
            mines = m;
            grid = g;
            if (mines > width * height)
            {
                throw new ArgumentOutOfRangeException();
            }

            timer.Tick += ClockTick;
        }

        public void Start()
        {
            loadResources();

            generateUI();

            resetGame();

        }

        private void loadResources()
        {
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "resources", "minesweeper");

            cell = new BitmapImage(new Uri(System.IO.Path.Combine(path, "cell.png")));
            number_cells[0] = new BitmapImage(new Uri(System.IO.Path.Combine(path, "empty_cell.png")));
            mine_cell = new BitmapImage(new Uri(System.IO.Path.Combine(path, "mine_cell.png")));
            not_mine_cell = new BitmapImage(new Uri(System.IO.Path.Combine(path, "not_mine_cell.png")));
            exploded_cell = new BitmapImage(new Uri(System.IO.Path.Combine(path, "exploded_cell.png")));
            flag_cell = new BitmapImage(new Uri(System.IO.Path.Combine(path, "flag_cell.png")));

            for (int i = 1; i < 9; i++)
            {
                number_cells[i] = new BitmapImage(new Uri(System.IO.Path.Combine(path, $"{i}_cell.png")));
            }

            WinSound = new SoundPlayer(System.IO.Path.Combine(path, "win.wav"));
            ExplosionSound = new SoundPlayer(System.IO.Path.Combine(path, "explosion.wav"));
            StartSound = new SoundPlayer(System.IO.Path.Combine(path, "start.wav"));
            ClickSound = new SoundPlayer(System.IO.Path.Combine(path, "click.wav"));
            FlagSound = new SoundPlayer(System.IO.Path.Combine(path, "flag.wav"));
           

        }


        private void generateUI()
        {
            buttons = new Button[width, height];

            for (int i = 0; i < width; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int j = 0; j < height; j++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Button b = new Button { Tag = new Tuple<int, int>(i, j) };


                    //b.OverridesDefaultStyle = true;
                    b.Width = 16;
                    b.Height = 16;
                    b.BorderThickness = new Thickness(0, 0, 0, 0);

                    Image im = new Image();
                    im.Source = cell;
                    im.Stretch = Stretch.Fill;

                    b.Content = im;
                    b.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                    b.VerticalContentAlignment = VerticalAlignment.Stretch;
                    b.Margin = new Thickness(0);
                    b.Padding = new Thickness(0);

                    b.PreviewMouseDown += Button_PreviewMouseDown;


                    Grid.SetColumn(b, i);
                    Grid.SetRow(b, j);

                    grid.Children.Add(b);
                    buttons[i, j] = b;

                }
            }
        }

        public void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            resetGame();
        }
        public void resetGame()
        {
            stopAnimation?.Cancel();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    buttons[i, j].Content = new Image
                    {
                        Source = cell,
                        Width = 16,
                        Height = 16
                    };
                }
            }
            gameStarted = false;
            gameEnded = false;
            MineCounter = mines;
            TimeCounter = 0;
            stopAnimation = new CancellationTokenSource();
        }

        private void generateGame(int xx, int yy)
        {
            left_cells = width * height;
            

            


            mine = new bool[width, height];
            revealed = new bool[width, height];
            flagged = new bool[width, height];
            neighbourMines = new int[width, height];
            List<(int, int)> allCells = new List<(int, int)> { };
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    mine[i, j] = false;
                    revealed[i, j] = false;
                    flagged[i, j] = false;
                    if (Math.Abs(xx - i) > 1 || Math.Abs(yy - j) > 1)
                    {
                        allCells.Add((i, j));
                    }
                }
            }
            Random random = new Random();
            for (int i = 0; i < allCells.Count; i++)
            {
                int j = random.Next(allCells.Count);
                (allCells[i], allCells[j]) = (allCells[j], allCells[i]);
            }
            for (int i = 0; i < mines; i++)
            {
                (int, int) c = allCells[i];
                mine[c.Item1, c.Item2] = true;
            }


            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    neighbourMines[i, j] = 0;
                    (int, int)[] neighbours = new (int, int)[]
                    {
                        (1,0),(0,1),(-1,0),(0,-1),(1,1),(1,-1),(-1,1),(-1,-1)
                    };
                    foreach ((int, int) n in neighbours)
                    {
                        int x = i + n.Item1;
                        int y = j + n.Item2;
                        if (x >= 0 && y >= 0 && x < width && y < height)
                        {
                            neighbourMines[i, j] += mine[x, y] ? 1 : 0;
                        }
                    }
                }
            }
        }


        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (gameEnded)
            {
                return;
            }
            if (sender is Button btn && btn.Tag is Tuple<int, int> coordinates)
            {
                int x = coordinates.Item1;
                int y = coordinates.Item2;

                if (!gameStarted)
                {
                    generateGame(x, y);
                    gameStarted = true;
                    timer.Start();
                    StartSound.PlaySound();
                }
            

            
                
                Button b = buttons[x, y];

                if (revealed[x, y])
                {
                    int flag_neighbours = 0;
                    int flag_neighbours2 = 0;

                    (int, int)[] neighbours = new (int, int)[]
                    {
                        (1,0),(0,1),(-1,0),(0,-1),(1,1),(1,-1),(-1,1),(-1,-1)
                    };

                    foreach ((int, int) n in neighbours)
                    {
                        int xx = x + n.Item1;
                        int yy = y + n.Item2;
                        if (xx >= 0 && yy >= 0 && xx < width && yy < height)
                        {
                            if (flagged[xx,yy] && !revealed[xx, yy]) {
                                flag_neighbours++;
                            }
                            if (!flagged[xx,yy] && !revealed[xx, yy])
                            {
                                flag_neighbours2++;
                            }
                        }
                    }

                    if (flag_neighbours == neighbourMines[x, y])
                    {
                        foreach ((int, int) n in neighbours)
                        {
                            int xx = x + n.Item1;
                            int yy = y + n.Item2;
                            if (xx >= 0 && yy >= 0 && xx < width && yy < height)
                            {
                                if (!revealed[xx,yy] && !flagged[xx, yy])
                                {
                                    if (mine[xx, yy])
                                    {
                                        Exploded(xx, yy);
                                    }
                                    else
                                    {
                                        Reveal(xx, yy);
                                    }
                                }
                            }
                        }
                        if (left_cells == mines)
                        {
                            gameEnded = true;
                            timer.Stop();
                            WinSound.PlaySound();
                            MessageBox.Show("You win!");
                        }
                    }
                    else if (flag_neighbours + flag_neighbours2 == neighbourMines[x, y])
                    {
                        foreach ((int, int) n in neighbours)
                        {
                            int xx = x + n.Item1;
                            int yy = y + n.Item2;
                            if (xx >= 0 && yy >= 0 && xx < width && yy < height)
                            {
                                if (!revealed[xx, yy] && !flagged[xx, yy])
                                {
                                    flagged[xx, yy] = true;
                                    updateImage(xx, yy);
                                    MineCounter--;
                                }
                            }
                        }
                    }


                    return;
                }

                if (e.ChangedButton == MouseButton.Left)
                {
                    if (flagged[x, y])
                    {
                        return;
                    }

                    if (mine[x, y])
                    {
                        Exploded(x, y);
                    }
                    else
                    {

                        Reveal(x, y);
                        ClickSound.PlaySound();
                        if (left_cells == mines)
                        {
                            gameEnded = true;
                            timer.Stop();
                            WinSound.PlaySound();
                            MessageBox.Show("You win!");
                        }
                    }

                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    flagged[x, y] = !flagged[x, y];
                    int change = flagged[x, y] ? -1 : 1;
                    MineCounter += change;
                    updateImage(x, y);
                    FlagSound.PlaySound();
                }


            }
        }

        private void Exploded(int x, int y)
        {
            List<(int, int)> Mines = new List<(int, int)> { };
            List<(int, int)> falseFlags = new List<(int, int)> { };

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (mine[i, j] && !flagged[i, j] && !(x==i && y==j))
                    {
                        Mines.Add((i, j));
                        //revealed[i, j] = true;
                        //updateImage(i, j);

                    }
                    else if (flagged[i, j] && !mine[i, j])
                    {
                        //revealed[i, j] = true;
                        //buttons[i, j].Content = new Image
                        //{
                        //    Source = not_mine_cell,
                        //    Width = 16,
                        //    Height = 16
                        //};
                        falseFlags.Add((i, j));
                    }
                }
            }
            Mines.Sort((m1, m2) => (Math.Abs(m1.Item1 - x) + Math.Abs(m1.Item2 - y)).CompareTo((Math.Abs(m2.Item1 - x) + Math.Abs(m2.Item2 - y))));

            ExplosionSound.PlaySound();

            RevealMines(Mines, stopAnimation.Token);

            for (int i = 0; i < falseFlags.Count; i++)
            {
                revealed[falseFlags[i].Item1, falseFlags[i].Item2] = true;
                buttons[falseFlags[i].Item1, falseFlags[i].Item2].Content = new Image
                {
                    Source = not_mine_cell,
                    Width = 16,
                    Height = 16
                };
            }

            buttons[x, y].Content = new Image
            {
                Source = exploded_cell,
                Width = 16,
                Height = 16
            };
            gameEnded = true;
            timer.Stop();
        }

        private async void RevealMines(List<(int,int)> Mines, CancellationToken c)
        {

            await Task.Delay(400, c);
            Random random = new Random();
            for (int i = 0; i < Mines.Count; i++)
            {
                int speed = (int) Math.Round(70.0 * (double)i / (double)Mines.Count);
                if (c.IsCancellationRequested)
                {
                    return;
                }
                
                revealed[Mines[i].Item1, Mines[i].Item2] = true;
                updateImage(Mines[i].Item1, Mines[i].Item2);
                ExplosionSound.PlaySound();
                try 
                { 
                    await Task.Delay(random.Next(200-speed,400-2*speed),c);
                }
                catch
                {
                    return;
                }

            }
        }

        private void updateImage(int x, int y)
        {
            if (revealed[x, y])
            {
                if (mine[x, y])
                {
                    buttons[x, y].Content = new Image
                    {
                        Source = mine_cell,
                        Width = 16,
                        Height = 16
                    };
                    return;
                }



                buttons[x, y].Content = new Image
                {
                    Source = number_cells[neighbourMines[x, y]],
                    Width = 16,
                    Height = 16
                };

            }
            else
            {
                if (flagged[x, y])
                {
                    buttons[x, y].Content = new Image
                    {
                        Source = flag_cell,
                        Width = 16,
                        Height = 16
                    };
                    return;
                }

                buttons[x, y].Content = new Image
                {
                    Source = cell,
                    Width = 16,
                    Height = 16
                };

            }

        }

        private void ClockTick(object sender, EventArgs e)
        {
            TimeCounter++;
        }





        private void Reveal(int x, int y)
        {
            revealed[x, y] = true;
            left_cells -= 1;
            updateImage(x, y);
            if (flagged[x, y])
            {
                MineCounter++;
            }

            if (mine[x, y] || neighbourMines[x, y] != 0)
            {
                return;
            }

            (int, int)[] neighbours = new (int, int)[]
            {
            (1,0),(0,1),(-1,0),(0,-1),(1,1),(1,-1),(-1,1),(-1,-1)
            };
            foreach ((int, int) n in neighbours)
            {
                int xx = x + n.Item1;
                int yy = y + n.Item2;
                if (xx >= 0 && yy >= 0 && xx < width && yy < height)
                {
                    if (!revealed[xx, yy])
                    {
                        Reveal(xx, yy);
                    }

                }

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
