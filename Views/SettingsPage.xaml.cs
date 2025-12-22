using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using TetrisApp.Models;
using TetrisApp.Services;

namespace TetrisApp.Views {
    public partial class SettingsPage : Page {
        private readonly MediaPlayer _clickSound = new MediaPlayer();

        private SettingsSnapshot _initial;
        private bool _isApplying;

        // Chỉ bật khi mày nhấn Enter ở TrackCombo (mở dropdown)
        private bool _isTrackTabMode;

        // 1. Dependency Property: Công tắc Hover
        public bool IsHoverEnabled {
            get { return (bool)GetValue(IsHoverEnabledProperty); }
            set { SetValue(IsHoverEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsHoverEnabledProperty =
            DependencyProperty.Register("IsHoverEnabled", typeof(bool), typeof(SettingsPage), new PropertyMetadata(true));

        public SettingsPage() {
            InitializeComponent();
        }

        // 2) LOAD: không focus vào control nào; Tab lần đầu mới vào MusicToggle
        private void SettingsPage_Loaded(object sender, RoutedEventArgs e) {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => {
                // Snapshot để Cancel revert
                _initial = new SettingsSnapshot {
                    IsMusicEnabled = AppSettings.IsMusicEnabled,
                    MusicVolume = AppSettings.MusicVolume,
                    SfxVolume = AppSettings.SfxVolume,
                    SelectedTrack = AppSettings.SelectedTrack ?? ""
                };

                _isApplying = true;

                if (MusicToggle != null) MusicToggle.IsChecked = _initial.IsMusicEnabled;
                if (MusicVolumeSlider != null) MusicVolumeSlider.Value = _initial.MusicVolume;
                if (SfxVolumeSlider != null) SfxVolumeSlider.Value = _initial.SfxVolume;

                if (TrackCombo != null) {
                    SetTrackSelectionByName(_initial.SelectedTrack);
                    TrackCombo.IsDropDownOpen = false;
                }

                _isTrackTabMode = false;
                _isApplying = false;

                // Không focus vào gì cả (nhưng vẫn nhận Tab)
                Keyboard.ClearFocus();
                RootGrid.Focus();
                Keyboard.Focus(RootGrid);
            }));
        }

        // ===== LIVE APPLY (feel thay đổi liền) =====
        // XAML đang gắn cho CheckBox + ComboBox
        private void SettingControl_Changed(object sender, RoutedEventArgs e) {
            if (_isApplying) return;
            ApplyLiveFromUI();
        }

        // XAML đang gắn cho Slider.ValueChanged
        private void SettingControl_Changed(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (_isApplying) return;
            ApplyLiveFromUI();
        }

        private void ApplyLiveFromUI() {
            // Update runtime ngay (feel liền) — nhưng chưa SaveUserData cho tới khi Accept
            AppSettings.IsMusicEnabled = MusicToggle?.IsChecked ?? true;
            AppSettings.MusicVolume = MusicVolumeSlider?.Value ?? 1.0;
            AppSettings.SfxVolume = SfxVolumeSlider?.Value ?? 1.0;

            if (TrackCombo?.SelectedItem is ComboBoxItem selectedItem)
                AppSettings.SelectedTrack = selectedItem.Content?.ToString() ?? "";

            if (Application.Current is App myApp) {
                myApp.UpdateBackgroundMusic();
            }
        }

        // ===== TAB/ENTER LOGIC =====
        private void Page_PreviewKeyDown(object sender, KeyEventArgs e) {
            // Tắt hover khi dùng keyboard điều hướng
            if (e.Key == Key.Down || e.Key == Key.Up ||
                e.Key == Key.Left || e.Key == Key.Right ||
                e.Key == Key.Tab) {
                IsHoverEnabled = false;
            }

            var currentFocus = Keyboard.FocusedElement;

            // Nếu chưa focus vào control nào: Tab/Up/Down -> nhảy vào MusicToggle (TabIndex=0)
            if (currentFocus == null || currentFocus == this || currentFocus is Frame || currentFocus == RootGrid) {
                if (e.Key == Key.Tab || e.Key == Key.Down || e.Key == Key.Up) {
                    MusicToggle?.Focus();
                    e.Handled = true;
                    return;
                }
            }

            // Enter
            if (e.Key == Key.Enter) {
                if (currentFocus == MusicToggle && MusicToggle != null) {
                    MusicToggle.IsChecked = !(MusicToggle.IsChecked ?? false);
                    PlayClickSound();
                    ApplyLiveFromUI();
                    e.Handled = true;
                    return;
                }

                if (currentFocus == TrackCombo && TrackCombo != null) {
                    TrackCombo.IsDropDownOpen = !TrackCombo.IsDropDownOpen;
                    _isTrackTabMode = TrackCombo.IsDropDownOpen; // bật track-mode khi Enter mở

                    if (_isTrackTabMode && TrackCombo.SelectedIndex < 0 && TrackCombo.Items.Count > 0)
                        TrackCombo.SelectedIndex = 0;

                    PlayClickSound();
                    ApplyLiveFromUI();
                    e.Handled = true;
                    return;
                }
            }

            // Toggle checkbox bằng trái/phải
            if (currentFocus == MusicToggle && MusicToggle != null) {
                if (e.Key == Key.Left) {
                    MusicToggle.IsChecked = false;
                    ApplyLiveFromUI();
                    e.Handled = true;
                    return;
                }
                if (e.Key == Key.Right) {
                    MusicToggle.IsChecked = true;
                    ApplyLiveFromUI();
                    e.Handled = true;
                    return;
                }
            }

            // Nếu đang ở TrackCombo và dropdown đang mở (track-mode) thì để TrackCombo tự xử lý Tab/Up/Down/Enter
            if (TrackCombo != null && TrackCombo.IsDropDownOpen) {
                return;
            }
            if (e.Key == Key.Down) {
                e.Handled = true;
                MoveFocus(FocusNavigationDirection.Next);
            }
            else if (e.Key == Key.Up) {
                e.Handled = true;
                MoveFocus(FocusNavigationDirection.Previous);
            }
        }

        // Track: Enter mở dropdown rồi Tab sẽ chạy qua các track; không Enter thì Tab nhảy xuống Cancel/Accept (mặc định WPF)
        private void TrackCombo_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (TrackCombo == null) return;

            int count = TrackCombo.Items.Count;

            // ENTER: mở dropdown (lần 1) / đóng dropdown (lần 2)
            if (e.Key == Key.Enter) {
                e.Handled = true;

                if (!TrackCombo.IsDropDownOpen) {
                    // Enter lần 1: mở và bật track-mode
                    TrackCombo.IsDropDownOpen = true;
                    _isTrackTabMode = true;

                    if (TrackCombo.SelectedIndex < 0 && count > 0)
                        TrackCombo.SelectedIndex = 0;

                    PlayClickSound();
                    ApplyLiveFromUI();
                    TrackCombo.Focus();
                    return;
                }
                else {
                    // Enter lần 2: chốt selection và đóng
                    TrackCombo.IsDropDownOpen = false;
                    _isTrackTabMode = false;

                    PlayClickSound();
                    ApplyLiveFromUI();
                    TrackCombo.Focus();
                    return;
                }
            }

            // ESC: đóng dropdown và thoát track-mode
            if (e.Key == Key.Escape && TrackCombo.IsDropDownOpen) {
                e.Handled = true;
                TrackCombo.IsDropDownOpen = false;
                _isTrackTabMode = false;
                TrackCombo.Focus();
                return;
            }

            // Nếu dropdown đóng vì lý do khác (click chuột), thoát track-mode
            if (!TrackCombo.IsDropDownOpen) {
                _isTrackTabMode = false;
                return;
            }

            // Track-mode + dropdown mở: TAB / UP / DOWN sẽ cycle trong list (wrap vòng)
            if (_isTrackTabMode && TrackCombo.IsDropDownOpen) {
                if (count <= 0) return;

                if (e.Key == Key.Tab) {
                    e.Handled = true;
                    int dir = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? -1 : 1;
                    CycleTrackSelection(dir, count);
                    return;
                }

                if (e.Key == Key.Up || e.Key == Key.Down) {
                    e.Handled = true;
                    int dir = (e.Key == Key.Down) ? 1 : -1;
                    CycleTrackSelection(dir, count);
                    return;
                }
            }
        }

