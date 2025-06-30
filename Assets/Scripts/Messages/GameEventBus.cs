using System;
using System.Collections;
using System.Collections.Generic;

namespace AtomicKitchenChaos.Messages
{
    public static class GameEventBus
    {
        private static readonly Dictionary<Type, List<Action<GameEventMessage>>> listeners = new();

        public static void Subscribe<T>(Action<T> callback) where T : GameEventMessage {
            Type type = typeof(T);
            if(!listeners.ContainsKey(type))
                listeners[type] = new List<Action<GameEventMessage>>();

            listeners[type].Add(msg => callback((T)msg));
        }

        public static void Unsubscribe<T>(Action<T> callback) where T : GameEventMessage {
            Type type = typeof(T);
            if( listeners.TryGetValue(type, out var list)) {
                list.RemoveAll(action => action.Equals((Action<GameEventMessage>)(msg => callback((T)msg))));
            }
        }

        public static void Publish(GameEventMessage message) {
            Type type = message.GetType();
            if (listeners.TryGetValue(type, out var list)) {
                foreach(var callback in list) {
                    callback.Invoke(message);
                }
            }
        }
    }
}
