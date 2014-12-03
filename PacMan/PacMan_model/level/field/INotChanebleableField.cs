using System.Collections.Generic;
using PacMan_model.level.cells;
using PacMan_model.util;

namespace PacMan_model.level.field {
    
    public interface INotChanebleableField {
        int GetWidth();
        int GetHeight();

        StaticCell GetCell(int x, int y);
        StaticCell GetCell(Point p);

        IList<StaticCell> GetCells();

        StaticCell[] GetNeighbors(Point cellPoint);

        Point[] GetNeighborsPoints(Point cellPoint);
    }


}
