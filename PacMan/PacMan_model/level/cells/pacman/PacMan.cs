//  author: Artem Sumanev

using System;
using PacMan_model.level.field;
using PacMan_model.util;

namespace PacMan_model.level.cells.pacman {
    internal sealed class PacMan : IPacManObserverable {
        private const int MaxLives = 3;
        // cell with position, lives and current speed
        private readonly PacManCell _pacman;

        //  current number of ate points
        private int _score;
        //  current number of lives
        private int _lives = MaxLives;

        //  field where pacman is able to move
        private Field _field;

        //  last direction where pacman was moved
        private Direction _currentDirection = Direction.DefaultDirection;

        //  number of tick (from 0 to pacman's-current-speed tiks)
        private int _currentTick;
        //  position where pacman will be after end of cycle of movement
        private Point _nextPosition;
        //  calls when pacman finish movement to next cell
        private Action _onEndOfMovementAction;

        #region Initialization

        public PacMan(Field field, Point startPosition) {
            if (null == field) {
                throw new ArgumentNullException("field");
            }
            if (null == startPosition) {
                throw new ArgumentNullException("startPosition");
            }
            _field = field;

            _pacman = new PacManCell(startPosition, TicksResolver.PacmanTicksPerMove, GetCurrentDirection);
        }

        public PacMan(Field field, Point startPosition, int lives) : this(field, startPosition) {
            if (lives < 1) {
                throw new ArgumentOutOfRangeException("lives");
            }

            _lives = lives;
        }

        #endregion

        #region Disposing

        public void Dispose() {
            UnsubsrcibeAll();

            _field = null;
        }

        private void UnsubsrcibeAll() {
            if (null != PacmanState) {
                foreach (var levelClient in PacmanState.GetInvocationList()) {
                    PacmanState -= levelClient as EventHandler<PacmanStateChangedEventArgs>;
                }
            }
        }

        #endregion

        #region Moving

        /// <summary>
        ///     moves pacman in some direction on set field
        ///     one call is one tick
        /// </summary>
        /// <param name="nextDirection">direction, where player will be moved</param>
        public void Move(Direction nextDirection) {
            if (null == nextDirection) {
                throw new ArgumentNullException("nextDirection");
            }
            //  if pacman is on its move
            if (0 != _currentTick) {
                KeepMoving();
            }
                //  else pacman stops
            else {
                var cellInNextDirection = _field.GetCell(nextDirection.GetNear(_pacman.GetPosition()));
                var cellInCurrentDirection = _field.GetCell(_currentDirection.GetNear(_pacman.GetPosition()));

                if (cellInNextDirection.IsFreeForMoving()) {
                    _currentDirection = nextDirection;
                    _nextPosition = cellInNextDirection.GetPosition();

                    cellInNextDirection.HandlePacmanMovement(this, _field);
                }
                else if (cellInCurrentDirection.IsFreeForMoving()) {
                    _nextPosition = cellInCurrentDirection.GetPosition();

                    cellInCurrentDirection.HandlePacmanMovement(this, _field);
                }
            }
        }

        public void StartMovingTo(Point newPosition, Action onEndOfMovement = null) {
            if (null == newPosition) {
                throw new ArgumentNullException("newPosition");
            }

            _onEndOfMovementAction = onEndOfMovement;

            KeepMoving();
        }

        public void Eat(ICellWithCost cell) {
            if (null == cell) {
                throw new ArgumentNullException("cell");
            }
            _score += cell.GetCost();

            NotifyChangedStatement();
        }

        public void Die() {
            --_lives;

            Stop();

            if (0 != _lives) {
                _pacman.MoveTo(_pacman.GetStartPosition());
            }

            NotifyDeath();
        }

        /// <summary>
        ///     resets ticks,
        ///     resets next position
        /// </summary>
        private void Stop() {
            _currentTick = 0;
            _nextPosition = null;
        }

        private void KeepMoving() {
            ++_currentTick;

            if (_pacman.GetSpeed() == _currentTick) {
                _pacman.MoveTo(_nextPosition);

                Stop();
                NotifyChangedStatement();

                if (null != _onEndOfMovementAction) {
                    _onEndOfMovementAction();
                    _onEndOfMovementAction = null;
                }
            }
        }

        private Direction GetCurrentDirection() {
            return _currentDirection;
        }

        #endregion

        #region Getters

        public Point GetPosition() {
            return _pacman.GetPosition();
        }

        public int GetScore() {
            return _score;
        }

        public int GetLives() {
            return _lives;
        }

        public MovingCell AsMovingCell() {
            return _pacman;
        }

        #endregion

        #region Events

        public event EventHandler<PacmanStateChangedEventArgs> PacmanState;

        public void ForceNotify() {
            NotifyChangedStatement();
        }

        private void OnStatementChanged(PacmanStateChangedEventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }
            e.Raise(this, ref PacmanState);
        }

        private void NotifyChangedStatement() {
            var e = new PacmanStateChangedEventArgs(_pacman.GetPosition(), _currentDirection, _lives, _score);
            OnStatementChanged(e);
        }

        private void NotifyDeath() {
            var e = new PacmanStateChangedEventArgs(_pacman.GetPosition(), _currentDirection, _lives, _score, true);
            OnStatementChanged(e);
        }

        #endregion

        #region As Cell

        private class PacManCell : MovingCell {
            //  number of ticks per second
            private readonly int _currentSpeed;
            private readonly Func<Direction> _getCurrentDirectionFunc;


            public PacManCell(Point startPosition, int initialSpeed, Func<Direction> getCurrentDirectionFunc)
                : base(startPosition) {
                if (null == startPosition) {
                    throw new ArgumentNullException("startPosition");
                }
                if (initialSpeed <= 0) {
                    throw new ArgumentOutOfRangeException("initialSpeed");
                }

                _currentSpeed = initialSpeed;
                _getCurrentDirectionFunc = getCurrentDirectionFunc;
            }

            /// <summary>
            /// returns number of ticks per one movement
            /// </summary>
            /// <returns>number of ticks per one movement</returns>
            public override int GetSpeed() {
                return _currentSpeed;
            }

            public override Direction GetCurrentDirection() {
                return _getCurrentDirectionFunc();
            }

            public void MoveTo(Point newPosition) {
                if (null == newPosition) {
                    throw new ArgumentNullException("newPosition");
                }
                Position = newPosition;
            }
        }

        #endregion
    }
}