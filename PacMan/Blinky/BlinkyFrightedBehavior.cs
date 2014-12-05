using PacMan_model.level.cells;
using PacMan_model.level.cells.ghosts.ghostBehavior;
using PacMan_model.level.field;

namespace Blinky
{
    public sealed class BlinkyFrightedBehavior : GhostRandomFrightedBehavior {

        private const int Speed = 14;

        public BlinkyFrightedBehavior(INotChanebleableField field, MovingCell target) : base(field, target) {}
        public override int GetSpeed() {
            return Speed;
        }
    }
}
