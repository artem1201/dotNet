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

        private readonly IDictionary<string, Key> ButtonNameToControlKey;

        public event EventHandler GameViewSizeChanged;
        public event EventHandler<KeyEventArgs> KeyPushed; 
        
        public GameView() {
            InitializeComponent();

            ButtonNameToControlKey = new Dictionary<string, Key> {
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

                e.Raise(this, ref KeyPushed);
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
            
            //TODO: raise event
        }
    }
}
