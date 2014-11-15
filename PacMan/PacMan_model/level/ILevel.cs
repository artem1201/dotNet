namespace PacMan_model.level {
    public interface ILevel : ILevelObserverable {

        LevelCondition GetLevelCondition();

        void RegisterOnDirectionObserver(IDirectionEventObserver directionEventObserver);

        void DoATick();
    }

}
