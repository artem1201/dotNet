//  author: Artem Sumanev

using PacMan_model.level.cells;
using PacMan_model.level.cells.ghosts.ghostBehavior;
using PacMan_model.level.field;
using PacMan_model.util;

namespace Blinky {
    public sealed class BlinkyStalkerBehavior : GhostStalkerBehavior {
        private const int Speed = 12;

        private readonly GhostBehavior _behavior;

        public BlinkyStalkerBehavior(INotChanebleableField field, MovingCell target)
            : base(field, target) {
            _behavior = new GhostStraightGoBehavior(this, field, target, new GhostRandomBehavior(this, field, target));
        }

        public override Point GetNextPoint(Point currentPoint) {
            return _behavior.GetNextPoint(currentPoint);
        }

        public override int GetSpeed() {
            return Speed;
        }
    }
}