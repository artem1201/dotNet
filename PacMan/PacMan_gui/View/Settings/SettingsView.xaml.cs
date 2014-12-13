using System;
using System.Windows;
using System.Windows.Input;
using PacMan_gui.Annotations;

namespace PacMan_gui.View.Settings {
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView {
        public MainWindow MainWindow { get; private set; }

        public SettingsView() {
            InitializeComponent();
        }

        private void SettingsView_OnLoaded(object sender, RoutedEventArgs e) {
            MainWindow = Window.GetWindow(this) as MainWindow;

            if (null == MainWindow) {
                throw new Exception("only window of class MainWindow is able to hadle champions table");
            }
        }

        private Action<Key> _onKeyPressedAction;
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

            _onKeyPressedAction(e.Key);
        }
    }
}