using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using PacMan_gui.Annotations;
using PacMan_model.level.cells;
using PacMan_model.level.cells.ghosts;

namespace PacMan_gui.ViewModel.level {
    internal static class CellToView {

        private static readonly IDictionary<StaticCellType, Func<double, double, Shape>> CellsCreateor;
        private static readonly IDictionary<string, Func<double, double, Shape>> GhostsCreator;
        
        static CellToView() {

            CellsCreateor = new Dictionary<StaticCellType, Func<double, double, Shape>> {
                {StaticCellType.FreeSpace, CreateFreeSpace},
                {StaticCellType.Wall, CreateWall},
                {StaticCellType.PacDot, CreatePacDot},
                {StaticCellType.Energizer, CreateEnergizer},
                {StaticCellType.Fruit, CreateFruit}
            };

            GhostsCreator = new Dictionary<string, Func<double, double, Shape>>();

            //TODO: solve ghost names and functions in some other way
            foreach (var ghostName in GhostsInfo.OrderedPossibleGhostNames) {

                if (ghostName.Equals("Blinky")) {
                    GhostsCreator.Add(ghostName, CreateBlinky);
                }
                else if (ghostName.Equals("Pinky")) {
                    GhostsCreator.Add(ghostName, CreatePinky);
                }
                else if (ghostName.Equals("Inky")) {
                    GhostsCreator.Add(ghostName, CreateInky);
                }
                else if (ghostName.Equals("Clyde")) {
                    GhostsCreator.Add(ghostName, CreateClyde);
                }
                else {
                    GhostsCreator.Add(ghostName, CreateDefaultGhost);    
                }
            }
        }



        private static Shape CreateFreeSpace(double width, double height) {
            return new Rectangle {Width = width, Height = height};
        }
        private static Shape CreateWall(double width, double height) {
            return new Rectangle {Fill = Brushes.Black, Width = width, Height = height };
        }
        private static Shape CreatePacDot(double width, double height) {
            return new Ellipse {Width = width / 3, Height = height / 3, Fill = Brushes.White};
        }
        private static Shape CreateEnergizer(double width, double height) {
            return new Ellipse {Width = width / 2, Height = height / 2, Fill = Brushes.White};
        }
        private static Shape CreateFruit(double width, double height) {
            //TODO:
            return CreateEnergizer(width, height);
        }

        private static Shape CreatePacMan(double width, double height) {
            return new Ellipse {Width = width, Height = height, Fill = Brushes.Yellow};
        }

        private static Shape CreateBlinky(double width, double height) {
            return new Ellipse { Width = width, Height = height / 2, Fill = Brushes.Red };
        }
        private static Shape CreatePinky(double width, double height) {
            return new Ellipse { Width = width, Height = height / 2, Fill = Brushes.Pink };
        }
        private static Shape CreateInky(double width, double height) {
            return new Ellipse { Width = width, Height = height / 2, Fill = Brushes.Blue };
        }
        private static Shape CreateClyde(double width, double height) {
            return new Ellipse { Width = width, Height = height / 2, Fill = Brushes.Orange };
        }
        private static Shape CreateDefaultGhost(double width, double height) {
            return new Ellipse { Width = width, Height = height / 2, Fill = Brushes.Green };
        }

        private static void SetPositionOnCanvas(Shape shape, double widthOnCanvas, double heightOnCanvas, double x, double y) {
            double dx = 0;
            double dy = 0;
            //  put ellipse in center of canvas's cell
            if (shape is Ellipse) {
                dx = (widthOnCanvas - shape.Width) / 2;
                dy = (heightOnCanvas - shape.Height) / 2;
            }

            Canvas.SetLeft(shape, widthOnCanvas * x + dx);
            Canvas.SetTop(shape, heightOnCanvas * y + dy);
        }

        public static Shape StaticCellToShape(StaticCell cell, double widthOnCanvas, double heightOnCanvas, double x, double y, string name = null) {

            if (null == cell) {
                throw new ArgumentNullException("cell");
            }
            if (widthOnCanvas <= 0) {
                throw new ArgumentOutOfRangeException("widthOnCanvas");
            }
            if (heightOnCanvas <= 0) {
                throw new ArgumentOutOfRangeException("heightOnCanvas");
            }
            if (x < 0) {
                throw new ArgumentOutOfRangeException("x");
            }
            if (y < 0) {
                throw new ArgumentOutOfRangeException("y");
            }


            if (!CellsCreateor.ContainsKey(cell.GetCellType())) {
                throw new ArgumentException("unknown cell type: " + cell.GetCellType());
            }
            var result = CellsCreateor[cell.GetCellType()](widthOnCanvas, heightOnCanvas);

            
            SetPositionOnCanvas(result, widthOnCanvas, heightOnCanvas, x, y);

            if (null != name) {
                result.Name = name;
            }

            return result;
        }

        public static Shape PacmanToShape(double widthOnCanvas, double heightOnCanvas, double x, double y, string name = null) {

            var result = CreatePacMan(widthOnCanvas, heightOnCanvas);

            SetPositionOnCanvas(result, widthOnCanvas, heightOnCanvas, x, y);

            if (null != name) {
                result.Name = name;
            }

            return result;
        }

        public static Shape GhostToShape([NotNull] string name, double widthOnCanvas, double heightOnCanvas, double x, double y) {
            if (null == name) {
                throw new ArgumentNullException("name");
            }


            var result = GhostsCreator[name](widthOnCanvas, heightOnCanvas);


            SetPositionOnCanvas(result, widthOnCanvas, heightOnCanvas, x, y);

            return result;
        }
    }

    
}
