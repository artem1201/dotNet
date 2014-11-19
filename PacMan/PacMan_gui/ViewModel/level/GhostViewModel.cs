using System;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using PacMan_gui.Annotations;
using PacMan_model.level.cells.ghosts;
using PacMan_model.util;

namespace PacMan_gui.ViewModel.level {
    internal class GhostViewModel {

        public Point Position { get; private set; }
        public string Name { get; private set; }

        public FieldViewModel FieldViewModel { get; set; }

        private readonly Canvas _canvas;
        private Shape _addedShape;

        private readonly IGhostObserverable _ghost;

        public GhostViewModel([NotNull] IGhostObserverable ghost, [NotNull] Canvas canvas,
            [NotNull] FieldViewModel fieldViewModel) {
            if (null == ghost) {
                throw new ArgumentNullException("ghost");
            }
            if (null == canvas) {
                throw new ArgumentNullException("canvas");
            }
            if (null == fieldViewModel) {
                throw new ArgumentNullException("fieldViewModel");
            }

            _ghost = ghost;
            _canvas = canvas;
            FieldViewModel = fieldViewModel;

            ghost.GhostState += OnGhostChanged;

            //Redraw();
        }

        private void OnGhostChanged(Object sender, EventArgs e) {

            if (null == e) {
                throw new ArgumentNullException("e");
            }

            var eventArgs = e as GhostStateChangedEventArgs;
            if (null == eventArgs) {
                throw new ArgumentException("e");
            }

            Position = eventArgs.Position;
            Name = eventArgs.Name;

            _canvas.Dispatcher.BeginInvoke(DispatcherPriority.Send, new WorkWithCanvas(RedrawGhostOnCanvas));
        }

        public void Redraw() {
            _ghost.ForceNotify();
        }

        private delegate void WorkWithCanvas();

        private void RedrawGhostOnCanvas() {
            ClearCanvas();


            double cellWidth = _canvas.ActualWidth / FieldViewModel.Width;
            double cellHeight = _canvas.ActualHeight / FieldViewModel.Height;
            _addedShape = CellToView.GhostToShape(Name, cellWidth, cellHeight, Position.GetX(), Position.GetY());

            _canvas.Children.Add(_addedShape);
        }

        private void ClearCanvas() {
            if (_canvas.Children.Contains(_addedShape)) {

                _canvas.Children.Remove(_addedShape);
            }
        }
    }
}
