using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;

namespace CandyCrush
{

    

    // Zkusil jsem to. Nevím jak moc dobrý to je rozdělený




    public class Model : INotifyPropertyChanged
    {
        private int score;
        private string time;
        private DispatcherTimer countdownTimer;
        public TimeSpan timeLeft;
        private MainWindow game;

        public event PropertyChangedEventHandler PropertyChanged;

        public string username { get; set; }

        public int Score
        {
            get { return score; }
            set
            {
                if (score != value)
                {
                    score = value;
                    OnPropertyChanged(nameof(Score));
                }
            }
        }

        public string Time
        {
            get { return time; }
            set
            {
                if (time != value)
                {
                    time = value;
                    OnPropertyChanged(nameof(Time));
                }
            }
        }

        public Model(int mode, string username, MainWindow game)
        {
            if (mode != 1)
            {
                StartCountdown();
            }

            this.username = username;
            this.game = game;
        }


        private void StartCountdown()
        {
            timeLeft = TimeSpan.FromSeconds(120);
            UpdateTimeDisplay();

            countdownTimer = new DispatcherTimer();
            countdownTimer.Interval = TimeSpan.FromSeconds(1);
            countdownTimer.Tick += CountdownTick;
            countdownTimer.Start();
        }

        private void CountdownTick(object sender, EventArgs e)
        {
            if (timeLeft.TotalSeconds > 0)
            {
                timeLeft = timeLeft.Subtract(TimeSpan.FromSeconds(1));
                UpdateTimeDisplay();
            }
            else
            {
                countdownTimer.Stop();

                // ukládání do csv
                using (StreamWriter sw = new StreamWriter("my.csv", true))
                {
                    sw.WriteLine($"{username},{Score}");
                }

                
                Scoreboard scoreboardWindow = new Scoreboard();
                game.Close();
                scoreboardWindow.Show();
            }
        }

        private void UpdateTimeDisplay() // toto je MOŽNÁ viewmodel
        {
            Time = timeLeft.ToString(@"mm\:ss");
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateScore(int points)
        {
            Score += points;
        }
    }
}
