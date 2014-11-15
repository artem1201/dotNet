using System;
using System.Collections.Generic;
using PacMan_model.level.cells;

namespace PacMan_model.level {
    public interface IFieldObserverable {

        event EventHandler<FieldStateChangedEventArs> FieldState;
        void ForceNotify();
    }

    public class FieldStateChangedEventArs : EventArgs {
        
        public FieldStateChangedEventArs(int width, int height, IList<StaticCell> cells, bool dotIsNoMore) {
            if (null == cells) {
                throw new ArgumentNullException("cells");
            }
            if (width <= 0) {
                throw new ArgumentOutOfRangeException("width");
            }
            if (height <= 0) {
                throw new ArgumentOutOfRangeException("height");
            }
            if (width * height != cells.Count) {
                throw new ArgumentException("Field initialization: invalid size of cells list");
            }

            Width = width;
            Height = height;
            Cells = cells;
            DotIsNoMore = dotIsNoMore;
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public IList<StaticCell> Cells { get; private set; }

        public bool DotIsNoMore { get; private set; }
    }
}
