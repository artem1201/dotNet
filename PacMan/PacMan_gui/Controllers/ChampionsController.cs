//  author: Artem Sumanev

using System;
using PacMan_gui.Annotations;
using PacMan_gui.View.Champions;
using PacMan_gui.ViewModel.champions;
using PacMan_model.champions;

namespace PacMan_gui.Controllers {
    internal sealed class ChampionsController : IDisposable {
        private readonly ChampionsTable _championsTable;
        private readonly ChampionsTableView _championsTableView;
        private readonly ChampionsViewModel _championsViewModel;

        private readonly Action _onExit;

        public ChampionsController([NotNull] Action onExit) {
            if (null == onExit) {
                throw new ArgumentNullException("onExit");
            }
            _championsTableView = new ChampionsTableView();

            _championsTable = new ChampionsTable();

            _onExit = onExit;

            _championsViewModel = new ChampionsViewModel(_championsTable);

            SetBinding();
        }

        public void Dispose() {
            _championsTable.Dispose();
        }

        public ChampionsTableView GetChampionsTableView() {
            return _championsTableView;
        }

        public ChampionsTable GetChampionsTable() {
            return _championsTable;
        }

        public void Run() {
            _championsTableView.ChampionsTableExit += OnExit;
        }

        private void SetBinding() {
            _championsTableView.ChampionsDataGrid.ItemsSource = _championsViewModel.ChampionsTableItems;
        }

        private void OnExit(object sender, EventArgs e) {
            _onExit();
        }
    }
}