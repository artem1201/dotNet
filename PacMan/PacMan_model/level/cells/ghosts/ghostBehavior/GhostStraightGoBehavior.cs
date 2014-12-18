using System;
using System.Collections.Generic;
using System.Linq;
using PacMan_model.level.field;
using PacMan_model.level.pathFinding;
using PacMan_model.util;

namespace PacMan_model.level.cells.ghosts.ghostBehavior {
    /// <summary>
    /// trise to go straight ahead to target
    /// may be not know where to go, so need some extra behavior which will know way
    /// </summary>
    public sealed class GhostStraightGoBehavior : GhostBehavior {
        private readonly int _sizeOfRememberedPath;
        private static readonly PointComparer Comparer = new PointComparer();
        private readonly ISet<Point> _previousPoints = new HashSet<Point>();

        private readonly GhostBehavior _extraBehavior;
        private readonly GhostBehavior _parentBehavior;

        public GhostStraightGoBehavior(
            GhostBehavior parentBehavior,
            INotChanebleableField field,
            MovingCell target,
            GhostBehavior extraBehavior)
            : base(field, target) {
            _parentBehavior = parentBehavior;
            _sizeOfRememberedPath = Field.GetHeight() > Field.GetWidth() ? Field.GetHeight() : Field.GetWidth();
            _extraBehavior = extraBehavior;
        }


        public override Point GetNextPoint(Point currentPoint) {
            if (null == currentPoint) {
                throw new ArgumentNullException("currentPoint");
            }

            var neigbors = currentPoint.GetOrderedClosesNeighbors(Target.GetPosition(), Field);

            if (_previousPoints.Count.Equals(_sizeOfRememberedPath)) {
                _previousPoints.Clear();
            }
            var result = neigbors.FirstOrDefault(neigbor => !_previousPoints.Contains(neigbor, Comparer));
            _previousPoints.Add(currentPoint);

            return result ?? _extraBehavior.GetNextPoint(currentPoint);
        }


        public override int GetSpeed() {
            return _parentBehavior.GetSpeed();
        }
    }
}