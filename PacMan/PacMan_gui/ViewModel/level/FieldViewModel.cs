using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using PacMan_model.level;

namespace PacMan_gui.ViewModel.level {
    internal class FieldViewModel {

        public int Width {
            get { return _field.GetWidth(); }
        }

        public int Height {
            get { return _field.GetHeight(); }
        }

        private INotChanebleableField _field;

        private Canvas _canvas;
        private IFieldObserverable _fieldObserverable;

        private readonly IList<Shape> _addedShapes = new List<Shape>(); 

        public FieldViewModel(IFieldObserverable field, Canvas canvas) {
            if (null == field) {
                throw new ArgumentNullException("field");
            }
            if (null == canvas) {
                throw new ArgumentNullException("canvas");
            }
            
            Init(field, canvas);

            //Redraw();
        }

        public void Init(IFieldObserverable field, Canvas canvas) {
            if (null == field) {
                throw new ArgumentNullException("field");
            }
            if (null == canvas) {
                throw new ArgumentNullException("canvas");
            }
            
            _canvas = canvas;
            _fieldObserverable = field;


            field.FieldState += OnFieldChanged;
        }

        private void OnFieldChanged(Object sender, EventArgs e) {

            if (null == e) {
                throw new ArgumentNullException("e");
            }

            var eventArgs = e as FieldStateChangedEventArs;
            if (null == eventArgs) {
                throw new ArgumentException("e");
            }

            _field = eventArgs.Field;

            _canvas.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new WorkWithCanvas(RedrawFieldOnCanvas));
        }

        public void Redraw() {
            _fieldObserverable.ForceNotify();
        }

        private delegate void WorkWithCanvas();

        private void RedrawFieldOnCanvas() {

            double cellWidth = (_canvas.ActualWidth / Width);
            double cellHeigth = (_canvas.ActualHeight / Height);
            
            System.Console.WriteLine("redraw field on: " + Width + ":" + Height);

            ClearCanvas();
           
            for (var i = 0; i < Height; ++i) {
                for (var j = 0; j < Width; ++j) {
                    var cellOnCanvas = CellToView.StaticCellToShape(_field.GetCell(j, i), cellWidth, cellHeigth, j, i);
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
