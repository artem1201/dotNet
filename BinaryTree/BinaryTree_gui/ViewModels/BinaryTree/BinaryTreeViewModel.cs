//  author: Artem Sumanev

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using BinaryTree.BinaryTree;
using BinaryTree_gui.Views;

namespace BinaryTree_gui.ViewModels.BinaryTree {
    internal sealed class BinaryTreeViewModel {
        private const int NodeSize = 50;
        private const int DistanceBetweenColumns = 50;
        private const int DistanceBetweenRows = 50;
        private const int ConnectionLineThickness = 2;

        private static readonly Brush NodeColor = Brushes.Red;
        private static readonly Brush ConnectionLineColor = Brushes.Black;

        //  contains node and its shape
        private readonly IDictionary<BinaryTreeNode, Shape> _binaryTreeNodeShapes =
            new Dictionary<BinaryTreeNode, Shape>();

        private Canvas _canvas;

        public BinaryTreeViewModel() {}

        public BinaryTreeViewModel(BinaryTreeNode rootNode) {
            if (null == rootNode) {
                throw new ArgumentNullException("rootNode");
            }

            Init(rootNode);
        }

        public Canvas Init(BinaryTreeNode rootNode) {
            if (null == rootNode) {
                throw new ArgumentNullException("rootNode");
            }

            _canvas = new Canvas();

            var width = rootNode.GetWidth(NodeSize, DistanceBetweenColumns);
            var height = rootNode.GetHeight(NodeSize, DistanceBetweenRows);

            //  initialize node shapes
            foreach (var binaryTreeNode in rootNode.PostOrderTravel()) {
                _binaryTreeNodeShapes.Add(binaryTreeNode, BinaryTreeNodeToShape(binaryTreeNode, width, height));
            }


            //  bind node and its childs with line
            //  add drag&drop
            foreach (var binaryTreeNodeShape in _binaryTreeNodeShapes) {
                var node = binaryTreeNodeShape.Key;
                var nodeShape = binaryTreeNodeShape.Value;
                if (null != node.GetLeftChild()) {
                    var connectionToLeftChild = GetConnectionBetweenNodes(
                        node,
                        node.GetLeftChild(),
                        width,
                        height);
                    BindNodes(nodeShape, _binaryTreeNodeShapes[node.GetLeftChild()], connectionToLeftChild);
                    _canvas.Children.Add(connectionToLeftChild);
                }
                if (null != node.GetRightChild()) {
                    var connectionToRightChild = GetConnectionBetweenNodes(
                        node,
                        node.GetRightChild(),
                        width,
                        height);
                    BindNodes(nodeShape, _binaryTreeNodeShapes[node.GetRightChild()], connectionToRightChild);
                    _canvas.Children.Add(connectionToRightChild);
                }

                nodeShape.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
                nodeShape.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
                nodeShape.Cursor = Cursors.Hand;

                _canvas.Children.Add(nodeShape);
            }

            _canvas.PreviewMouseMove += OnMouseMove;

            _canvas.Width = width;
            _canvas.Height = height;

            return _canvas;
        }

        #region Shapes

        private static Shape BinaryTreeNodeToShape(BinaryTreeNode node, int canvasWidth, int canvasHeight) {
            if (null == node) {
                throw new ArgumentNullException("node");
            }
            if (canvasWidth <= 0) {
                throw new ArgumentOutOfRangeException("canvasWidth");
            }
            if (canvasHeight <= 0) {
                throw new ArgumentOutOfRangeException("canvasHeight");
            }
            var nodeShape = new Ellipse {
                Width = NodeSize,
                Height = NodeSize,
                Fill = NodeColor,
                ToolTip =
                    new ToolTip {
                        Content = new BinaryTreeContentView(node.GetName(), node.GetContent(), node.GetAttributes())
                    }
            };

            Canvas.SetLeft(nodeShape, CalculateXCoordinate(node, canvasWidth));
            Canvas.SetTop(
                nodeShape,
                CalculateYCoordinate(node, canvasHeight));

            Panel.SetZIndex(nodeShape, 1);

            return nodeShape;
        }

