using System;
using System.Collections.ObjectModel;
using System.Windows;
using PacMan_gui.Annotations;
using PacMan_model.champions;

namespace PacMan_gui.ViewModel.champions {
    internal class ChampionsViewModel {

        public ObservableCollection<ChampionsTableItem> ChampionsTableItems { get; private set; }


        public ChampionsViewModel(IChampionsTableOberverable championsTableOberverable) {

            ChampionsTableItems = new ObservableCollection<ChampionsTableItem>();

            championsTableOberverable.ChampionsTableState += OnChampionsTableState;

            championsTableOberverable.ForceNotify();
        }

        private void OnChampionsTableState(object sender,
            [NotNull] ChampionsTableChangedEventArs championsTableChangedEventArs) {
            if (null == championsTableChangedEventArs) {
                throw new ArgumentNullException("championsTableChangedEventArs");
            }

            Application.Current.Dispatcher.BeginInvoke(new Action(
                delegate {
                    ChampionsTableItems.Clear();

                    foreach (var champion in championsTableChangedEventArs.Champions) {
                        ChampionsTableItems.Add(new ChampionsTableItem(champion.Item2, champion.Item1));
                    }
                }));
        }
    }

    internal class ChampionsTableItem {
        public ChampionsTableItem([NotNull] string name, int score) {
            if (null == name) {
                throw new ArgumentNullException("name");
            }
            Score = score;
            Name = name;
        }

        public string Name { get; private set; }
        public int Score { get; private set; }
    }
}
