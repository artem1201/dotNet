using System;

namespace PacMan_model.util {
    /*
     * <summary>
     *  direction of moving objects on field
     * </summary>
     */

    public enum Direction {
        Left
        ,
        Right
        ,
        Up
        ,
        Down
    }

    public static class DirectoinUtil {
        public static Point GetLeftOf(this Point p) {
            return new Point(p.GetX() - 1, p.GetY());
        }

        public static Point GetRightOf(this Point p) {
            return new Point(p.GetX() + 1, p.GetY());
        }

        public static Point GetBelowOf(this Point p) {
            return new Point(p.GetX(), p.GetY() - 1);
        }

        public static Point GetAboveOf(this Point p) {
            return new Point(p.GetX(), p.GetY() + 1);
        }

        public static Point GetNearByDirection(this Point p, Direction direction) {
            switch (direction) {
                case Direction.Left:
                    return p.GetLeftOf();
                case Direction.Right:
                    return p.GetRightOf();
                case Direction.Up:
                    return p.GetAboveOf();
                case Direction.Down:
                    return p.GetBelowOf();
                default:
                    throw new ArgumentException("unknown direction" + direction.ToString());
            }
        }

        public static bool IsLeftOf(this Point point, Point otherPoint) {
            return point.GetLeftOf().Equals(otherPoint);
        }

        public static bool IsRightOf(this Point point, Point otherPoint) {
            return point.GetRightOf().Equals(otherPoint);
        }

        public static bool IsAboveOf(this Point point, Point otherPoint) {
            return point.GetAboveOf().Equals(otherPoint);
        }

        public static bool IsBelowOf(this Point point, Point otherPoint) {
            return point.GetBelowOf().Equals(otherPoint);
        }
    }
}