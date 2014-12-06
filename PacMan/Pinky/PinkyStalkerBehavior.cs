using System;
using PacMan_model.level.cells;
using PacMan_model.level.cells.ghosts.ghostBehavior;
using PacMan_model.level.field;
using PacMan_model.util;

namespace Pinky {
    public sealed class PinkyStalkerBehavior : GhostStalkerBehavior {
        private const int Speed = 13;

        private const int PinkyArea = 8;

        private readonly GhostBehavior _behaviorWhenTargetIsInArea;
        private readonly GhostBehavior _behaviorWhenTargetIsOutOfArea;

        public PinkyStalkerBehavior(INotChanebleableField field, MovingCell target)
            : base(field, target) {
            _behaviorWhenTargetIsInArea = new GhostStraightGoBehavior(
                this,
                field,
                target,
                extraBehavior:new GhostRandomBehavior(this, field, target));
            _behaviorWhenTargetIsOutOfArea = new GhostRandomBehavior(this, field, target);
        }

        public override int GetSpeed() {
            return Speed;
        }

        public override Point GetNextPoint(Point currentPoint) {
            if (null == currentPoint) {
                throw new ArgumentNullException("currentPoint");
            }

            return Target.GetPosition().IsPointInsideSquare(currentPoint, PinkyArea)
                ? _behaviorWhenTargetIsInArea.GetNextPoint(currentPoint)
                : _behaviorWhenTargetIsOutOfArea.GetNextPoint(currentPoint);
        }
    }
}