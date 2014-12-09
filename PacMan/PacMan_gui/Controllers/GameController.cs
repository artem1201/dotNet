using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using PacMan_gui.Annotations;
using PacMan_gui.View.Level;
using PacMan_gui.ViewModel.level;
using PacMan_model.level;
using PacMan_model.util;

namespace PacMan_gui.Controllers {
    internal sealed class GameController : IDirectionEventObserver {
        private static readonly string RootDir = Directory.GetCurrentDirectory();
        private readonly string _pathToCompany = RootDir + "\\Company";
        private readonly string _pathToGhosts = RootDir + "\\AI";


        private readonly IDictionary<Key, Direction> _keysToDirection;
        private readonly ISet<Key> _pauseKeys; 

        private readonly IGame _game;
        private readonly GameViewModel _gameViewModel;
        private readonly GameView _gameView;

        //  is called when game is over
        //  parameters are best score and current score
        private readonly Action<int> _onGameEndCallback;

        #region Initialization

        public GameController(/*GameView gameView,*/ Action<int> onGameEndCallback,
            [NotNull] IDictionary<Key, Direction> keysToDirection,
            [NotNull] ISet<Key> pauseKeys) {
//            if (null == gameView) {
//                throw new ArgumentNullException("gameView");
//            }
            if (null == keysToDirection) {
                throw new ArgumentNullException("keysToDirection");
            }
            if (null == pauseKeys) {
                throw new ArgumentNullException("pauseKeys");
            }

            _onGameEndCallback = onGameEndCallback;
            

            _keysToDirection = keysToDirection;
            _pauseKeys = pauseKeys;


            _gameView = new GameView(keysToDirection.Keys.Concat(pauseKeys).ToArray());
            _gameView.GameViewSizeChanged += OnGameViewSizeChanged;
            _gameView.ControlOccurs += OnControlEvent;
            _gameView.BackPressed += OnBack;


            _game = new Game(_pathToCompany, _pathToGhosts, 0);
            _game.RegisterOnDirectionObserver(this);
            _game.LevelFinished += OnLevelFinished;


            _gameViewModel = new GameViewModel(_game, _gameView.GetGameFieldCanvas(), OnPacmanDeath);
            BindDataWithGameView();
        }

        public void Run(int bestScore) {
            
            _game.NewGame(bestScore);
            _gameViewModel.Init(_game, _gameView.GameCanvas, OnPacmanDeath);

            RedrawGame();
            

            _game.Start();
        }

        #endregion

        #region Getters

        public GameView GetGameView() {
            return _gameView;
        }

        #endregion

        #region Binding

        private void BindDataWithGameView() {
            BindTextBlock(_gameView.ScoreTextBlock, _gameViewModel, "CurrentLevelScore");
            BindTextBlock(_gameView.CompanyScoreTextBlock, _gameViewModel, "CurrentScore");
            BindTextBlock(_gameView.BestScoreTextBlock, _gameViewModel, "BestScore");

            BindTextBlock(_gameView.LivesTextBlock, _gameViewModel.PacManViewModel, "LivesNumber");

            BindTextBlock(_gameView.PausedTitleTextBlock, _gameViewModel, "Paused");

            BindCanvasBackGround(
                _gameView.GetGameFieldCanvas(),
                _gameViewModel,
                "Condition",
                new ColorResolver.LevelConditionToColorConverter());
        }

        private static void BindTextBlock(TextBlock element, Object source, string propertyName) {
            var binding = new Binding(propertyName) {Source = source};
            element.SetBinding(TextBlock.TextProperty, binding);
        }

        private static void BindCanvasBackGround(
            Canvas canvas,
            Object source,
            string propertyName,
            IValueConverter converter) {
            var binding = new Binding(propertyName) {Source = source, Converter = converter};
            canvas.SetBinding(Panel.BackgroundProperty, binding);
        }

        #endregion

        #region Redrawing

        private void RedrawGame() {
            if (null != _gameViewModel) {
                _gameViewModel.Redraw();
                _gameViewModel.FieldViewModel.Redraw();
                _gameViewModel.PacManViewModel.Redraw();

                foreach (var ghostViewModel in _gameViewModel.GhostViewModels) {
                    ghostViewModel.Redraw();
                }
            }
        }

        #endregion

        #region Events

        private void OnLevelFinished(object sender, EventArgs emptyEventArgs) {
            //  handle viewing next level and start it

            if (_game.IsFinished()) {
                MessageBox.Show(_game.IsWon() ? "You win all levels!" : "You fail!", "Level finished");

                _game.Dispose();

                _onGameEndCallback(_game.GetGameScore());
            }
            else {
                MessageBox.Show("You win this level! Try next one", "Level finished");

                _game.LoadNextLevel();

                _gameViewModel.Init(_game, _gameView.GetGameFieldCanvas());

                RedrawGame();

                _game.Start();
            }
        }

        private void OnGameViewSizeChanged(Object o, EventArgs e) {
            /*
            if (null == o) {
                throw new ArgumentNullException("o");
            }
            if (null == e) {
                throw new ArgumentNullException("e");
            }
            */
            RedrawGame();
        }

        private void OnPacmanDeath(int livesNumber) {
            if (0 != livesNumber) {
                MessageBox.Show("You have been died! Only " + livesNumber + " lives left");

                _game.Start();
            }
        }

        public event EventHandler<DirectionChangedEventArgs> DirectionChanged;

        private void OnControlEvent(Object sender, ControlEventArs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            if (_keysToDirection.ContainsKey(e.PushedKey)) {
                var directionChangedArgs = new DirectionChangedEventArgs(_keysToDirection[e.PushedKey]);
                NotifyDirectionChanged(directionChangedArgs);
            }
            else if (_pauseKeys.Contains(e.PushedKey)) {
                if (_game.IsOn()) {
                    _game.Pause();
                }
                else {
                    _game.Start();
                }

                _gameViewModel.SetPaused(!_game.IsOn());
            }
            else {
                throw new ArgumentException("unknown key pushed");
            }
        }

        private void OnBack(object sender, EventArgs eventArgs) {
            _game.Dispose();
            _onGameEndCallback(-1);
        }

        private void NotifyDirectionChanged(DirectionChangedEventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            e.Raise(this, ref DirectionChanged);
        }

        #endregion
    }
}