using AtomicKitchenChaos.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AtomicKitchenChaos.Editor.MessageMappers {
    public static class MessageMapper {
        public static readonly Dictionary<Type, Type> ATOMIC_OBJECT_REQUEST_UNLOCK_MAPPER = new() {
            { typeof(NetWorthMessage), typeof(NetWorthMessageEditor) },
            { typeof(CounterUnlockMessage), typeof(CounterUnlockMessageEditor) },
            { typeof(RecipeUnlockMessage), typeof(RecipeUnlockMessageEditor) },
            { typeof(AtomicObjectRequestSuccessMessage), typeof(AtomicObjectRequestSuccessMessageEditor) },
        };

        public static readonly Dictionary<Type, Type> GAME_OUTCOME_UNLOCK_MAPPER = new() {
            { typeof(NetWorthMessage), typeof(NetWorthMessageEditor) },
        };
    }
}