        private static void BindNodes(Shape first, Shape second, Shape bindShape) {
            if (null == first) {
                throw new ArgumentNullException("first");
            }
            if (null == second) {
                throw new ArgumentNullException("second");
            }

            var x1 = new Binding {
                Path = new PropertyPath(Canvas.LeftProperty),
                Converter = new MyConverter(),
                ConverterParameter = first
            };
            var y1 = new Binding {
                Path = new PropertyPath(Canvas.TopProperty),
                Converter = new MyConverter(),
                ConverterParameter = first
            };
            var x2 = new Binding {
                Path = new PropertyPath(Canvas.LeftProperty),
                Converter = new MyConverter(),
                ConverterParameter = second
            };
            var y2 = new Binding {
                Path = new PropertyPath(Canvas.TopProperty),
                Converter = new MyConverter(),
                ConverterParameter = second
            };

            x1.Source = y1.Source = first;
            x2.Source = y2.Source = second;

            bindShape.SetBinding(Line.X1Property, x1);
            bindShape.SetBinding(Line.Y1Property, y1);
            bindShape.SetBinding(Line.X2Property, x2);
            bindShape.SetBinding(Line.Y2Property, y2);
        }

        private static Shape GetConnectionBetweenNodes(
            BinaryTreeNode firstNode,
            BinaryTreeNode secondNode,
            int canvasWidth,
            int canvasHeight) {
            if (null == firstNode) {
                throw new ArgumentNullException("firstNode");
            }
            if (null == secondNode) {
                throw new ArgumentNullException("secondNode");
            }
            if (canvasWidth <= 0) {
                throw new ArgumentOutOfRangeException("canvasWidth");
            }
            if (canvasHeight <= 0) {
                throw new ArgumentOutOfRangeException("canvasHeight");
            }

            var lineFromFirstToSecond = new Line {
                Stroke = ConnectionLineColor,
                StrokeThickness = ConnectionLineThickness
            };

            return lineFromFirstToSecond;
        }

        private static int CalculateXCoordinate(BinaryTreeNode node, int width) {
            if (null == node) {
                throw new ArgumentNullException("node");
            }
            if (width <= 0) {
                throw new ArgumentOutOfRangeException("width");
            }

            //  find x-center and offset with nodes and empty places between nodes
            return (node.Position.X * (NodeSize + DistanceBetweenColumns));
        }

        private static int CalculateYCoordinate(BinaryTreeNode node, int height) {
            if (null == node) {
                throw new ArgumentNullException("node");
            }
            if (height <= 0) {
                throw new ArgumentOutOfRangeException("height");
            }

            //  offset is border + nodes before current + length between layers + half of current node
            return (height)
                   - (DistanceBetweenRows + (node.Position.Y - 1) * (NodeSize + DistanceBetweenRows));
        }

        private class MyConverter : IValueConverter {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
                var d = (Double) value;

                return d + NodeSize / 2;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
                var d = (Double) value;

                return d - NodeSize / 2;
            }
        }

        #endregion

        #region DragAndDrop

        private Shape _movingNode;
        private double _firstXPos;
        private double _firstYPos;

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            _movingNode = sender as Shape;

            if (null == _movingNode) {}

            _firstXPos = e.GetPosition(_movingNode).X;
            _firstYPos = e.GetPosition(_movingNode).Y;
        }

        private void OnMouseMove(object sender, MouseEventArgs e) {
            if (MouseButtonState.Pressed == e.LeftButton) {
                Canvas.SetLeft(_movingNode, e.GetPosition(_movingNode.Parent as FrameworkElement).X - _firstXPos);
                Canvas.SetTop(_movingNode, e.GetPosition(_movingNode.Parent as FrameworkElement).Y - _firstYPos);

                //  check if have to shrink canvas width
                if (Canvas.GetLeft(_movingNode) + NodeSize > _canvas.Width) {
                    //  if moving node is out from canvas - resize canvas
                    //  moving node becomes most right
                    _canvas.Width = Canvas.GetLeft(_movingNode) + NodeSize;
                }

                //  check if have to shrink canvas height
                if (Canvas.GetTop(_movingNode) + NodeSize > _canvas.Height) {
                    //  if moving node is out from canvas - resize canvas
                    //  moving node becomes most lower
                    _canvas.Height = Canvas.GetTop(_movingNode) + NodeSize;
                }
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            _movingNode = null;
        }

        #endregion
    }
}