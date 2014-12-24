using System;
using System.Linq;
using System.Xml.Linq;

namespace BinaryTree.BinaryTree.Loaders {
    internal static class BinaryTreeLoader {
        public static BinaryTreeNode LoadTreeFrom(string path) {
            if (null == path) {
                throw new ArgumentNullException("path");
            }

            if (!BinaryTreeXmlParameters.IsPathNameCorrenct(path)) {
                throw new InvalidBinaryTreeFile(path, InvalidBinaryTreeFile.InvalidFileMessage);
            }

            //TODO: rewrite without recursion
            try {
                //  load xml from file
                var input = XDocument.Load(path);

                //  check if there is data
                if (null == input.Root) {
                    throw new InvalidBinaryTreeFile(path, InvalidBinaryTreeFile.InvalidFileMessage);
                }

                //  check if there is only one root element
                if (1 != input.Root.Elements().Count()) {
                    throw new InvalidBinaryTreeFile(path, InvalidBinaryTreeFile.TooManyRootElementsMessage);
                }

                //  proccess root element to binary tree
                return XmlElementToBinaryTreeNode(path, input.Root.Elements().Single());
            }
            catch (InvalidBinaryTreeFile) {
                throw;
            }
            catch (Exception e) {
                throw new InvalidBinaryTreeFile(path, e.Message);
            }
        }

        private static BinaryTreeNode XmlElementToBinaryTreeNode(string path, XElement rootElement) {
            if (null == path) {
                throw new ArgumentNullException("path");
            }
            if (null == rootElement) {
                throw new ArgumentNullException("rootElement");
            }
            //  check if sent element has only two or none child elements
            if (2 != rootElement.Elements().Count() || 0 != rootElement.Elements().Count()) {
                throw new InvalidBinaryTreeFile(path, "element has not two children: " + rootElement);
            }

            //TODO: rewrite with loop

            //  create another node with name and content from element
            var result = new BinaryTreeNode(rootElement.Name.ToString(), rootElement.Value);
            //  add attributes to node
            foreach (var attribute in rootElement.Attributes()) {
                result.AddAtribute(attribute.Name.ToString(), attribute.Value);
            }

            var firstElement = rootElement.Elements().First();
            var secondElement = rootElement.Elements().Last();

            return result
                .SetLeftChild(XmlElementToBinaryTreeNode(path, firstElement))
                .SetRightChild(XmlElementToBinaryTreeNode(path, secondElement));
        }
    }

    internal sealed class InvalidBinaryTreeFile : Exception {
        public const string TooManyRootElementsMessage = "root element is not unique";
        public const string InvalidFileMessage = "input file is invalid";

        public string Path { get; private set; }
        public string Cause { get; private set; }

        internal InvalidBinaryTreeFile(string path, string cause) {
            if (null == path) {
                throw new ArgumentNullException("path");
            }
            if (null == cause) {
                throw new ArgumentNullException("cause");
            }
            Cause = cause;
            Path = path;
        }
    }
}