using System;
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

        public GameView() {
            InitializeComponent();
        }

        public Canvas GetGameFieldCanvas() {
            return GameCanvas;
        }

        public void AddKeyBinding(KeyBinding binding) {
            var window = Window.GetWindow(this);
            if (window != null) {
                window.InputBindings.Add(
                    binding);
            }
            else {
                throw new ArgumentException("GameView has no window");
            }
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

        private void GameView_OnKeyDown(object sender, KeyEventArgs e) {
            //throw new NotImplementedException();
        }

        private void GameView_OnLoaded(object sender, RoutedEventArgs e) {
            var window = Window.GetWindow(this);
            if (window != null) {
                window.KeyDown += GameView_OnKeyDown;
            }
        }
    }

    public class ControlEventArs : EventArgs {
        public ControlEventArs(Key pushedKey) {
            PushedKey = pushedKey;
        }

        public Key PushedKey { get; private set; }
    }
}