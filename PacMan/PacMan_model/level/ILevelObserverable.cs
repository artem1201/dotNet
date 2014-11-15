using System;
using System.Collections.Generic;
using PacMan_model.level.cells.ghosts;
using PacMan_model.level.cells.pacman;

namespace PacMan_model.level {
    public interface ILevelObserverable {

        event EventHandler<LevelStateChangedEventArgs> LevelState;
        void ForceNotify();

        IPacManObserverable PacMan { get; }
        IFieldObserverable Field { get; }
        IList<IGhostObserverable> Ghosts { get; }
    }


    public class LevelStateChangedEventArgs : EventArgs {

        public LevelStateChangedEventArgs(LevelCondition condition) {
            Condition = condition;
        }

        public LevelCondition Condition { get; private set; }
    }
}
