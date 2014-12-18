using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using PacMan_model.level.field;

namespace PacMan_gui.ViewModel.level {
    internal sealed class FieldViewModel {
        private readonly IList<Shape> _addedShapes = new List<Shape>();
        private readonly Canvas _canvas;
        private IFieldObserverable _fieldObserverable;

        #region Initialization

        public FieldViewModel(IFieldObserverable fieldObserverable, Canvas canvas) {
            if (null == fieldObserverable) {
                throw new ArgumentNullException("fieldObserverable");
            }
            if (null == canvas) {
                throw new ArgumentNullException("canvas");
            }
            _canvas = canvas;
            Init(fieldObserverable);
        }

        public void Init(IFieldObserverable newFieldObserverable) {
            if (null == newFieldObserverable) {
                throw new ArgumentNullException("newFieldObserverable");
            }

            newFieldObserverable.FieldState += OnFieldChanged;

            _fieldObserverable = newFieldObserverable;
        }

        #endregion

        #region Events

        private void OnFieldChanged(Object sender, FieldStateChangedEventArs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }

            Width = e.Field.GetWidth();
            Height = e.Field.GetHeight();

            _canvas.Dispatcher.BeginInvoke(
                DispatcherPriority.Send,
                new Action<INotChanebleableField>(RedrawFieldOnCanvas),
                e.Field);
        }

        #endregion

        #region Redrawing

        public void Redraw() {
            _fieldObserverable.ForceNotify();
        }


        private void RedrawFieldOnCanvas(INotChanebleableField field) {
            var cellWidth = (_canvas.ActualWidth / field.GetWidth());
            var cellHeigth = (_canvas.ActualHeight / field.GetHeight());

            ClearCanvas();

            for (var i = 0; i < field.GetHeight(); ++i) {
                for (var j = 0; j < field.GetWidth(); ++j) {
                    var cellOnCanvas = CellToView.StaticCellToShape(field.GetCell(j, i), cellWidth, cellHeigth, j, i);
                    _addedShapes.Add(cellOnCanvas);
                    _canvas.Children.Add(cellOnCanvas);
                }
            }
        }

        public void ClearCanvas() {
            foreach (var addedShape in _addedShapes.Where(addedShape => _canvas.Children.Contains(addedShape))) {
                _canvas.Children.Remove(addedShape);
            }

            _addedShapes.Clear();
        }

        #endregion

        public int Width { get; private set; }
        public int Height { get; private set; }
    }
}