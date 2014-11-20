using System;
using PacMan_model.level;
using PacMan_model.level.cells;
using PacMan_model.level.cells.ghosts.ghostBehavior;
using PacMan_model.util;

namespace TestGhost {
    public class TestFrightedBehavior : GhostFrightedBehavior {

        private const int Speed = 11;

        public TestFrightedBehavior(INotChanebleableField field, MovingCell target) : base(field, target) {
            
        }
        
        
        public override int GetSpeed() {
            return Speed;
        }

        public override Point GetNextPoint(Point currentPoint) {
            if (null == currentPoint) {
                throw new ArgumentNullException("currentPoint");
            }

            if (Field.GetCell(currentPoint.GetRightOf()) is Wall) {
                return currentPoint;
            }
            return currentPoint.GetRightOf();
        }

    }
}
