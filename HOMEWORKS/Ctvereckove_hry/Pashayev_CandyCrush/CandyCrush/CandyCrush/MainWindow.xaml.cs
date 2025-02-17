using System.Drawing;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CandyCrush
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Button mainButton = null;
        private int scoreThisTurn;
        private bool textActive;

        private Model model; // na propojení model.cs
        private Random rand = new Random();

        private Dictionary<Button, int> buttonID = new Dictionary<Button, int>();
        private Button[,] buttonList = new Button[8, 8];
        private List<Button> adjacentX = new List<Button>();
        private List<Button> adjacentY = new List<Button>();
        


        

        public MainWindow(int mode, string username)
        {
            InitializeComponent();

            if (mode == 1)
            {
                timerect.Opacity = 0;
            }

            model = new Model(mode, username, this);
            this.DataContext = model;

            GenerateButtons();
        }

        private Button[,] GenerateButtons()
        {

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Button button = new Button();

                    button.Click += ButtonClicked;

                    // Nastavení pozice v Gridu
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);

                    // Přidání tlačítka do Gridu
                    gridGame.Children.Add(button);

                    int buttonNumber = rand.Next(1, 6);
                    buttonID[button] = buttonNumber;

                    // Přidání tlačítka do pole tlačítek
                    buttonList[i, j] = button;
                }
            }
            while (CheckBoard())
            {
                FixBoard();
            }
            ColourBoard();
            return buttonList;
        }

        private void FixBoard()
        {
            for (int i = 0; i < buttonList.GetLength(0); i++)
            {
                for (int j = 0; j < buttonList.GetLength(1); j++)
                {
                    int buttonColour = buttonID[buttonList[i, j]];

                    int horizontalCount = CountMatchesX(buttonList[i, j]);
                    int verticalCount = CountMatchesY(buttonList[i, j]);

                    if (horizontalCount >= 2 || verticalCount >= 2)
                    {
                        while (buttonID[buttonList[i, j]] == buttonColour)
                        {
                            buttonID[buttonList[i, j]] = rand.Next(1, 6);
                        }
                    }
                }
            }
        }

        private void ButtonClicked(object sender, RoutedEventArgs e)
        {
            var clickedButton = sender as Button;

            if (mainButton == null)
            {
                mainButton = clickedButton;
                mainButton.BorderBrush = Brushes.Cyan;
                return;
            }

            int mainRow = Grid.GetRow(mainButton);
            int mainCol = Grid.GetColumn(mainButton);
            int clickedRow = Grid.GetRow(clickedButton);
            int clickedCol = Grid.GetColumn(clickedButton);

            if (CheckIfAdjacent(mainRow, mainCol, clickedRow, clickedCol))
            {
                SwapButtons(mainButton, clickedButton);
            }
            else
            {
                MessageBox.Show("Please select an adjacent item.");
            }
            mainButton.ClearValue(Button.BorderBrushProperty);
            mainButton = null;
        }

        private bool CheckIfAdjacent(int mainx, int mainy, int secx, int secy) // můžeme pouze vybrat tlačítka vedle sebe (candy crush rules)
        {
            if ((Math.Abs(mainx - secx) == 1 && mainy == secy) || (mainx == secx && Math.Abs(mainy - secy) == 1))
            {
                return true;
            }
            else { return false; }
        }

        private void SwapButtons(Button button1, Button button2)
        {
            (buttonID[button1], buttonID[button2]) = (buttonID[button2], buttonID[button1]); 

            if (!ValidateMove(button1) && !ValidateMove(button2))
            {
                (buttonID[button1], buttonID[button2]) = (buttonID[button2], buttonID[button1]);
                MessageBox.Show("Move invalid (move should result in 3 or more of the same item in a row)");
            }
            else
            {
                scoreThisTurn = 0;
                ColourBoard();
                if (ValidateMove(button1)) { BreakButtons(); }
                if (ValidateMove(button2)) { BreakButtons(); }
            }
        }

        private void ColourBoard() // obrázky uložené ve tvaru "1.png, 2.png..." takže lehce to můžu propojít s ID tlačítek
        {
            string projectPath = Directory.GetCurrentDirectory();
            string resourcesPath = System.IO.Path.Combine(projectPath, "Images");

            foreach (Button button in buttonList)
            {
                int buttonNumber = buttonID[button];

                string imagePath = System.IO.Path.Combine(resourcesPath, $"{buttonNumber}.png");

                if (File.Exists(imagePath))
                {
                    button.ClearValue(Button.BorderBrushProperty);
                    ImageBrush brush = new ImageBrush();
                    brush.ImageSource = new BitmapImage(new Uri(imagePath));
                    button.Background = brush;
                }
                else
                {
                    button.Background = Brushes.LightGray;
                }
            }
        }

        

        private int CountMatchesX(Button button)  // počet tlačítek se stejnými ID tlačítka button ve směru x
        {
            int row = Grid.GetRow(button);
            int col = Grid.GetColumn(button);
            return CountContinuous(row, col, 0, 1, buttonID[button]) + CountContinuous(row, col, 0, -1, buttonID[button]);
        }
        private int CountMatchesY(Button button) // počet tlačítek se stejnými ID tlačítka button ve směru y
        {
            int row = Grid.GetRow(button);
            int col = Grid.GetColumn(button);
            return CountContinuous(row, col, 1, 0, buttonID[button]) + CountContinuous(row, col, -1, 0, buttonID[button]);
        }

        private int CountContinuous(int row, int col, int rowDelta, int colDelta, int swappedButtonID) // počítá průběžná id ve směru x/y v závislosti na rowDelta / colDelta
        {
            int count = 0;
            int x = row + rowDelta;
            int y = col + colDelta;

            while (IsValidPosition(x, y) && buttonID[buttonList[x, y]] == swappedButtonID)
            {

                if (rowDelta == 0) { adjacentX.Add(buttonList[x, y]); }
                else if (colDelta == 0) { adjacentY.Add(buttonList[x, y]); }

                count++;
                x += rowDelta;
                y += colDelta;
            }

            return count;
        }


        private bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < buttonList.GetLength(0) && y >= 0 && y < buttonList.GetLength(1);
        }

        private bool ValidateMove(Button button)
        {
            int horizontalCount = CountMatchesX(button);
            int verticalCount = CountMatchesY(button);

            if (horizontalCount >= 2 || verticalCount >= 2) { return true; }
            return false;
        }

        private async void BreakButtons()
        {
            List<Button> toBreak = new List<Button>();
            

            for (int i = 0; i < buttonList.GetLength(0); i++)
            {
                for (int j = 0; j < buttonList.GetLength(1); j++)
                {
                    List<Button> toBreakX = new List<Button>(); 
                    List<Button> toBreakY = new List<Button>();

                    Button button = buttonList[i, j];

                    if (buttonID[button] == 0)
                    {
                        continue;
                    }

                    adjacentX.Clear();
                    adjacentY.Clear();

                    int x = CountMatchesX(button);
                    int y = CountMatchesY(button);

                    toBreakX.AddRange(adjacentX);
                    toBreakY.AddRange(adjacentY);

                    if (x >= 2 || y >= 2)
                    {
                        if (x >= 2)
                        {
                            toBreak.AddRange(toBreakX);
                            foreach (Button brokenButton in toBreakX)
                            {
                                if (CountMatchesY(brokenButton) >= 2) // pokud jsou tlačítka ve tvaru "L" bez rohu tak tohle pomáhá aby tlačítky se zlomily ve všech směrech
                                {
                                    toBreak.AddRange(adjacentY);
                                }
                                else { adjacentY.Clear(); }
                            }
                        }

                        if (y >= 2)
                        {
                            toBreak.AddRange(toBreakY);
                            foreach (Button brokenButton in toBreakY)
                            {
                                if(CountMatchesX(brokenButton) >= 2) // to samý tady
                                {
                                    toBreak.AddRange(adjacentX);
                                }
                                else { adjacentX.Clear(); }
                            }
                        }

                        toBreak.Add(button);

                        foreach (Button brokenButton in toBreak)
                        {
                            buttonID[brokenButton] = 0;
                        }

                        model.UpdateScore(x + y + 1);
                        scoreThisTurn = scoreThisTurn + x + y + 1;

                        if (scoreThisTurn > 6 && !textActive)
                        {
                            textActive = true;
                            ShowText();
                        }
                    }
                }
            }
            await FadeButtons(toBreak); // čeká až skončí animace zmizení tlačítek

            if (CheckBoard()) // pokračuje ničit tlačítka po generování
            {
                BreakButtons();
            }
        }

        private void MoveButtonsDown() // its raining buttons
        {
            for (int col = 0; col < buttonList.GetLength(1); col++)
            {
                for (int row = buttonList.GetLength(0) - 1; row > 0; row--)
                {
                    if (buttonID[buttonList[row, col]] == 0)
                    {
                        for (int moveRow = row - 1; moveRow >= 0; moveRow--)
                        {
                            if (buttonID[buttonList[moveRow, col]] != 0)
                            {
                                buttonID[buttonList[row, col]] = buttonID[buttonList[moveRow, col]];
                                buttonID[buttonList[moveRow, col]] = 0;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private bool CheckBoard()
        {
            int horizontalCount = 0;
            int verticalCount = 0;

            for (int i = 0; i < buttonList.GetLength(0); i++)
            {
                for (int j = 0; j < buttonList.GetLength(1); j++)
                {
                    int buttID = buttonID[buttonList[i, j]];

                    horizontalCount = CountMatchesX(buttonList[i, j]);
                    verticalCount = CountMatchesY(buttonList[i, j]);


                    if (horizontalCount >= 2 || verticalCount >= 2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void GenerateNewButtons()
        {
            for (int i = 0; i < buttonList.GetLength(0); i++)
            {
                for (int j = 0; j < buttonList.GetLength(1); j++)
                {
                    if (buttonID[buttonList[i, j]] == 0)
                    {
                        buttonID[buttonList[i, j]] = rand.Next(1, 6);
                    }
                }
            }
        }
        private async Task FadeButtons(List<Button> candies) // animace - tlačítka zmizí a objeví se
        {
            var tcs = new TaskCompletionSource<bool>();

            bool animationCompleted = false;

            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.1));

            fadeOut.Completed += (s, e) =>
            {
                MoveButtonsDown();
                GenerateNewButtons();
                ColourBoard();

                foreach (Button button in candies)
                {
                    button.BeginAnimation(Button.OpacityProperty, fadeIn);
                }
            };

            fadeIn.Completed += (s, e) =>
            {
                if (!animationCompleted)
                {
                    animationCompleted = true;
                    tcs.SetResult(true);
                }
            };

            foreach (Button candy in candies)
            {
                candy.BeginAnimation(Button.OpacityProperty, fadeOut);
            }

            await tcs.Task;
        }

        private void ShowText() // typické Candy Crush hlášky ve stylu Tesco (objeví se uprostřed obrazovky)
        {
            string[] textOptions =
            {
                "Skibidi",
                "Crazy",
                "Nice",
                "Mashallah",
                "Hezky, Braško",
                "Dayum",
                "Jupííííí",
                "Bomboclat",
                "B)",
                "Sigma ඞ",
                "Tesco- licious",
                "Tesco moment",
                "I <3 Tesco",
                "TESCOOOOO",
                "F*ck Billa"
            };

            TextBlock txt = new TextBlock
            {
                Text = textOptions[rand.Next(textOptions.Length)],
                Opacity = 0,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                IsHitTestVisible = false,
                Style = (Style)FindResource("CustomFont")
            };

            Grid.SetColumn(txt, 0);
            Grid.SetRow(txt, 1);
            Grid.SetColumnSpan(txt, 3);
            mainGrid.Children.Add(txt);

            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1)) { BeginTime = TimeSpan.FromSeconds(2) };


            var storyboard = new Storyboard(); // storyboard na pořadí animací
            storyboard.Children.Add(fadeIn);
            storyboard.Children.Add(fadeOut);

            Storyboard.SetTarget(fadeIn, txt);
            Storyboard.SetTarget(fadeOut, txt);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath(TextBlock.OpacityProperty));
            Storyboard.SetTargetProperty(fadeOut, new PropertyPath(TextBlock.OpacityProperty));

            storyboard.Completed += (s, e) =>
            {
                txt.Visibility = Visibility.Collapsed;
                gridGame.Children.Remove(txt);
                textActive = false;
            };
            storyboard.Begin();
        }
    }
}