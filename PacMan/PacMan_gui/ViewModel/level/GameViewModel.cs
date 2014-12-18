using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using PacMan_gui.Annotations;
using PacMan_model.level;
using PacMan_model.level.cells.pacman;

namespace PacMan_gui.ViewModel.level {
    internal sealed class GameViewModel : INotifyPropertyChanged {
        private const string PausedMessage = "Paused";
        private const string NotPausedMessage = "";

        private int _bestScore;
        private LevelCondition _condition;
        private int _currentLevelScore;
        private int _currentScore;
        private IGame _game;
        private string _pausedMessage;

        #region Initialization

        public GameViewModel([NotNull] IGame game, [NotNull] Canvas canvas, [NotNull] Action<int> onPacmanDeathAction) {
            if (null == game) {
                throw new ArgumentNullException("game");
            }
            if (null == canvas) {
                throw new ArgumentNullException("canvas");
            }
            if (null == onPacmanDeathAction) {
                throw new ArgumentNullException("onPacmanDeathAction");
            }


            Init(game, canvas, onPacmanDeathAction);

            BestScore = game.GetBestScore();
            CurrentScore = game.GetGameScore();
            CurrentLevelScore = game.GetLevelScore();
            SetPaused(false);
        }

        public void Init([NotNull] IGame game, [NotNull] Canvas canvas, Action<int> onPacmanDeathAction = null) {
            _game = game;

            game.Level.LevelState += OnLevelChanged;
            game.Level.PacMan.PacmanState += OnPacManChanged;

            if (null == FieldViewModel) {
                FieldViewModel = new FieldViewModel(game.Level.Field, canvas);
            }
            else {
                FieldViewModel.Init(game.Level.Field);
            }

            if (null == PacManViewModel) {
                if (onPacmanDeathAction != null) {
                    PacManViewModel = new PacManViewModel(
                        game.Level.PacMan,
                        canvas,
                        FieldViewModel,
                        onPacmanDeathAction);
                }
                else {
                    throw new ArgumentNullException("onPacmanDeathAction");
                }
            }
            else {
                PacManViewModel.Init(game.Level.PacMan, FieldViewModel, onPacmanDeathAction);
            }

            if (null != GhostViewModels) {
                foreach (var ghostViewModel in GhostViewModels) {
                    ghostViewModel.ClearCanvas();
                }
                GhostViewModels.Clear();
            }
            else {
                GhostViewModels = new List<GhostViewModel>();
            }


            foreach (var ghostObserverable in game.Level.Ghosts) {
                GhostViewModels.Add(new GhostViewModel(ghostObserverable, this, canvas, FieldViewModel));
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPacManChanged(
            [NotNull] object sender,
            [NotNull] PacmanStateChangedEventArgs pacmanStateChangedEventArgs) {
            if (null == pacmanStateChangedEventArgs) {
                throw new ArgumentNullException("pacmanStateChangedEventArgs");
            }


            //  change scores
            BestScore = _game.GetBestScore();
            CurrentScore = _game.GetGameScore();
            CurrentLevelScore = _game.GetLevelScore();
        }

        private void OnLevelChanged(Object sender, LevelStateChangedEventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            Condition = e.Condition;
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Redrawing

        public void Redraw() {
            _game.Level.ForceNotify();
        }

        #endregion

        public PacManViewModel PacManViewModel { get; private set; }
        public FieldViewModel FieldViewModel { get; private set; }
        public IList<GhostViewModel> GhostViewModels { get; private set; }

        #region Properies

        public LevelCondition Condition {
            get { return _condition; }
            private set {
                if (value == _condition) {
                    return;
                }
                _condition = value;
                OnPropertyChanged();
            }
        }

        public int BestScore {
            get { return _bestScore; }
            private set {
                if (value == _bestScore) {
                    return;
                }
                _bestScore = value;
                OnPropertyChanged();
            }
        }

        public int CurrentScore {
            get { return _currentScore; }
            private set {
                if (value == _currentScore) {
                    return;
                }
                _currentScore = value;
                OnPropertyChanged();
            }
        }

        public int CurrentLevelScore {
            get { return _currentLevelScore; }
            private set {
                if (value == _currentLevelScore) {
                    return;
                }
                _currentLevelScore = value;
                OnPropertyChanged();
            }
        }

        public string Paused {
            get { return _pausedMessage; }
            private set {
                if (value == _pausedMessage) {
                    return;
                }
                _pausedMessage = value;
                OnPropertyChanged();
            }
        }

        public void SetPaused(bool paused) {
            Paused = paused ? PausedMessage : NotPausedMessage;
        }

        #endregion
    }
}