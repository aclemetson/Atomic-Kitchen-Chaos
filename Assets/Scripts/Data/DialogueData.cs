using AtomicKitchenChaos.Messages;
using UnityEngine.Events;
using System;

namespace AtomicKitchenChaos.Data {
    [Serializable]
    public struct DialogueData {

        public string dialogueName;
        public GameEventMessage triggeringMessage;
        public DialogueEntryData[] dialogueEntries;

        [Serializable]
        public struct DialogueEntryData {
            public string message;
            public bool pauseGame;
            public UnityEvent onDialogueShown;
            public UnityEvent onDialogueFinished;
        }
    }
}
