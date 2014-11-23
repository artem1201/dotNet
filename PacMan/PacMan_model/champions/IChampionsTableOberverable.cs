using System;
using System.Collections.Generic;

namespace PacMan_model.champions {
    public interface IChampionsTableOberverable {

        event EventHandler<ChampionsTableChangedEventArs> ChampionsTableState;
        void ForceNotify();

        ICollection<Tuple<int, string>> GetResults();
    }

    public class ChampionsTableChangedEventArs : EventArgs {

        public ChampionsTableChangedEventArs(ICollection<Tuple<int, string>> champions) {

            Champions = champions;
        }

        public ICollection<Tuple<int, string>> Champions { get; private set; }
    }
}
