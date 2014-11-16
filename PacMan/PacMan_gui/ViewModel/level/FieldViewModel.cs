using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using PacMan_model.level;
using PacMan_model.level.cells;

namespace PacMan_gui.ViewModel.level {
    internal class FieldViewModel {

        public int Width { get; private set; }
        public int Height { get; private set; }
        public IList<StaticCell> Cells { get; private set; }

        private readonly Canvas _canvas;
        private readonly IFieldObserverable _field;

        private readonly IList<Shape> _addedShapes = new List<Shape>(); 

        public FieldViewModel(IFieldObserverable field, Canvas canvas) {
            if (null == field) {
                throw new ArgumentNullException("field");
            }
            if (null == canvas) {
                throw new ArgumentNullException("canvas");
            }
            _canvas = canvas;
            _field = field;


            field.FieldState += OnFieldChanged;

            //Redraw();
        }

        private void OnFieldChanged(Object sender, EventArgs e) {

            if (null == e) {
                throw new ArgumentNullException("e");
            }

            var eventArgs = e as FieldStateChangedEventArs;
            if (null == eventArgs) {
                throw new ArgumentException("e");
            }

            Width = eventArgs.Width;
            Height = eventArgs.Height;
            Cells = eventArgs.Cells;

            _canvas.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new WorkWithCanvas(RedrawFieldOnCanvas));
        }

        public void Redraw() {
            _field.ForceNotify();
        }

        private delegate void WorkWithCanvas();

        private void RedrawFieldOnCanvas() {

            double cellWidth = (_canvas.ActualWidth / Width);
            double cellHeigth = (_canvas.ActualHeight / Height);
            
            ClearCanvas();
            
            _addedShapes.Clear();
            for (var i = 0; i < Height; ++i) {
                for (var j = 0; j < Width; ++j) {
                    var cellOnCanvas = CellToView.StaticCellToShape(Cells[i * Width + j], cellWidth, cellHeigth, j, i);
                    _addedShapes.Add(cellOnCanvas);
                    _canvas.Children.Add(cellOnCanvas);
                }
            }
        }

        private void ClearCanvas() {
            foreach (var addedShape in _addedShapes.Where(addedShape => _canvas.Children.Contains(addedShape))) {
                _canvas.Children.Remove(addedShape);
            }
        }
    }
}
