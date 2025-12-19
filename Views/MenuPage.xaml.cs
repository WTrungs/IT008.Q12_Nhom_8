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
            NavigationService?.Navigate(new Uri("Views/Difficulty.xaml", UriKind.Relative));
        }
        private void SettingsButton_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(new Uri("Views/SettingsPage.xaml", UriKind.Relative));
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
        private void ContinueButton_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(new Uri("Views/GamePage.xaml", UriKind.Relative));
        }
        private void SaveGameButton_Click(object sender, RoutedEventArgs e) {
        
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(new Uri("Views/LoginPage.xaml", UriKind.Relative));
        }
        private void ChangeAvatar_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(new Uri("Views/ChangeAvatarPage.xaml", UriKind.Relative));
        }
        private void Rename_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(new Uri("Views/ScoresPage.xaml", UriKind.Relative));
        }
        private void AboutButton_Click(object sender, RoutedEventArgs e) {
            AboutOverlay.Visibility = Visibility.Visible;
        }
        private void CloseAbout_Click(object sender, RoutedEventArgs e) {
            AboutOverlay.Visibility = Visibility.Collapsed;
        }
        private void AboutOverlay_MouseDown(object sender, MouseButtonEventArgs e) {
            AboutOverlay.Visibility = Visibility.Collapsed;
        }
        private void AboutDialog_MouseDown(object sender, MouseButtonEventArgs e) {
            e.Handled = true;
        }
    }
}
