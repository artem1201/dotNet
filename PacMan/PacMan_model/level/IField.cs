using System;
using System.Collections.Generic;
using PacMan_model.level.cells;
using PacMan_model.util;

namespace PacMan_model.level {
    public interface IField : INotChanebleableField, IFieldObserverable, IDisposable {
        void Init(int width, int height, IList<StaticCell> cells);

        int GetNumberOfDots();

        void SetCell(int x, int y, StaticCell cell);
        void SetSell(Point p, StaticCell cell);
    }


    internal class CellOutOfField : Exception {
        private readonly Point _where;

        public CellOutOfField(Point where) {
            _where = where;
        }

        public string GetMessage() {
            return "at " + _where.GetX() + ":" + _where.GetY();
        }
    }
}