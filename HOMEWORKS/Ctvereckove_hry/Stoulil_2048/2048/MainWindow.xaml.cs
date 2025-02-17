using _2048.ViewModel;
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

namespace _2048
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel vm = new MainWindowViewModel();
            DataContext = vm;

    
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //start
            ((MainWindowViewModel)DataContext).InitializeGame(GameGrid);
        }

        //pohyby
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) ((MainWindowViewModel)DataContext).Move("Up");
            else if (e.Key == Key.Down) ((MainWindowViewModel)DataContext).Move("Down");
            else if (e.Key == Key.Left) ((MainWindowViewModel)DataContext).Move("Left");
            else if (e.Key == Key.Right) ((MainWindowViewModel)DataContext).Move("Right");
        }
    }
}