//  author: Artem Sumanev

using System;
using System.Collections.Generic;
using System.Linq;
using PacMan_model.level.cells;
using PacMan_model.level.cells.ghosts;
using PacMan_model.level.cells.pacman;
using PacMan_model.level.field;
using PacMan_model.util;

namespace PacMan_model.level {
    internal sealed class Level : ILevel {
        private readonly IPacMan _pacman;

        //  depends on user input
        //  determines pacman's direction
        private Direction _currentDirection;

        private readonly IField _field;

        private readonly IList<IGhost> _ghosts;


        //  if Stalking is set - ghost stalks pacman
        //  if Fright is set - pacman is able to eat ghosts
        private LevelCondition _condition = LevelCondition.Stalking;

        private int _currentFrightedModeTicksNumber;

        private readonly ICollection<IDirectionEventObserver> _observers = new List<IDirectionEventObserver>();

        #region Initialization

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

        #endregion

        #region Disposing

        public void Dispose() {
            UnsubsrcibeAll();

            _currentDirection = null;
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

        #endregion

        #region Ticking

        public void DoATick() {
            if (null != _currentDirection) {
                _pacman.Move(_currentDirection);
            }


            CheckDeath();

            foreach (var ghost in _ghosts) {
                ghost.Move();
            }

            CheckDeath();

            if (LevelCondition.Fright == _condition) {
                ++_currentFrightedModeTicksNumber;

                if (TicksResolver.FirghtedModeTicksNumber == _currentFrightedModeTicksNumber) {
                    _currentFrightedModeTicksNumber = 0;
                    ChangeToStalkingCondition();
                }
            }
        }

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

        #endregion

        #region Events

        public void RegisterOnDirectionObserver(IDirectionEventObserver directionEventObserver) {
            directionEventObserver.DirectionChanged += OnDirectionChanged;

            _observers.Add(directionEventObserver);
        }


        public event EventHandler<LevelStateChangedEventArgs> LevelState;

        public void ForceNotify() {
            NotifyChangedStatement();
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

            if (LevelCondition.Fright != _condition) {
                ChangeToFrightCondition();
            }
        }

        private void OnDirectionChanged(Object sender, DirectionChangedEventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            _currentDirection = e.Direction;
        }

        #endregion

        #region Observing

        public IPacManObserverable PacMan { get; private set; }
        public IFieldObserverable Field { get; private set; }
        public IList<IGhostObserverable> Ghosts { get; private set; }

        #endregion

        #region Condition

        public LevelCondition GetLevelCondition() {
            return _condition;
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

        #endregion
    }

    public enum LevelCondition {
        Stalking
        ,
        Fright
    }
}