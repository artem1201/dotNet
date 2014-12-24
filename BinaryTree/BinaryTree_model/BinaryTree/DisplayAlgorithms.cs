using System;

namespace BinaryTree.BinaryTree {
    public static class DisplayAlgorithms {
        public static void DisplayBinaryTreeWithReingoldTilford(this BinaryTreeNode rootNode) {
            if (null == rootNode) {
                throw new ArgumentNullException("rootNode");
            }

            rootNode.SetPosition(new Position(0, rootNode.GetHeight()));

            //TODO: rewrite without recursion

            if (rootNode.IsLeaf()) {
                return;
            }

            if (null != rootNode.GetLeftChild()) {
                DisplayBinaryTreeWithReingoldTilford(rootNode.GetLeftChild());
            }

            if (null != rootNode.GetRightChild()) {
                DisplayBinaryTreeWithReingoldTilford(rootNode.GetRightChild());
            }

            if (null == rootNode.GetLeftChild() || null == rootNode.GetRightChild()) {
                return;
            }

            rootNode.GetLeftChild().MoveToLeft(1);
            rootNode.GetRightChild().MoveToRight(1);
        }
    }
}