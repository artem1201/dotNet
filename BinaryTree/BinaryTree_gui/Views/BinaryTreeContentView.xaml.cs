//  author: Artem Sumanev

using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace BinaryTree_gui.Views {
    /// <summary>
    /// Interaction logic for BinaryTreeContentView.xaml
    /// </summary>
    public partial class BinaryTreeContentView {
        public BinaryTreeContentView(string name, string content, IEnumerable<KeyValuePair<string, string>> attributes) {
            if (null == name) {
                throw new ArgumentNullException("name");
            }
            if (null == attributes) {
                throw new ArgumentNullException("attributes");
            }
            InitializeComponent();

            NodeName.Text = name;
            if (null != content) {
                NodeContent.Text = content;
            }
            else {
                Children.Remove(NodeContent);
            }

            foreach (var attribute in attributes) {
                AttributesPanel.Children.Add(new TextBlock {Text = (attribute.Key + " = " + attribute.Value)});
            }
        }
    }
}