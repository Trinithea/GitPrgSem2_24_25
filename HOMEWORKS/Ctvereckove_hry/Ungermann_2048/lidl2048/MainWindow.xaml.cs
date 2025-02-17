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
using lidl2048.ViewModel;

namespace lidl2048
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _vm;
        public MainWindow()
        {
            InitializeComponent();

            // prvni cislo nemenit!!!! druhe muzete:)
            // to druhe cislo meni zakladni cislo hry => 2 tedy znamena ze hrajeme 2048
            _vm = new MainWindowViewModel(GameGrid, 4, 2);
            DataContext = _vm;

            KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    _vm.HandleVertical(MainWindowViewModel.Direction.UP);
                    break;
                case Key.Down:
                    _vm.HandleVertical(MainWindowViewModel.Direction.DOWN);
                    break;
                case Key.Left:
                    _vm.HandleHorizontal(MainWindowViewModel.Direction.LEFT);
                    break;
                case Key.Right:
                    _vm.HandleHorizontal(MainWindowViewModel.Direction.RIGHT);
                    break;
            }
        }

        private void ButtonRestart_Click(object sender, RoutedEventArgs e)
        {
            _vm.RestartGame();
        }
    }
}