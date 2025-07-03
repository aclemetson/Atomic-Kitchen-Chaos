using AtomicKitchenChaos.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AtomicKitchenChaos.UI
{
    public class DialoguePanelUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button nextButton;

        [Header("Config")]
        [SerializeField] private DialogueData dialogueData;

        private int currentIndex = 0;
        private bool isRunning = false;

        private void Start() {
            nextButton.onClick.AddListener(Next);
        }

        public void StartDialogue() {
            if (dialogueData.dialogueEntries.Length == 0)
                return;

            isRunning = true;
            currentIndex = 0;
            gameObject.SetActive(true);
            ShowCurrentEntry();
        }

        public void ShowCurrentEntry() {
            var entry = dialogueData.dialogueEntries[currentIndex];
            messageText.text = entry.message;

            if (entry.pauseGame) {
                Time.timeScale = 0f;
            }

            entry.onDialogueShown?.Invoke();
        }

        public void Next() {
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
            isRunning = false;
            Time.timeScale = 1f;
        }
    }
}
