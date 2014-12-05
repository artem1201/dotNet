﻿using System;
using PacMan_model.util;

namespace PacMan_model.level.cells.pacman {
    public interface IPacMan : IPacManObserverable {



        void Move(Direction nextDirection);

        Point GetPosition();
        int GetScore();
        int GetLives();

        MovingCell AsMovingCell();

        void StartMovingTo(Point point, Action onEndOfMovement = null);

        void Eat(ICellWithCost cell);

        void Die();
    }


    
}
