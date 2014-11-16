using PacMan_model.util;

namespace PacMan_model.level.cells.ghosts.ghostBehavior {
    public interface IGhostBehavior {

        int GetSpeed();
        Point GetNextPoint();
    }
}
