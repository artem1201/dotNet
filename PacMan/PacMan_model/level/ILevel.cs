//  author: Artem Sumanev

namespace PacMan_model.level {
    internal interface ILevel : ILevelObserverable {
        LevelCondition GetLevelCondition();

        void RegisterOnDirectionObserver(IDirectionEventObserver directionEventObserver);

        void DoATick();
    }
}