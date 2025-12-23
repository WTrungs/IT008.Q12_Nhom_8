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
        private readonly MediaPlayer clickSound = new MediaPlayer();
        private SettingsSnapshot initial;
        private bool isApplying;
        private bool isLoaded;
        // Chỉ bật khi mày nhấn Enter ở TrackCombo (mở dropdown)
        private bool isTrackTabMode;

        // Dependency Property: Công tắc Hover
        public static readonly DependencyProperty IsHoverEnabledProperty = DependencyProperty.Register("IsHoverEnabled", typeof(bool), typeof(SettingsPage), new PropertyMetadata(true));
        public bool IsHoverEnabled {
            get { return (bool)GetValue(IsHoverEnabledProperty); }
            set { SetValue(IsHoverEnabledProperty, value); }
        }

        public SettingsPage() {
            InitializeComponent();

            Loaded += SettingsPage_Loaded;
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e) {
            if (isLoaded) {
                ApplyUIFromSettings();
                return;
            }

            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => {

                initial = new SettingsSnapshot {
                    IsMusicEnabled = AppSettings.IsMusicEnabled,
                    MusicVolume = AppSettings.MusicVolume,
                    SfxVolume = AppSettings.SfxVolume,
                    SelectedTrack = AppSettings.SelectedTrack ?? ""
                };

                isApplying = true;

                if (MusicToggle != null) MusicToggle.IsChecked = initial.IsMusicEnabled;
                if (MusicVolumeSlider != null) MusicVolumeSlider.Value = initial.MusicVolume * 100.0;
                if (SfxVolumeSlider != null) SfxVolumeSlider.Value = initial.SfxVolume * 100.0;

                if (TrackCombo != null) {
                    SetTrackSelectionByName(initial.SelectedTrack);
                    TrackCombo.IsDropDownOpen = false;
                }

                isTrackTabMode = false;
                isApplying = false;
                isLoaded = true;
                Keyboard.ClearFocus();
                RootGrid.Focus();
                Keyboard.Focus(RootGrid);
            }));
        }

        // ===== LIVE APPLY =====
        private void SettingControl_Changed(object sender, RoutedEventArgs e) {
            if (isApplying) return;
            ApplyLiveFromUI();
        }

        // XAML đang gắn cho Slider.ValueChanged
        private void SettingControl_Changed(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (isApplying) return;
            ApplyLiveFromUI();
        }
        private void TrackCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (isApplying) return;
            ApplyLiveFromUI();
        }
        private void SettingControl_RoutedChanged(object sender, RoutedEventArgs e) {
            if (isApplying) return;
            ApplyLiveFromUI();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (isApplying) return;
            ApplyLiveFromUI();
        }

        private void ApplyLiveFromUI() {
            AppSettings.IsMusicEnabled = MusicToggle?.IsChecked ?? true;
            AppSettings.MusicVolume = (MusicVolumeSlider?.Value ?? 100.0) / 100.0;
            AppSettings.SfxVolume = (SfxVolumeSlider?.Value ?? 100.0) / 100.0;

            if (TrackCombo?.SelectedItem is ComboBoxItem item) {
                AppSettings.SelectedTrack = item.Tag?.ToString()?.Trim() ?? "";
            }

            if (Application.Current is App myApp) myApp.UpdateBackgroundMusic();
        }
        private void ApplyUIFromSettings() {
            isApplying = true;
            MusicToggle.IsChecked = AppSettings.IsMusicEnabled;
            MusicVolumeSlider.Value = AppSettings.MusicVolume * 100.0;
            SfxVolumeSlider.Value = AppSettings.SfxVolume * 100.0;
            isApplying = false;
        }

        // ===== TAB/ENTER LOGIC =====
        private void Page_PreviewKeyDown(object sender, KeyEventArgs e) {
            // Turn off hover when using keyboard
            if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Tab) {
                IsHoverEnabled = false;
            }

            var currentFocus = Keyboard.FocusedElement;

            if (currentFocus == null || currentFocus == this || currentFocus is Frame || currentFocus == RootGrid) {
                if (e.Key == Key.Tab || e.Key == Key.Down || e.Key == Key.Up) {
                    MusicToggle?.Focus();
                    e.Handled = true;
                    return;
                }
            }

            // Enter
            if (e.Key == Key.Enter && currentFocus == MusicToggle && MusicToggle != null) {
                MusicToggle.IsChecked = !(MusicToggle.IsChecked ?? false);
                PlayClickSound();
                ApplyLiveFromUI();
                e.Handled = true;
                return;
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
                    isTrackTabMode = true;

                    if (TrackCombo.SelectedIndex < 0 && count > 0)
                        TrackCombo.SelectedIndex = 0;

                    PlayClickSound();
                    ApplyLiveFromUI();
                    TrackCombo.Focus();
                    return;
                }
                else {
                    // Enter lần 2: selection và đóng
                    TrackCombo.IsDropDownOpen = false;
                    isTrackTabMode = false;

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
                isTrackTabMode = false;
                TrackCombo.Focus();
                return;
            }

            // If not in track-mode or dropdown closed, exit track-mode
            if (!TrackCombo.IsDropDownOpen) {
                isTrackTabMode = false;
                return;
            }

            // Track-mode + dropdown mở: TAB / UP / DOWN sẽ cycle trong list (wrap vòng)
            if (isTrackTabMode && TrackCombo.IsDropDownOpen) {
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

            ApplyLiveFromUI();

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
                clickSound.Open(new Uri(soundPath));
                clickSound.Volume = AppSettings.SfxVolume;
                clickSound.Stop();
                clickSound.Play();
            }
            catch { }
        }

        // ===== ACCEPT / CANCEL =====
        private async void Accept_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            ApplyLiveFromUI();

            var username = SupabaseService.CurrentUser?.Username; 
            LocalSettingsService.SaveFromAppSettings(username);

            NavigationService?.Navigate(new MenuPage()); 
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();

            // Revert to initial settings
            isApplying = true;

            AppSettings.IsMusicEnabled = initial.IsMusicEnabled;
            AppSettings.MusicVolume = initial.MusicVolume;
            AppSettings.SfxVolume = initial.SfxVolume;
            AppSettings.SelectedTrack = initial.SelectedTrack;

            if (MusicToggle != null) MusicToggle.IsChecked = initial.IsMusicEnabled;
            if (MusicVolumeSlider != null) MusicVolumeSlider.Value = initial.MusicVolume * 100.0;
            if (SfxVolumeSlider != null) SfxVolumeSlider.Value = initial.SfxVolume * 100.0;
            if (TrackCombo != null) {
                SetTrackSelectionByName(initial.SelectedTrack);
                TrackCombo.IsDropDownOpen = false;
            }

            isTrackTabMode = false;
            isApplying = false;

            if (Application.Current is App myApp)
                myApp.UpdateBackgroundMusic();

            NavigationService?.Navigate(new MenuPage());
        }

        // ===== helpers =====
        private void SetTrackSelectionByName(string trackKey) {
            if (TrackCombo == null) return;

            int found = -1;

            for (int i = 0; i < TrackCombo.Items.Count; i++) {
                if (TrackCombo.Items[i] is ComboBoxItem cbi) {
                    string key = cbi.Tag?.ToString() ?? "";
                    if (string.Equals(key, trackKey, StringComparison.OrdinalIgnoreCase)) {
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