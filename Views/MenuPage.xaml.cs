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
    public partial class MenuPage : Page {
        public MenuPage() {
            InitializeComponent();
        }
        private void NewGameButton_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(new GamePage());
        }
        private void SettingsButton_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(new SettingsPage());
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void BackToLoginButton_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(new LoginPage());
        }
    }
}
