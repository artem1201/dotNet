using PacMan_model.level.field;
using PacMan_model.level.pathFinding;
using PacMan_model.util;

namespace PacMan_model.level.cells.ghosts.ghostBehavior {
    public abstract class GhostRandomFrightedBehavior : GhostFrightedBehavior {
        protected GhostRandomFrightedBehavior(INotChanebleableField field, MovingCell target) : base(field, target) {}

        public override Point GetNextPoint(Point currentPoint) {
            return currentPoint.GetRandonNeighbor(Field);
        }
    }
}