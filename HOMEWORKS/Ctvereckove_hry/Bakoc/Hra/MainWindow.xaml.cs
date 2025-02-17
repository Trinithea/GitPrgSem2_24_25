using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hra
{
   
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            
        }

        private void PlayGomoku(object sender, RoutedEventArgs e)
        {
            Gomoku game = new Gomoku();
            this.Hide();
            game.ShowDialog();
            this.Show();
        }

        private void PlayPexeso(object sender, RoutedEventArgs e)
        {
            PexesoSettings settings = new PexesoSettings();
            settings.ShowDialog();
            if (settings.confirmed)
            {
                Pexeso game = new Pexeso((int)settings.Players);
                this.Hide();
                game.ShowDialog();
                this.Show();
            }
        }

        private void PlayExplodingAtoms(object sender, RoutedEventArgs e)
        {
            ExplodingAtoms game = new ExplodingAtoms();
            this.Hide();
            game.ShowDialog();
            this.Show();
        }

        private void PlayMinesweeper(object sender, RoutedEventArgs e)
        {
            MinesweeperSettings settings = new MinesweeperSettings();
            settings.ShowDialog();

            if (settings.confirmed)
            {
                Minesweeper game = new Minesweeper((int)settings.GameWidth,(int)settings.GameHeight,(int)settings.Mines);
                this.Hide();
                game.ShowDialog();
                this.Show();
            }
            
        }
    }
}