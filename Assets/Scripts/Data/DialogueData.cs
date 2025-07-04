using AtomicKitchenChaos.Messages;
using UnityEngine.Events;
using System;

namespace AtomicKitchenChaos.Data {
    [Serializable]
    public struct DialogueData {

        public string dialogueName;
        public GameEventMessage[] triggeringMessages;
        public bool[] messagesHaveTriggered;
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
