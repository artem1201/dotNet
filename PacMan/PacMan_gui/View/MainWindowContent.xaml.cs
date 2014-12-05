using System;
using System.Threading;
using System.Windows;
using PacMan_gui.Annotations;
using PacMan_gui.Controllers;
using PacMan_gui.View.About;
using PacMan_gui.View.Champions;
using PacMan_gui.View.Level;
using PacMan_model.champions;

namespace PacMan_gui.View {
    /// <summary>
    /// Interaction logic for MainWindowContent.xaml
    /// </summary>
    public partial class MainWindowContent {
        private readonly MainWindow _mainWindow;

        private readonly ChampionsTable _championsTable;
        private readonly ChampionsTableView _championsTableView;
        private readonly ChampionsController _championsController;


        private readonly AboutBox _aboutBox;

        #region Initialization

        public MainWindowContent([NotNull] MainWindow mainWindow) {
            if (null == mainWindow) {
                throw new ArgumentNullException("mainWindow");
            }
            InitializeComponent();

            _mainWindow = mainWindow;
            Application.Current.Exit += (sender, args) => OnExit();

            _championsTable = new ChampionsTable();
            _championsTableView = new ChampionsTableView();
            _championsController = new ChampionsController(_championsTableView, _championsTable, OnBackToMainWindow);

            _aboutBox = new AboutBox(OnBackToMainWindow);
        }

        #endregion

        //  changes content of window to main window
        private void OnBackToMainWindow() {
            Application.Current.Dispatcher.Invoke(
                delegate { _mainWindow.ContentControl.Content = this; });
        }

        #region Playing

        private void PlayButton_OnClick(object sender, RoutedEventArgs e) {
            _mainWindow.ContentControl.Content = new GameView();

            (_mainWindow.ContentControl.Content as GameView).Loaded += delegate {
                var gameController = new GameController(_mainWindow.ContentControl.Content as GameView, OnGameEnds);
                gameController.Run();
            };
        }

        private void OnGameEnds(int gameScore) {
            OnBackToMainWindow();

            if (_championsTable.IsNewRecord(gameScore)) {
                AddNewRecord(gameScore);
            }
        }

        #endregion

        #region Exit

        private void ExitButton_OnClick(object sender, RoutedEventArgs e) {
            OnExit();
            _mainWindow.Close();
        }

        private void OnExit() {
            //  save champions to file
            _championsTable.Dispose();
        }

        #endregion

        #region Champions

        private static readonly string ChampionNameEmpty = String.Empty;

        private string _newChampionName = ChampionNameEmpty;
        private bool _addNewRecord;
        private readonly AutoResetEvent _nameEnteredEvent = new AutoResetEvent(false);

        private void AddNewRecord(int score) {
            //  show input box
            Application.Current.Dispatcher.BeginInvoke(
                new Action(delegate { InputNameBox.Visibility = Visibility.Visible; }));

            //  wait for name
            _nameEnteredEvent.WaitOne();
            //  reset waiter
            _nameEnteredEvent.Reset();

            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate {
                        //  clean input area and hide input box
                        InputNameTextBox.Text = ChampionNameEmpty;
                        InputNameBox.Visibility = Visibility.Collapsed;
                    }));


            if (_addNewRecord) {
                //  add new champion
                _championsTable.AddNewResult(score, _newChampionName);
            }

            //  clean champion name
            _newChampionName = ChampionNameEmpty;
        }

        //  calls when user entered his name
        private void OkButton_OnClick(object sender, RoutedEventArgs e) {
            //  check if name is valid
            //  else wait for valid one
            if (WrongName(InputNameTextBox.Text)) {
                MessageBox.Show("Invalid name! Try again");
                return;
            }

            //  set name
            _newChampionName = InputNameTextBox.Text;

            _addNewRecord = true;

            //  notify name has been entered
            _nameEnteredEvent.Set();
        }

        private static bool WrongName(string name) {
            return null != name && name.Equals(ChampionNameEmpty);
        }

        private void ChampionsButton_OnClick(object sender, RoutedEventArgs e) {
            _mainWindow.ContentControl.Content = _championsTableView;

            _championsController.Run();
        }

        private void NoButton_OnClick(object sender, RoutedEventArgs e) {
            _addNewRecord = false;

            //  notify name has been entered
            _nameEnteredEvent.Set();
        }

        #endregion

        #region About

        private void AboutButton_OnClick(object sender, RoutedEventArgs e) {
            _mainWindow.ContentControl.Content = _aboutBox;
        }

        #endregion
    }
}