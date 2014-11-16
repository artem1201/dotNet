using System;
using System.Collections.Generic;

namespace PacMan_model.level.cells.ghosts.ghostBehavior {
    internal interface IGhostBehaviorFactory {

        bool ContainsName(string name);
        string GetGhostNameByNumber(int ghostNumber);
        ICollection<string> GetNames();

        IGhostBehavior GetStalkerBehavior(string name);
        IGhostBehavior GetFrightedBehavior(string name);
        IGhostBehavior GetBehavior(string name, bool isFrightModeEnabled = false);

        IGhostBehavior GetStalkerBehavior(int ghostNumber);
        IGhostBehavior GetFrightedBehavior(int ghostNumber);
        IGhostBehavior GetBehavior(int ghostNumber, bool isFrightModeEnabled = false);
    }

    public class UnknownGhostName : Exception {
        public UnknownGhostName(string name) {
            GhostName = name;
        }

        public string GhostName { get; private set; }
    }

    public class InvalidBehaviorsDirectory : Exception {
        public InvalidBehaviorsDirectory(string directoryName) {
            DirectoryName = directoryName;
        }

        public string DirectoryName { get; private set; }
    }
}
