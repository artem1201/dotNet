using System.Collections.Generic;
using System.Linq;
using PacMan_model.level.cells;
using PacMan_model.level.cells.ghosts.ghostBehavior;
using PacMan_model.level.field;
using PacMan_model.level.pathFinding;
using PacMan_model.util;

namespace Inky {
    public sealed class InkyStalkerBehavior : GhostStalkerBehavior {
        private const int Speed = 12;

        private const int InkyArea = 4;
        private readonly GhostBehavior _insideInkyAreaBehavior;
        private readonly GhostBehavior _extraBehavior;

        private readonly int _sizeOfRememberedPath;
        private static readonly PointComparer Comparer = new PointComparer();
        private readonly ISet<Point> _previousPoints = new HashSet<Point>();

        public InkyStalkerBehavior(INotChanebleableField field, MovingCell target)
            : base(field, target) {
            _insideInkyAreaBehavior = new GhostStraightGoBehavior(
                this,
                Field,
                Target,
                new GhostRandomBehavior(this, Field, Target));
            _extraBehavior = new GhostRandomBehavior(this, Field, Target);
            _sizeOfRememberedPath = Field.GetHeight() > Field.GetWidth() ? Field.GetHeight() : Field.GetWidth();
        }

        public override Point GetNextPoint(Point currentPoint) {
            if (Target.GetPosition().IsPointInsideSquare(currentPoint, InkyArea)) {
                return _insideInkyAreaBehavior.GetNextPoint(currentPoint);
            }

            var nextPoint = Target.GetCurrentDirection().GetNearWithDistance(Target.GetPosition(), InkyArea);

            if (!Field.GetCell(nextPoint).IsFreeForMoving()) {
                return currentPoint;
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
            return Speed;
        }
    }
}