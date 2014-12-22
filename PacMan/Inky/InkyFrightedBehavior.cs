//  author: Artem Sumanev

using PacMan_model.level.cells;
using PacMan_model.level.cells.ghosts.ghostBehavior;
using PacMan_model.level.field;
using PacMan_model.util;

namespace Inky {
    public sealed class InkyFrightedBehavior : GhostFrightedBehavior {
        private const int Speed = 10;

        public InkyFrightedBehavior(INotChanebleableField field, MovingCell target) : base(field, target) {}

        public override Point GetNextPoint(Point currentPoint) {
            return currentPoint;
        }

        public override int GetSpeed() {
            return Speed;
        }
    }
}