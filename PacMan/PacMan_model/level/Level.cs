using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using PacMan_model.level.cells;
using PacMan_model.level.cells.ghosts;
using PacMan_model.level.cells.pacman;
using PacMan_model.util;

namespace PacMan_model.level {
    sealed class Level : ILevel {

        private readonly IPacMan _pacman;

        //  depends on user input
        //  determines pacman's direction
        private Direction? _currentDirection;

        //private readonly IField _field;

        private readonly IList<IGhost> _ghosts;

        
        //  if Stalking is set - ghost stalks pacman
        //  if Fright is set - pacman is able to eat ghosts
        private LevelCondition _condition = LevelCondition.Stalking;

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
            //_field = field;
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

        public void DoATick() {
            
            if (null != _currentDirection) _pacman.Move(_currentDirection.Value);

            foreach (var ghost in _ghosts) {
                ghost.Move();
            }

            //TODO: move this calling in event when someone of pacman and ghosts changes place
            CheckDeath();

        }

        public LevelCondition GetLevelCondition() {
            return _condition;
        }

        public void RegisterOnDirectionObserver(IDirectionEventObserver directionEventObserver) {
            directionEventObserver.DirectionChanged += OnDirectionChanged;
        }


        public event EventHandler<LevelStateChangedEventArgs> LevelState;
        public void ForceNotify() {
            NotifyChangedStatement();
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
            var temp = Volatile.Read(ref LevelState);

            if (temp != null) temp(this, e);
        }

        /// <summary>
        /// is called when level's statment has been changed
        /// </summary>
        private void NotifyChangedStatement() {
            var e = new LevelStateChangedEventArgs(_condition);
            OnStatementChanged(e);
        }


        private void OnEnergizerEaten(Object sender, EventArgs e) {
            switch (_condition) {
                case LevelCondition.Stalking:
                    ChangeToFrightCondition();
                    break;
                case LevelCondition.Fright:
                    ChangeToStalkingCondition();
                    break;
                default:
                    throw new InvalidEnumArgumentException("unknown level condition: " + _condition.ToString());
            }

            NotifyChangedStatement();
        }

        private void ChangeToFrightCondition() {
            _condition = LevelCondition.Fright;

            foreach (var ghost in _ghosts) {
                ghost.MakeFrighted();
            }
        }

        private void ChangeToStalkingCondition() {
            _condition = LevelCondition.Stalking;

            foreach (var ghost in _ghosts) {
                ghost.MakeStalker();
            }
        }

        private void OnDirectionChanged(Object sender, EventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            var eventArgs = e as DirectionChangedEventArgs;
            if (null == eventArgs) {
                throw new ArgumentException("e");
            }

            _currentDirection = eventArgs.Direction;
        }
    }



    public enum LevelCondition {
        Stalking
        , Fright
    }
    
}

