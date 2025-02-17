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

namespace minesweeper
{
    /// <summary>
    /// Interakční logika pro MineButton.xaml
    /// </summary>
    public partial class MineButton : UserControl
    {
        public bool IsMine { get; }
        public int[] Position { get; }
        public bool Flagged;

        private bool isClickable = true;

        public bool IsClickable
        {
            get { return isClickable; }
            set { isClickable = value; this.button.Visibility = value ? Visibility.Visible : Visibility.Hidden; }
        }


        public RoutedEventHandler ButtonClick;
        public MineButton(bool isMine, int[] position)
        {
            InitializeComponent();
            IsMine = isMine;
            Position = position;
        }

        

        private void button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                ButtonClick.Invoke(this, e);
        }
    }
}
