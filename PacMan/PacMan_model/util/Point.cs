using System;
using System.Collections.Generic;
using System.Linq;

namespace PacMan_model.util {
    public sealed class Point : IEquatable<Point> {
        private readonly int _x;
        private readonly int _y;

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

        public int GetY() {
            return _y;
        }

        public int ManhattanMetric(Point other) {
            return Math.Abs(GetX() - other.GetX()) + Math.Abs(GetY() - other.GetY());
        }

        public double EuclidMetrict(Point other) {
            return
                Math.Sqrt(Math.Pow(GetX() - other.GetX(), 2) + Math.Pow(GetY() - other.GetY(), 2));
        }
    }

    public static class PointUtil {
        public static IEnumerable<Point> GetSquareAround(this Point point, int radius) {
            if (null == point) {
                throw new ArgumentNullException("point");
            }
            if (0 >= radius) {
                throw new ArgumentOutOfRangeException("radius");
            }

            var corners = CornersOfSquareAroundPoint.GetCornersOfSquareAroundPoint(point, radius);

            for (var y = corners.LeftBottomPoint.GetY(); y < corners.RightTopPoint.GetY(); ++y) {
                for (var x = corners.LeftBottomPoint.GetX(); x < corners.RightTopPoint.GetX(); ++x) {
                    yield return new Point(x, y);
                }
            }
        }

        public static bool IsPointInsideSquare(this Point point, Point center, int radius) {
            if (null == point) {
                throw new ArgumentNullException("point");
            }
            if (null == center) {
                throw new ArgumentNullException("center");
            }
            if (0 >= radius) {
                throw new ArgumentOutOfRangeException("radius");
            }

            var corners = CornersOfSquareAroundPoint.GetCornersOfSquareAroundPoint(center, radius);

            if ((corners.LeftBottomPoint.GetX() < point.GetX()) &&
                (point.GetX() < corners.RightTopPoint.GetX()) && (corners.LeftBottomPoint.GetY() < point.GetY())
                && (point.GetY() < corners.RightTopPoint.GetY())) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// corners of square around some other point
        /// </summary>
        private struct CornersOfSquareAroundPoint {
            public Point LeftBottomPoint { get; private set; }
            public Point RightTopPoint { get; private set; }

            private Point CenterPoint { get; set; }

            private CornersOfSquareAroundPoint(Point leftBottomPoint, Point rightTopPoint, Point centerPoint)
                : this() {
                if (null == leftBottomPoint) {
                    throw new ArgumentNullException("leftBottomPoint");
                }
                if (null == rightTopPoint) {
                    throw new ArgumentNullException("rightTopPoint");
                }
                if (null == centerPoint) {
                    throw new ArgumentNullException("centerPoint");
                }
                LeftBottomPoint = leftBottomPoint;
                RightTopPoint = rightTopPoint;
                CenterPoint = centerPoint;
            }

            public static CornersOfSquareAroundPoint GetCornersOfSquareAroundPoint(Point center, int radius) {
                var neighbors =
                    Enumerable
                        .Repeat(center, Direction.Directions.Length)
                        .Zip(Direction.Directions, (point1, direction) => direction.GetNearWithDistance(point1, radius))
                        .ToArray();

                var xx = neighbors.Select(point1 => point1.GetX()).ToArray();
                var yy = neighbors.Select(point1 => point1.GetY()).ToArray();

                return new CornersOfSquareAroundPoint(
                    new Point(xx.Min(), yy.Min()),
                    new Point(xx.Max(), yy.Max()),
                    center);
            }
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