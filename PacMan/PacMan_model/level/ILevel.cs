using System;

namespace PacMan_model.level {
    internal interface ILevel : ILevelObserverable, IDisposable {

        LevelCondition GetLevelCondition();

        void Pause();
        void Resume();

        void RegisterOnDirectionObserver(IDirectionEventObserver directionEventObserver);

        void DoATick();
    }

}
