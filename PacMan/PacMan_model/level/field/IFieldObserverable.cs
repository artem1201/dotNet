using System;

namespace PacMan_model.level.field {
    public interface IFieldObserverable : IDisposable {

        event EventHandler<FieldStateChangedEventArs> FieldState;
        event EventHandler DotsEnds;
        void ForceNotify();
    }

    public class FieldStateChangedEventArs : EventArgs {
        
        public FieldStateChangedEventArs(INotChanebleableField field) {

            Field = field;
        }

        public INotChanebleableField Field { get; private set;}
    }
}
