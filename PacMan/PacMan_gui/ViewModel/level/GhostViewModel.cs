//  author: Artem Sumanev

using System;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using PacMan_gui.Annotations;
using PacMan_model.level;
using PacMan_model.level.cells.ghosts;
using PacMan_model.util;

namespace PacMan_gui.ViewModel.level {
    internal sealed class GhostViewModel {
        private readonly Canvas _canvas;
        private readonly GameViewModel _gameViewModel;
        private Shape _addedShape;

        private IGhostObserverable _ghost;
        public Point Position { get; private set; }
        public string Name { get; private set; }

        public FieldViewModel FieldViewModel { get; set; }

        private LevelCondition LevelCondition {
            get { return _gameViewModel.Condition; }
        }

        #region Initialization

        public GhostViewModel(
            [NotNull] IGhostObserverable ghost,
            [NotNull] GameViewModel gameViewModel,
            [NotNull] Canvas canvas,
            [NotNull] FieldViewModel fieldViewModel) {
            if (null == ghost) {
                throw new ArgumentNullException("ghost");
            }
            if (null == gameViewModel) {
                throw new ArgumentNullException("gameViewModel");
            }
            if (null == canvas) {
                throw new ArgumentNullException("canvas");
            }
            if (null == fieldViewModel) {
                throw new ArgumentNullException("fieldViewModel");
            }
            _gameViewModel = gameViewModel;
            _canvas = canvas;

            Init(ghost, fieldViewModel);
        }

        public void Init(
            [NotNull] IGhostObserverable ghost,
            [NotNull] FieldViewModel fieldViewModel) {
            if (null == ghost) {
                throw new ArgumentNullException("ghost");
            }
            if (null == fieldViewModel) {
                throw new ArgumentNullException("fieldViewModel");
            }

            _ghost = ghost;


            FieldViewModel = fieldViewModel;

            ghost.GhostState += OnGhostChanged;
        }

        #endregion

        #region Events

        private void OnGhostChanged(Object sender, GhostStateChangedEventArgs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            Position = e.Position;
            Name = e.Name;

            _canvas.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(RedrawGhostOnCanvas));
        }

        #endregion

        #region Redrawing

        public void Redraw() {
            _ghost.ForceNotify();
        }

        private void RedrawGhostOnCanvas() {
            ClearCanvas();


            var cellWidth = _canvas.ActualWidth / FieldViewModel.Width;
            var cellHeight = _canvas.ActualHeight / FieldViewModel.Height;
            _addedShape = CellToView.GhostToShape(
                Name,
                LevelCondition,
                cellWidth,
                cellHeight,
                Position.GetX(),
                Position.GetY());

            _canvas.Children.Add(_addedShape);
        }

        public void ClearCanvas() {
            if ((_addedShape != null) && _canvas.Children.Contains(_addedShape)) {
                _canvas.Children.Remove(_addedShape);
            }
        }

        #endregion
    }
}