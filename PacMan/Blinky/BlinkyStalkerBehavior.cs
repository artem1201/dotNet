using System;
using System.Collections.Generic;
using System.Linq;
using PacMan_model.level.cells;
using PacMan_model.level.cells.ghosts.ghostBehavior;
using PacMan_model.level.field;
using PacMan_model.util;

namespace Blinky
{
    public sealed class BlinkyStalkerBehavior : GhostStalkerBehavior {
        private const int Speed = 12;

        private readonly int _sizeOfRememberedPath;
        private static readonly PointComparer Comparer = new PointComparer();
        private readonly ISet<Point> _previousPoints = new HashSet<Point>();
//        private Point _previousPoint;
        public BlinkyStalkerBehavior(INotChanebleableField field, MovingCell target) : base(field, target) {
            _sizeOfRememberedPath = Field.GetHeight() > Field.GetWidth() ? Field.GetHeight() : Field.GetWidth();
        }

        public override int GetSpeed() {
            return Speed;
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

//            var result = neigbors.FirstOrDefault(neigbor => !neigbor.Equals(_previousPoint));
//            _previousPoint = currentPoint;



            return result ?? currentPoint.GetRandonNeighbor(Field);
        }
    }
}
