﻿//  author: Artem Sumanev

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using PacMan_gui.Annotations;
using PacMan_gui.View;
using PacMan_gui.View.Level;
using PacMan_gui.ViewModel.level;
using PacMan_model.level;
using PacMan_model.util;

namespace PacMan_gui.Controllers {
    internal sealed class GameController : IDirectionEventObserver {
        private static readonly string RootDir = Directory.GetCurrentDirectory();

        private readonly IDictionary<Button, Direction> _directionButtonToDirection;

        private readonly IGame _game;
        private readonly GameView _gameView;
        private readonly GameViewModel _gameViewModel;
        private readonly IDictionary<Key, Direction> _keysToDirection;

        //  is called when game is over
        //  parameters are best score and current score
        private readonly Action<int> _onGameEndCallback;
        private readonly string _pathToCompany = RootDir + "\\Company";
        private readonly string _pathToGhosts = RootDir + "\\AI";
        private readonly ISet<Key> _pauseKeys;

        #region Initialization

        public GameController(
            Action<int> onGameEndCallback,
            [NotNull] Action terminateAppAction,
            [NotNull] IDictionary<Key, Direction> keysToDirection,
            [NotNull] ISet<Key> pauseKeys) {
            if (null == terminateAppAction) {
                throw new ArgumentNullException("terminateAppAction");
            }
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

            _directionButtonToDirection = new Dictionary<Button, Direction>();
            foreach (var direction in Direction.Directions) {
                var directionButton = new Button {Name = direction.GetName(), Content = direction.GetName()};
                _directionButtonToDirection.Add(directionButton, direction);
                _gameView.ForDirectionButtonsPanel.Children.Add(directionButton);
            }

            try {
                _game = new Game(_pathToCompany, _pathToGhosts, 0);
            }
            catch (CannotPlayGameException e) {
                _gameView.MainWindow.ShowMessage(e.GetMessage(), "Error");
                terminateAppAction();
                return;
            }
            _game.RegisterOnDirectionObserver(this);
            _game.LevelFinished += OnLevelFinished;


            _gameViewModel = new GameViewModel(_game, _gameView.GetGameFieldCanvas(), OnPacmanDeath);
        }

        public void Run(int bestScore) {
            try {
                _game.NewGame(bestScore);
            }
            catch (CannotPlayGameException e) {
                _gameView.MainWindow.ShowMessage(e.GetMessage(), "Error");
                OnBackAction();
            }
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

            foreach (var directionButton in _gameView.ForDirectionButtonsPanel.Children.OfType<Button>()) {
                BindControlButton(directionButton, _onDirectionButtonCommand);
            }

            _isBinded = true;
        }

        private static void BindTextBlock(
            [NotNull] TextBlock element,
            [NotNull] Object source,
            [NotNull] string propertyName) {
            if (null == element) {
                throw new ArgumentNullException("element");
            }
            if (null == source) {
                throw new ArgumentNullException("source");
            }
            if (null == propertyName) {
                throw new ArgumentNullException("propertyName");
            }
            var binding = new Binding(propertyName) {Source = source};
            element.SetBinding(TextBlock.TextProperty, binding);
        }

        private static void BindCanvasBackGround(
            [NotNull] Canvas canvas,
            [NotNull] Object source,
            [NotNull] string propertyName,
            [NotNull] IValueConverter converter) {
            if (null == canvas) {
                throw new ArgumentNullException("canvas");
            }
            if (null == source) {
                throw new ArgumentNullException("source");
            }
            if (null == propertyName) {
                throw new ArgumentNullException("propertyName");
            }
            if (null == converter) {
                throw new ArgumentNullException("converter");
            }
            var binding = new Binding(propertyName) {Source = source, Converter = converter};
            canvas.SetBinding(Panel.BackgroundProperty, binding);
        }

        private static void BindControlButton([NotNull] ButtonBase button, [NotNull] ICommand command) {
            if (null == button) {
                throw new ArgumentNullException("button");
            }
            if (null == command) {
                throw new ArgumentNullException("command");
            }
            button.Command = command;
            button.CommandParameter = button;
        }

