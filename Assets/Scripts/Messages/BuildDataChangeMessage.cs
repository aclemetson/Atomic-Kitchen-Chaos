using UnityEngine.Events;

namespace AtomicKitchenChaos.Messages {
    public class BuildDataChangeMessage : GameEventMessage {
        public BuildData[] buildData;

        public struct BuildData {
            public string name;
            public long price;
            public UnityAction selectAction;
        }
    }
}
