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

        private static readonly IList<Key> ControlKeys = new List<Key> {
            Key.Up
            , Key.Down
            , Key.Left
            , Key.Right
            
            , Key.P
            , Key.Space

            , Key.W
            , Key.A
            , Key.S
            , Key.D
        };

        private readonly IDictionary<string, Key> _buttonNameToControlKey;

        public event EventHandler GameViewSizeChanged;
        public event EventHandler<ControlEventArs> ControlOccurs;
        public event EventHandler BackPressed;
        
        public GameView() {
            InitializeComponent();

            _buttonNameToControlKey = new Dictionary<string, Key> {
                {UpButton.Name, Key.Up},
                {DownButton.Name, Key.Down},
                {LeftButton.Name, Key.Left},
                {RightButton.Name, Key.Right},
                {PauseButton.Name, Key.P}
            };
        }

        public Canvas GetGameFieldCanvas() {
            return GameCanvas;
        }

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

            if (ControlKeys.Contains(e.Key)) {

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
            if (false == _buttonNameToControlKey.ContainsKey(pressedButton.Name)) {
                throw new ArgumentException("unknown button clicked");
            }

            (new ControlEventArs(_buttonNameToControlKey[pressedButton.Name])).Raise(this, ref ControlOccurs);
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
    }

    public class ControlEventArs : EventArgs {

        public ControlEventArs(Key pushedKey) {

            PushedKey = pushedKey;
        }

        public Key PushedKey { get; private set; }
    }
}
