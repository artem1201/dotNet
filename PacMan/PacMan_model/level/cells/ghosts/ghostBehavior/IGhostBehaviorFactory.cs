using System;
using System.Collections.Generic;

namespace PacMan_model.level.cells.ghosts.ghostBehavior {
    internal interface IGhostBehaviorFactory {

        bool ContainsName(string name);
        string GetGhostNameByNumber(int ghostNumber);
        ICollection<string> GetAvaialbleNames();

        GhostStalkerBehavior GetStalkerBehavior(string name, INotChanebleableField field, MovingCell target);
        GhostFrightedBehavior GetFrightedBehavior(string name, INotChanebleableField field, MovingCell target);
        GhostBehavior GetBehavior(string name, INotChanebleableField field, MovingCell target, bool isFrightModeEnabled = false);

        GhostStalkerBehavior GetStalkerBehavior(int ghostNumber, INotChanebleableField field, MovingCell target);
        GhostFrightedBehavior GetFrightedBehavior(int ghostNumber, INotChanebleableField field, MovingCell target);
        GhostBehavior GetBehavior(int ghostNumber, INotChanebleableField field, MovingCell target, bool isFrightModeEnabled = false);
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
