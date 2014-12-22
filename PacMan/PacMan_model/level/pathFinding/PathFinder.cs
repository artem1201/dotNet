//  author: Artem Sumanev

using System;
using System.Collections.Generic;
using System.Linq;
using PacMan_model.level.field;
using PacMan_model.util;

namespace PacMan_model.level.pathFinding {
    public static class PathFinder {
        private static readonly Random Random = new Random((int) DateTime.Now.Ticks);

        public static IEnumerable<Point> GetOrderedClosesNeighbors(
            this Point currentPoint,
            Point goalPoint,
            INotChanebleableField field) {
            return field.GetNeighbors(currentPoint)
                .Where(neighbor => neighbor.IsFreeForMoving())
                .OrderBy(neighbor => (neighbor.GetPosition().EuclidMetrict(goalPoint)))
                .Select(neighbor => neighbor.GetPosition());
        }

        public static Point GetRandonNeighbor(this Point currentPoint, INotChanebleableField field) {
            var neighbors = field.GetNeighbors(currentPoint)
                .Where(neighbor => neighbor.IsFreeForMoving()).ToArray();

            return neighbors.ElementAt(Random.Next() % neighbors.Count()).GetPosition();
        }
    }
}