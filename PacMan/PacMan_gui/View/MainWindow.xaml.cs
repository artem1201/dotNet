namespace PacMan_gui.View {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        
        public MainWindow() {
            InitializeComponent();
            ContentControl.Content = new MainWindowContent(this);
        }
    }
}
