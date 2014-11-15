
using System;

namespace PacMan_model.util {

    public class Point {
        protected bool Equals(Point other) {
            return _x == other._x && _y == other._y;
        }

        private int _x;
        private int _y;

        public Point(int x, int y) {

            if (x < 0) {
                throw new ArgumentOutOfRangeException("x");
            }

            if (y < 0) {
                throw new ArgumentOutOfRangeException("y");
            }

            _x = x;
            _y = y;

        }
        
        public int GetX() {
            return _x;
        }

        public void SetX(int x) {
            _x = x;
        }

        public int GetY() {
            return _y;
        }

        public void SetY(int y) {
            _y = y;
        }
    }

    
}
