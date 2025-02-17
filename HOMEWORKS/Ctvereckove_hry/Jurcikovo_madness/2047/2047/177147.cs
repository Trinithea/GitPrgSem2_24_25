using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace _2047
{
    public class _177147 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private int number = 2;
        private int size;
        private Style labelStyle = Application.Current.FindResource("SquareGameStyle") as Style;
        private Grid mainGrid;
        private Label[,] tileMap;
        private Brush[] bgColors = new Brush[]
        {
            new SolidColorBrush(Color.FromRgb(242, 230, 217)),
            new SolidColorBrush(Color.FromRgb(222, 192, 160)),
            new SolidColorBrush(Color.FromRgb(255, 166, 77)),
            new SolidColorBrush(Color.FromRgb(255, 128, 0)),
            new SolidColorBrush(Color.FromRgb(255, 153, 128)),
            new SolidColorBrush(Color.FromRgb(255, 71, 26)),
            new SolidColorBrush(Color.FromRgb(255, 224, 102)),
            new SolidColorBrush(Color.FromRgb(255, 219, 77)),
            new SolidColorBrush(Color.FromRgb(255, 214, 51)),
            new SolidColorBrush(Color.FromRgb(255, 209, 26)),
            new SolidColorBrush(Color.FromRgb(255, 204, 0)),
            new SolidColorBrush(Color.FromRgb(230, 184, 0)),
            new SolidColorBrush(Color.FromRgb(204, 163, 0)),
            new SolidColorBrush(Color.FromRgb(179, 143, 0)),
            new SolidColorBrush(Color.FromRgb(153, 122, 0)),
            new SolidColorBrush(Color.FromRgb(128, 102, 0)),
        };
        private int score;

        public int Score
        {
            get { return score; }
            private set { score = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Score")); }
        }



        public _177147(int _size, Grid grid)
        {
            mainGrid = grid;
            size = _size;
            tileMap = new Label[size, size];

            for (int i = 0; i < size; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                for (int j = 0; j < size; j++)
                {
                    Label text = createLabel(-1);
                    tileMap[i, j] = text;
                }
            }

            Random rng = new();
            Label FirstNum = createLabel(0);
            tileMap[rng.Next(size), rng.Next(size)] = FirstNum;
            renderTiles();
            mainGrid.Loaded += Game_Loaded;
            mainGrid.PreviewKeyDown += keyDown;
        }


        private Label createLabel(int power)
        {
            Label label = new Label()
            {
                Style = labelStyle,
                Foreground = power >= 0 ? getForegroundBrush(power):null,
                Background = power >= 0 ? bgColors[power]:null,
                Content = power >= 0 ? Math.Pow(2, power + 1).ToString() : null,
                
            };

            return label;
        }

        private Brush getForegroundBrush(int power)
        {
            if (power < 2)
                return Brushes.Black;
            else
                return Brushes.White;
        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    moveUp();
                    break;
                case Key.Down:
                    moveDown();
                    break;
                case Key.Right:
                    moveRight();
                    break;
                case Key.Left:
                    moveLeft();
                    break;
            }
            placeRandomTile();
            renderTiles();
        }
        private void Game_Loaded(object sender, RoutedEventArgs e)
        {
            mainGrid.Focusable = true;
            mainGrid.Focus();
            Keyboard.Focus(mainGrid);
        }

        private void placeRandomTile()
        {
            List<int[]> freePositions = new List<int[]>();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (tileMap[i, j].Content == null)
                        freePositions.Add([i, j]);
                }
            }
            if (freePositions.Count == 0)
            {
                MessageBox.Show("you lost");
                mainGrid.IsEnabled = false;
                return;
            }
            Random rng = new Random();
            int[] newpos = freePositions[rng.Next(freePositions.Count)];
            tileMap[newpos[0], newpos[1]] = createLabel(rng.Next(10) == 0 ? 1 : 0);
        }

        private void moveRight()
        {
            for (int i = 0; i < size; i++)
            {
                Queue<Label> queue = new Queue<Label>();
                Label? prevLabel = null;
                for (int j = size - 1; j >= 0; j--)
                {
                    Label currentLabel = tileMap[i, j];
                    if (string.IsNullOrEmpty((string?)currentLabel.Content))
                        continue;

                    if (prevLabel == null)
                    {
                        prevLabel = currentLabel;
                        continue;
                    }
                        

                    if (currentLabel.Content.ToString() == prevLabel.Content.ToString())
                    {
                        int power = (int)Math.Log2(double.Parse(currentLabel.Content.ToString()));
                        Score += (int)Math.Pow(2, power + 1);
                        Label newLabel = createLabel(power);
                        queue.Enqueue(newLabel);
                        prevLabel = null;
                    }
                    else
                    {
                        queue.Enqueue(prevLabel);
                        prevLabel = currentLabel;
                    }  
                }

                if (prevLabel != null)
                    queue.Enqueue(prevLabel);

                for (int j = size - 1; j >= 0; j--)
                {
                    Label label = queue.Count > 0 ? queue.Dequeue() : createLabel(-1);
                    tileMap[i, j] = label;
                }
            }
        }

        private void moveLeft()
        {
            for (int i = 0; i < size; i++)
            {
                Queue<Label> queue = new Queue<Label>();
                Label? prevLabel = null;
                for (int j = 0; j < size; j++)
                {
                    Label currentLabel = tileMap[i, j];
                    if (string.IsNullOrEmpty((string?)currentLabel.Content))
                        continue;

                    if (prevLabel == null)
                    {
                        prevLabel = currentLabel;
                        continue;
                    }


                    if (currentLabel.Content.ToString() == prevLabel.Content.ToString())
                    {
                        int power = (int)Math.Log2(double.Parse(currentLabel.Content.ToString()));
                        Score += (int)Math.Pow(2, power + 1);
                        Label newLabel = createLabel(power);
                        queue.Enqueue(newLabel);
                        prevLabel = null;
                    }
                    else
                    {
                        queue.Enqueue(prevLabel);
                        prevLabel = currentLabel;
                    }
                }

                if (prevLabel != null)
                    queue.Enqueue(prevLabel);

                for (int j = 0; j < size; j++)
                {
                    Label label = queue.Count > 0 ? queue.Dequeue() : createLabel(-1);
                    tileMap[i, j] = label;
                }
            }
        }

        private void moveDown()
        {
            for (int i = 0; i < size; i++)
            {
                Queue<Label> queue = new Queue<Label>();
                Label? prevLabel = null;
                for (int j = size - 1; j >= 0; j--)
                {
                    Label currentLabel = tileMap[j, i];
                    if (string.IsNullOrEmpty((string?)currentLabel.Content))
                        continue;

                    if (prevLabel == null)
                    {
                        prevLabel = currentLabel;
                        continue;
                    }


                    if (currentLabel.Content.ToString() == prevLabel.Content.ToString())
                    {
                        int power = (int)Math.Log2(double.Parse(currentLabel.Content.ToString()));
                        Score += (int)Math.Pow(2, power + 1);
                        Label newLabel = createLabel(power);
                        queue.Enqueue(newLabel);
                        prevLabel = null;
                    }
                    else
                    {
                        queue.Enqueue(prevLabel);
                        prevLabel = currentLabel;
                    }
                }

                if (prevLabel != null)
                    queue.Enqueue(prevLabel);

                for (int j = size - 1; j >= 0; j--)
                {
                    Label label = queue.Count > 0 ? queue.Dequeue() : createLabel(-1);
                    tileMap[j, i] = label;
                }
            }
        }

        private void moveUp()
        {
            for (int i = 0; i < size; i++)
            {
                Queue<Label> queue = new Queue<Label>();
                Label? prevLabel = null;
                for (int j = 0; j < size; j++)
                {
                    Label currentLabel = tileMap[j, i];
                    if (string.IsNullOrEmpty((string?)currentLabel.Content))
                        continue;

                    if (prevLabel == null)
                    {
                        prevLabel = currentLabel;
                        continue;
                    }


                    if (currentLabel.Content.ToString() == prevLabel.Content.ToString())
                    {
                        int power = (int)Math.Log2(double.Parse(currentLabel.Content.ToString()));
                        Score += (int)Math.Pow(2, power + 1);
                        Label newLabel = createLabel(power);
                        queue.Enqueue(newLabel);
                        prevLabel = null;
                    }
                    else
                    {
                        queue.Enqueue(prevLabel);
                        prevLabel = currentLabel;
                    }
                }

                if (prevLabel != null)
                    queue.Enqueue(prevLabel);

                for (int j = 0; j < size; j++)
                {
                    Label label = queue.Count > 0 ? queue.Dequeue() : createLabel(-1);
                    tileMap[j, i] = label;
                }
            }
        }




        private void renderTiles()
        {
            mainGrid.Children.Clear();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Label text = tileMap[i, j];
                    Grid.SetRow(text, i);
                    Grid.SetColumn(text, j);
                    mainGrid.Children.Add(text);
                }
            }
        }

    }
}
