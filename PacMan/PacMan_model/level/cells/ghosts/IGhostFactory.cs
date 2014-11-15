using PacMan_model.level.cells.pacman;
using PacMan_model.util;

namespace PacMan_model.level.cells.ghosts {
    public interface IGhostFactory {

        IGhost CreateGhost(int numberOfGhost, Point position, IField field = null, IPacMan target = null);

    }
}
