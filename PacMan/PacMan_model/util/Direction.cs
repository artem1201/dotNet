using System;

namespace PacMan_model.util {
    public sealed class Direction {
        private readonly Point _basisVector;
        private readonly int _id;
        private Direction(Point point, int id) {
            _basisVector = point;
            _id = id;
        }

        public static int Up = 0;
        public static int Down = 1;
        public static int Left = 2;
        public static int Right = 3;

        public static Direction[] Directions = {
            new Direction(new Point(0, -1), Up),
            new Direction(new Point(0, 1), Down),
            new Direction(new Point(-1, 0), Left),
            new Direction(new Point(1, 0), Right)
        };

        private static readonly string[] DirectionNames = {
            "Up",
            "Down",
            "Left",
            "Right"
        };

        public string GetName() {
            return DirectionNames[_id];
        }


        /// <summary>
        /// returns point which is neighbor of sent point by direction
        /// </summary>
        /// <param name="point">point to find its neighbor</param>
        /// <returns>neighbor of sent point</returns>
        public Point GetNear(Point point) {
            if (null == point) {
                throw new ArgumentNullException("point");
            }
            return GetNearWithDistance(point, NearDistance);
        }

        public static readonly int NearDistance = 1;

        /// <summary>
        /// returns point which is separated from sent point 
        /// by sent number of points in sent direction
        /// if direction is Left and distance is 3 
        /// then method will return third point to the left of sent point
        /// </summary>
        /// <param name="point">point to find its neighbor</param>
        /// <param name="distance">number of separated points</param>
        /// <returns>neighbor of sent point</returns>
        public Point GetNearWithDistance(Point point, int distance) {
            if (null == point) {
                throw new ArgumentNullException("point");
            }
            if (0 >= distance) {
                throw new ArgumentOutOfRangeException("distance");
            }

            return new Point(
                distance * _basisVector.GetX() + point.GetX(),
                distance * _basisVector.GetY() + point.GetY());
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