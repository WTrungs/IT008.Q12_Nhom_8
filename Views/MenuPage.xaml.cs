using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using TetrisApp.Models;
using TetrisApp.Services;

namespace TetrisApp.Views
{
    public partial class MenuPage : Page
    {
        private MediaPlayer _clickSound = new MediaPlayer();

        public MenuPage()
        {
            InitializeComponent();
        }

        private void PlayClickSound()
        {
            try
            {
                string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/click.mp3");
                _clickSound.Open(new Uri(soundPath));
                _clickSound.Volume = AppSettings.SfxVolume;
                _clickSound.Stop();
                _clickSound.Play();
            }
            catch { }
        }

        private void MenuPage_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            {
                Keyboard.Focus(RootGrid);
                RootGrid.Focus();
            }));

            // Kiểm tra và hiển thị tên người dùng
            if (SupabaseService.CurrentUser != null)
            {
                UserNameText.Text = SupabaseService.CurrentUser.Username;
            }
            else
            {
                UserNameText.Text = "Guest";
            }
        }

        private void NewGameButton_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (HamburgerToggle.IsChecked != true) return;
            var old = e.OldFocus as DependencyObject;
            if (IsDescendantOf(old, SideMenu)) HamburgerToggle.IsChecked = false;
        }

        private static bool IsDescendantOf(DependencyObject? child, DependencyObject parent)
        {
            while (child != null)
            {
                if (ReferenceEquals(child, parent)) return true;
                child = VisualTreeHelper.GetParent(child);
            }
            return false;
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/Difficulty.xaml", UriKind.Relative));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/SettingsPage.xaml", UriKind.Relative));
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            ((MainWindow)Application.Current.MainWindow).ShowOverlay("Thoát trò chơi", "Bạn có chắc chắn muốn thoát không?", true, async (result) => {
                if (result)
                {
                    await SupabaseService.SaveUserData();
                    await System.Threading.Tasks.Task.Delay(300);
                    Application.Current.Shutdown();
                }
            });
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            if (SupabaseService.CurrentUser == null)
            {
                ((MainWindow)Application.Current.MainWindow).ShowOverlay("Lỗi", "Bạn chưa đăng nhập!");
                return;
            }
            string saveData = SupabaseService.CurrentUser.GameSaveData;
            if (string.IsNullOrEmpty(saveData))
            {
                ((MainWindow)Application.Current.MainWindow).ShowOverlay("Thông báo", "Bạn chưa có ván chơi nào được lưu.");
                return;
            }
            NavigationService?.Navigate(new GamePage(saveData));
        }

        private void SaveGameButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            ((MainWindow)Application.Current.MainWindow).ShowOverlay("Thông báo", "Game sẽ tự động lưu khi bạn thoát bàn chơi.");
        }

        // [ĐÃ SỬA] Nút Log out: Gọi hàm Logout để xóa dữ liệu
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();

            // Xóa thông tin user
            SupabaseService.Logout();

            // Về màn hình đăng nhập
            NavigationService?.Navigate(new Uri("Views/LoginPage.xaml", UriKind.Relative));
        }

        private void ChangeAvatar_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/ChangeAvatarPage.xaml", UriKind.Relative));
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/ScoresPage.xaml", UriKind.Relative));
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            AboutOverlay.Visibility = Visibility.Visible;
        }

        private void CloseAbout_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            AboutOverlay.Visibility = Visibility.Collapsed;
        }

        private void AboutOverlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AboutOverlay.Visibility = Visibility.Collapsed;
        }

        private void AboutDialog_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void HamburgerToggle_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                HamburgerToggle.IsChecked = !(HamburgerToggle.IsChecked ?? false);
                e.Handled = true;
            }
        }
    }
}