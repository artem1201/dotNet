//  author: Artem Sumanev

using System;
using System.Collections.Generic;
using BinaryTree.util;

namespace BinaryTree.BinaryTree {
    /// <summary>
    /// represents node of binary tree
    /// </summary>
    public sealed class BinaryTreeNode {
        //  root of left subtree of current node
        private BinaryTreeNode _leftChild;
        //  root of right subtree of current node
        private BinaryTreeNode _rightChild;
        //  root node of current node
        private BinaryTreeNode _parent;

        //  current node's name
        private string _name;
        //  current node's content
        private string _content;
        //  set of current node's attributes
        private readonly IDictionary<string, string> _attributes;

        internal BinaryTreeNode() {
            _attributes = new Dictionary<string, string>();

            Position = new Point();
        }

        internal BinaryTreeNode(string name, string content)
            : this() {
            if (null == name) {
                throw new ArgumentNullException("name");
            }
            if (null == content) {
                throw new ArgumentNullException("content");
            }
            _name = name;
            _content = content;
        }

        #region Setters

        /// <summary>
        /// sets current node's left subtree's root
        /// sets parent of left child to current node
        /// </summary>
        /// <param name="leftChild">left subtree's root</param>
        /// <returns>current node</returns>
        internal BinaryTreeNode SetLeftChild(BinaryTreeNode leftChild) {
            if (null == leftChild) {
                throw new ArgumentNullException("leftChild");
            }

            _leftChild = leftChild;
            _leftChild._parent = this;

            return this;
        }

        /// <summary>
        /// sets current node's right subtree's root
        /// sets parent of right child to current node
        /// </summary>
        /// <param name="rightChild">right subtree's root</param>
        /// <returns>current node</returns>
        internal BinaryTreeNode SetRightChild(BinaryTreeNode rightChild) {
            if (null == rightChild) {
                throw new ArgumentNullException("rightChild");
            }

            _rightChild = rightChild;
            _rightChild._parent = this;

            return this;
        }

        internal BinaryTreeNode SetName(string name) {
            if (null == name) {
                throw new ArgumentNullException("name");
            }

            _name = name;

            return this;
        }

        internal BinaryTreeNode SetContent(string content) {
            if (null == content) {
                throw new ArgumentNullException("content");
            }

            _content = content;

            return this;
        }

        internal BinaryTreeNode AddAtribute(string attributeName, string value) {
            if (null == attributeName) {
                throw new ArgumentNullException("attributeName");
            }
            if (null == value) {
                throw new ArgumentNullException("value");
            }

            _attributes.Add(attributeName, value);

            return this;
        }

        internal BinaryTreeNode ClearAttributes() {
            _attributes.Clear();

            return this;
        }

        #endregion

        #region Getters

        public string GetName() {
            return _name;
        }

        public string GetContent() {
            return _content;
        }

        public BinaryTreeNode GetLeftChild() {
            return _leftChild;
        }

        public BinaryTreeNode GetRightChild() {
            return _rightChild;
        }

        public BinaryTreeNode GetParent() {
            return _parent;
        }

        public IDictionary<string, string> GetAttributes() {
            return _attributes;
        }

        #endregion

        public bool IsLeaf() {
            return null == _leftChild && null == _rightChild;
        }

        #region Displaying

        public Point Position { get; private set; }

        public BinaryTreeNode GetMostLeftOfChildrens() {
            if (IsLeaf()) {
                return this;
            }

            var result = this;
            if (null != _leftChild) {
                var mostLeftOfSubtree = _leftChild.GetMostLeftOfChildrens();
                if (mostLeftOfSubtree.Position.X < result.Position.X) {
                    result = mostLeftOfSubtree;
                }
            }

            if (null != _rightChild) {
                var mostLeftOfSubtree = _rightChild.GetMostLeftOfChildrens();
                if (mostLeftOfSubtree.Position.X < result.Position.X) {
                    result = mostLeftOfSubtree;
                }
            }

            return result;
        }

        public BinaryTreeNode GetMostRightOfChildrens() {
            if (IsLeaf()) {
                return this;
            }

            var result = this;
            if (null != _leftChild) {
                var mostLeftOfSubtree = _leftChild.GetMostRightOfChildrens();
                if (mostLeftOfSubtree.Position.X > result.Position.X) {
                    result = mostLeftOfSubtree;
                }
            }

            if (null != _rightChild) {
                var mostLeftOfSubtree = _rightChild.GetMostRightOfChildrens();
                if (mostLeftOfSubtree.Position.X > result.Position.X) {
                    result = mostLeftOfSubtree;
                }
            }

            return result;
        }

        #endregion
    }

    public static class BinaryTreeUtil {
        public static IEnumerable<BinaryTreeNode> PostOrderTravel(this BinaryTreeNode root) {
            if (null == root) {
                throw new ArgumentNullException("root");
            }

            //  bool is flag for visiting
            var nodeStack = new Stack<BinaryTreeForVisiting>();
            nodeStack.Push(new BinaryTreeForVisiting(root));

            while (0 != nodeStack.Count) {
                var currentNode = nodeStack.Peek();
                if (null != currentNode.Node.GetLeftChild() && !currentNode.LeftChildVisited) {
                    nodeStack.Push(new BinaryTreeForVisiting(currentNode.Node.GetLeftChild()));
                    currentNode.LeftChildVisited = true;
                }
                else if (null != currentNode.Node.GetRightChild() && !currentNode.RightChildVisited) {
                    nodeStack.Push(new BinaryTreeForVisiting(currentNode.Node.GetRightChild()));
                    currentNode.RightChildVisited = true;
                }
                else {
                    nodeStack.Pop();
                    yield return currentNode.Node;
                }
            }
        }

        public static int GetWidth(
            this BinaryTreeNode root,
            int nodeSize,
            int distanceBetweenColumns) {
            if (null == root) {
                throw new ArgumentNullException("root");
            }

            if (nodeSize <= 0) {
                throw new ArgumentOutOfRangeException("nodeSize");
            }

            if (distanceBetweenColumns <= 0) {
                throw new ArgumentOutOfRangeException("distanceBetweenColumns");
            }

            var numberOfNodesInHorizontal = root.GetMostRightOfChildrens().Position.X;
            var horisontalOffsetsNumber = numberOfNodesInHorizontal + 1;


            return numberOfNodesInHorizontal * nodeSize + horisontalOffsetsNumber * distanceBetweenColumns;
        }

        public static int GetHeight(
            this BinaryTreeNode root,
            int nodeSize,
            int distanceBetweenRows) {
            if (null == root) {
                throw new ArgumentNullException("root");
            }

            if (nodeSize <= 0) {
                throw new ArgumentOutOfRangeException("nodeSize");
            }

            if (distanceBetweenRows <= 0) {
                throw new ArgumentOutOfRangeException("distanceBetweenRows");
            }

            var numberOfNodesInVertical = root.Position.Y;
            var verticalOffsetsNumber = root.Position.Y + 1;

            return numberOfNodesInVertical * nodeSize + verticalOffsetsNumber * distanceBetweenRows;
        }

        private class BinaryTreeForVisiting {
            public BinaryTreeNode Node { get; private set; }
            public bool LeftChildVisited { get; set; }
            public bool RightChildVisited { get; set; }

            public BinaryTreeForVisiting(BinaryTreeNode node) {
                if (null == node) {
                    throw new ArgumentNullException("node");
                }
                Node = node;
                LeftChildVisited = false;
                RightChildVisited = false;
            }
        }
    }
}