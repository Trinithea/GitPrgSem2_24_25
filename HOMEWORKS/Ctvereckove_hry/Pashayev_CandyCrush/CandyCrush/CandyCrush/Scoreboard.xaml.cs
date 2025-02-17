using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using static System.Formats.Asn1.AsnWriter;

namespace CandyCrush
{
    public partial class Scoreboard : Window
    {
        public Scoreboard()
        {
            InitializeComponent();
            LoadScores();
        }
        public class ScoreData
        {
            public string Username { get; set; }
            public int Score { get; set; }
        }

        private void LoadScores()
        {
            List<ScoreData> scores = new List<ScoreData>();

            string filePath = "my.csv";

            if (File.Exists(filePath))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] cells = line.Split(',');

                            if (int.TryParse(cells[1], out int x))
                            {
                                scores.Add(new ScoreData
                                {
                                    Username = cells[0],
                                    Score = x
                                });
                            }
                        }
                    }

                    var sortedScores = scores.OrderByDescending(s => s.Score).ToList();
                    ScoresList.ItemsSource = sortedScores;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Can't read. May thou hath the file openeth?", ":(", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Where is my.csv.", "???", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
