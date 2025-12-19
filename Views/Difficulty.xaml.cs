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
using TetrisApp.Models;


namespace TetrisApp.Views
{
    public partial class Difficulty : Page
    {
        // Khai báo trình phát nhạc dùng chung cho trang này
        private MediaPlayer _clickSound = new MediaPlayer();

        public Difficulty()
        {
            InitializeComponent();
        }

        // Hàm dùng chung để phát tiếng click khi bấm nút
        private void PlayClickSound()
        {
            try
            {
                // Đường dẫn đến file âm thanh trong thư mục Assets
                string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/click.mp3");

                _clickSound.Open(new Uri(soundPath));
                _clickSound.Volume = AppSettings.SfxVolume; // Sử dụng âm lượng SFX từ cài đặt
                _clickSound.Stop(); // Đảm bảo phát lại từ đầu nếu bấm nhanh
                _clickSound.Play();
            }
            catch
            {
                // Tránh lỗi nếu file Assets/click.mp3 không tồn tại
            }
        }

        private void EasyButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound(); // Phát âm thanh click
            // Chuyển sang GamePage với tham số độ khó là "Easy"
            NavigationService?.Navigate(new Uri("Views/GamePage.xaml", UriKind.Relative), "Easy");
        }

        private void MediumButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound(); // Phát âm thanh click
            // Chuyển sang GamePage với tham số độ khó là "Medium"
            NavigationService?.Navigate(new Uri("Views/GamePage.xaml", UriKind.Relative), "Medium");
        }

        private void HardButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound(); // Phát âm thanh click
            // Chuyển sang GamePage với tham số độ khó là "Hard"
            NavigationService?.Navigate(new Uri("Views/GamePage.xaml", UriKind.Relative), "Hard");
        }
    }
}