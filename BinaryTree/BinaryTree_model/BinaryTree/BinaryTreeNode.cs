//  author: Artem Sumanev

using System;
using System.Collections.Generic;

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

        //  current node's position on net
        private Position _position;

        //  height of tree with current node as root
        private int _height;

        //  reference to child which has the most left position
        private BinaryTreeNode _mostLeftNodeOfTree;
        //  reference to child which has the most left position
        private BinaryTreeNode _mostRightNodeOfTree;

        public BinaryTreeNode() {
            _attributes = new Dictionary<string, string>();
            _mostLeftNodeOfTree = this;
            _mostRightNodeOfTree = this;
        }

        public BinaryTreeNode(string name, string content) : this() {
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
        public BinaryTreeNode SetLeftChild(BinaryTreeNode leftChild) {
            if (null == leftChild) {
                throw new ArgumentNullException("leftChild");
            }

            _leftChild = leftChild;
            _leftChild.SetParent(this);

            _height = CalculateHeightWithSubtree(_leftChild);
            FindBorderChild(_leftChild);

            return this;
        }

        /// <summary>
        /// sets current node's right subtree's root
        /// sets parent of right child to current node
        /// </summary>
        /// <param name="rightChild">right subtree's root</param>
        /// <returns>current node</returns>
        public BinaryTreeNode SetRightChild(BinaryTreeNode rightChild) {
            if (null == rightChild) {
                throw new ArgumentNullException("rightChild");
            }

            _rightChild = rightChild;
            _rightChild.SetParent(this);

            _height = CalculateHeightWithSubtree(_rightChild);
            FindBorderChild(_rightChild);

            return this;
        }

        private int CalculateHeightWithSubtree(BinaryTreeNode subtree) {
            if (subtree.GetHeight() + 1 > _height) {
                return subtree.GetHeight() + 1;
            }
            return _height;
        }

        private void FindBorderChild(BinaryTreeNode child) {
            if (null == child) {
                throw new ArgumentNullException("child");
            }
            if (child._mostLeftNodeOfTree.Position.IsToTheLeftOf(_mostLeftNodeOfTree.Position)) {
                _mostLeftNodeOfTree = child._mostLeftNodeOfTree;
            }

            if (child._mostRightNodeOfTree.Position.IsToTheRightOf(_mostRightNodeOfTree.Position)) {
                _mostRightNodeOfTree = child._mostRightNodeOfTree;
            }
        }

        /// <summary>
        /// sets current node's parent node
        /// </summary>
        /// <param name="parent">parent of current node</param>
        /// <returns>current node</returns>
        public BinaryTreeNode SetParent(BinaryTreeNode parent) {
            if (null == parent) {
                throw new ArgumentNullException("parent");
            }

            _parent = parent;

            return this;
        }

        public BinaryTreeNode SetName(string name) {
            if (null == name) {
                throw new ArgumentNullException("name");
            }

            _name = name;

            return this;
        }

        public BinaryTreeNode SetContent(string content) {
            if (null == content) {
                throw new ArgumentNullException("content");
            }

            _content = content;

            return this;
        }

        public BinaryTreeNode AddAtribute(string attributeName, string value) {
            if (null == attributeName) {
                throw new ArgumentNullException("attributeName");
            }
            if (null == value) {
                throw new ArgumentNullException("value");
            }

            _attributes.Add(attributeName, value);

            return this;
        }

        public BinaryTreeNode ClearAttributes() {
            _attributes.Clear();

            return this;
        }

        public BinaryTreeNode SetPosition(Position position) {
            _position = position;

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

        public int GetHeight() {
            return _height;
        }

        public BinaryTreeNode GetMostLeftNode() {
            return _mostLeftNodeOfTree;
        }

        public BinaryTreeNode GetMostRightNode() {
            return _mostRightNodeOfTree;
        }

        #endregion

        public bool IsLeaf() {
            return null == _leftChild && null == _rightChild;
        }

        public Position Position {
            get { return _position; }
            set { _position = value; }
        }
        /// <summary>
        /// moves from current position on sent number of position to left
        /// </summary>
        /// <param name="position">number of position</param>
        public void MoveToLeft(int position) {
            if (position < 0) {
                throw new ArgumentOutOfRangeException("position");
            }

            _position.MoveHorizontalRelatively(-position);
            if (null != _leftChild) {
                _leftChild.MoveToLeft(position);
            }
            if (null != _rightChild) {
                _rightChild.MoveToLeft(position);
            }
        }

        /// <summary>
        /// moves from current position on sent number of position to right
        /// </summary>
        /// <param name="position">number of position</param>
        public void MoveToRight(int position) {
            if (position < 0) {
                throw new ArgumentOutOfRangeException("position");
            }

            _position.MoveHorizontalRelatively(position);
            if (null != _leftChild) {
                _leftChild.MoveToRight(position);
            }
            if (null != _rightChild) {
                _rightChild.MoveToRight(position);
            }
        }
    }

    /// <summary>
    /// Coordinates of binary tree node on net
    /// </summary>
    public struct Position
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        public Position(int x, int y)
            : this()
        {
            X = x;
            Y = y;
        }

        public void MoveHorizontalRelatively(int steps)
        {
            X += steps;
        }

        public bool IsToTheLeftOf(Position other)
        {
            if (X < other.X)
            {
                return true;
            }

            return false;
        }

        public bool IsToTheRightOf(Position other)
        {
            if (X < other.X)
            {
                return true;
            }

            return false;
        }
    }
}