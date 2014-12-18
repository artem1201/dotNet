using System;
using PacMan_model.level.field;
using PacMan_model.level.pathFinding;
using PacMan_model.util;

namespace PacMan_model.level.cells.ghosts.ghostBehavior {
    /// <summary>
    /// returns random neighbor of current cell if there is possible movement
    /// else returns null (if ghost is inside walls square)
    /// </summary>
    public sealed class GhostRandomBehavior : GhostBehavior {
        public GhostRandomBehavior(GhostBehavior parentBehavior, INotChanebleableField field, MovingCell target)
            : base(field, target) {
            _parentBehavior = parentBehavior;
        }

        private readonly GhostBehavior _parentBehavior;

        public override Point GetNextPoint(Point currentPoint) {
            if (null == currentPoint) {
                throw new ArgumentNullException("currentPoint");
            }
            return currentPoint.GetRandonNeighbor(Field);
        }

        public override int GetSpeed() {
            return _parentBehavior.GetSpeed();
        }
    }
}