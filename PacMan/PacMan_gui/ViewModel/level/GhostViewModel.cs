using System;
using PacMan_model.level.cells.ghosts;
using PacMan_model.util;

namespace PacMan_gui.ViewModel.level {
    public class GhostViewModel {

        public Point Position { get; private set; }
        public string Name { get; private set; }

        private readonly IGhostObserverable _ghost;

        public GhostViewModel(IGhostObserverable ghost) {
            ghost.GhostState += OnGhostChanged;

            _ghost = ghost;
        }

        private void OnGhostChanged(Object sender, EventArgs e) {

            if (null == e) {
                throw new ArgumentNullException("e");
            }

            var eventArgs = e as GhostStateChangedEventArgs;
            if (null == eventArgs) {
                throw new ArgumentException("e");
            }

            Position = eventArgs.Position;
            Name = eventArgs.Name;

            //Redraw();
        }

        public void Redraw() {
            _ghost.ForceNotify();
        }
    }
}
