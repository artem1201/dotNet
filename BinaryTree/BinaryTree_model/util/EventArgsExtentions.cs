//  author: Artem Sumanev

using System;

namespace BinaryTree.util {
    public static class EventArgsExtentions {
        public static void Raise<TEventArgs>(
            this TEventArgs eventArgs,
            Object sender,
            ref EventHandler<TEventArgs> eventHandler) {
            if (null == sender) {
                throw new ArgumentNullException("sender");
            }

            if (null != eventHandler) {
                eventHandler(sender, eventArgs);
            }
        }

        public static void Raise(
            this EventArgs eventArgs,
            Object sender,
            ref EventHandler eventHandler) {
            if (null == sender) {
                throw new ArgumentNullException("sender");
            }

            if (null != eventHandler) {
                eventHandler(sender, eventArgs);
            }
        }
    }
}