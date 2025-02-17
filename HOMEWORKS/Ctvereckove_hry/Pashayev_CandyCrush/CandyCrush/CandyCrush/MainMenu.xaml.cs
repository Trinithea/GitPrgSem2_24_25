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

namespace CandyCrush
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        public string username;


        public MainMenu()
        {
            InitializeComponent();
        }

        private void normal_click(object sender, RoutedEventArgs e)
        {
            if (CheckUsername())
            {
                MainWindow mainWindow = new MainWindow(0, username);
                mainWindow.ShowDialog();
            }
            
        }

        private void endless_click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(1, username);
            mainWindow.ShowDialog();
        }

        private void scoreboard_click(object sender, RoutedEventArgs e)
        {
            Scoreboard scoreboard = new Scoreboard();
            scoreboard.ShowDialog();
        }

        private void SubmitName(object sender, RoutedEventArgs e)
        {
            if(CheckUsername())
            {
                MessageBox.Show("Nickname accepted. You can play! :D");
            }
        }

        private bool CheckUsername()
        {
            username = SearchTermTextBox.Text;
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter a Nickname.");
                return false;
            }
            return true;
        }
    }
}
