using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

        private readonly IDictionary<Button, Direction> _directionButtonToDirection;

        private readonly IGame _game;
        private readonly GameViewModel _gameViewModel;
        private readonly GameView _gameView;

        //  is called when game is over
        //  parameters are best score and current score
        private readonly Action<int> _onGameEndCallback;

        #region Initialization

        public GameController(
            Action<int> onGameEndCallback,
            [NotNull] IDictionary<Key, Direction> keysToDirection,
            [NotNull] ISet<Key> pauseKeys) {
            if (null == keysToDirection) {
                throw new ArgumentNullException("keysToDirection");
            }
            if (null == pauseKeys) {
                throw new ArgumentNullException("pauseKeys");
            }

            _onGameEndCallback = onGameEndCallback;


            _keysToDirection = keysToDirection;
            _pauseKeys = pauseKeys;


            _gameView = new GameView();
            _gameView.GameViewSizeChanged += OnGameViewSizeChanged;


            _onBackButtonCommand = new OnBackButtonCommand(OnBackAction);

            _onPauseButtonCommand = new OnPauseButtonCommand(DoPause);
            _onDirectionButtonCommand = new OnDirectionButtonCommand(OnDirectionButtonAction);

            _onPauseKeyCommand = new OnPauseKeyCommand(DoPause);
            _onDirectionKeyCommand = new OnDirectionKeyCommand(OnDirectionKeyAction);

            _stopableCommands = new[] {
                _onBackButtonCommand,
                _onPauseButtonCommand,
                _onDirectionButtonCommand,
                _onPauseKeyCommand,
                _onDirectionKeyCommand
            };

            //TODO: hardcoded directions and button
            //add buttons for directions dynamicly
            _directionButtonToDirection = new Dictionary<Button, Direction> {
                {_gameView.UpButton, Direction.Directions[Direction.Down]},
                {_gameView.DownButton, Direction.Directions[Direction.Up]},
                {_gameView.LeftButton, Direction.Directions[Direction.Left]},
                {_gameView.RightButton, Direction.Directions[Direction.Right]}
            };

            _game = new Game(_pathToCompany, _pathToGhosts, 0);
            _game.RegisterOnDirectionObserver(this);
            _game.LevelFinished += OnLevelFinished;


            _gameViewModel = new GameViewModel(_game, _gameView.GetGameFieldCanvas(), OnPacmanDeath);
        }

        public void Run(int bestScore) {
            _game.NewGame(bestScore);
            _gameViewModel.Init(_game, _gameView.GameCanvas, OnPacmanDeath);

            BindDataWithGameView();

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

        private bool _isBinded;

        private void BindDataWithGameView() {
            BindKeys(_gameView, _onPauseKeyCommand, _pauseKeys);
            BindKeys(_gameView, _onDirectionKeyCommand, _keysToDirection.Keys);

            if (_isBinded) {
                return;
            }

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

            BindControlButton(_gameView.PauseButton, _onPauseButtonCommand);
            BindControlButton(_gameView.BackButton, _onBackButtonCommand);

            //TODO: hardcode again
            BindControlButton(_gameView.UpButton, _onDirectionButtonCommand);
            BindControlButton(_gameView.DownButton, _onDirectionButtonCommand);
            BindControlButton(_gameView.LeftButton, _onDirectionButtonCommand);
            BindControlButton(_gameView.RightButton, _onDirectionButtonCommand);

            _isBinded = true;
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

        private static void BindControlButton(ButtonBase button, ICommand command) {
            button.Command = command;
            button.CommandParameter = button;
        }

        private static void BindKeys(GameView gameView, ICommand command, IEnumerable<Key> keys) {
            foreach (var key in keys) {
                gameView.AddKeyBinding(new KeyBinding {Command = command, CommandParameter = key, Key = key});
            }
        }

        private static void UnbindKeys(GameView gameView, ICommand command, IEnumerable<Key> keys) {
            foreach (var key in keys) {
                gameView.RemoveBindingByKeyAndCommand(key, command);
            }
        }

        #endregion

        #region Commands

        private readonly IStopableCommand _onPauseButtonCommand;
        private readonly IStopableCommand _onBackButtonCommand;
        private readonly IStopableCommand _onDirectionButtonCommand;

        private readonly IStopableCommand _onPauseKeyCommand;
        private readonly IStopableCommand _onDirectionKeyCommand;

        private readonly IStopableCommand[] _stopableCommands;

        private void StopCommands() {
            foreach (var stopableCommand in _stopableCommands) {
                stopableCommand.SetCanExecute(false);
            }
        }

        private void StartCommands() {
            foreach (var stopableCommand in _stopableCommands) {
                stopableCommand.SetCanExecute(true);
            }
        }

        private interface IStopableCommand : ICommand {
            void SetCanExecute(bool canExecute);
        }

        private class OnBackButtonCommand : IStopableCommand {
            private bool _canExecute;
            private readonly Action _onBackAction;

            public OnBackButtonCommand(Action onBackAction, bool canExecute = true) {
                _onBackAction = onBackAction;
                _canExecute = canExecute;
            }

            public void SetCanExecute(bool canExecute) {
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) {
                return _canExecute;
            }

            public void Execute(object parameter) {
                _onBackAction();
            }

            public event EventHandler CanExecuteChanged;
        }

        private class OnPauseButtonCommand : IStopableCommand {
            private bool _canExecute;
            private readonly Action _onPauseAction;

            public OnPauseButtonCommand(Action onPauseAction, bool canExecute = true) {
                _onPauseAction = onPauseAction;
                _canExecute = canExecute;
            }

            public void SetCanExecute(bool canExecute) {
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) {
                return _canExecute;
            }

            public void Execute(object parameter) {
                _onPauseAction();
            }

            public event EventHandler CanExecuteChanged;
        }

        private class OnDirectionButtonCommand : IStopableCommand {
            private bool _canExecute;
            private readonly Action<Button> _onDirectionButtonAction;

            public OnDirectionButtonCommand(Action<Button> onDirectionButtonAction, bool canExecute = true) {
                _onDirectionButtonAction = onDirectionButtonAction;
                _canExecute = canExecute;
            }

            public void SetCanExecute(bool canExecute) {
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) {
                return _canExecute;
            }

            public void Execute([NotNull] object parameter) {
                if (null == parameter) {
                    throw new ArgumentNullException("parameter");
                }

                var directionButton = parameter as Button;
                if (null == directionButton) {
                    throw new ArgumentException("parameter of OnDirectionButtonCommand can be only button");
                }

                _onDirectionButtonAction(directionButton);
            }

            public event EventHandler CanExecuteChanged;
        }

        private class OnPauseKeyCommand : IStopableCommand {
            private bool _canExecute;
            private readonly Action _onPauseAction;

            public OnPauseKeyCommand(Action onPauseAction, bool canExecute = true) {
                _onPauseAction = onPauseAction;
                _canExecute = canExecute;
            }

            public void SetCanExecute(bool canExecute) {
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) {
                return _canExecute;
            }

            public void Execute(object parameter) {
                _onPauseAction();
            }

            public event EventHandler CanExecuteChanged;
        }

        private class OnDirectionKeyCommand : IStopableCommand {
            private bool _canExecute;
            private readonly Action<Key> _onDirectionKeyAction;

            public OnDirectionKeyCommand(Action<Key> onDirectionKeyAction, bool canExecute = true) {
                _onDirectionKeyAction = onDirectionKeyAction;
                _canExecute = canExecute;
            }

            public void SetCanExecute(bool canExecute) {
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) {
                return _canExecute;
            }

            public void Execute([NotNull] object parameter) {
                if (null == parameter) {
                    throw new ArgumentNullException("parameter");
                }

                if (false == parameter is Key) {
                    throw new ArgumentException("parameter of OnDirectionKeyCommand can be only Key");
                }

                _onDirectionKeyAction((Key) parameter);
            }

            public event EventHandler CanExecuteChanged;
        }

        #endregion

        #region Actions

        private void ShowMessage([NotNull] string message) {
            if (null == message) {
                throw new ArgumentNullException("message");
            }
            StopCommands();
            MessageBox.Show(message);
            StartCommands();
        }

        private void ShowMessage([NotNull] string message, [NotNull] string title) {
            if (null == message) {
                throw new ArgumentNullException("message");
            }
            if (null == title) {
                throw new ArgumentNullException("title");
            }
            StopCommands();
            MessageBox.Show(message, title);
            StartCommands();
        }

        private void OnDirectionKeyAction(Key directionKey) {
            if (_keysToDirection.ContainsKey(directionKey)) {
                NotifyDirectionChanged(new DirectionChangedEventArgs(_keysToDirection[directionKey]));
            }
            else {
                throw new ArgumentException("unknown key tries to be direction key");
            }
        }

        private void OnDirectionButtonAction([NotNull] Button directionButton) {
            if (null == directionButton) {
                throw new ArgumentNullException("directionButton");
            }

            if (_directionButtonToDirection.ContainsKey(directionButton)) {
                NotifyDirectionChanged(new DirectionChangedEventArgs(_directionButtonToDirection[directionButton]));
            }
            else {
                throw new ArgumentException("unknown button tries to be direction button");
            }
        }

        private void DoPause() {
            if (_game.IsOn()) {
                _game.Pause();
            }
            else {
                _game.Start();
            }

            _gameViewModel.SetPaused(!_game.IsOn());
        }

        private void Dispose() {
            _game.Dispose();
            UnbindKeys(_gameView, _onPauseKeyCommand, _pauseKeys);
            UnbindKeys(_gameView, _onDirectionKeyCommand, _keysToDirection.Keys);
        }

        private void OnBackAction() {
            Dispose();
            _onGameEndCallback(-1);
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
                ShowMessage(_game.IsWon() ? "You win all levels!" : "You fail!", "Level finished");

                Dispose();
                _onGameEndCallback(_game.GetGameScore());
            }
            else {
                ShowMessage("You win this level! Try next one", "Level finished");

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
                ShowMessage("You have been died! Only " + livesNumber + " lives left");

                _game.Start();
            }
        }

        public event EventHandler<DirectionChangedEventArgs> DirectionChanged;

        private void NotifyDirectionChanged(DirectionChangedEventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            e.Raise(this, ref DirectionChanged);
        }

        #endregion
    }
}