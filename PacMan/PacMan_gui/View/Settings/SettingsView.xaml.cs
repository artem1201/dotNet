using System;
using System.Windows;

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
    }
}