using PacMan_model.level.cells;
using PacMan_model.level.cells.ghosts.ghostBehavior;
using PacMan_model.level.field;
using PacMan_model.util;

namespace Blinky
{
    public sealed class BlinkyFrightedBehavior : GhostFrightedBehavior {

        private const int Speed = 13;

        public BlinkyFrightedBehavior(INotChanebleableField field, MovingCell target) : base(field, target) {}
        public override int GetSpeed() {
            return Speed;
        }

        public override Point GetNextPoint(Point currentPoint) {
            return currentPoint.GetRandonNeighbor(Field);
        }
    }
}
