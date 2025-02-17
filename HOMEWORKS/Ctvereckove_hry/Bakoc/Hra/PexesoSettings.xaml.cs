using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
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
    /// Interakční logika pro PexesoSettings.xaml
    /// </summary>
    public partial class PexesoSettings : Window, INotifyPropertyChanged
    {

        public bool confirmed = false;


        private double players;
        public double Players
        {
            get { return players; }
            set
            {
                if (players != value)
                {
                    players = value;
                    label.Content = $"Players: {(int)Players}";
                    OnPropertyChanged();
                }
            }
        }
        public PexesoSettings()
        {
            InitializeComponent();
            Players = 2;
            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            confirmed = true;
            this.Close();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