        private void CycleTrackSelection(int dir, int count) {
            int cur = TrackCombo.SelectedIndex;
            if (cur < 0) cur = 0;

            int next = (cur + dir) % count;
            if (next < 0) next += count;

            TrackCombo.SelectedIndex = next;

            // feel liền
            ApplyLiveFromUI();

            // giữ focus ở combo (khỏi bị nhảy)
            TrackCombo.Focus();
        }

        private void MoveFocus(FocusNavigationDirection direction) {
            var focusedElement = Keyboard.FocusedElement;
            if (focusedElement is UIElement uiElement)
                uiElement.MoveFocus(new TraversalRequest(direction));
            else if (focusedElement is FrameworkContentElement contentElement)
                contentElement.MoveFocus(new TraversalRequest(direction));
        }

        // ===== SOUND =====
        private void PlayClickSound() {
            try {
                string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "click.mp3");
                _clickSound.Open(new Uri(soundPath));
                _clickSound.Volume = AppSettings.SfxVolume;
                _clickSound.Stop();
                _clickSound.Play();
            }
            catch { }
        }

        // ===== ACCEPT / CANCEL =====
        private async void Accept_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();

            // Lưu cái đang chỉnh (đã apply live rồi, nhưng accept mới gọi save)
            ApplyLiveFromUI();

