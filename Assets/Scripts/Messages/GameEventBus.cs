using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine.Events;

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

        public static void AssignGenericUnlockSubscription(UnlockMessage unlockMessage, UnityAction action) {
            unlockMessage.AddUnlockAction(action);

            var messageType = unlockMessage.GetType();
            var subscribeMethod = typeof(GameEventBus)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == "Subscribe" && m.GetParameters().Length == 1)
                .MakeGenericMethod(messageType);

            var actionType = typeof(Action<>).MakeGenericType(messageType);
            var parameter = Expression.Parameter(messageType, "msg");

            var methodInfo = messageType.GetMethod("SubscriptionCheck", new[] { typeof(GameEventMessage) });
            var castedParameter = Expression.Convert(parameter, typeof(GameEventMessage));
            var call = Expression.Call(Expression.Constant(unlockMessage), methodInfo, castedParameter);
            var lambda = Expression.Lambda(actionType, call, parameter);
            var compiledDelegate = lambda.Compile();

            subscribeMethod.Invoke(null, new object[] { compiledDelegate });
        }
    }
}
