using AtomicKitchenChaos.Messages;
using System;
using System.Collections.Generic;

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
            { typeof(QuarkCountMessage), typeof(QuarkCountMessageEditor) },
            { typeof(AtomicFinalSubmissionRequestMessage), typeof(AtomicFinalSubmissionRequestMessageEditor) },
        };

        public static readonly Dictionary<Type, Type> DIALOGUE_TRIGGER_MAPPER = new() {
            { typeof(LevelStartMessage), typeof(EmptyMessageEditor) },
            { typeof(GameOverMessage), typeof(EmptyMessageEditor) },
            { typeof(NetWorthMessage), typeof(NetWorthMessageEditor) },
            { typeof(QuarkCountMessage), typeof(QuarkCountMessageEditor) },
            { typeof(CounterUnlockMessage), typeof(CounterUnlockMessageEditor) },
            { typeof(RecipeUnlockMessage), typeof (RecipeUnlockMessageEditor) },
            { typeof(AtomicObjectRequestSuccessMessage), typeof(AtomicObjectRequestSuccessMessageEditor) },
            { typeof(DialogueHasFinishedMessage), typeof(DialogueHasFinishedMessageEditor) },
            { typeof(CombinerCompleteObjectMessage), typeof(CombinerCompleteObjectMessageEditor) },
        };
    }
}
