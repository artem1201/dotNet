using System;
using PacMan_model.util;

namespace PacMan_model.level.cells.ghosts {
    public interface IGhost : ICellWithCost, IGhostObserverable, IDisposable {

        

        void SetTarget(MovingCell target);
        void SetField(INotChanebleableField field);


        void Move();

        void MakeStalker();
        void MakeFrighted();

        Point GetPosition();
        string GetName();

        void Restart();

        void Die();

    }

    
}
