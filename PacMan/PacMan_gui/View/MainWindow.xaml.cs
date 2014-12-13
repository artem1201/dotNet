using System;
using System.Windows;
using PacMan_gui.Annotations;

namespace PacMan_gui.View {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        public MainWindow() {
            InitializeComponent();
            ContentControl.Content = new MainWindowContent(this);
        }

        public void ShowMessage([NotNull] string message) {
            if (null == message) {
                throw new ArgumentNullException("message");
            }

            Application.Current.Dispatcher.Invoke(
                () => { MessageBox.Show(this, message); });
        }

        public void ShowMessage([NotNull] string message, [NotNull] string title) {
            if (null == message) {
                throw new ArgumentNullException("message");
            }
            if (null == title) {
                throw new ArgumentNullException("title");
            }
            Application.Current.Dispatcher.Invoke(
                () => { MessageBox.Show(this, message, title); });
        }
    }
}