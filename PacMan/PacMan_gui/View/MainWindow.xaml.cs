//  author: Artem Sumanev

using System;
using System.Windows;
using PacMan_gui.Annotations;

namespace PacMan_gui.View {
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        public MainWindow() {
            InitializeComponent();



            try {
                ContentControl.Content = new MainWindowContent(this);
            }
            catch (Exception) {
                this.ShowMessage("Unknown error occurs");
                Close();
            }
        }
    }

    public static class WindowUtil {
        public static void ShowMessage(this Window window, [NotNull] string message) {
            if (null == message) {
                throw new ArgumentNullException("message");
            }

            if (null != window) {
                Application.Current.Dispatcher.Invoke(
                    () => { MessageBox.Show(window, message); });
            }
            else {
                Application.Current.Dispatcher.Invoke(
                    () => { MessageBox.Show(message); });
            }
        }

        public static void ShowMessage(this Window window, [NotNull] string message, [NotNull] string title) {
            if (null == message) {
                throw new ArgumentNullException("message");
            }
            if (null == title) {
                throw new ArgumentNullException("title");
            }

            if (null != window) {
                Application.Current.Dispatcher.Invoke(
                    () => { MessageBox.Show(window, message, title); });
            }
            else {
                MessageBox.Show(message, title);
            }
        }
    }
}