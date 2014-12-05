using System;
using PacMan_gui.View.Champions;
using PacMan_gui.ViewModel.champions;
using PacMan_model.champions;

namespace PacMan_gui.Controllers {
    internal sealed class ChampionsController {
        private readonly ChampionsTableView _championsTableView;
//        private ChampionsTable _championsTable;

        private readonly ChampionsViewModel _championsViewModel;

        private readonly Action _onExit;

        public ChampionsController(ChampionsTableView championsTableView, ChampionsTable championsTable, Action onExit) {
            _championsTableView = championsTableView;
//            _championsTable = championsTable;
            _onExit = onExit;

            _championsViewModel = new ChampionsViewModel(championsTable);
        }

        public void Run() {
            SetBinding();
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