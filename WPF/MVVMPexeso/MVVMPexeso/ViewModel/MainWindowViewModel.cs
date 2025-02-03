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
using MVVMPexeso.Model;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace MVVMPexeso.ViewModel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private enum Mode { Easy, Medium, Hard }
        private Mode GameMode { get; set; }

        private Color DefaultColor = Colors.AliceBlue;
        private Color HiglightColor  = Colors.Green;
        private List<Card> Cards { get; set; }
        private List<Button> Buttons { get; set; }
        private Dictionary<Mode, int> cardsPerGameMode { get; set; }

        private Button firstSelectedButton = null;
        private Button secondSelectedButton = null;
        private Grid Grid {  get; set; }

        #region Data Binding
        // Vlastnosti, na nichž máme data binding: skóre a jestli se zrovna hraje
        private int _score;
        public int Score
        {
            get => _score;
            set
            {
                if (_score != value)
                {
                    _score = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _gameIsRunning;
        public bool GameIsRunning
        {
            get { return !_gameIsRunning; }
            set { 
                if(_gameIsRunning != value)
                {
                    _gameIsRunning = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



        #endregion

        #region Herní logika
        // Samotná herní logika
        public void StartGame(string mode, Grid grid)
        {
            GameMode = (Mode)Enum.Parse(typeof(Mode), mode);
            Cards = new List<Card>();
            cardsPerGameMode = new Dictionary<Mode, int>()
            {
                {Mode.Easy, 36},
                {Mode.Medium, 64},
                {Mode.Hard,   100}
            };
            Grid = grid;
            CreateGameCards();
            ShuffleCards();
            CreateGameGrid();
            GameIsRunning = true;

        }
        private void CreateGameCards()
        {
            // přidáme dvojice karet
            for (int i = 0; i < cardsPerGameMode[GameMode] / 2; i++)
            {
                Cards.Add(new Card(i));
                Cards.Add(new Card(i));
            }
        }
        private int GetNumberOfCards() => Cards.Count;
        private void ShuffleCards()
        {
            Random rng = new Random();
            int n = Cards.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (Cards[i], Cards[j]) = (Cards[j], Cards[i]); // Swap
            }
        }

        private void CreateGameGrid()
        {
            int sqrt = Convert.ToInt32(Math.Sqrt(cardsPerGameMode[GameMode]));
            Grid.RowDefinitions.Clear();
            Grid.ColumnDefinitions.Clear();

            for (int i = 0; i < sqrt; i++)
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            for (int i = 0; i < sqrt; i++)
                Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            for(int i = 0;i < sqrt; i++)
            {
                for (int j = 0; j < sqrt; j++)
                {
                    Button button = new Button();
                    //button.Content = Items[sqrt*i + j].Id;
                    button.Tag = sqrt * i + j;
                    button.Click += CardSelected;
                    button.Background = new SolidColorBrush(Colors.AliceBlue);

                    // Nastavení pozice v Gridu
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);

                    // Přidání tlačítka do Gridu
                    Grid.Children.Add(button);
                }
            }

        }

        private void StartTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                MessageBox.Show("Timer completed!");
            };
            timer.Start();
        }

        private async void CardSelected(object sender, RoutedEventArgs e)
        {
            if (firstSelectedButton == null || secondSelectedButton == null)
            {
                ChangeButtonColor((Button)sender, HiglightColor);
                ((Button)sender).Content = Cards[(int)((Button)sender).Tag].Id;
                if (firstSelectedButton == null)
                    firstSelectedButton = (Button)sender;
                else if (secondSelectedButton == null)
                {
                    secondSelectedButton = (Button)sender;
                    await Task.Delay(2000); // Wait for 2 seconds
                }
            }            
                
            if(firstSelectedButton != null && secondSelectedButton != null)
            {
                if (DoCardsMatch())
                    RemovePair();

                ChangeButtonColor(firstSelectedButton, DefaultColor);
                ChangeButtonColor(secondSelectedButton, DefaultColor);
                firstSelectedButton.Content = "";
                secondSelectedButton.Content = "";

                firstSelectedButton = null;
                secondSelectedButton = null;
            }
            
        }

        private void ChangeButtonColor(Button button, Color color)
        {
            button.Background = new SolidColorBrush(color);
        }

        private bool DoCardsMatch()
        {
            var item1 = Cards[(int)firstSelectedButton.Tag];
            var item2 = Cards[(int)secondSelectedButton.Tag];
            return item1.Id == item2.Id;
        }

        private void RemovePair()
        {
            var item1 = Cards[(int)firstSelectedButton.Tag];
            var item2 = Cards[(int)secondSelectedButton.Tag];
            
            //Items.Remove(item1); Items.Remove(item2);   
            
            this.Grid.Children.Remove(firstSelectedButton);
            this.Grid.Children.Remove(secondSelectedButton);

            Score += 1;

            if (Score == cardsPerGameMode[GameMode] / 2)
                GameIsRunning = false;
        }
        #endregion

    }

}

    

