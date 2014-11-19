using PacMan_model.util;

namespace PacMan_model.level.cells.ghosts.ghostBehavior {
    public abstract class GhostBehavior {

        //  field where ghost is moving
        protected INotChanebleableField Field;
        //  targeg for ghost's hunting
        protected MovingCell Target;

        protected GhostBehavior(INotChanebleableField field, MovingCell target) {
            Field = field;
            Target = target;
        }

        public abstract int GetSpeed();
        public abstract Point GetNextPoint(Point currentPoint);
    }
}
