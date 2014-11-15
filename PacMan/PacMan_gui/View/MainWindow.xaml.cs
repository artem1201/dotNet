using System.Windows;

namespace PacMan_gui.View {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        public MainWindow() {
            InitializeComponent();

        }

        private void BtnExit_OnClick(object sender, RoutedEventArgs e) {
            //TODO: add exit

            MessageBox.Show("I will exit later");
        }
    }
}
