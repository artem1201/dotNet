using PacMan_model.level.cells;
using PacMan_model.level.cells.ghosts.ghostBehavior;
using PacMan_model.level.field;
using PacMan_model.util;

namespace Pinky {
    public sealed class PinkyFrightedBehavior : GhostFrightedBehavior {
        private const int Speed = 14;

        private readonly GhostBehavior _behavior;

        public PinkyFrightedBehavior(INotChanebleableField field, MovingCell target) : base(field, target) {
            _behavior = new GhostRandomBehavior(this, field, target);
        }

        public override Point GetNextPoint(Point currentPoint) {
            return _behavior.GetNextPoint(currentPoint);
        }

        public override int GetSpeed() {
            return Speed;
        }
    }
}