using System;
using System.Collections.Generic;
using System.Threading;
using PacMan_model.level.cells;
using PacMan_model.util;

namespace PacMan_model.level {
    class Field : IField {

        private int _width;
        private int _height;

        //  number of cells with Dots (PacDot, Energizer ...)
        private int _numberOfDots;

        private readonly Wall _wallAroundField = new Wall(new Point(0, 0));

        //  x - width
        //  y - height
        //  cell at (x, y)-position is cells.at(y * width + x)
        private IList<StaticCell> _cells;

        public Field() {}

        public Field(int width, int height, IList<StaticCell> cells) {

            Init(width, height, cells);
        }

        public event EventHandler<FieldStateChangedEventArs> FieldState;
        public void ForceNotify() {
            NotifyChangedStatement();
        }

        public void Init(int width, int height, IList<StaticCell> cells) {
            if (null == cells) {
                throw new ArgumentNullException("cells");
            }

            if (width <= 0) {
                throw new ArgumentOutOfRangeException("width");
            }
            if (height <= 0) {
                throw new ArgumentOutOfRangeException("height");
            }
            if (width * height != cells.Count) {
                throw new ArgumentException("Field initialization: invalid size of cells list");
            }
            
            
            _width = width;
            _height = height;

            _cells = cells;

            CalculateDots();

            //NotifyChangedStatement();
        }

        public int GetWidth() {
            return _width;
        }

        public int GetHeight() {
            return _height;
        }

        public int GetNumberOfDots() {
            return _numberOfDots;
        }

        public StaticCell GetCell(int x, int y) {

            if ((x >= _width) || (y >= _height)) {

                return _wallAroundField;
            }

            return _cells[y * _width + x];
        }

        public StaticCell GetCell(Point p) {
            return GetCell(p.GetX(), p.GetY());
        }

        public IList<StaticCell> GetCells() {
            return _cells;
        }

        public void SetCell(int x, int y, StaticCell cell) {
            if (null == cell) {
                throw new ArgumentNullException("cell");
            }

            if ((x >= _width) || (y >= _height)) {

                throw new CellOutOfField(new Point(x, y));
            }


            //  if old cell is cell with cost
            //  if new cell is not cell with cost
            //  decrement number of cells with costs
            if ( (_cells[y * _width + x] is ICellWithCost) && (false == (cell is ICellWithCost)) ) {

                --_numberOfDots;
            }
            //  if old cell is not cell with cost
            //  if new cell is cell with cost
            //  increment number of cell 
            else if ( (false == (_cells[y * _width + x] is ICellWithCost)) && (cell is ICellWithCost)) {

                ++_numberOfDots;
            }

            _cells[y * _width + x] = cell;

            NotifyChangedStatement();
        }

        public void SetSell(Point p, StaticCell cell) {
            if (null == p) {
                throw new ArgumentNullException("p");
            }
            if (null == cell) {
                throw new ArgumentNullException("cell");
            }
            SetCell(p.GetX(), p.GetY(), cell);
        }


        protected virtual void OnStatementChanged(FieldStateChangedEventArs e) {
            if (null == e) {
                throw new ArgumentNullException("e");
            }
            var temp = Volatile.Read(ref FieldState);

            if (temp != null) temp(this, e);
        }

        private void NotifyChangedStatement() {
            var e = new FieldStateChangedEventArs(_width, _height, _cells, (_numberOfDots == 0));
            OnStatementChanged(e);
        }


        private void CalculateDots() {

            foreach (var cell in _cells) {
                if (cell is ICellWithCost) {
                    ++_numberOfDots;
                }
            }

        }
    }
}