        private static void BindKeys(
            [NotNull] GameView gameView,
            [NotNull] ICommand command,
            [NotNull] IEnumerable<Key> keys) {
            if (null == gameView) {
                throw new ArgumentNullException("gameView");
            }
            if (null == command) {
                throw new ArgumentNullException("command");
            }
            if (null == keys) {
                throw new ArgumentNullException("keys");
            }
            foreach (var key in keys) {
                gameView.AddKeyBinding(new KeyBinding {Command = command, CommandParameter = key, Key = key});
            }
        }

        private static void UnbindKeys(
            [NotNull] GameView gameView,
            [NotNull] ICommand command,
            [NotNull] IEnumerable<Key> keys) {
            if (null == gameView) {
                throw new ArgumentNullException("gameView");
            }
            if (null == command) {
                throw new ArgumentNullException("command");
            }
            if (null == keys) {
                throw new ArgumentNullException("keys");
            }
            foreach (var key in keys) {
                gameView.RemoveBindingByKeyAndCommand(key, command);
            }
        }

        #endregion

        #region Commands

        private readonly ICommand _onBackButtonCommand;
        private readonly ICommand _onDirectionButtonCommand;

        private readonly ICommand _onDirectionKeyCommand;
        private readonly ICommand _onPauseButtonCommand;
        private readonly ICommand _onPauseKeyCommand;

        private sealed class OnBackButtonCommand : ICommand {
            private readonly Action _onBackAction;

            public OnBackButtonCommand(Action onBackAction) {
                _onBackAction = onBackAction;
            }

            public bool CanExecute(object parameter) {
                return true;
            }

            public void Execute(object parameter) {
                _onBackAction();
            }

            public event EventHandler CanExecuteChanged;
        }

        private sealed class OnDirectionButtonCommand : ICommand {
            private readonly Action<Button> _onDirectionButtonAction;

            public OnDirectionButtonCommand(Action<Button> onDirectionButtonAction) {
                _onDirectionButtonAction = onDirectionButtonAction;
            }

            public bool CanExecute(object parameter) {
                return true;
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

        private sealed class OnDirectionKeyCommand : ICommand {
            private readonly Action<Key> _onDirectionKeyAction;

            public OnDirectionKeyCommand(Action<Key> onDirectionKeyAction) {
                _onDirectionKeyAction = onDirectionKeyAction;
            }

            public bool CanExecute(object parameter) {
                return true;
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

        private sealed class OnPauseButtonCommand : ICommand {
            private readonly Action _onPauseAction;

            public OnPauseButtonCommand(Action onPauseAction) {
                _onPauseAction = onPauseAction;
            }

            public bool CanExecute(object parameter) {
                return true;
            }

            public void Execute(object parameter) {
                _onPauseAction();
            }

            public event EventHandler CanExecuteChanged;
        }

        private sealed class OnPauseKeyCommand : ICommand {
            private readonly Action _onPauseAction;

            public OnPauseKeyCommand(Action onPauseAction) {
                _onPauseAction = onPauseAction;
            }

            public bool CanExecute(object parameter) {
                return true;
            }

            public void Execute(object parameter) {
                _onPauseAction();
            }

            public event EventHandler CanExecuteChanged;
        }

        #endregion

        #region Actions

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

        public event EventHandler<DirectionChangedEventArgs> DirectionChanged;

        private void OnLevelFinished(object sender, EventArgs emptyEventArgs) {
            //  handle viewing next level and start it

            if (_game.IsFinished()) {
                _gameView.MainWindow.ShowMessage(_game.IsWon() ? "You win all levels!" : "You fail!", "Level finished");

                Dispose();
                _onGameEndCallback(_game.GetGameScore());
            }
            else {
                _gameView.MainWindow.ShowMessage("You win this level! Try next one", "Level finished");

                try {
                    _game.LoadNextLevel();
                }
                catch (CannotPlayGameException e) {
                    _gameView.MainWindow.ShowMessage(e.GetMessage(), "Error");
                    OnBackAction();
                    return;
                }
                _gameViewModel.Init(_game, _gameView.GetGameFieldCanvas());

                RedrawGame();

                _game.Start();
            }
        }

        private void OnGameViewSizeChanged(Object o, EventArgs e) {
            RedrawGame();
        }

        private void OnPacmanDeath(int livesNumber) {
            if (0 != livesNumber) {
                _gameView.MainWindow.ShowMessage("You have been died! Only " + livesNumber + " lives left");

                _game.Start();
            }
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