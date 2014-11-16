using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using PacMan_model.level.cells;

namespace PacMan_gui.ViewModel.level {
    internal static class CellToView {

        private delegate Shape CreateViewFromCell(double width, double height);

        private static readonly IDictionary<StaticCellFactory.StaticCellType, CreateViewFromCell> CellsCreateor;
        static CellToView() {

            CellsCreateor = new Dictionary<StaticCellFactory.StaticCellType, CreateViewFromCell> {
                {StaticCellFactory.StaticCellType.FreeSpace, CreateFreeSpace},
                {StaticCellFactory.StaticCellType.Wall, CreateWall},
                {StaticCellFactory.StaticCellType.PacDot, CreatePacDot},
                {StaticCellFactory.StaticCellType.Energizer, CreateEnergizer},
                {StaticCellFactory.StaticCellType.Fruit, CreateFruit}
            };
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
            return CreateEnergizer(width, height);
        }

        private static Shape CreatePacMan(double width, double height) {
            return new Ellipse {Width = width, Height = height, Fill = Brushes.Yellow};
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
    }

    
}
