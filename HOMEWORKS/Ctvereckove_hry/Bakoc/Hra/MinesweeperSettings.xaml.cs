using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Hra
{
    /// <summary>
    /// Interakční logika pro MinesweeperSettings.xaml
    /// </summary>
    public partial class MinesweeperSettings : Window, INotifyPropertyChanged
    {

        public bool confirmed = false;

        private double width = 9;
        private double height = 9;
        private double mines = 10;

        public double GameWidth
        {
            get
            {
                return width;
            }
            set
            {
                if (width != value)
                {
                    width = value;
                    widthLabel.Content = $"Width: {GameWidth}";
                    PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(GameWidth)));
                }
            }
        }

        public double GameHeight
        {
            get
            {
                return height;
            }
            set
            {
                if (height != value)
                {
                    height = value;
                    heightLabel.Content = $"Height: {GameHeight}";
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GameHeight)));
                }
            }
        }
        public double Mines
        {
            get
            {
                return mines;
            }
            set
            {
                if (mines != value)
                {
                    mines = value;
                    minesLabel.Content = $"Mines: {Mines}";
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Mines)));
                }
            }
        }

        
        public MinesweeperSettings()
        {
            
            InitializeComponent();
            
            this.DataContext = this;
        }

        private void widthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            widthLabel.Content = $"Width: {(int)GameWidth}";
            if (minesSlider != null && heightSlider != null)
            {
                minesSlider.Maximum = Math.Min(GameWidth * GameHeight - 10, 500);
            }
        }
        private void heightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            heightLabel.Content = $"Height: {(int)GameHeight}";
            if (minesSlider != null && widthSlider != null)
            {
                minesSlider.Maximum = Math.Min(GameWidth * GameHeight - 10, 500);
            }
        }
        private void minesSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            minesLabel.Content = $"Mines: {(int)Mines}";
        }
        private void Begginer_Click(object sender, RoutedEventArgs e)
        {
            GameWidth = 9;
            GameHeight = 9;
            Mines = 10;

        }
        private void Intermediate_Click(object sender, RoutedEventArgs e)
        {
            GameWidth = 16;
            GameHeight = 16;
            Mines = 40;

        }
        private void Expert_Click(object sender, RoutedEventArgs e)
        {
            GameWidth = 30;
            GameHeight = 16;
            Mines = 99;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (minesSlider.Value >= widthSlider.Value * heightSlider.Value - 9)
            {
                MessageBox.Show("Too small board for this many mines!!");
                return;
            }
            confirmed = true;
            this.Close();

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }


}
