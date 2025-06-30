using UnityEngine;

namespace AtomicKitchenChaos.Messages
{
    [CreateAssetMenu()]
    public class AtomicObjectRequestUnlockMessage : GameEventMessage
    {
        public GameEventMessage[] unlockRequirements;

        public override void EditorDrawingFunction() {
            
        }
    }
}
