//  author: Artem Sumanev

namespace BinaryTree.BinaryTree.Loaders {
    internal static class BinaryTreeXmlParameters {
        public static string TagName = "node";

        private const string FileNameExtension = ".xml";

        public static bool IsPathNameCorrenct(string path) {
            if (!path.EndsWith(FileNameExtension)) {
                return false;
            }

            return true;
        }
    }
}