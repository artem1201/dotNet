using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PacMan_model.util;

namespace PacMan_gui.View.Level {
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView {
        public event EventHandler GameViewSizeChanged;

        private Window _windowOfGameView;

        public GameView() {
            InitializeComponent();
            _windowOfGameView = Window.GetWindow(this);
        }

        public Canvas GetGameFieldCanvas() {
            return GameCanvas;
        }

        public void AddKeyBinding(KeyBinding binding) {
            _windowOfGameView = Window.GetWindow(this);

            Application.Current.Dispatcher.Invoke(
                () => {
                    if (null != _windowOfGameView) {
                        _windowOfGameView.InputBindings.Add(
                            binding);
                    }
                    else {
                        throw new ArgumentException("GameView has no window");
                    }
                });
        }

        public void RemoveBindingByKeyAndCommand(Key key, ICommand command) {
            Application.Current.Dispatcher.Invoke(
                () => {
                    if (null != _windowOfGameView) {
                        IList<KeyBinding> bindingsToRemove =
                            _windowOfGameView.InputBindings.OfType<KeyBinding>()
                                .Where(
                                    keyBinding => (keyBinding.Command.Equals(command)) && (keyBinding.Key.Equals(key)))
                                .ToList();


                        foreach (var keyBinding in bindingsToRemove) {
                            _windowOfGameView.InputBindings.Remove(keyBinding);
                        }
                    }
                    else {
                        throw new ArgumentException("GameView has no window");
                    }
                });
        }

        #region Events

        private void GameView_OnSizeChanged(object o, SizeChangedEventArgs e) {
            /*
            if (null == o) {
                throw new ArgumentNullException("o");
            }
            if (null == e) {
                throw new ArgumentNullException("e");
            }
            */


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