            await SupabaseService.SaveUserData();
            NavigationService?.Navigate(new MenuPage());
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();

            // Revert runtime + UI về snapshot ban đầu
            _isApplying = true;

            AppSettings.IsMusicEnabled = _initial.IsMusicEnabled;
            AppSettings.MusicVolume = _initial.MusicVolume;
            AppSettings.SfxVolume = _initial.SfxVolume;
            AppSettings.SelectedTrack = _initial.SelectedTrack;

            if (MusicToggle != null) MusicToggle.IsChecked = _initial.IsMusicEnabled;
            if (MusicVolumeSlider != null) MusicVolumeSlider.Value = _initial.MusicVolume;
            if (SfxVolumeSlider != null) SfxVolumeSlider.Value = _initial.SfxVolume;
            if (TrackCombo != null) {
                SetTrackSelectionByName(_initial.SelectedTrack);
                TrackCombo.IsDropDownOpen = false;
            }

            _isTrackTabMode = false;
            _isApplying = false;

            if (Application.Current is App myApp)
                myApp.UpdateBackgroundMusic();

            NavigationService?.Navigate(new MenuPage());
        }

        // ===== helpers =====
        private void SetTrackSelectionByName(string trackName) {
            if (TrackCombo == null) return;

            int found = -1;

            for (int i = 0; i < TrackCombo.Items.Count; i++) {
                if (TrackCombo.Items[i] is ComboBoxItem cbi) {
                    string name = cbi.Content?.ToString() ?? "";
                    if (string.Equals(name, trackName, StringComparison.OrdinalIgnoreCase)) {
                        found = i;
                        break;
                    }
                }
            }

            TrackCombo.SelectedIndex = found >= 0 ? found : 0;
        }

        private struct SettingsSnapshot {
            public bool IsMusicEnabled;
            public double MusicVolume;
            public double SfxVolume;
            public string SelectedTrack;
        }
    }
}