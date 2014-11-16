using System;
using PacMan_model.level.cells.ghosts.ghostBehavior;
using PacMan_model.level.cells.pacman;
using PacMan_model.util;

namespace PacMan_model.level.cells.ghosts {
    internal class GhostFactory : IGhostFactory {

        private readonly IGhostBehaviorFactory _ghostBehaviorFactory;

        public GhostFactory(string pathToGhostsBehaviors) {
            _ghostBehaviorFactory = new GhostBehaviorFactory(pathToGhostsBehaviors);
        }

        public IGhost CreateGhost(int numberOfGhost, Point position, IField field = null, IPacMan target = null) {
        
            return new Ghost(position, _ghostBehaviorFactory.GetGhostNameByNumber(numberOfGhost), _ghostBehaviorFactory, target, field);
        }
    }
}
