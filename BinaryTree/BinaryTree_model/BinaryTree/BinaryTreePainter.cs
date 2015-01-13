//  author: Artem Sumanev

using System;
using System.Drawing;

namespace BinaryTree.BinaryTree {
    public static class BinaryTreePainter {
        private const int NodeSize = 40;
        private const int LengthBetweenLayers = 30;
        private const int LengthBetweenColumns = 30;

        private static readonly Brush NodePen = Brushes.Red;
        private static readonly Pen LinePen = Pens.Black;

        public static Bitmap BinaryTreeToImage(this BinaryTreeNode rootNode) {
            if (null == rootNode) {
                throw new ArgumentNullException("rootNode");
            }


            var numberOfNodesInHorizontal = rootNode.GetMostRightOfChildrens().Position.X;
            var horisontalOffsetsNumber = numberOfNodesInHorizontal + 1;
           
            var numberOfNodesInVertical = rootNode.Position.Y;
            var verticalOffsetsNumber = rootNode.Position.Y + 1;
            
            var width = numberOfNodesInHorizontal * NodeSize + horisontalOffsetsNumber * LengthBetweenColumns;
            var height = numberOfNodesInVertical * NodeSize + verticalOffsetsNumber * LengthBetweenLayers;

            var bitmap = new Bitmap(width, height);


            using (var graphics = Graphics.FromImage(bitmap)) {
                foreach (var node in rootNode.PostOrder()) {
                    var currentPoint = new Point(
                        CalculateXCoordinate(node, width),
                        CalculateYCoordinate(node, height));

                    if (null != node.GetLeftChild()) {
                        var childPoint = new Point(
                            CalculateXCoordinate(node.GetLeftChild(), width),
                            CalculateYCoordinate(node.GetLeftChild(), height));

                        graphics.DrawLine(LinePen, currentPoint, childPoint);
                    }

                    if (null != node.GetRightChild()) {
                        var childPoint = new Point(
                            CalculateXCoordinate(node.GetRightChild(), width),
                            CalculateYCoordinate(node.GetRightChild(), height));
                        graphics.DrawLine(LinePen, currentPoint, childPoint);
                    }

                    //  cause Point is for center of ellipse - find left upper coords
                    graphics.FillEllipse(
                        NodePen,
                        currentPoint.X - (NodeSize / 2),
                        currentPoint.Y - (NodeSize / 2),
                        NodeSize,
                        NodeSize);
                }
            }

            return bitmap;
        }

        private static int CalculateXCoordinate(BinaryTreeNode node, int width) {
            if (null == node) {
                throw new ArgumentNullException("node");
            }
            if (0 >= width) {
                throw new ArgumentOutOfRangeException("width");
            }

            //  find x-center and offset with nodes and empty places between nodes
            return (node.Position.X * (NodeSize + LengthBetweenColumns));
        }

        private static int CalculateYCoordinate(BinaryTreeNode node, int height) {
            if (null == node) {
                throw new ArgumentNullException("node");
            }
            if (0 >= height) {
                throw new ArgumentOutOfRangeException("height");
            }

            //  offset is border + nodes before current + length between layers + half of current node
            return (height)
                   - (LengthBetweenLayers + (node.Position.Y - 1) * (NodeSize + LengthBetweenLayers) + NodeSize / 2);
        }
    }
}