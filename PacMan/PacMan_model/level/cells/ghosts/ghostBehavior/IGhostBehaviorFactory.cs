using System;
using System.Collections.Generic;
using PacMan_model.level.field;

namespace PacMan_model.level.cells.ghosts.ghostBehavior {
    internal interface IGhostBehaviorFactory {
        bool ContainsName(string name);
        string GetGhostNameByNumber(int ghostNumber);
        ICollection<string> GetAvaialbleNames();

        GhostStalkerBehavior GetStalkerBehavior(string name, INotChanebleableField field, MovingCell target);
        GhostFrightedBehavior GetFrightedBehavior(string name, INotChanebleableField field, MovingCell target);

        GhostBehavior GetBehavior(
            string name,
            INotChanebleableField field,
            MovingCell target,
            bool isFrightModeEnabled = false);

        GhostStalkerBehavior GetStalkerBehavior(int ghostNumber, INotChanebleableField field, MovingCell target);
        GhostFrightedBehavior GetFrightedBehavior(int ghostNumber, INotChanebleableField field, MovingCell target);

        GhostBehavior GetBehavior(
            int ghostNumber,
            INotChanebleableField field,
            MovingCell target,
            bool isFrightModeEnabled = false);
    }

    
}