using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
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
        private readonly IGame _game;
        private int _bestScore;
        private int _currentScore;
        private int _currentLevelScore;
        private LevelCondition _condition;

        public GameViewModel(IGame game, Canvas canvas) {
            //_canvas = canvas;
            _game = game;

            game.Level.LevelState += OnLevelChanged;
            game.Level.PacMan.PacmanState += OnPacManChanged;

            BestScore = game.GetBestScore();
            CurrentScore = game.GetGameScore();
            CurrentLevelScore = game.GetLevelScore();

            FieldViewModel = new FieldViewModel(game.Level.Field, canvas);
            PacManViewModel = new PacManViewModel(game.Level.PacMan, canvas, FieldViewModel);
            GhostViewModels = new List<GhostViewModel>(game.Level.Ghosts.Count);
            
            foreach (var ghostObserverable in game.Level.Ghosts) {
                GhostViewModels.Add(new GhostViewModel(ghostObserverable, canvas, FieldViewModel));
            }

            //Redraw();
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

        private void OnLevelChanged(Object sender, EventArgs e) {

            if (null == e) {
                throw new ArgumentNullException("e");
            }

            var eventArgs = e as LevelStateChangedEventArgs;
            if (null == eventArgs) {
                throw new ArgumentException("e");
            }


            Condition = eventArgs.Condition;
        }

        public void Redraw() {
            _game.Level.ForceNotify();
            FieldViewModel.Redraw();
            PacManViewModel.Redraw();

            foreach (var ghostViewModel in GhostViewModels) {
                ghostViewModel.Redraw();
            }
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
