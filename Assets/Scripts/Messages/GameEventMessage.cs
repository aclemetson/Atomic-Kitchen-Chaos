using System;
using UnityEngine;

namespace AtomicKitchenChaos.Messages {
    public abstract class GameEventMessage : ScriptableObject {
        public string displayName;

        public abstract void EditorDrawingFunction();
    }

}