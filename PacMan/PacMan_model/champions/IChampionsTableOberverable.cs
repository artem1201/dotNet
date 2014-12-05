using System;
using System.Collections.Generic;

namespace PacMan_model.champions {
    public interface IChampionsTableOberverable {
        event EventHandler<ChampionsTableChangedEventArs> ChampionsTableState;
        void ForceNotify();

        ICollection<ChampionsTable.ChampionsRecord> GetResults();
    }

    public class ChampionsTableChangedEventArs : EventArgs {
        public ChampionsTableChangedEventArs(ICollection<ChampionsTable.ChampionsRecord> champions) {
            Champions = champions;
        }

        public ICollection<ChampionsTable.ChampionsRecord> Champions { get; private set; }
    }
}