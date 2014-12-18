using System;
using PacMan_model.util;

namespace PacMan_model.level.cells.pacman {
    public interface IPacManObserverable : IDisposable {
        event EventHandler<PacmanStateChangedEventArgs> PacmanState;

        void ForceNotify();
    }

    public sealed class PacmanStateChangedEventArgs : EventArgs {
        public PacmanStateChangedEventArgs(
            Point position,
            Direction direction,
            int lives,
            int score,
            bool hasDied = false) {
            if (null == position) {
                throw new ArgumentNullException("position");
            }
            Position = position;
            Direction = direction;
            Lives = lives;
            Score = score;
            HasDied = hasDied;
        }

        public Point Position { get; private set; }
        public Direction Direction { get; private set; }
        public int Lives { get; private set; }
        public int Score { get; private set; }
        public bool HasDied { get; private set; }
    }
}