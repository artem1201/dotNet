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


        IEnumerable<StaticCell> GetNeighbors(Point cellPoint);
        IEnumerable<Point> GetNeighborsPoints(Point cellPoint);


//        IEnumerable<StaticCell> GetRemoteNeighbors(Point cellPoint, int distance);
//        IEnumerable<Point> GetRemoteNeighborsPoints(Point cellPoint, int distance);
    }


}
