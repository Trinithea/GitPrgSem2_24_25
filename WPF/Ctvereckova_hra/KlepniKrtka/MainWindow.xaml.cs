using System.Diagnostics;
using System.IO;
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
using System.Timers;

namespace KlepniKrtka
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();            
            GenerateButtons();
        }
        
        private Button[,] GenerateButtons()
        {
            Button[,] buttons = new Button[5, 5];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Button button = new Button();
                    button.Click += ButtonClicked;

                    // Nastavení pozice v Gridu
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);

                    // Přidání tlačítka do Gridu
                    gridGame.Children.Add(button);

                    // Přidání tlačítka do pole tlačítek
                    buttons[i, j] = button;
                }
            }
            return buttons;
        }       
        private void ButtonClicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tlačítko kliknuto");
        }
    }

    
}