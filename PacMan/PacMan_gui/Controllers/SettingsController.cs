using System;
using System.Windows.Input;
using PacMan_gui.Annotations;
using PacMan_gui.View.Settings;
using PacMan_gui.ViewModel.settings;

namespace PacMan_gui.Controllers
{
    internal class SettingsController {

        private readonly SettingsView _settingsView;
        private readonly SettingsViewModel _settingsViewModel;

        private readonly Action _onExit;

        public SettingsController([NotNull] Action onExit) {
            if (null == onExit) {
                throw new ArgumentNullException("onExit");
            }
            _onExit = onExit;


            _settingsView = new SettingsView();
            _settingsViewModel = new SettingsViewModel();

            _settingsView.DataContext = _settingsViewModel;
            _settingsView.KeysSettingsDataGrid.ItemsSource = _settingsViewModel.KeySettingsItems;


            _settingsView.ExitButton.Command = new OnBackButtonCommand(OnBackToMainWindow);
        }

        #region Bindings

        private class OnBackButtonCommand : ICommand {

            private readonly Action _onBackAction;
            public OnBackButtonCommand([NotNull] Action onBackAction) {
                if (null == onBackAction) {
                    throw new ArgumentNullException("onBackAction");
                }
                _onBackAction = onBackAction;
            }

            public bool CanExecute(object parameter) {
                return true;
            }

            public void Execute(object parameter) {
                _onBackAction();
            }

            public event EventHandler CanExecuteChanged;
        }

        #endregion

        #region Getters

        public SettingsView GetSettingsView() {
            return _settingsView;
        }

        public SettingsViewModel GetSettingsViewModel() {
            return _settingsViewModel;
        }

        #endregion

        #region Actions

        private void OnBackToMainWindow() {
            
            //TODO: ask if i should save changes
            
            _settingsViewModel.SaveChanges();

            _onExit();
        }

        #endregion
    }
}
