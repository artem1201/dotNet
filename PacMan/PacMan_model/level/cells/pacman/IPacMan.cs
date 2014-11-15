using System;
using PacMan_model.util;

namespace PacMan_model.level.cells.pacman {
    public interface IPacMan : IPacManObserverable {



        void Move(Direction nextDirection);

        void MoveLeft();
        void MoveRight();
        void MoveUp();
        void MoveDown();

        Point GetPosition();
        int GetScore();
        int GetLives();

        void StartMovingTo(Point point, Action onEndOfMovement = null);

        void Eat(ICellWithCost cell);

        void Die();
    }


    
}
