//  author: Artem Sumanev

using System;

namespace BinaryTree.util {
    public class Point {
        public int X { get; internal set; }
        public int Y { get; internal set; }

        public void Set(Point other) {
            if (null == other) {
                throw new ArgumentNullException("other");
            }
            X = other.X;
            Y = other.Y;
        }

        public void Set(int x, int y) {
            X = x;
            Y = y;
        }
    }
}