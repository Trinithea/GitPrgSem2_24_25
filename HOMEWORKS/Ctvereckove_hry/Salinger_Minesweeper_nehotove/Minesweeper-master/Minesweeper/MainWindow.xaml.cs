using System;
using System.Collections.ObjectModel;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
            MainWindowViewModel vm = new MainWindowViewModel();
            DataContext = vm;
            LoadTextures();
        }
        private string _size;






        private void LoadTextures()
        {
            ((MainWindowViewModel)DataContext).LoadTexturesVM();
            
        }
        private void MLBD(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(@"/resources/faces/smilefacedown.png", UriKind.Relative);
            bitmapImage.EndInit();
            face.Source = bitmapImage;
        }
        private void MLBU(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(@"/resources/faces/smileface.png", UriKind.Relative);
            bitmapImage.EndInit();
            face.Source = bitmapImage;
        }




        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            _size = ((RadioButton)sender).Content.ToString();
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)DataContext).StartGame(_size, uniformGrid, face);
        }
        




    }
}