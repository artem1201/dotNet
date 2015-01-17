//  author: Artem Sumanev

using System;
using System.Drawing.Imaging;
using System.Windows;
using BinaryTree.BinaryTree;
using BinaryTree.BinaryTree.Loaders;
using BinaryTree_gui.ViewModels.BinaryTree;
using Microsoft.Win32;

namespace BinaryTree_gui.Views {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        private BinaryTreeNode _loadedBinaryTreeNode;

        private const string ErrorTitle = "Error";
        private const string NoTreeLoadedMessage = "no tree loaded";

        private static readonly ImageFormat FormatToSave = ImageFormat.Png;
        private const string OutputFileFilter = "PNG Files (*.png)|*.png";
        private static readonly string OutputFileExtension = "." + FormatToSave.ToString().ToLower();

        public MainWindow() {
            InitializeComponent();
        }

        private void InitBinaryTreeView(BinaryTreeNode rootNode) {
            if (null == rootNode) {
                throw new ArgumentNullException("rootNode");
            }
            var binaryTreeViewModel = new BinaryTreeViewModel();
            var binaryTreeCanvas = binaryTreeViewModel.Init(rootNode);
            BinaryTreeCanvasScrollViewer.Content = binaryTreeCanvas;
        }

        //TODO: to commands
        //TODO: good errors check
        private void ExitItem_OnClick(object sender, RoutedEventArgs e) {
            Close();
        }

        private void OpenFileItem_OnClick(object sender, RoutedEventArgs e) {
            var openFileDialog = new OpenFileDialog {Multiselect = false};
            if (true == openFileDialog.ShowDialog(this)) {
                try {
                    _loadedBinaryTreeNode = BinaryTreeLoader.LoadTreeFrom(openFileDialog.FileName);
                }
                catch (Exception) {
                    MessageBox.Show(this, "cannot load this file", ErrorTitle);
                }

                try {
                    _loadedBinaryTreeNode.DisplayBinaryTreeWithReingoldTilford();
                }
                catch (Exception) {
                    _loadedBinaryTreeNode = null;
                    MessageBox.Show(this, "cannot display this file", ErrorTitle);
                }

                InitBinaryTreeView(_loadedBinaryTreeNode);
            }
        }

        private void SaveAsImageItem_OnClick(object sender, RoutedEventArgs e) {
            if (null != _loadedBinaryTreeNode) {
                var saveFileDialog = new SaveFileDialog {DefaultExt = OutputFileExtension, Filter = OutputFileFilter};
                if (true == saveFileDialog.ShowDialog(this)) {
                    try {
                        var image = _loadedBinaryTreeNode.BinaryTreeToImage();
                        image.Save(saveFileDialog.FileName, FormatToSave);
                    }
                    catch (Exception) {
                        MessageBox.Show(this, "cannot save in file", ErrorTitle);
                    }
                }
            }
            else {
                MessageBox.Show(this, NoTreeLoadedMessage, ErrorTitle);
            }
        }

        private void RestoreItem_OnClick(object sender, RoutedEventArgs e) {
            if (null != _loadedBinaryTreeNode) {
                InitBinaryTreeView(_loadedBinaryTreeNode);
            }
            else {
                MessageBox.Show(this, NoTreeLoadedMessage, ErrorTitle);
            }
        }
    }
}