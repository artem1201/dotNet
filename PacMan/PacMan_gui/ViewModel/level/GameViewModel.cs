using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using PacMan_gui.Annotations;
using PacMan_model.level;
using PacMan_model.level.cells.pacman;

namespace PacMan_gui.ViewModel.level {
    internal class GameViewModel : INotifyPropertyChanged {

        public PacManViewModel PacManViewModel { get; private set; }
        public FieldViewModel FieldViewModel { get; private set; }
        public IList<GhostViewModel> GhostViewModels { get; private set; }

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

        //private readonly Canvas _canvas;
        private IGame _game;
        private int _bestScore;
        private int _currentScore;
        private int _currentLevelScore;
        private LevelCondition _condition;

        public GameViewModel([NotNull] IGame game, [NotNull] Canvas canvas) {
            if (null == game) {
                throw new ArgumentNullException("game");
            }
            if (null == canvas) {
                throw new ArgumentNullException("canvas");
            }
            //_canvas = canvas;


            Init(game, canvas);

            BestScore = game.GetBestScore();
            CurrentScore = game.GetGameScore();
            CurrentLevelScore = game.GetLevelScore();

            //Redraw();
        }

        public void Init([NotNull] IGame game, [NotNull] Canvas canvas) {


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
                PacManViewModel = new PacManViewModel(game.Level.PacMan, canvas, FieldViewModel);
            }
            else {
                PacManViewModel.Init(game.Level.PacMan, FieldViewModel);
            }


            GhostViewModels = new List<GhostViewModel>(game.Level.Ghosts.Count);

            foreach (var ghostObserverable in game.Level.Ghosts) {
                GhostViewModels.Add(new GhostViewModel(ghostObserverable, this, canvas, FieldViewModel));
            }
        }

        private void OnPacManChanged([NotNull] object sender, [NotNull] PacmanStateChangedEventArgs pacmanStateChangedEventArgs) {
            
//            if (null == sender) {
//                throw new ArgumentNullException("sender");
//            }
           
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

        public void Redraw() {
            _game.Level.ForceNotify();
//            FieldViewModel.Redraw();
//            PacManViewModel.Redraw();
//
//            foreach (var ghostViewModel in GhostViewModels) {
//                ghostViewModel.Redraw();
//            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
