using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media; // Thư viện để dùng MediaPlayer
using System.Windows.Navigation;
using System.Windows.Threading;
using TetrisApp.Models; // Để lấy âm lượng từ AppSettings

namespace TetrisApp.Views {
    public partial class MenuPage : Page {
        // 1. Khai báo trình phát nhạc dùng chung
        private MediaPlayer _clickSound = new MediaPlayer();

        public MenuPage() {
            InitializeComponent();
        }
        // 2. Hàm dùng chung để phát tiếng click
        private void PlayClickSound() {
            try {
                // Đường dẫn tới file trong thư mục Assets
                string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/click.mp3");

                _clickSound.Open(new Uri(soundPath));
                _clickSound.Volume = AppSettings.SfxVolume; // Dùng âm lượng đã lưu
                _clickSound.Stop();
                _clickSound.Play();
            }
            catch {
                // Tránh crash nếu thiếu file âm thanh
            }
        }
        private void MenuPage_Loaded(object sender, RoutedEventArgs e) {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            {
                Keyboard.Focus(RootGrid);
                RootGrid.Focus();
            }));
        }
        private void NewGameButton_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            if (HamburgerToggle.IsChecked != true) return;

            var old = e.OldFocus as DependencyObject;
            if (IsDescendantOf(old, SideMenu)) {
                HamburgerToggle.IsChecked = false;
            }
        }
        private static bool IsDescendantOf(DependencyObject? child, DependencyObject parent) {
            while (child != null) {
                if (ReferenceEquals(child, parent)) return true;
                child = VisualTreeHelper.GetParent(child);
            }
            return false;
        }
        private void NewGameButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/Difficulty.xaml", UriKind.Relative));
        }
        private void SettingsButton_Click(object sender, RoutedEventArgs e)  {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/SettingsPage.xaml", UriKind.Relative));
        }
        private async void ExitButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            await System.Threading.Tasks.Task.Delay(300);
            Application.Current.Shutdown();
        }
        private void ContinueButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/GamePage.xaml", UriKind.Relative));
        }
        private void SaveGameButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            // Logic lưu game của bạn
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/LoginPage.xaml", UriKind.Relative));
        }
        private void ChangeAvatar_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/ChangeAvatarPage.xaml", UriKind.Relative));
        }
        private void Rename_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/ScoresPage.xaml", UriKind.Relative));
        }
        private void AboutButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            AboutOverlay.Visibility = Visibility.Visible;
        }
        private void CloseAbout_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            AboutOverlay.Visibility = Visibility.Collapsed;
        }
        private void AboutOverlay_MouseDown(object sender, MouseButtonEventArgs e) {
            AboutOverlay.Visibility = Visibility.Collapsed;
        }
        private void AboutDialog_MouseDown(object sender, MouseButtonEventArgs e) {
            e.Handled = true;
        }
        private void HamburgerToggle_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                HamburgerToggle.IsChecked = !(HamburgerToggle.IsChecked ?? false);
                e.Handled = true;
            }
        }
    }
}