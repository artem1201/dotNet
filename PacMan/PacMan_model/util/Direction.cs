using System;

namespace PacMan_model.util {

    public sealed class Direction {
        private readonly Point _basisVector;

        private Direction(Point point) {
            _basisVector = point;
        }

        public static int Up = 0;
        public static int Down = 1;
        public static int Left = 2;
        public static int Right = 3;

        public static Direction[] Directions = {
            new Direction(new Point(0, 1)),
            new Direction(new Point(0, -1)),
            new Direction(new Point(-1, 0)),
            new Direction(new Point(1, 0))
        };

        

        public Point GetNear(Point point) {
            if (null == point) {
                throw new ArgumentNullException("point");
            }
            return new Point(_basisVector.GetX() + point.GetX(), _basisVector.GetY() + point.GetY());
        }

        /// <summary>
        /// Called on some direction
        /// if you can move from second point to first point due to this direction
        /// method returns true
        /// </summary>
        /// <param name="goal">point where you may will be</param>
        /// <param name="start">point where you may be</param>
        /// <returns>
        /// if you can move from second point to first point due to this direction
        /// method returns true
        /// else return false
        /// </returns>
        public bool IsDirectionFromOneNeighborToSecond(Point goal, Point start) {
            if (null == goal) {
                throw new ArgumentNullException("goal");
            }
            if (null == start) {
                throw new ArgumentNullException("start");
            }

            return GetNear(start).Equals(goal);
        }
    }
}