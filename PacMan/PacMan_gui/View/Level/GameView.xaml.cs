using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PacMan_gui.Annotations;
using PacMan_model.util;

namespace PacMan_gui.View.Level {
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView {
        public event EventHandler GameViewSizeChanged;
        public event EventHandler<ControlEventArs> ControlOccurs;
        public event EventHandler BackPressed;

        private readonly ICollection<Key> _controlKeys;
//        private readonly Func<Button, Key> _controlButtonToKey;

        public GameView([NotNull] ICollection<Key> controlKeys/*, [NotNull] Func<Button, Key> controlButtonToKey*/) {
            if (null == controlKeys) {
                throw new ArgumentNullException("controlKeys");
            }
//            if (null == controlButtonToKey) {
//                throw new ArgumentNullException("controlButtonToKey");
//            }
            InitializeComponent();

            _controlKeys = controlKeys;
//            _controlButtonToKey = controlButtonToKey;
        }

        public Canvas GetGameFieldCanvas() {
            return GameCanvas;
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

        private void GameView_OnKeyDown(object sender, [NotNull] KeyEventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            if (_controlKeys.Contains(e.Key)) {
                (new ControlEventArs(e.Key)).Raise(this, ref ControlOccurs);
            }
        }

        private void ControlButton_OnClick([NotNull] object sender, [NotNull] RoutedEventArgs e) {
            if (null == sender) {
                throw new ArgumentNullException("sender");
            }
//            if (null == e) {
//                throw new ArgumentNullException("e");
//            }

            var pressedButton = sender as Button;

            if (null == pressedButton) {
                throw new ArgumentException("only buttons allowed");
            }
            //TODO:

//            (new ControlEventArs(_controlButtonToKey(pressedButton))).Raise(this, ref ControlOccurs);
        }

        private void GameView_OnLoaded(object sender, RoutedEventArgs e) {
            var window = Window.GetWindow(this);
            if (window != null) {
                window.KeyDown += GameView_OnKeyDown;
            }
            else {
                throw new Exception("window is not specified");
            }
        }

        private void PauseButton_OnClick(object sender, RoutedEventArgs e) {
            ControlButton_OnClick(sender, e);
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e) {
            EventArgs.Empty.Raise(this, ref BackPressed);
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