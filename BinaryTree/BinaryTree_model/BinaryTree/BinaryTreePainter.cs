//  author: Artem Sumanev

using System;
using System.Drawing;

namespace BinaryTree.BinaryTree {
    public static class BinaryTreePainter {

        private const int NodeRadius = 20;
        private const int LengthBetweenLayers = 30;
        private const int LengthBetweenColumns = 30;

        public static Bitmap BinaryTreeToImage(this BinaryTreeNode rootNode) {
            if (null == rootNode) {
                throw new ArgumentNullException("rootNode");
            }

            //  +2 is for borders
            var binaryTreeWidth = (Math.Abs(rootNode.GetMostLeftNode().Position.X)
                         + Math.Abs(rootNode.GetMostRightNode().Position.X)) + 2;
            //  +1 is for borders
            var binaryTreeHeght = rootNode.GetHeight() + 1;

            var width = binaryTreeWidth * (2 * NodeRadius + LengthBetweenColumns);
            var height = binaryTreeHeght * LengthBetweenLayers;

            var bitmap = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(bitmap)) {
                
            }

            return bitmap;
        }

        private static readonly Pen NodePen = Pens.Red;
        private static void DrawBinaryTree(this BinaryTreeNode root, Graphics graphics, int xAlignment, int yAlignment) {
            if (null == root) {
                throw new ArgumentNullException("root");
            }
            if (null == graphics) {
                throw new ArgumentNullException("graphics");
            }

        }
    }
}