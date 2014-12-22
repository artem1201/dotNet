//  author: Artem Sumanev

using System;
using PacMan_model.util;

namespace PacMan_model.level {
    public interface IDirectionEventObserver {
        event EventHandler<DirectionChangedEventArgs> DirectionChanged;
    }

    public class DirectionChangedEventArgs : EventArgs {
        public Direction Direction { get; private set; }

        public DirectionChangedEventArgs(Direction direction) {
            Direction = direction;
        }
    }
}