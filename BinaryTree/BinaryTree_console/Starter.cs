//  author: Artem Sumanev

using System;
using System.Drawing.Imaging;
using BinaryTree.BinaryTree;
using BinaryTree.BinaryTree.Loaders;

namespace BinaryTree_console {
    internal sealed class Starter {
        private static readonly ImageFormat FormatToSave = ImageFormat.Png;
        private static readonly string OutputFileExtension = "." + FormatToSave.ToString().ToLower();

        public static void Main(string[] args) {
            if (null == args) {
                throw new ArgumentNullException("args");
            }

            try {
                Run(new InputParameters(args));
            }
            catch (InvalidInputParameters e) {
                Console.Error.WriteLine("Error: " + e.Cause);
            }
        }

        private static void Run(InputParameters parameters) {
            BinaryTreeNode rootNode;

            try {
                rootNode = BinaryTreeLoader.LoadTreeFrom(parameters.FromFile);

                rootNode.DisplayBinaryTreeWithReingoldTilford();
            }
            catch (InvalidBinaryTreeFile e) {
                throw new InvalidInputParameters(
                    InvalidInputParameters.GetInvalidInputFileMessage(parameters.FromFile, e.Cause));
            }
            catch (Exception) {
                throw new InvalidInputParameters(InvalidInputParameters.GetInvalidInputFileMessage(parameters.FromFile));
            }

            try {
                var image = rootNode.BinaryTreeToImage();
                image.Save(parameters.ToFile, FormatToSave);
            }
            catch (Exception) {
                throw new InvalidInputParameters(InvalidInputParameters.GetInvalidOutputFileMessage(parameters.ToFile));
            }
        }

        private struct InputParameters {
            public string FromFile { get; private set; }
            public string ToFile { get; private set; }

            public InputParameters(string[] args) : this() {
                if (null == args) {
                    throw new ArgumentNullException("args");
                }

                if (2 != args.Length) {
                    throw new InvalidInputParameters(
                        InvalidInputParameters.GetInvalidNumberOfParametersMessage(2, args.Length));
                }

                FromFile = args[0];
                ToFile = args[1];

                if (!ToFile.EndsWith(OutputFileExtension)) {
                    throw new InvalidInputParameters(
                        InvalidInputParameters
                            .GetInvalidOutputFileMessage(ToFile, "invalid extenstion, expected: " + OutputFileExtension));
                }
            }
        }

        private class InvalidInputParameters : Exception {
            public static string GetInvalidNumberOfParametersMessage(int expected, int actual) {
                return "Invalid number of parameters: " + actual + ". Expected: " + expected;
            }

            public static string GetInvalidInputFileMessage(string path, string cause) {
                if (null == path) {
                    throw new ArgumentNullException("path");
                }
                if (null == cause) {
                    throw new ArgumentNullException("cause");
                }
                return GetInvalidInputFileMessage(path) + " cause: " + cause;
            }

            public static string GetInvalidInputFileMessage(string path) {
                if (null == path) {
                    throw new ArgumentNullException("path");
                }

                return "Cannot load from file \"" + path + "\"";
            }

            public static string GetInvalidOutputFileMessage(string path, string cause) {
                if (null == path) {
                    throw new ArgumentNullException("path");
                }
                if (null == cause) {
                    throw new ArgumentNullException("cause");
                }

                return GetInvalidOutputFileMessage(path) + " cause: " + cause;
            }

            public static string GetInvalidOutputFileMessage(string path) {
                if (null == path) {
                    throw new ArgumentNullException("path");
                }

                return "Cannot save to file \"" + path + "\"";
            }


            public string Cause { get; private set; }

            public InvalidInputParameters(string cause) {
                Cause = cause;
            }
        }
    }
}