using System;
using System.Windows;
using PacMan_model.util;

namespace PacMan_gui.View.Champions {
    /// <summary>
    /// Interaction logic for ChampionsTable.xaml
    /// </summary>
    public partial class ChampionsTableView {
        public ChampionsTableView() {
            InitializeComponent();
        }

        public EventHandler ChampionsTableExit;

        private void ExitButton_OnClick(object sender, RoutedEventArgs e) {
            NotifyExit();
        }

        private void NotifyExit() {
            EventArgs.Empty.Raise(this, ref ChampionsTableExit);
        }
    }
}
