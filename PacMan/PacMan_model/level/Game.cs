using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using PacMan_model.level.cells.ghosts;
using PacMan_model.level.cells.pacman;
using PacMan_model.util;

namespace PacMan_model.level {
    public class Game : IGame {

        //  directory where levels are
        private string _pathToLevels;

        private readonly Ticker _ticker;

        //  current level of company
        private ILevel _currentLevel;
        private readonly ILevelLoader _levelLoader;

        //  best score of current company
        private int _bestScore;
        //  score of current game
        private int _currentScore;
        //  score of current level
        private int _currentLevelScore;


        private bool _isWon;
        private bool _isFinished;

        private string[] _levelFiles;
        private int _currentLevelNumber;


        private readonly ICollection<IDirectionEventObserver> _observers = new List<IDirectionEventObserver>(); 

        private bool HasNextLevel {
            get { return _levelFiles.Length - 1 != _currentLevelNumber; }
        }

        public Game(string pathToLevels, string pathToGhosts, int bestScore) {
            if (null == pathToLevels) {
                throw new ArgumentNullException("pathToLevels");
            }
            if (null == pathToGhosts) {
                throw new ArgumentNullException("pathToGhosts");
            }
            
            _levelLoader = new LevelLoader(new GhostFactory(pathToGhosts));


            _ticker = new Ticker(DoATick);



            NewGame(bestScore, pathToLevels);
        }

        public void NewGame(int bestScore, string pathToLevels = null) {
            if (null != pathToLevels) {
                
                _pathToLevels = pathToLevels;
            }

            _ticker.Stop();

            _isWon = false;
            _isFinished = false;

            _observers.Clear();

            _bestScore = bestScore;
            _currentScore = 0;
            _currentLevelScore = 0;

            try {

                _levelFiles = Directory.GetFiles(_pathToLevels);
            }
            catch (DirectoryNotFoundException) {
                throw new InvalidLevelDirectory(_pathToLevels);
            }

            if (0 == _levelFiles.Length) {
                throw new InvalidLevelDirectory(_pathToLevels); 
            }

            _currentLevelNumber = -1;
            LoadNextLevel();
        }

        public bool LoadNextLevel() {
            _ticker.Stop();

            if (_levelFiles.Length - 1 == _currentLevelNumber) {
                return false;
            }

            ++_currentLevelNumber;

            _currentLevelScore = 0;

            using (var nextLevelSource = new FileStream(_levelFiles[_currentLevelNumber], FileMode.Open)) {

                if (null != _currentLevel) {
                    _currentLevel.Dispose();
                }

                _currentLevel = _levelLoader.LoadFromSource(nextLevelSource);

                _currentLevel.PacMan.PacmanState += OnPacManChanged;
                _currentLevel.Field.FieldState += OnFieldChanged;

                foreach (var directionEventObserver in _observers) {
                    _currentLevel.RegisterOnDirectionObserver(directionEventObserver);
                }
            }

            return true;
        }

        public void Start() {
           _ticker.Start();
        }

        public void Pause() {
            _ticker.Stop();
        }

        public void Stop() {
            _ticker.Abort();
        }

        public bool IsOn() {
            return _ticker.IsOn();
        }

        public int GetGameScore() {
            return _currentScore;
        }

        public int GetLevelScore() {
            return _currentLevelScore;
        }

        public int GetBestScore() {
            return _bestScore;
        }

        public void Win() {
            _ticker.Stop();

            if (!HasNextLevel) {
                //  no more levels
                //  total win    
                _isWon = true;
                _isFinished = true;
            }

            NotifyLevelFinished();

        }

        public void Loose() {
            _ticker.Stop();
            
            _isWon = false;
            _isFinished = true;

            NotifyLevelFinished();
        }

        public bool IsWon() {
            return _isWon;
        }

        public bool IsFinished() {
            return _isFinished;
        }

        public void RegisterOnDirectionObserver(IDirectionEventObserver directionEventObserver) {
            if (null == directionEventObserver) {
                throw new ArgumentNullException("directionEventObserver");
            }

            if (null != _currentLevel) {
                _currentLevel.RegisterOnDirectionObserver(directionEventObserver);    
            }

            _observers.Add(directionEventObserver);
        }

        public ILevelObserverable Level {
            get { return _currentLevel; }
        }



        private void DoATick() {

            _currentLevel.DoATick();
        }

        private void OnPacManChanged(Object sender, PacmanStateChangedEventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            _currentScore += (e.Score - _currentLevelScore);
            _currentLevelScore = e.Score;
            

            if (_currentScore > _bestScore) {
                _bestScore = _currentScore;
            }

            if (e.HasDied) {
                 _ticker.Stop();

                if (0 == e.Lives) {
                    Loose();
                }
            }
        }

        private void OnFieldChanged(Object sender, FieldStateChangedEventArs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            if ((e.DotIsNoMore) && (!IsFinished())) {
                Win();
            }
        }

        public event EventHandler LevelFinished;

        private void NotifyLevelFinished() {
            OnLevelFinishedNotify(EventArgs.Empty);
        }

        protected virtual void OnLevelFinishedNotify(EventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            e.Raise(this, ref LevelFinished);
        }

        private class Ticker {

            private const int Delay = 1;

            private readonly Timer _timer;
            private readonly Action _tickAction;


            public Ticker(Action tickAction) {
                _tickAction = tickAction;
                
                _timer = new Timer(Delay);
                _timer.Elapsed += Tick;
            }

            public bool IsOn() {
                return _timer.Enabled;
            }

            public void Start() {
                _timer.Start();
            }

            public void Stop() {
                _timer.Stop();
            }

            public void Abort() {
                Stop();
                _timer.Dispose();

            }

            private void Tick(Object source, ElapsedEventArgs e) {

                _tickAction();
            }
        }
    }
}
