using System;
using System.Windows;
using PacMan_model.util;

namespace PacMan_gui.View.Champions {
    /// <summary>
    ///     Interaction logic for ChampionsTable.xaml
    /// </summary>
    public sealed partial class ChampionsTableView {
        public EventHandler ChampionsTableExit;

        public ChampionsTableView() {
            InitializeComponent();
        }

        public MainWindow MainWindow { get; private set; }

        private void ExitButton_OnClick(object sender, RoutedEventArgs e) {
            NotifyExit();
        }

        private void NotifyExit() {
            EventArgs.Empty.Raise(this, ref ChampionsTableExit);
        }

        private void ChampionsTableView_OnLoaded(object sender, RoutedEventArgs e) {
            MainWindow = Window.GetWindow(this) as MainWindow;

            if (null == MainWindow) {
                throw new Exception("only window of class MainWindow is able to hadle champions table");
            }
        }
    }
}