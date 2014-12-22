//  author: Artem Sumanev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PacMan_gui.Annotations;
using PacMan_model.util;

namespace PacMan_gui.View.Level {
    /// <summary>
    ///     Interaction logic for GameView.xaml
    /// </summary>
    public sealed partial class GameView {
        public GameView() {
            InitializeComponent();
        }

        public MainWindow MainWindow { get; private set; }
        public event EventHandler GameViewSizeChanged;

        public Canvas GetGameFieldCanvas() {
            return GameCanvas;
        }

        public void AddKeyBinding([NotNull] KeyBinding binding) {
            if (null == binding) {
                throw new ArgumentNullException("binding");
            }
            Application.Current.Dispatcher.Invoke(
                () => {
                    MainWindow.InputBindings.Add(
                        binding);
                });
        }

        public void RemoveBindingByKeyAndCommand(Key key, [NotNull] ICommand command) {
            if (null == command) {
                throw new ArgumentNullException("command");
            }
            Application.Current.Dispatcher.Invoke(
                () => {
                    IList<KeyBinding> bindingsToRemove =
                        MainWindow.InputBindings.OfType<KeyBinding>()
                            .Where(
                                keyBinding => (keyBinding.Command.Equals(command)) && (keyBinding.Key.Equals(key)))
                            .ToList();


                    foreach (var keyBinding in bindingsToRemove) {
                        MainWindow.InputBindings.Remove(keyBinding);
                    }
                });
        }

        private void GameView_OnLoaded(object sender, RoutedEventArgs e) {
            MainWindow = Window.GetWindow(this) as MainWindow;

            if (null == MainWindow) {
                throw new Exception("only window of class MainWindow is able to hadle game view");
            }
        }

        #region Events

        private void GameView_OnSizeChanged(object o, SizeChangedEventArgs e) {
            EventArgs.Empty.Raise(this, ref GameViewSizeChanged);
        }

        #endregion
    }

    public class ControlEventArs : EventArgs {
        public ControlEventArs(Key pushedKey) {
            PushedKey = pushedKey;
        }

        public Key PushedKey { get; private set; }
    }
}