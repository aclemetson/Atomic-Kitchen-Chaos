using AtomicKitchenChaos.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AtomicKitchenChaos.UI
{
    internal class DialoguePanelUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button nextButton;

        private DialogueData dialogueData;
        private int currentIndex = 0;
        private UnityEvent closeMenuWindow;

        public DialogueData DialogueData => dialogueData;

        private void Start() {
            nextButton.onClick.AddListener(Next);
        }

        internal void SetCloseMenu(UnityAction closeAction) {
            closeMenuWindow = new();
            closeMenuWindow.AddListener(closeAction);
        }

        internal void StartDialogue(DialogueData dialogueData) {
            this.dialogueData = dialogueData;

            if (dialogueData.dialogueEntries.Length == 0)
                return;

            currentIndex = 0;
            gameObject.SetActive(true);
            ShowCurrentEntry();
        }

        internal void ShowCurrentEntry() {
            var entry = dialogueData.dialogueEntries[currentIndex];
            messageText.text = entry.message;

            if (entry.pauseGame) {
                Time.timeScale = 0f;
            }

            entry.onDialogueShown?.Invoke();
        }

        internal void Next() {
            var entry = dialogueData.dialogueEntries[currentIndex];
            entry.onDialogueFinished?.Invoke();

            if (entry.pauseGame)
                Time.timeScale = 1f;

            currentIndex++;

            if(currentIndex >= dialogueData.dialogueEntries.Length) {
                EndDialogue();
            } else {
                ShowCurrentEntry();
            }
        }

        private void EndDialogue() {
            gameObject.SetActive(false);
            Time.timeScale = 1f;
            closeMenuWindow?.Invoke();
        }
    }
}
