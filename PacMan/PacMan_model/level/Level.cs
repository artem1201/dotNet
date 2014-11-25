using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using PacMan_model.level.cells;
using PacMan_model.level.cells.ghosts;
using PacMan_model.level.cells.pacman;
using PacMan_model.util;

namespace PacMan_model.level {
    internal sealed class Level : ILevel {

        private readonly IPacMan _pacman;

        //  depends on user input
        //  determines pacman's direction
        private Direction? _currentDirection;

        private readonly IField _field;

        private readonly IList<IGhost> _ghosts;

        
        //  if Stalking is set - ghost stalks pacman
        //  if Fright is set - pacman is able to eat ghosts
        private LevelCondition _condition = LevelCondition.Stalking;
        //  time for firighted mode in ms
//        private const int FrightedTimeMs = 5000;
//        private readonly Timer _frightedTimer;



        private int _currentFrightedModeTicksNumber = 0;

        private readonly ICollection<IDirectionEventObserver> _observers = new List<IDirectionEventObserver>();

        public Level(IPacMan pacman, IField field, IList<IGhost> ghosts) {
            if (null == pacman) {
                throw new ArgumentNullException("pacman");
            }
            if (null == field) {
                throw new ArgumentNullException("field");
            }
            if (null == ghosts) {
                throw new ArgumentNullException("ghosts");
            }
            _pacman = pacman;
            _field = field;
            _ghosts = ghosts;

//            _frightedTimer = new Timer(FrightedTimeMs) {AutoReset = false};
//            _frightedTimer.Elapsed += OnFrightedModeEnds;

            PacMan = _pacman;
            Field = field;
            Ghosts = new List<IGhostObserverable>(_ghosts.Count);
            foreach (var ghost in _ghosts) {
                Ghosts.Add(ghost);
            }

            foreach (var energizer in field.GetCells().OfType<Energizer>()) {
                energizer.EnergizerEaten += OnEnergizerEaten;
            }
        }

        public void DoATick() {

            if (null != _currentDirection) _pacman.Move(_currentDirection.Value);
            
            CheckDeath();
            
            foreach (var ghost in _ghosts) {
                ghost.Move();
            }

            //TODO: move this calling in event when someone of pacman or ghosts changes place
            CheckDeath();

            if (LevelCondition.Fright == _condition) {
                ++_currentFrightedModeTicksNumber;

                if (TicksResolver.FirghtedModeTicksNumber == _currentFrightedModeTicksNumber) {
                    _currentFrightedModeTicksNumber = 0;
                    ChangeToStalkingCondition();
                }
            }
        }

        public LevelCondition GetLevelCondition() {
            return _condition;
        }

        public void Pause() {

//            if (_frightedTimer.Enabled) {
//                _frightedTimer.Stop();    
//            }
        }

        public void Resume() {

            if (LevelCondition.Fright == _condition) {
//                _frightedTimer.Start();    
            }
        }

        public void RegisterOnDirectionObserver(IDirectionEventObserver directionEventObserver) {

            directionEventObserver.DirectionChanged += OnDirectionChanged;

            _observers.Add(directionEventObserver);
        }


        public event EventHandler<LevelStateChangedEventArgs> LevelState;
        public void ForceNotify() {
            NotifyChangedStatement();
        }

        public void Dispose() {

            UnsubsrcibeAll();
            
//            _frightedTimer.Stop();
//            _frightedTimer.Dispose();
            _currentDirection = null;

//            _pacman = null;
//            _field = null;
//            _ghosts.Clear();

        }

        private void UnsubsrcibeAll() {

            if (null != LevelState) {
                foreach (var levelClient in LevelState.GetInvocationList()) {
                    LevelState -= levelClient as EventHandler<LevelStateChangedEventArgs>;
                }
            }

            foreach (var directionEventObserver in _observers) {
                directionEventObserver.DirectionChanged -= OnDirectionChanged;
            }

            _pacman.Dispose();
            _field.Dispose();
            foreach (var ghost in _ghosts) {
                ghost.Dispose();
            }
        }

        public IPacManObserverable PacMan { get; private set; }
        public IFieldObserverable Field { get; private set; }
        public IList<IGhostObserverable> Ghosts { get; private set; }

        private void CheckDeath() {

            foreach (var ghost in _ghosts.Where(ghost => _pacman.GetPosition().Equals(ghost.GetPosition()))) {
                
                switch (_condition) {
                    case LevelCondition.Stalking:
                        GhostsWins();
                        break;
                    case LevelCondition.Fright:
                        _pacman.Eat(ghost);
                        ghost.Die();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void GhostsWins() {
            _pacman.Die();
            _currentDirection = null;
            foreach (var ghost in _ghosts) {
                ghost.Restart();
            }
        }


        /// <summary>
        /// notifies listeners about changing level state
        /// </summary>
        /// <param name="e"></param>
        private void OnStatementChanged(LevelStateChangedEventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }
            e.Raise(this, ref LevelState);
        }

        /// <summary>
        /// is called when level's statment has been changed
        /// </summary>
        private void NotifyChangedStatement() {
            var e = new LevelStateChangedEventArgs(_condition);
            OnStatementChanged(e);
        }


        private void OnEnergizerEaten(Object sender, EventArgs e) {

            _currentFrightedModeTicksNumber = 0;

//            _frightedTimer.Stop();
//            _frightedTimer.Start();

            if (LevelCondition.Fright != _condition) {
                ChangeToFrightCondition();   
            }



//            switch (_condition) {
//                case LevelCondition.Stalking:
//                    ChangeToFrightCondition();
//                    break;
//                case LevelCondition.Fright:
//                    ChangeToStalkingCondition();
//                    break;
//                default:
//                    throw new InvalidEnumArgumentException(Resources.Level_OnEnergizerEaten_unknown_level_condition__ + _condition.ToString());
//            }
        }

        private void OnFrightedModeEnds(Object parameter, ElapsedEventArgs elapsedEventArgs) {
            
            ChangeToStalkingCondition();
        }

        private void ChangeToFrightCondition() {
            _condition = LevelCondition.Fright;

            foreach (var ghost in _ghosts) {
                ghost.MakeFrighted();
            }

            NotifyChangedStatement();
        }

        private void ChangeToStalkingCondition() {
            _condition = LevelCondition.Stalking;

            foreach (var ghost in _ghosts) {
                ghost.MakeStalker();
            }

            NotifyChangedStatement();
        }

        private void OnDirectionChanged(Object sender, DirectionChangedEventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            _currentDirection = e.Direction;
        }
    }

    public enum LevelCondition {
        Stalking
        , Fright
    }
    
}

