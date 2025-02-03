﻿using MVVMPexeso.ViewModel;
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

namespace MVVMPexeso
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // DataContext pro hledání bindingovaných (propojených) vlastností nastavíme na náš ViewModel
            MainWindowViewModel vm = new MainWindowViewModel();
            DataContext = vm;
        }
        private string gameMode;        

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {            
            // nastavíme herní mód
            gameMode = ((RadioButton)sender).Content.ToString();
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            // zavoláme funkci ViewMOdelu, která implementuje samotnou logiku hry
            ((MainWindowViewModel)DataContext).StartGame(gameMode, pexesoGrid);
        }
    }
}