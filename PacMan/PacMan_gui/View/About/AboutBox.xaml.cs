using System;
using System.Windows;

namespace PacMan_gui.View.About {
    /// <summary>
    ///     Interaction logic for About.xaml
    /// </summary>
    public sealed partial class AboutBox {
        private readonly Action _onExitAction;

        public AboutBox(Action onExitAction) {
            InitializeComponent();

            _onExitAction = onExitAction;
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e) {
            _onExitAction();
        }
    }
}