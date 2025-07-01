using AtomicKitchenChaos.Messages;
using System;
using System.Collections.Generic;

namespace AtomicKitchenChaos.Editor.MessageMappers {
    public static class MessageMapper {
        public static readonly Dictionary<Type, Type> ATOMIC_OBJECT_REQUEST_UNLOCK_MAPPER = new() {
            { typeof(NetWorthMessage), typeof(NetWorthMessageEditor) },
            { typeof(CounterUnlockMessage), typeof(CounterUnlockMessageEditor) },
        };
    }
}
