//  author: Artem Sumanev

using System;
using PacMan_model.util;

namespace PacMan_model.level.cells {
    /// <summary>
    ///     main class of all cells 
    ///     on level's field
    /// </summary>
    public abstract class Cell {
        protected Point Position;

        protected Cell(Point position) {
            if (null == position) {
                throw new ArgumentNullException("position");
            }
            Position = position;
        }

        public Point GetPosition() {
            return Position;
        }
    }
}