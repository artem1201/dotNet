using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using PacMan_model.level;

namespace PacMan_gui.ViewModel.level {
    internal class FieldViewModel {

        public int Width { get; private set; }
        public int Height { get; private set; }

        //private INotChanebleableField _field;

        private readonly Canvas _canvas;
        private IFieldObserverable _fieldObserverable;

        private readonly IList<Shape> _addedShapes = new List<Shape>(); 

        public FieldViewModel(IFieldObserverable fieldObserverable, Canvas canvas) {
            if (null == fieldObserverable) {
                throw new ArgumentNullException("fieldObserverable");
            }
            if (null == canvas) {
                throw new ArgumentNullException("canvas");
            }
            _canvas = canvas;
            Init(fieldObserverable);

            //Redraw();
        }

        public void Init(IFieldObserverable newFieldObserverable) {
            if (null == newFieldObserverable) {
                throw new ArgumentNullException("newFieldObserverable");
            }

            newFieldObserverable.FieldState += OnFieldChanged;

            _fieldObserverable = newFieldObserverable;
        }

        private void OnFieldChanged(Object sender, FieldStateChangedEventArs e) {

            if (null == e) {
                throw new ArgumentNullException("e");
            }

            Width = e.Field.GetWidth();
            Height = e.Field.GetHeight();

            //System.Console.WriteLine("call redraw field on: " + Width + ":" + Height);

            _canvas.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action<INotChanebleableField>(RedrawFieldOnCanvas), e.Field);
        }

        public void Redraw() {
            _fieldObserverable.ForceNotify();
        }

       
        private void RedrawFieldOnCanvas(INotChanebleableField field) {

            double cellWidth = (_canvas.ActualWidth / field.GetWidth());
            double cellHeigth = (_canvas.ActualHeight / field.GetHeight());

            //System.Console.WriteLine("redraw field on: " + field.GetWidth() + ":" + field.GetHeight());

            ClearCanvas();

            for (var i = 0; i < field.GetHeight(); ++i) {
                for (var j = 0; j < field.GetWidth(); ++j) {
                    var cellOnCanvas = CellToView.StaticCellToShape(field.GetCell(j, i), cellWidth, cellHeigth, j, i);
                    _addedShapes.Add(cellOnCanvas);
                    _canvas.Children.Add(cellOnCanvas);
                }
            }
        }

        private void ClearCanvas() {
            foreach (var addedShape in _addedShapes.Where(addedShape => _canvas.Children.Contains(addedShape))) {
                _canvas.Children.Remove(addedShape);
            }

            _addedShapes.Clear();
        }
    }
}
