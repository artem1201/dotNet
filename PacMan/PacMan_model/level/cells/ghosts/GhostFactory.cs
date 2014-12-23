//  author: Artem Sumanev

using PacMan_model.level.cells.ghosts.ghostBehavior;
using PacMan_model.level.field;
using PacMan_model.util;

namespace PacMan_model.level.cells.ghosts {
    internal sealed class GhostFactory {
        private readonly GhostBehaviorFactory _ghostBehaviorFactory;

        public GhostFactory(string pathToGhostsBehaviors) {
            _ghostBehaviorFactory = new GhostBehaviorFactory(pathToGhostsBehaviors);
        }

        public Ghost CreateGhost(
            int numberOfGhost,
            Point position,
            INotChanebleableField field = null,
            MovingCell target = null) {
            return new Ghost(
                position,
                _ghostBehaviorFactory.GetGhostNameByNumber(numberOfGhost),
                _ghostBehaviorFactory,
                target,
                field);
        }
    }

    public static class GhostsInfo {
        public static readonly string[] OrderedPossibleGhostNames = {"Blinky", "Pinky", "Inky", "Clyde"};
    }
}