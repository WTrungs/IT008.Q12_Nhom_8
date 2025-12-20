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
using TetrisApp.Views;

namespace TetrisApp {
    public partial class MainWindow : Window {
        private Action<bool>? _overlayClosedCallback;
        private IInputElement? _previousFocus;

        public MainWindow() {
            InitializeComponent();
            MainFrame.Navigate(new LoginPage());
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (OverlayLayer.Visibility == Visibility.Visible) return;

            if (e.Key == Key.Escape) {
                Application.Current.Shutdown();
            }
        }

        public void ShowOverlay(string title, string message, bool showCancel = false, Action<bool>? onClosed = null) {
            _overlayClosedCallback = onClosed;

            OverlayTitleText.Text = title;
            OverlayMessageText.Text = message;
            OverlayCancelButton.Visibility = showCancel ? Visibility.Visible : Visibility.Collapsed;

            _previousFocus = Keyboard.FocusedElement;

            MainFrame.IsHitTestVisible = false;

            OverlayLayer.IsHitTestVisible = true;
            OverlayLayer.Visibility = Visibility.Visible;

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(() => {
                Keyboard.Focus(OverlayLayer);
                OverlayLayer.Focus();
            }));
        }

        private void CloseOverlay(bool result) {
            OverlayLayer.IsHitTestVisible = false;
            OverlayLayer.Visibility = Visibility.Collapsed;

            MainFrame.IsHitTestVisible = true;

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(() => {
                if (_previousFocus is UIElement el && el.IsVisible && el.Focusable)
                    el.Focus();
                else
                    MainFrame.Focus();

                _previousFocus = null;
            }));

            var cb = _overlayClosedCallback;
            _overlayClosedCallback = null;
            cb?.Invoke(result);
        }

        private void OverlayOkButton_Click(object sender, RoutedEventArgs e) => CloseOverlay(true);
        private void OverlayCancelButton_Click(object sender, RoutedEventArgs e) => CloseOverlay(false);

        private void OverlayLayer_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (OverlayLayer.Visibility != Visibility.Visible) return;

            if (e.Key == Key.Escape) {
                e.Handled = true;
                CloseOverlay(false);
            }
        }
    }
}