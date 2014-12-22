//  author: Artem Sumanev

using PacMan_model.level.field;
using PacMan_model.util;

namespace PacMan_model.level.cells.ghosts {
    internal interface IGhostFactory {
        IGhost CreateGhost(
            int numberOfGhost,
            Point position,
            INotChanebleableField field = null,
            MovingCell target = null);
    }


    public static class GhostsInfo {
        public static readonly string[] OrderedPossibleGhostNames = {"TestGhost", "Blinky", "Pinky", "Inky", "Clyde"};
    }
}