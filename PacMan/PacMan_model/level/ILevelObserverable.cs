using System;
using System.Collections.Generic;
using PacMan_model.level.cells.ghosts;
using PacMan_model.level.cells.pacman;
using PacMan_model.level.field;

namespace PacMan_model.level {
    public interface ILevelObserverable : IDisposable {

        event EventHandler<LevelStateChangedEventArgs> LevelState;
        void ForceNotify();

        IPacManObserverable PacMan { get; }
        IFieldObserverable Field { get; }
        IList<IGhostObserverable> Ghosts { get; }
    }


    public sealed class LevelStateChangedEventArgs : EventArgs {

        public LevelStateChangedEventArgs(LevelCondition condition) {
            Condition = condition;
        }

        public LevelCondition Condition { get; private set; }
    }
}
