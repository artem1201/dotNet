using PacMan_model.level.cells.ghosts.ghostBehavior;

namespace PacMan_model.level.cells.ghosts {
    interface IGhostBehaviorFactory {

        IGhostBehavior GetStalkerBehavior(string name);
        IGhostBehavior GetFrightedBehavior(string name);
        IGhostBehavior GetBehavior(string name, bool isFrightModeEnabled = false);

        int GetStalkerSpeed(string name);
        int GetFrightedSpeed(string name);
        int GetSpeed(string name, bool isFrightModeEnabled = false);
    }
}
