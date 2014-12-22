//  author: Artem Sumanev

using PacMan_model.level.cells;
using PacMan_model.level.cells.ghosts.ghostBehavior;
using PacMan_model.level.field;
using PacMan_model.util;

namespace Blinky {
    public sealed class BlinkyFrightedBehavior : GhostFrightedBehavior {
        private const int Speed = 14;
        private readonly GhostBehavior _behavior;

        public BlinkyFrightedBehavior(INotChanebleableField field, MovingCell target) : base(field, target) {
            _behavior = new GhostRandomBehavior(this, field, target);
        }

        public override int GetSpeed() {
            return Speed;
        }

        public override Point GetNextPoint(Point currentPoint) {
            return _behavior.GetNextPoint(currentPoint);
        }
    }
}