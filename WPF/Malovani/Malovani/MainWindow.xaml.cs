using Microsoft.Win32;
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
using System.Drawing;
using Color = System.Drawing.Color;

namespace Malovani
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isDrawing = false; // Indikátor, zda kreslíme
        private System.Windows.Point _startPoint; // Počáteční bod kreslení
        private double _circleRadius = 1; // Poloměr kruhu
        private System.Windows.Media.Color _color = System.Windows.Media.Color.FromRgb(0,0,0);
        public MainWindow()
        {
            InitializeComponent();
            
        }

        public void Save(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Obrázky (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Všechny soubory (*.*)|*.*"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result.Value)
            {
                // Save document
                string filename = dlg.FileName;
            }
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".txt"; // Default file extension
            dialog.Filter = "Obrázky (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Všechny soubory (*.*)|*.*"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
            }
        }

        private void PickColor(object sender, RoutedEventArgs e)
        {
           RGBColorPicker rgbColorPicker = new RGBColorPicker();
            rgbColorPicker.ShowDialog(); // nepustí nás do původního okna, dokud nezavřeme nově otevřené
            if (rgbColorPicker.Success)
            {
                _color = rgbColorPicker.Color;
                btnColor.Background = new SolidColorBrush(rgbColorPicker.Color);
            }
        }

        // Událost při stisknutí myši
        private void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                _isDrawing = true;
                _circleRadius = sliderThickness.Value;
            }
        }

        // Událost při pohybu myši
        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                // Získání aktuální pozice myši
                System.Windows.Point currentPoint = e.GetPosition(DrawingCanvas);

                // Vytvoření kruhu (Ellipse)
                Ellipse circle = new Ellipse
                {
                    Width = _circleRadius,
                    Height = _circleRadius,
                    Fill = new SolidColorBrush(_color) // Barva kruhu
                };

                // Nastavení pozice kruhu
                Canvas.SetLeft(circle, currentPoint.X - _circleRadius);
                Canvas.SetTop(circle, currentPoint.Y - _circleRadius);

                // Přidání kruhu na Canvas
                DrawingCanvas.Children.Add(circle);
            }
        }

        // Událost při uvolnění tlačítka myši
        private void DrawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDrawing)
            {
                _isDrawing = false;
            }
        }

        
    }

}