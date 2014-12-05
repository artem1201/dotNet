using PacMan_model.level.field;
using PacMan_model.util;

namespace PacMan_model.level.cells.ghosts {
    public interface IGhost : ICellWithCost, IGhostObserverable {

        

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
