using System;
using PacMan_model.util;

namespace PacMan_model.level.cells {
    /// <summary>
    ///     cell's class
    ///     which can be moved on field
    /// </summary>
    public abstract class MovingCell : Cell {
        private readonly Point _startPosition;

        protected MovingCell(Point startPosition) : base(startPosition) {
            if (null == startPosition) {
                throw new ArgumentNullException("startPosition");
            }

            _startPosition = startPosition;
        }

        public Point GetStartPosition() {
            return _startPosition;
        }

        public abstract int GetSpeed();

        public abstract Direction GetCurrentDirection();
    }
}