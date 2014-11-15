using System;
using System.Threading;
using PacMan_model.level.cells.ghosts.ghostBehavior;
using PacMan_model.level.cells.pacman;
using PacMan_model.util;

namespace PacMan_model.level.cells.ghosts {
    class Ghost : IGhost {

        

        private const int Cost = 400;


        private readonly string _name;

        private readonly GhostCell _ghost;

        //  number of tick (from 0 to pacman's-current-speed tiks)
        private int _currentTick;
        //  position where ghost will be after end of cycle of movement
        private Point _nextPosition;


        private readonly IGhostBehaviorFactory _ghostBehaviorFactory;
        private IGhostBehavior _currentBehavior;

        private IPacMan _target;
        private IField _field;

        public Ghost(Point startPosition, string name, IGhostBehaviorFactory ghostBehaviorFactory, IPacMan target, IField field) {
            
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

            MakeStalker();
        }

        public event EventHandler<GhostStateChangedEventArgs> GhostState;
        public void ForceNotify() {
            NotifyChangedStatement();
        }

        public void SetTarget(IPacMan target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }
            _target = target;
        }

        public void SetField(IField field) {
            if (null == field) {
                throw new ArgumentNullException("field");
            }
            _field = field;
        }

        public int GetCost() {
            return Cost;
        }

        public void Move() {

            if (0 != _currentTick) {
                _nextPosition = _currentBehavior.GetNextPoint();

                StartMoving();
            }
            else {
                KeepMoving();
            }
            
        }

        public void MakeStalker() {
            _ghost.SetSpeed(_ghostBehaviorFactory.GetStalkerSpeed(_name));
            _currentBehavior = _ghostBehaviorFactory.GetStalkerBehavior(_name);
        }

        public void MakeFrighted() {
            _ghost.SetSpeed(_ghostBehaviorFactory.GetFrightedSpeed(_name));
            _currentBehavior = _ghostBehaviorFactory.GetFrightedBehavior(_name);
        }

        public Point GetPosition() {
            return _ghost.GetPosition();
        }

        public string GetName() {
            return _name;
        }

        public void Restart() {
            _ghost.MoveTo(_ghost.GetStartPosition());

            Stop();
        }

        public void Die() {

            Stop();

            _ghost.MoveTo(_ghost.GetStartPosition());

            NotifyChangedStatement();
        }

        private void Stop() {
            _currentTick = 0;
            _nextPosition = null;
        }

        private void StartMoving() {
            KeepMoving();
        }

        private void KeepMoving() {

            ++_currentTick;

            if (_ghost.GetSpeed() == _currentTick) {

                Stop();

               _ghost.MoveTo(_nextPosition);

                NotifyChangedStatement();
            }
        }

        protected virtual void OnStatementChanged(GhostStateChangedEventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }
            var temp = Volatile.Read(ref GhostState);

            if (temp != null) temp(this, e);
        }

        private void NotifyChangedStatement() {
            var e = new GhostStateChangedEventArgs(_name, _ghost.GetPosition());
            OnStatementChanged(e);
        }
    }

    internal class GhostCell : MovingCell {

        //  number of ticks per one movement
        private int _currentSpeed;

        public GhostCell(Point startPosition) : base(startPosition) {
            if (null == startPosition) {
                throw new ArgumentNullException("startPosition");
            }
        }


        public GhostCell(Point startPosition, int speed) : base(startPosition) {
            if (null == startPosition) {
                throw new ArgumentNullException("startPosition");
            }
            if (speed <= 0) {
                throw new ArgumentOutOfRangeException("speed");
            }
            _currentSpeed = speed;
        }

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
}
