using System;
using PacMan_model.util;

namespace PacMan_model.level.cells.ghosts {
    public interface IGhostObserverable {

        event EventHandler<GhostStateChangedEventArgs> GhostState;
        void ForceNotify();
    }


    public class GhostStateChangedEventArgs : EventArgs {

        private readonly string _name;

        public GhostStateChangedEventArgs(string name, Point position) {
            if (null == name) {
                throw new ArgumentNullException("name");
            }
            if (null == position) {
                throw new ArgumentNullException("position");
            }
            _name = name;
            Position = position;
        }

        public string Name {
            get {return _name;}
        }

        public Point Position { get; private set; }
    }
}
