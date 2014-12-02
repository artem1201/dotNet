using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using PacMan_gui.Annotations;
using PacMan_model.level.cells.pacman;
using PacMan_model.util;
using Point = PacMan_model.util.Point;

namespace PacMan_gui.ViewModel.level {
    internal sealed class PacManViewModel : INotifyPropertyChanged {
        public Point Position { get; private set; }
        public Direction Direction { get; private set; }

        public int LivesNumber {
            get { return _livesNumber; }
            private set {
                if (value == _livesNumber) {
                    return;
                }
                _livesNumber = value;
                OnPropertyChanged();
            }
        }


        public FieldViewModel FieldViewModel { get; set; }

        private readonly Canvas _canvas;
        private IPacManObserverable _pacMan;
        private Shape _addedShape;
        private int _livesNumber;
        private Action<int> _onPacmanDeathAction;

        #region Initialization

        public PacManViewModel(
            IPacManObserverable pacMan,
            Canvas canvas,
            FieldViewModel fieldViewModel,
            [NotNull] Action<int> onPacmanDeathAction) {
            if (null == pacMan) {
                throw new ArgumentNullException("pacMan");
            }
            if (null == canvas) {
                throw new ArgumentNullException("canvas");
            }
            if (null == fieldViewModel) {
                throw new ArgumentNullException("fieldViewModel");
            }
            if (null == onPacmanDeathAction) {
                throw new ArgumentNullException("onPacmanDeathAction");
            }

            _canvas = canvas;
            Init(pacMan, fieldViewModel, onPacmanDeathAction);

            //Redraw();
        }

        public void Init(IPacManObserverable pacMan, FieldViewModel fieldViewModel, Action<int> onPacmanDeathAction) {
            if (null == pacMan) {
                throw new ArgumentNullException("pacMan");
            }
            if (null == fieldViewModel) {
                throw new ArgumentNullException("fieldViewModel");
            }

            if (null != onPacmanDeathAction) {
                _onPacmanDeathAction = onPacmanDeathAction;
            }

            _pacMan = pacMan;
            pacMan.PacmanState += OnPacManChanged;

            FieldViewModel = fieldViewModel;
        }

        #endregion

        #region Events

        private void OnPacManChanged(Object sender, PacmanStateChangedEventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            Position = e.Position;
            Direction = e.Direction;
            LivesNumber = e.Lives;

            if (e.HasDied) {
                _onPacmanDeathAction(e.Lives);
            }

            _canvas.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(RedrawPacManOnCanvas));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Redrawing

        public void Redraw() {
            _pacMan.ForceNotify();
        }

        private void RedrawPacManOnCanvas() {
            ClearCanvas();


            double cellWidth = _canvas.ActualWidth / FieldViewModel.Width;
            double cellHeight = _canvas.ActualHeight / FieldViewModel.Height;
            _addedShape = CellToView.PacmanToShape(cellWidth, cellHeight, Position.GetX(), Position.GetY());

            _canvas.Children.Add(_addedShape);
        }

        private void ClearCanvas() {
            if ((null != _addedShape) && _canvas.Children.Contains(_addedShape)) {
                _canvas.Children.Remove(_addedShape);
            }
        }

        #endregion
    }
}