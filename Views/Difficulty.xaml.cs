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

namespace TetrisApp.Views {
    public partial class Difficulty : Page {
        public Difficulty() {
            InitializeComponent();
        }
        private void EasyButton_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(new Uri("Views/GamePage.xaml", UriKind.Relative), "Easy");
        }
        private void MediumButton_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(new Uri("Views/GamePage.xaml", UriKind.Relative), "Medium");
        }
        private void HardButton_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(new Uri("Views/GamePage.xaml", UriKind.Relative), "Hard");
        }
    }
}
