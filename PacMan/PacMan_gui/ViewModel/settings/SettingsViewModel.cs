using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PacMan_gui.Annotations;
using PacMan_gui.View;
using PacMan_gui.View.Settings;
using PacMan_model.util;
using OnFirstKeyChangingByActionName = System.Action<string>;

namespace PacMan_gui.ViewModel.settings {
    internal sealed class SettingsViewModel : INotifyPropertyChanged {
        public IDictionary<Key, Direction> KeysToDirection { get; private set; }
        public ISet<Key> PauseKeys { get; private set; }
        public bool IsChanged { get; private set; }

        private const string PauseActionName = "Pause";
        public ObservableCollection<KeySettingsItem> KeySettingsItems { get; private set; }

        private readonly SettingsView _settingsView;

        private const string ViewState = "";
        private const string ChangeState = "press some key";
        private string _settingsState;

        public string SettingsState {
            get { return _settingsState; }
            private set {
                if (value.Equals(_settingsState)) {
                    return;
                }
                _settingsState = value;
                OnPropertyChanged();
            }
        }

        #region Initialization

        public SettingsViewModel([NotNull] SettingsView settingsView) {
            if (null == settingsView) {
                throw new ArgumentNullException("settingsView");
            }

            _settingsView = settingsView;

            KeysToDirection = new Dictionary<Key, Direction> {
                {Key.Up, Direction.Directions[Direction.Up]},
//                {Key.W, Direction.Directions[Direction.Up]},
                {Key.Left, Direction.Directions[Direction.Left]},
//                {Key.A, Direction.Directions[Direction.Left]} ,
                {Key.Down, Direction.Directions[Direction.Down]},
//                {Key.S, Direction.Directions[Direction.Down]},
                {Key.Right, Direction.Directions[Direction.Right]},
//                {Key.D, Direction.Directions[Direction.Right]}
            };

            PauseKeys = new HashSet<Key> {Key.Space, /*Key.P*/};


            KeySettingsItems = new ObservableCollection<KeySettingsItem>();
            InitObserverableCollectionFromKeysContainers();

            OnFirstKeyChangingCommand = new OnSomeActionButtonCommand(OnFirstKeyChanging);
            _settingsState = ViewState;

            IsChanged = false;
        }

        private void InitObserverableCollectionFromKeysContainers() {
            KeySettingsItems.Clear();


            foreach (var keyToDirection in KeysToDirection) {
                KeySettingsItems.Add(new KeySettingsItem(keyToDirection.Value.GetName(), keyToDirection.Key));
            }
            KeySettingsItems.Add(new KeySettingsItem(PauseActionName, PauseKeys.First()));
        }

        private void InitKeysContainersFromObserverableCollection() {
            PauseKeys.Clear();
            KeysToDirection.Clear();
            
            foreach (var keySettingsItem in KeySettingsItems) {
                //  Add pause
                if (PauseActionName.Equals(keySettingsItem.ActionName)) {
                    PauseKeys.Add(keySettingsItem.FirstKey);

                    continue;
                }
                var item = keySettingsItem;
                foreach (var direction in Direction.Directions.Where(direction => direction.GetName().Equals(item.ActionName))) {
                    KeysToDirection.Add(keySettingsItem.FirstKey, direction);
                }
            }
        }

        #endregion

        #region Commands

        public ICommand OnFirstKeyChangingCommand { get; private set; }

        private class OnSomeActionButtonCommand : ICommand {
            private readonly OnFirstKeyChangingByActionName _action;

            public OnSomeActionButtonCommand([NotNull] OnFirstKeyChangingByActionName action) {
                if (null == action) {
                    throw new ArgumentNullException("action");
                }
                _action = action;
            }

            public bool CanExecute(object parameter) {
                return true;
            }

            public void Execute([NotNull] object parameter) {
                if (null == parameter) {
                    throw new ArgumentNullException("parameter");
                }

                var actionName = parameter as string;
                if (null == actionName) {
                    throw new ArgumentException("parameter of OnSomeActionButtonCommand must be string");
                }

                _action(actionName);
            }

            public event EventHandler CanExecuteChanged;
        }

        #endregion

        #region Actions

        public void SaveChanges() {
            InitKeysContainersFromObserverableCollection();
        }

        public void Refresh() {
            InitObserverableCollectionFromKeysContainers();
        }

        private void OnFirstKeyChanging(string actionName) {
            var currentItem = KeySettingsItems.SingleOrDefault(item => item.ActionName.Equals(actionName));

            if (null == currentItem) {
                throw new Exception("user tries to change unknown action key");
            }

            SettingsState = ChangeState;
            _settingsView.StartListenToKeys(
                key => {
                    if (IsKeyAlreadyInUse(key)) {
                        _settingsView.MainWindow.ShowMessage("key " + key + " is occupied, try another");
                        return;
                    }

                    currentItem.FirstKey = key;

                    _settingsView.StopListenToKeys();
                    SettingsState = ViewState;

                    IsChanged = true;
                });
        }

        private bool IsKeyAlreadyInUse(Key key) {
            return KeySettingsItems.Any(keySettingsItem => keySettingsItem.ContainsKey(key));
        }

        #endregion

        internal sealed class KeySettingsItem : INotifyPropertyChanged {

            private Key _firstKey;
            public Key FirstKey {
                get { return _firstKey; }
                set {
                    if (value.Equals(_firstKey)) {
                        return;
                    }
                    _firstKey = value;
                    OnPropertyChanged1("FirstKeyName");
                }
            }


//            private Key _secondKey;
//            public Key SecondKey
//            {
//                get { return _secondKey; }
//                set
//                {
//                    if (value.Equals(_secondKey))
//                    {
//                        return;
//                    }
//                    _secondKey = value;
//                    OnPropertyChanged1("SecondKeyName");
//                }
//            }

            public string ActionName { get; private set; }

            public string FirstKeyName {
                get { return FirstKey.ToString(); }
            }
//            public string SecondKeyName
//            {
//                get { return SecondKey.ToString(); }
//            }

            

            public KeySettingsItem([NotNull] string action, Key firstKey/*, Key secondKey*/) {
                if (null == action) {
                    throw new ArgumentNullException("action");
                }
                _firstKey = firstKey;
//                _secondKey = secondKey;
                ActionName = action;
            }

            

            public bool ContainsKey(Key key) {
                if (_firstKey.Equals(key)) {
                    return true;
                }

                return false;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            private void OnPropertyChanged1([CallerMemberName] string propertyName = null) {
                var handler = PropertyChanged;
                if (handler != null) {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}