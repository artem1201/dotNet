
using System;
using System.Collections.Generic;

namespace PacMan_model.util {

    public class Point : IEquatable<Point>
    {
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

        public bool Equals(Point other) {
            if (other == null) {
                return false;
            }
            return _x == other._x && _y == other._y;
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

    //TODO: remove if it is default
    public class PointComparer : IEqualityComparer<Point>
    {
        public bool Equals(Point x, Point y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Point obj)
        {
            return obj.GetHashCode();
        }
    }
    
}
