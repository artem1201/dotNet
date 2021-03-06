﻿//  author: Artem Sumanev

using System.Collections.Generic;
using PacMan_model.level.cells;
using PacMan_model.util;

namespace PacMan_model.level.field {
    public interface INotChanebleableField : IFieldObserverable {
        int GetWidth();
        int GetHeight();

        StaticCell GetCell(int x, int y);
        StaticCell GetCell(Point p);

        IList<StaticCell> GetCells();


        IEnumerable<StaticCell> GetNeighbors(Point cellPoint);
        IEnumerable<Point> GetNeighborsPoints(Point cellPoint);
    }
}