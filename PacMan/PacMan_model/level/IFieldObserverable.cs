using System;

namespace PacMan_model.level {
    public interface IFieldObserverable {

        event EventHandler<FieldStateChangedEventArs> FieldState;
        void ForceNotify();
    }

    public class FieldStateChangedEventArs : EventArgs {
        
        public FieldStateChangedEventArs(INotChanebleableField field, bool dotIsNoMore) {

            Field = field;
            DotIsNoMore = dotIsNoMore;
        }

        public INotChanebleableField Field { get; private set;}

        public bool DotIsNoMore { get; private set; }
    }
}
