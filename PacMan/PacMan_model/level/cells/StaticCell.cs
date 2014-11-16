using System;
using System.ComponentModel;
using System.Threading;
using PacMan_model.level.cells.pacman;
using PacMan_model.Properties;
using PacMan_model.util;

namespace PacMan_model.level.cells {

    /// <summary>
    ///     type of cell which will never be moved on field
    /// </summary>
    public abstract class StaticCell : Cell {
        protected StaticCell(Point position) : base(position) {}


        public abstract StaticCellType GetCellType();

        /// <summary>
        /// moves pacman to this cell
        /// </summary>
        /// <param name="pacman">pacman which is moved</param>
        /// <param name="field">field where pacman is moved</param>
        public abstract void HandlePacmanMovement(IPacMan pacman, IField field);


        public abstract bool IsFreeForMoving();
    }

    public enum StaticCellType {
        FreeSpace
        ,
        Wall
            ,
        PacDot
            ,
        Energizer
            , Fruit
    }

    internal static class StaticCellFactory {

        public static StaticCell CreateStaticCell(StaticCellType type, Point position) {
            if (null == position) {
                throw new ArgumentNullException("position");
            }

            switch (type) {
                case StaticCellType.FreeSpace:
                    return new FreeSpace(position);
                case StaticCellType.Wall:
                    return new Wall(position);
                case StaticCellType.PacDot:
                    return new PacDot(position);
                case StaticCellType.Energizer:
                    return new Energizer(position);
                case StaticCellType.Fruit:
                    //TODO: add Fruit
                    return new PacDot(position);
                default:
                    throw new InvalidEnumArgumentException(Resources.StaticCellFactory_CreateStaticCell_unknown_static_cell_type__ + type.ToString());
            }
        }
    }

    internal sealed class FreeSpace : StaticCell {
        public FreeSpace(Point position) : base(position) {}

        public override StaticCellType GetCellType() {
            return StaticCellType.FreeSpace;
        }

        public override void HandlePacmanMovement(IPacMan pacman, IField field) {
            if (null == pacman) {
                throw new ArgumentNullException("pacman");
            }

            pacman.StartMovingTo(Position);
        }

        public override bool IsFreeForMoving() {
            return true;
        }
    }


    internal sealed class Wall : StaticCell {
        public Wall(Point position) : base(position) {}

        public override StaticCellType GetCellType() {
            return StaticCellType.Wall;
        }

        public override void HandlePacmanMovement(IPacMan pacman, IField field) {
        }

        public override bool IsFreeForMoving() {
            return false;
        }
    }


    /// <summary>
    ///     type of cell with cost
    ///     its value would be added to score
    ///     if player get it
    /// </summary>
    public interface ICellWithCost {
 
        int GetCost();

    }


    internal sealed class PacDot : StaticCell, ICellWithCost {


        private const int Cost = 10;


        public PacDot(Point position) : base(position) {}

        public int GetCost() {
            return Cost;
        }

        public override StaticCellType GetCellType() {
            return StaticCellType.PacDot;
        }

        public override void HandlePacmanMovement(IPacMan pacman, IField field) {
            if (null == pacman) {
                throw new ArgumentNullException("pacman");
            }
            if (null == field) {
                throw new ArgumentNullException("field");
            }

            

            pacman.StartMovingTo(Position,
                delegate {
                    pacman.Eat(this);
                    field.SetSell(Position, new FreeSpace(Position));
                });
        }

        public override bool IsFreeForMoving() {
            return true;
        }
    }

    internal sealed class Energizer : StaticCell, ICellWithCost {

        private const int Cost = 50;

        public event EventHandler EnergizerEaten;

        public Energizer(Point position) : base(position) {}

        public int GetCost() {
            return Cost;
        }

        public override StaticCellType GetCellType() {
            return StaticCellType.Energizer;
        }

        public override void HandlePacmanMovement(IPacMan pacman, IField field) {
            
            if (null == pacman) {
                throw new ArgumentNullException("pacman");
            }
            if (null == field) {
                throw new ArgumentNullException("field");
            }

            

            

            pacman.StartMovingTo(Position,
                delegate {
                    pacman.Eat(this);
                    field.SetSell(Position, new FreeSpace(Position));
                    OnEnergizerEaten();
                });

            
        }

        public override bool IsFreeForMoving() {
            return true;
        }

        private void OnEnergizerEaten() {
            var temp = Volatile.Read(ref EnergizerEaten);

            if (temp != null) temp(this, EventArgs.Empty);
        }
    }

    //TODO: add fruits

}
