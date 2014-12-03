using System;
using System.Collections.Generic;
using System.Linq;
using PacMan_model.level.field;
using PacMan_model.util;

namespace PacMan_model.level.pathFinding {
    public static class PathFinder {

        private readonly static Random Random = new Random((int) DateTime.Now.Ticks);

        public static IEnumerable<Point> GetOrderedClosesNeighbors(
            this Point currentPoint,
            Point goalPoint,
            INotChanebleableField field) {
            return field.GetNeighbors(currentPoint)
                .Where(neighbor => neighbor.IsFreeForMoving())
                .OrderBy(neighbor => EuclidMetrict(neighbor.GetPosition(), goalPoint))
                .Select(neighbor => neighbor.GetPosition());
        }

        public static Point GetRandonNeighbor(this Point currentPoint, INotChanebleableField field) {
            var neighbors = field.GetNeighbors(currentPoint)
                .Where(neighbor => neighbor.IsFreeForMoving()).ToArray();

            return neighbors.ElementAt(Random.Next() % neighbors.Count()).GetPosition();
        }


        private static int ManhattanMetric(this Point first, Point second) {
            return Math.Abs(first.GetX() - second.GetX()) + Math.Abs(first.GetY() - second.GetY());
        }

        private static double EuclidMetrict(this Point first, Point second) {
            return
                Math.Sqrt(Math.Pow(first.GetX() - second.GetX(), 2) + Math.Pow(first.GetY() - second.GetY(), 2));
        }
    }
}