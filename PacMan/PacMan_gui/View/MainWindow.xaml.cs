using System.Windows;
using PacMan_gui.View.Level;

namespace PacMan_gui.View {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        public MainWindow() {
            InitializeComponent();
        }

        private void ExitButton_OnClick(object sender, RoutedEventArgs e) {
            Close();
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e) {

            //TODO: do normal screen changing in one window

            var gameView = new GameView {Background = Background};
            var gameController = new GameController(gameView);

            gameView.Show();
            gameController.Run();
        }
    }
}
