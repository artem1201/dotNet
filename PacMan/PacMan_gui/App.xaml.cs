using System.Windows;
using PacMan_gui.View.Level;

namespace PacMan_gui {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App {
        private void App_OnStartup(object sender, StartupEventArgs e) {
            var gameView = new GameView();
            var gameController = new GameController(gameView);

            

            gameView.Show();
            gameController.Run();
        }
    }
}
