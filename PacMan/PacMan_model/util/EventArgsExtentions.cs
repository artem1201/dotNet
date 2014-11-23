using System;

namespace PacMan_model.util {
    public static class EventArgsExtentions {

        public static void Raise<TEventArgs>(this TEventArgs eventArgs, Object sender,
            ref EventHandler<TEventArgs> eventHandler) {
            if (null == sender) {
                throw new ArgumentNullException("sender");
            }

/*
            var temp = Volatile.Read(ref eventHandler);

            if (null != temp) {
                temp(sender, eventArgs);
            }
*/

            if (null != eventHandler) {
                eventHandler(sender, eventArgs);
            }
        }

        public static void Raise(this EventArgs eventArgs, Object sender,
            ref EventHandler eventHandler) {
            if (null == sender) {
                throw new ArgumentNullException("sender");
            }


            /*
                        var temp = Volatile.Read(ref eventHandler);

                        if (null != temp) {
                            temp(sender, eventArgs);
                        }
            */

            if (null != eventHandler) {
                eventHandler(sender, eventArgs);
            }
        }
    }

}
