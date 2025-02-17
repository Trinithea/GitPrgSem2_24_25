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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pisisvorky
{
    /// <summary>
    /// Interakční logika pro buton.xaml
    /// </summary>
    public partial class TileButton : UserControl
    {
        public int[] Position { get; }
        public RoutedEventHandler ButtonClick;
        public TileButton(int[] position)
        {
            InitializeComponent();
            Position = position;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick?.Invoke(this, new RoutedEventArgs());
        }
    }
}
