using AtomicKitchenChaos.Messages;
using UnityEngine;

namespace AtomicKitchenChaos.Data
{
    public struct GameOutcomeData
    {
        public string gameOutcomeName;
        public GameOutcome[] gameOutcomes;

        public enum GameOutcomeStatus {
            Success,
            Failure,
            Optional
        }

        public struct GameOutcome {
            [SerializeReference]
            public GameEventMessage message;
            public GameOutcomeStatus status;
        }
    }
}
