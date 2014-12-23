//  author: Artem Sumanev

#region

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

#endregion

namespace PacMan_gui.ViewModel.settings {
    internal sealed class SettingsViewModel : INotifyPropertyChanged {
        private const string PauseActionName = "Pause";

        private const Key CancelListenToKey = Key.Escape;

        private const string ViewState = "";
        private const string ChangeState = "press some key";
        private readonly SettingsView _settingsView;
        private string _settingsState;
        public IDictionary<Key, Direction> KeysToDirection { get; private set; }
        public ISet<Key> PauseKeys { get; private set; }
        public bool IsChanged { get; private set; }
        public ObservableCollection<KeySettingsItem> KeySettingsItems { get; private set; }

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
                {Key.Left, Direction.Directions[Direction.Left]},
                {Key.Down, Direction.Directions[Direction.Down]},
                {Key.Right, Direction.Directions[Direction.Right]},
            };

            PauseKeys = new HashSet<Key> {Key.Space};


            KeySettingsItems = new ObservableCollection<KeySettingsItem>();
            InitObserverableCollectionFromKeysContainers();

            _onFirstKeyChangingCommand = new OnSomeActionButtonCommand(OnFirstKeyChanging);
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
                foreach (
                    var direction in
                        Direction.Directions.Where(direction => direction.GetName().Equals(item.ActionName))) {
                    KeysToDirection.Add(keySettingsItem.FirstKey, direction);
                }
            }
        }

        #endregion

        #region Commands
        private readonly OnSomeActionButtonCommand _onFirstKeyChangingCommand;
        public ICommand OnFirstKeyChangingCommand {
            get { return _onFirstKeyChangingCommand; }
//            private set { _onFirstKeyChangingCommand = value; }
        }

        private sealed class OnSomeActionButtonCommand : ICommand {
            private readonly OnFirstKeyChangingByActionName _action;
            private bool _canExecute;

            public OnSomeActionButtonCommand([NotNull] OnFirstKeyChangingByActionName action, bool canExecute = true) {
                if (null == action) {
                    throw new ArgumentNullException("action");
                }
                _action = action;
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) {
                return _canExecute;
            }

            public void SetExecute(bool canExecute) {
                _canExecute = canExecute;
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

        private bool _alreadyListening;
        
        private void OnFirstKeyChanging(string actionName) {
            var currentItem = KeySettingsItems.SingleOrDefault(item => item.ActionName.Equals(actionName));

            if (null == currentItem) {
                throw new Exception("user tries to change unknown action key");
            }

            if (!_alreadyListening) {
                SettingsState = ChangeState;
                _onFirstKeyChangingCommand.SetExecute(false);
                _settingsView.StartListenToKeys(
                    key => {
                        if (CancelListenToKey != key && !currentItem.ContainsKey(key)) {
                            if (IsKeyAlreadyInUse(key)) {
                                _settingsView.MainWindow.ShowMessage("key " + key + " is occupied, try another");
                                return;
                            }

                            currentItem.FirstKey = key;
                            IsChanged = true;
                        }


                        _settingsView.StopListenToKeys();
                        SettingsState = ViewState;
                        _onFirstKeyChangingCommand.SetExecute(true);
                        _alreadyListening = false;
                    });

                _alreadyListening = true;
            }
        }

        private bool IsKeyAlreadyInUse(Key key) {
            return KeySettingsItems.Any(keySettingsItem => keySettingsItem.ContainsKey(key));
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        internal sealed class KeySettingsItem : INotifyPropertyChanged {
            private Key _firstKey;

            public KeySettingsItem([NotNull] string action, Key firstKey) {
                if (null == action) {
                    throw new ArgumentNullException("action");
                }
                _firstKey = firstKey;
                ActionName = action;
            }

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


            public string ActionName { get; private set; }

            public string FirstKeyName {
                get { return FirstKey.ToString(); }
            }


            public event PropertyChangedEventHandler PropertyChanged;

            public bool ContainsKey(Key key) {
                return _firstKey == key;
            }

            [NotifyPropertyChangedInvocator]
            private void OnPropertyChanged1([CallerMemberName] string propertyName = null) {
                var handler = PropertyChanged;
                if (handler != null) {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}