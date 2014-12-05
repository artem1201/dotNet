using System;
using System.Collections.Generic;

namespace PacMan_model.util {
    public sealed class Point : IEquatable<Point> {
        private int _x;
        private int _y;

        public Point(int x, int y) {
//            if (x < 0) {
//                throw new ArgumentOutOfRangeException("x");
//            }
//
//            if (y < 0) {
//                throw new ArgumentOutOfRangeException("y");
//            }

            _x = x;
            _y = y;
        }

        public bool Equals(Point other) {
            if (null == other) {
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
    public class PointComparer : IEqualityComparer<Point> {
        public bool Equals(Point x, Point y) {
            if (null == x) {
                throw new ArgumentNullException("x");
            }
            if (null == y) {
                throw new ArgumentNullException("y");
            }
            return x.Equals(y);
        }

        public int GetHashCode(Point obj) {
            if (null == obj) {
                throw new ArgumentNullException("obj");
            }
            return obj.GetHashCode();
        }
    }
}