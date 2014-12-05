using System;
using PacMan_model.level.cells.ghosts.ghostBehavior;
using PacMan_model.level.field;
using PacMan_model.util;

namespace PacMan_model.level.cells.ghosts {
    internal sealed class Ghost : IGhost {
        private const int Cost = 400;


        private readonly string _name;

        private readonly GhostCell _ghost;

        //  number of tick (from 0 to pacman's-current-speed tiks)
        private int _currentTick;
        //  position where ghost will be after end of cycle of movement
        private Point _nextPosition;


        private readonly IGhostBehaviorFactory _ghostBehaviorFactory;
        private GhostBehavior _currentBehavior;

        private void SetCurrentBehavior(GhostBehavior behavior) {
            if (null == behavior) {
                throw new ArgumentNullException("behavior");
            }

            _currentBehavior = behavior;
            _ghost.SetSpeed(_currentBehavior.GetSpeed());
        }


        private MovingCell _target;
        private INotChanebleableField _field;

        #region Initialization

        public Ghost(
            Point startPosition,
            string name,
            IGhostBehaviorFactory ghostBehaviorFactory,
            MovingCell target,
            INotChanebleableField field) {
            if (null == startPosition) {
                throw new ArgumentNullException("startPosition");
            }
            if (null == name) {
                throw new ArgumentNullException("name");
            }
            if (null == ghostBehaviorFactory) {
                throw new ArgumentNullException("ghostBehaviorFactory");
            }
            if (null == field) {
                throw new ArgumentNullException("field");
            }

            _name = name;

            _ghostBehaviorFactory = ghostBehaviorFactory;

            _ghost = new GhostCell(startPosition);

            _target = target;
            _field = field;

            //MakeStalker();
        }

        #endregion

        #region Disposing

        public void Dispose() {
            UnsubsrcibeAll();

            _field = null;
        }

        private void UnsubsrcibeAll() {
            if (null != GhostState) {
                foreach (var levelClient in GhostState.GetInvocationList()) {
                    GhostState -= levelClient as EventHandler<GhostStateChangedEventArgs>;
                }
            }
        }

        #endregion

        #region Setters

        public void SetTarget(MovingCell target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            lock (this) {
                _target = target;
            }
        }

        public void SetField(INotChanebleableField field) {
            if (null == field) {
                throw new ArgumentNullException("field");
            }
            lock (this) {
                _field = field;
            }
        }

        #endregion

        #region Getters

        public int GetCost() {
            return Cost;
        }

        public Point GetPosition()
        {
            return _ghost.GetPosition();
        }

        public string GetName()
        {
            return _name;
        }

        #endregion

        #region Behavior

        public void MakeStalker() {
            SetCurrentBehavior(_ghostBehaviorFactory.GetStalkerBehavior(_name, _field, _target));
        }

        public void MakeFrighted() {
            SetCurrentBehavior(_ghostBehaviorFactory.GetFrightedBehavior(_name, _field, _target));
        }

        #endregion

        #region Moving

        public void Move() {
            if (0 != _currentTick) {
                StartMoving();
            }
            else {
                KeepMoving();
            }
        }

        public void Restart() {
            Stop();
            _ghost.MoveTo(_ghost.GetStartPosition());
        }

        public void Die() {
            Stop();
            _ghost.MoveTo(_ghost.GetStartPosition());


            NotifyChangedStatement();
        }

        private void StartMoving() {
            _nextPosition = _currentBehavior.GetNextPoint(_ghost.GetPosition());

            KeepMoving();
        }

        private void KeepMoving() {
            ++_currentTick;

            if (_ghost.GetSpeed() == _currentTick) {
                _ghost.MoveTo(_nextPosition);

                Stop();

                NotifyChangedStatement();
            }
        }

        private void Stop() {

           _currentTick = 0;
            _nextPosition = null;
        }

        #endregion

        #region Events

        public event EventHandler<GhostStateChangedEventArgs> GhostState;

        public void ForceNotify() {
            NotifyChangedStatement();
        }

        private void OnStatementChanged(GhostStateChangedEventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            e.Raise(this, ref GhostState);
        }

        private void NotifyChangedStatement() {
            var e = new GhostStateChangedEventArgs(_name, _ghost.GetPosition());
            OnStatementChanged(e);
        }

        #endregion

        #region As Cell

        private class GhostCell : MovingCell {
            //  number of ticks per one movement
            private int _currentSpeed;

            public GhostCell(Point startPosition)
                : base(startPosition) {
                if (null == startPosition) {
                    throw new ArgumentNullException("startPosition");
                }
            }


/*
            public GhostCell(Point startPosition, int speed)
                : base(startPosition)
            {
                if (null == startPosition)
                {
                    throw new ArgumentNullException("startPosition");
                }
                if (speed <= 0)
                {
                    throw new ArgumentOutOfRangeException("speed");
                }
                _currentSpeed = speed;
            }
*/

            public void SetSpeed(int speed) {
                if (speed <= 0) {
                    throw new ArgumentOutOfRangeException("speed");
                }
                _currentSpeed = speed;
            }

            /// <summary>
            ///     returns number of ticks per one movement
            /// </summary>
            /// <returns>number of ticks per one movement</returns>
            public override int GetSpeed() {
                return _currentSpeed;
            }


            public void MoveTo(Point nextPosition) {
                if (null == nextPosition) {
                    throw new ArgumentNullException("nextPosition");
                }

                Position = nextPosition;
            }
        }

        #endregion
    }
}