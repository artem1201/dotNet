using System;
using System.IO;
using System.Threading;
using PacMan_model.level.cells.ghosts;
using PacMan_model.level.cells.pacman;

namespace PacMan_model.level {
    public class Game : IGame {

        //  directory where levels are
        private string _pathToLevels;

        //  directory where ghosts are
        private string _pathToGhosts;

        private readonly Ticker _ticker;

        //  current level of company
        private ILevel _currentLevel;
        private readonly ILevelLoader _levelLoader;

        //TODO: add initialization
        private IGhostFactory _ghostFactory;

        //  best score of current company
        private int _bestScore;
        //  score of current game
        private int _currentScore;
        //  score of current level
        private int _currentLevelScore;


        private bool _isWon;
        private bool _isLost;

        private string[] _levelFiles;
        private int _currentLevelNumber;

        public Game(string pathToLevels, string pathToGhosts, int bestScore) {
            if (null == pathToLevels) {
                throw new ArgumentNullException("pathToLevels");
            }
            if (null == pathToGhosts) {
                throw new ArgumentNullException("pathToGhosts");
            }
            
            _pathToGhosts = pathToGhosts;

            _ghostFactory = new GhostFactory();

            _levelLoader = new LevelLoader(_ghostFactory);


            _ticker = new Ticker(DoATick);



            NewGame(bestScore, pathToLevels);
        }

        public void NewGame(int bestScore, string pathToLevels = null) {
            if (null != pathToLevels) {
                
                _pathToLevels = pathToLevels;
            }

            _ticker.Stop();

            _isWon = false;
            _isLost = false;

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

            var hasNextLevel = LoadNextLevel();

            if (!hasNextLevel) {
                //  no more levels
                //  total win    
                _isWon = true;
                _isLost = false;
            }

            NotifyLevelFinished(hasNextLevel);

        }

        public void Loose() {
            _ticker.Stop();
            
            _isWon = false;
            _isLost = true;

            NotifyLevelFinished(false);
        }

        public bool IsWon() {
            return _isWon;
        }

        public bool IsLost() {
            return _isLost;
        }

        public void RegisterOnDirectionObserver(IDirectionEventObserver directionEventObserver) {
            _currentLevel.RegisterOnDirectionObserver(directionEventObserver);
        }

        public ILevelObserverable Level {
            get { return _currentLevel; }
        }

        private void DoATick() {

//            Stopwatch stopwatch = Stopwatch.StartNew();

            _currentLevel.DoATick();

//            stopwatch.Stop();
//
//            System.Console.WriteLine("One tick: " + stopwatch.ElapsedMilliseconds);
        }

        private bool LoadNextLevel() {
            _ticker.Stop();

            if (_levelFiles.Length - 1 == _currentLevelNumber) {
                return false;
            }

            ++_currentLevelNumber;

            using (var nextLevelSource = new FileStream(_levelFiles[_currentLevelNumber], FileMode.Open)) {

                _currentLevel = _levelLoader.LoadFromSource(nextLevelSource);

                _currentLevel.PacMan.PacmanState += OnPacManChanged;
                _currentLevel.Field.FieldState += OnFieldChanged;
            }

            return true;
        }
        
        private void OnPacManChanged(Object sender, EventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            var eventArgs = e as PacmanStateChangedEventArgs;
            if (null == eventArgs) {
                throw new ArgumentException("e");
            }

            _currentScore += (eventArgs.Score - _currentLevelScore);
            _currentLevelScore = eventArgs.Score;
            

            if (_currentScore > _bestScore) {
                _bestScore = _currentScore;
            }

            if (eventArgs.HasDied) {
                _ticker.Stop();

                if (0 == eventArgs.Lives) {
                    Loose();
                }
            }
        }

        private void OnFieldChanged(Object sender, EventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            var eventArgs = e as FieldStateChangedEventArs;
            if (null == eventArgs) {
                throw new ArgumentException("e");
            }


            if ((eventArgs.DotIsNoMore) && (!IsLost())) {
                Win();
            }
        }

        public event EventHandler<LevelFinishedEventArs> LevelFinished;

        private void NotifyLevelFinished(bool hasNextLevel) {
            OnLevelFinishedNotify(new LevelFinishedEventArs(hasNextLevel));
        }

        protected virtual void OnLevelFinishedNotify(LevelFinishedEventArs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }
            var temp = Volatile.Read(ref LevelFinished);

            if (temp != null) temp(this, e);
        }

        private class Ticker {

            private const int Delay = 1;

            public delegate void TickPerformer();

            private readonly TickPerformer _tickPerformer;
            private readonly Timer _timer;
            private bool _isOn;

            public Ticker(TickPerformer tickPerformer) {
                _tickPerformer = tickPerformer;

                TimerCallback tcb = Tick;

                _timer = new Timer(tcb, null, Timeout.Infinite, Delay);
            }

            public bool IsOn() {
                return _isOn;
            }

            public void Start() {
                _timer.Change(0, Delay);
                _isOn = true;
            }

            public void Stop() {
                _timer.Change(Timeout.Infinite, Delay);
                _isOn = false;
            }

            public void Abort() {
                Stop();
                _timer.Dispose();

            }

            private void Tick(Object parameter) {
                _tickPerformer();
            }
        }
    }
}
