//  author: Artem Sumanev

using System;

namespace BinaryTree.BinaryTree {
    public static class DisplayAlgorithms {
        public static void DisplayBinaryTreeWithReingoldTilford(this BinaryTreeNode rootNode) {
            if (null == rootNode) {
                throw new ArgumentNullException("rootNode");
            }

            foreach (var node in rootNode.PostOrderTravel()) {
                node.CalculateBinaryTreeNodeCoordinates();
            }
        }

        private static void CalculateBinaryTreeNodeCoordinates(this BinaryTreeNode node) {
            if (null == node) {
                throw new ArgumentNullException("node");
            }

            if (node.IsLeaf()) {
                node.Position.Set(1, 1);
            }
                //  there is only left subtree
            else if (null == node.GetRightChild()) {
                //  current node is above and to the right of its left subtree
                node.Position.Set(node.GetLeftChild().Position.X + 1, node.GetLeftChild().Position.Y + 1);
            }
                //  there is only right subtree
            else if (null == node.GetLeftChild()) {
                //  current node is above and to the left of its right subtree
                node.Position.Set(node.GetLeftChild().Position.X, node.GetRightChild().Position.Y + 1);
                //  move subtree to the right of its parent
                node.GetRightChild().ShiftTree(1, 0);
            }
            else {
                //  find most right x-coord of left subtree
                var leftChildMostRightX = node.GetLeftChild().GetMostRightOfChildrens().Position.X;
                //  find most left x-coord of right subtree
                var rightChildMostLeftX = node.GetRightChild().GetMostLeftOfChildrens().Position.X;
                //  move right subtree relatively to left subtree 
                //  so distance between most left coord of left subtree and most right coord of right tree
                //  will become 2

                if (node.GetLeftChild().Position.Y - node.GetRightChild().Position.Y >= 0) {
                    node.GetRightChild()
                        .ShiftTree(
                            leftChildMostRightX - rightChildMostLeftX + 2,
                            node.GetLeftChild().Position.Y - node.GetRightChild().Position.Y);
                }
                else {
                    node.GetRightChild()
                        .ShiftTree(
                            leftChildMostRightX - rightChildMostLeftX + 2,
                            0);

                    node.GetLeftChild().ShiftTree(0, node.GetRightChild().Position.Y - node.GetLeftChild().Position.Y);
                }


                //  move current node between its left and right subtrees
                node.Position.Set(
                    node.GetLeftChild().Position.X
                    + (node.GetRightChild().Position.X - node.GetLeftChild().Position.X) / 2,
                    node.GetLeftChild().Position.Y + 1);
            }
        }

        private static void ShiftTree(this BinaryTreeNode rootNode, int xOffset, int yOffset) {
            if (null == rootNode) {
                throw new ArgumentNullException("rootNode");
            }

            foreach (var node in rootNode.PostOrderTravel()) {
                node.Position.X += xOffset;
                node.Position.Y += yOffset;
            }
        }
    }
}