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
using System.Windows.Threading;
using TetrisApp.Models; 

namespace TetrisApp.Views {
    public partial class LoginPage : Page {
        // Khai báo trình phát nhạc dùng chung cho trang Login
        private MediaPlayer _clickSound = new MediaPlayer();

        public LoginPage() {
            InitializeComponent();
        }
        private void LoginPage_Loaded(object sender, RoutedEventArgs e) {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            {
                Keyboard.Focus(RootGrid);   
                RootGrid.Focus();
            }));
        }        // Hàm phát tiếng click
        private void PlayClickSound() {
            try {
                // Đường dẫn đến file âm thanh trong thư mục Assets
                string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/click.mp3");

                _clickSound.Open(new Uri(soundPath));
                _clickSound.Volume = AppSettings.SfxVolume; // Lấy âm lượng SFX từ cài đặt
                _clickSound.Stop();
                _clickSound.Play();
            }
            catch {
                // Tránh lỗi nếu thiếu file Assets/click.mp3
            }
        }

        // Nút Login
        private void LoginButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound(); // Phát tiếng click
            NavigationService?.Navigate(new MenuPage());
        }

        // Nút Continue as Guest (Đã sửa tên hàm cho khớp với XAML của bạn)
        private void ContinueAsGuestButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound(); // Phát tiếng click
            NavigationService?.Navigate(new MenuPage());
        }

        // Link quên mật khẩu
        private void ForgotPasswordLink_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            // Thêm logic xử lý tại đây nếu cần
        }

        // Link đăng ký
        private void SignUpLink_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            // Thêm logic xử lý tại đây nếu cần
        }
    }
}