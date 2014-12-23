//  author: Artem Sumanev

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using PacMan_gui.Annotations;

namespace PacMan_gui.View.Settings {
    /// <summary>
    ///     Interaction logic for SettingsView.xaml
    /// </summary>
    public sealed partial class SettingsView {
        private Action<Key> _onKeyPressedAction;

        public SettingsView() {
            InitializeComponent();
        }

        public MainWindow MainWindow { get; private set; }

        private void SettingsView_OnLoaded(object sender, RoutedEventArgs e) {
            MainWindow = Window.GetWindow(this) as MainWindow;

            if (null == MainWindow) {
                throw new Exception("only window of class MainWindow is able to hadle champions table");
            }
        }

        public void StartListenToKeys([NotNull] Action<Key> onKeyPressedAction) {
            if (null == onKeyPressedAction) {
                throw new ArgumentNullException("onKeyPressedAction");
            }
            _onKeyPressedAction = onKeyPressedAction;

            MainWindow.KeyDown += SettingsView_OnKeyDown;
        }

        public void StopListenToKeys() {
            MainWindow.KeyDown -= SettingsView_OnKeyDown;
        }

        private void SettingsView_OnKeyDown(object sender, [NotNull] KeyEventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            if (null != _onKeyPressedAction) {
                _onKeyPressedAction(e.Key);
            }
        }

        private static readonly ISet<Key> NavigatonKeys = new HashSet<Key> {
            Key.Up,
            Key.Down,
            Key.Left,
            Key.Right,
            Key.Space
        };

        private void KeysSettingsDataGrid_OnPreviewKeyDown(object sender, KeyEventArgs e) {
            if (NavigatonKeys.Contains(e.Key)) {
                e.Handled = true;
            }
            SettingsView_OnKeyDown(sender, e);
        }
    }
}