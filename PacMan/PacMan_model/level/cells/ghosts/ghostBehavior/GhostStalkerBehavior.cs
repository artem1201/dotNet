namespace PacMan_model.level.cells.ghosts.ghostBehavior {
    public abstract class GhostStalkerBehavior : GhostBehavior {
        protected GhostStalkerBehavior(INotChanebleableField field, MovingCell target) : base(field, target) {}
    }
}
