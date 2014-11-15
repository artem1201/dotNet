using System;
using PacMan_model.level.cells.pacman;
using PacMan_model.util;

namespace PacMan_model.level.cells.ghosts {
    class GhostFactory : IGhostFactory {
        public IGhost CreateGhost(int numberOfGhost, Point position, IField field = null, IPacMan target = null) {
            throw new NotImplementedException();
        }
    }
}
