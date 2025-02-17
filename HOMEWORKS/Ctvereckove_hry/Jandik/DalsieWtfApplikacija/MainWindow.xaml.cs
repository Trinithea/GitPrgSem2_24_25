using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.CompilerServices;
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

namespace DalsieWtfApplikacija
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public int HracNaTahu;
        private int E1;
        private int E2;
        private int ExtraPole1
        {
            get { return E1; }
            set
            {
                E1 = value;
                Kominista.Content = "Komínista: " + value + " šv. polí";
            }
        }

        private int ExtraPole2
        {
            get { return E2; }
            set
            {
                E2 = value;
                Lyberal.Content = "Lyberál: " + value + " šv. polí";
            }
        }
        public Pole[,] pole_poli;
        private string l;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string L
        {
            get { return l; }
            set { l = value; OnPropertyChanged(); }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
          => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            pole_poli = new Pole[11, 11];
            HracNaTahu = 1;
            ExtraPole1 = 5;
            ExtraPole2 = 5;
            for (int i = 0; i < 121; i++)
            {
                pole_poli[i / 11, i % 11] = new Pole(i, this);
            }
        }
        public void ZmenHrace(bool extra_pole)
        {
            if (HracNaTahu == 1)
            {
                HracNaTahu = 2;
                Textovac.Content = "Hraje lyberál";
                if (ExtraPole2 == 0) Svycarovac.IsEnabled = false;
                else Svycarovac.IsEnabled = true;
            }
            else if (HracNaTahu == 2)
            {
                HracNaTahu = 1;
                Textovac.Content = "Hraje komínista";
                if (ExtraPole1 == 0) Svycarovac.IsEnabled = false;
                else Svycarovac.IsEnabled = true;
            }
            if (HracNaTahu == -1)
            {
                ExtraPole1--;
                if (ExtraPole1 == 0) { Svycarovac.IsChecked = false; Svycarovac.IsEnabled = false; }
            }
            else if (HracNaTahu == -2)
            {
                ExtraPole2--;
                if (ExtraPole2 == 0) { Svycarovac.IsChecked = false; Svycarovac.IsEnabled = false; }
            }
        }
        public void CH(object sender, EventArgs e)
        {
            int Zasoba = 0;
            if (HracNaTahu == 1) Zasoba = ExtraPole1;
            else Zasoba = ExtraPole2;
            //if (Zasoba == 0) { Svycarovac.IsChecked = true; Svycarovac.IsChecked = false; }
            HracNaTahu *= -1;
        }
        public void UNCH(object sender, EventArgs e)
        {
            HracNaTahu *= -1;
        }
        public void ZkontrolujJestliKoministaNeboLyberalNevyhral(int hrac, int x, int y)
        {
            for (int a = 0; a < 11; a++) for (int b = 0; b < 11; b++)
                {
                    if (a == x) if (b == y) continue;
                    int dx = a - x;
                    int dy = b - y;
                    if (pole_poli[x + dx, y + dy].Status == hrac) if (x + dx - dy >= 0) if (x + dx - dy <= 10) if (y + dy + dx >= 0) if (y + dy + dx <= 10) if (pole_poli[x + dx - dy, y + dy + dx].Status == hrac) if (x - dy >= 0) if (x - dy <= 10) if (y + dx >= 0) if (y + dx <= 10) if (pole_poli[x - dy, y + dx].Status == hrac)
                                                            {
                                                                if (hrac == 1) MessageBox.Show("Vyhrál komínista.");
                                                                else MessageBox.Show("Vyhrál lyberál.");
                                                                this.Close();
                                                            }
                }
        }
    }
    public class Pole
    {
        public int Status;
        int X;
        int Y;
        public Label Gombik;
        public MainWindow MW;
        public Pole(int i, MainWindow mw)
        {
            if (i != 0) if (i != 10) if (i != 110) if (i != 120)
                        {
                            X = i / 11;
                            Y = i % 11;
                            Status = 0;
                            Gombik = new Label();
                            Grid.SetRow(Gombik, X);
                            Grid.SetColumn(Gombik, Y);
                            Gombik.MouseUp += Klyk;
                            Gombik.BorderThickness = new Thickness(2);
                            Gombik.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                            MW = mw;
                            MW.MrizJakNaPankraci.Children.Add(Gombik);
                        }
        }
        public void Klyk(object sender, EventArgs e)
        {
            if (Status == 0)
            {
                Status = MW.HracNaTahu;
                MW.ZmenHrace(false);
                Color c = Color.FromRgb(0, 0, 0);
                switch (Status)
                {
                    case 1:
                        c = Color.FromRgb(255, 0, 0);
                        MW.ZkontrolujJestliKoministaNeboLyberalNevyhral(1, X, Y);
                        break;
                    case 2:
                        c = Color.FromRgb(0, 0, 255);
                        MW.ZkontrolujJestliKoministaNeboLyberalNevyhral(2, X, Y);
                        break;
                    case -1:
                        c = Color.FromRgb(127, 127, 127);
                        break;
                    case -2:
                        c = Color.FromRgb(127, 127, 127);
                        break;
                }
                Gombik.Background = new SolidColorBrush(c);
            }
            else switch (Status)
                {
                    case 1:
                        MessageBox.Show("Tam už není moc místa, tam už je komínista.");
                        break;
                    case 2:
                        MessageBox.Show("Tam už hrál lyberál.");
                        break;
                    case -1:
                        MessageBox.Show("Do tohoto místa dal komínista švýcarské pole.");
                        break;
                    case -2:
                        MessageBox.Show("Tady dal lyberál švýcarské pole.");
                        break;
                }
        }
    }
}