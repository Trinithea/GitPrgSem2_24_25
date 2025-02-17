using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.ComponentModel;

namespace patnactka
{
    internal class Game : INotifyPropertyChanged
    {
        private int[] tiles;
        private Button[,] buttons = new Button[4, 4];
        private Grid PuzzleGrid;

        public event PropertyChangedEventHandler? PropertyChanged;

        private int moves;

        public int Moves
        {
            get { return moves; }
            private set { moves = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Moves")); }
        }


        public Game(Grid grid)
        {
            PuzzleGrid = grid;
            GeneratePuzzleGrid();
            ShuffleAndPopulate();
        }

        private void GeneratePuzzleGrid()
        {
            PuzzleGrid.RowDefinitions.Clear();
            PuzzleGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < 4; i++)
            {
                PuzzleGrid.RowDefinitions.Add(new RowDefinition());
                PuzzleGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    Button btn = new Button
                    {
                        FontSize = 24,
                        FontWeight = FontWeights.Bold,
                        Background = Brushes.White,
                        Margin = new Thickness(3)
                    };
                    btn.Click += Tile_Click;
                    Grid.SetRow(btn, row);
                    Grid.SetColumn(btn, col);
                    PuzzleGrid.Children.Add(btn);
                    buttons[row, col] = btn;
                }
            }
        }

        private void ShuffleAndPopulate()
        {
            Moves = 0;
            tiles = GenerateSolvablePuzzle();
            UpdateBoard();
        }

        private void UpdateBoard()
        {
            for (int i = 0; i < 16; i++)
            {
                int row = i / 4, col = i % 4;
                int value = tiles[i];

                buttons[row, col].Content = value == 0 ? "" : value.ToString();
                buttons[row, col].IsEnabled = value != 0;
            }
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button clicked)
                return;

            int clickedRow = Grid.GetRow(clicked);
            int clickedCol = Grid.GetColumn(clicked);

            int blankRow = 0, blankCol = 0;
            for (int i = 0; i < 16; i++)
            {
                if (tiles[i] == 0)
                {
                    blankRow = i / 4;
                    blankCol = i % 4;
                    break;
                }
            }

            if (Math.Abs(blankRow - clickedRow) + Math.Abs(blankCol - clickedCol) == 1)
            {
                (tiles[blankRow * 4 + blankCol], tiles[clickedRow * 4 + clickedCol]) =
                (tiles[clickedRow * 4 + clickedCol], tiles[blankRow * 4 + blankCol]);

                UpdateBoard();
                Moves++;

                if (IsSolved())
                {
                    MessageBox.Show($"You won in {Moves} moves");
                    ShuffleAndPopulate();
                }
            }
        }

        private bool IsSolved()
        {
            for (int i = 0; i < 15; i++)
            {
                if (tiles[i] != i + 1) return false;
            }
            return tiles[15] == 0;
        }

        private int[] GenerateSolvablePuzzle()
        {
            Random rand = new Random();
            int[] puzzle;
            do
            {
                puzzle = Enumerable.Range(0, 16).OrderBy(_ => rand.Next()).ToArray();
            } while (!IsSolvable(puzzle));

            return puzzle;
        }

        private bool IsSolvable(int[] puzzle)
        {
            int inversions = CountInversions(puzzle);
            int blankRow = GetBlankRowFromBottom(puzzle);
            return (inversions + blankRow) % 2 == 0;
        }

        private int CountInversions(int[] puzzle)
        {
            int count = 0;
            for (int i = 0; i < 15; i++)
            {
                for (int j = i + 1; j < 16; j++)
                {
                    if (puzzle[i] > puzzle[j] && puzzle[i] != 0 && puzzle[j] != 0)
                        count++;
                }
            }
            return count;
        }

        private int GetBlankRowFromBottom(int[] puzzle)
        {
            int index = Array.IndexOf(puzzle, 0);
            int rowFromTop = index / 4;
            return 3 - rowFromTop;
        }
    }
}
