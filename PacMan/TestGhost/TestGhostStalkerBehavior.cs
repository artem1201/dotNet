using System;
using PacMan_model.level;
using PacMan_model.level.cells;
using PacMan_model.level.cells.ghosts.ghostBehavior;
using PacMan_model.util;

namespace TestGhost
{
    public class TestGhostStalkerBehavior : GhostStalkerBehavior {

        private const int Speed = 10;
 
        public TestGhostStalkerBehavior(INotChanebleableField field, MovingCell target) : base(field, target) {}
        
        
        public override int GetSpeed() {
            return Speed;
        }

        public override Point GetNextPoint(Point currentPoint) {
            if (null == currentPoint) {
                throw new ArgumentNullException("currentPoint");
            }

            if (Field.GetCell(currentPoint.GetLeftOf()) is Wall) {
                return currentPoint;
            }
            else {
                return currentPoint.GetLeftOf();
            }
        }
    }
}
