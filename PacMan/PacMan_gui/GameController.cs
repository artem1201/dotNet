using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using PacMan_gui.View.Level;
using PacMan_gui.ViewModel.level;
using PacMan_model.level;
using PacMan_model.util;

namespace PacMan_gui {
    class GameController : IDirectionEventObserver {

        //TODO: deal with paths
        private const string PathToCompany = "C:\\Users\\artem\\Documents\\Visual Studio 2012\\Projects\\PacMan\\Company";

        private static readonly IDictionary<Key, Direction> KeyToDirection;
        private static readonly ISet<Key> PauseKeys;
        private Brush _canvasColorBeforePause;

        static GameController()  {
            KeyToDirection = new Dictionary<Key, Direction> {
                {Key.W, Direction.Down},
                {Key.A, Direction.Left},
                {Key.S, Direction.Up},
                {Key.D, Direction.Right},
                {Key.Up, Direction.Down},
                {Key.Left, Direction.Left},
                {Key.Down, Direction.Up},
                {Key.Right, Direction.Right}
            };

            PauseKeys = new HashSet<Key> {Key.P, Key.Space};
        }

        private IGame _game;
        private GameViewModel _gameViewModel;
        private readonly GameView _gameView;


        public GameController(GameView gameView) {
            if (null == gameView) {
                throw new ArgumentNullException("gameView");
            }

            _gameView = gameView;
            _gameView.GameViewSizeChanged += OnGameViewSizeChanged;
        }

        public void Run() {
            _game = new Game(PathToCompany, "", 0);
            _game.RegisterOnDirectionObserver(this);
            _game.LevelFinished += OnLevelFinished;

            _gameViewModel = new GameViewModel(_game, _gameView.GetGameFieldCanvas());
            _gameViewModel.Redraw();

            BindDataWithGameView();

            _gameView.KeyPushed += OnKeyPushed;

            _game.Start();
        }

        private void OnLevelFinished(object sender, LevelFinishedEventArs levelFinishedEventArs) {

            //  handle viewing next level and start it
            if (levelFinishedEventArs.HasNextLevel) {
                MessageBox.Show("You win this level! Try next one", "Level finished");

                _gameViewModel.Redraw();

                _game.Start();
            }
            else {
                MessageBox.Show(_game.IsWon() ? "You win all levels!" : "You fail!", "Level finished");

                //TODO: championship and go to main window
            }
        }

        private void BindDataWithGameView() {

            BindTextBlock(_gameView.ScoreTextBlock, _gameViewModel, "CurrentLevelScore");
            BindTextBlock(_gameView.CompanyScoreTextBlock, _gameViewModel, "CurrentScore");
            BindTextBlock(_gameView.BestScoreTextBlock, _gameViewModel, "BestScore");
            BindTextBlock(_gameView.LivesTextBlock, _gameViewModel.PacManViewModel, "LivesNumber");

            BindCanvasBackGround(_gameView.GetGameFieldCanvas(), _gameViewModel, "Condition", new ColorResolver.LevelConditionToColorConverter());
        }

        private static void BindTextBlock(TextBlock element, Object source, string propertyName) {
            var binding = new Binding(propertyName) {Source = source};
            element.SetBinding(TextBlock.TextProperty, binding);
        }

        private static void BindCanvasBackGround(Canvas canvas, Object source, string propertyName, IValueConverter converter) {
            
            var binding = new Binding(propertyName) { Source = source, Converter = converter};
            canvas.SetBinding(Panel.BackgroundProperty, binding);
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
            if (null != _gameViewModel) {
                _gameViewModel.Redraw();
                _gameViewModel.FieldViewModel.Redraw();
                _gameViewModel.PacManViewModel.Redraw();
                
                foreach (var ghostViewModel in _gameViewModel.GhostViewModels) {
                    ghostViewModel.Redraw();
                }
            }
        }

        public event EventHandler<DirectionChangedEventArgs> DirectionChanged;

        private void OnKeyPushed(Object sender, EventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            var eventArgs = e as KeyEventArgs;
            if (null == eventArgs) {
                throw new ArgumentException("e");
            }

            if (KeyToDirection.ContainsKey(eventArgs.Key)) {
                var directionChangedArgs = new DirectionChangedEventArgs(KeyToDirection[eventArgs.Key]);
                NotifyDirectionChanged(directionChangedArgs);
            }
            else if (PauseKeys.Contains(eventArgs.Key)) {

                if (_game.IsOn()) {
                    _game.Pause();

                    _canvasColorBeforePause = _gameView.GetGameFieldCanvas().Background;
                    _gameView.GetGameFieldCanvas().Background = ColorResolver.PauseColor;
                }
                else {
                    _game.Start();
                    _gameView.GetGameFieldCanvas().Background = _canvasColorBeforePause;
                }
            }
            else {
                throw new ArgumentException("unknown key pushed");
            }
        }
        
        protected virtual void NotifyDirectionChanged(DirectionChangedEventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }
            var temp = Volatile.Read(ref DirectionChanged);

            if (temp != null) temp(this, e);
        }


    }
}
