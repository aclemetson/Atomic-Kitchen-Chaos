using AtomicKitchenChaos.Counters;
using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.InputActions;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.SceneManagement;
using AtomicKitchenChaos.UI;
using AtomicKitchenChaos.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace AtomicKitchenChaos.Level
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;

        private LevelData levelData;
        private LevelRequirementData levelRequirementData;
        private GameOutcomeData gameOutcomeData;
        private DialogueBundleData dialogueBundleData;
        private DialogueData[] dialogueDatas;
        private RequestManager requestManager;
        private string levelPath;
        private bool levelEndsOnDialogue = false;

        private FinalSubmissionCounter finalSubmissionCounter = null;

        public LevelRequirementData LevelRequirementData => levelRequirementData;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }

            levelPath = SceneLoader.LevelDataPath;

            DataHandler.TryLoadFromFile(levelPath, out levelData);
            DataHandler.TryLoadFromFile(levelData.levelRequirementPath, out levelRequirementData);
            DataHandler.TryLoadFromFile(levelRequirementData.gameOutcomePath, out gameOutcomeData);
            DataHandler.TryLoadFromFile(levelData.dialogueBundlePath, out dialogueBundleData);

            List<DialogueData> data = new();
            foreach (var dialogueDataPath in dialogueBundleData.dialogueDataPaths) {
                if(DataHandler.TryLoadFromFile(dialogueDataPath, out DialogueData dialogueData)) {
                    data.Add(dialogueData);
                }
            }
            dialogueDatas = data.ToArray();

            AnyKeyInput.Init();
            requestManager = new RequestManager(levelRequirementData);
        }

        private void Start() { 
            foreach(var c in levelData.Counters) {
                LoadCounter(c);
            }

            foreach (var gameOutcome in gameOutcomeData.gameOutcomes) {
                if (gameOutcome.message.GetType() == typeof(AtomicFinalSubmissionRequestMessage)) {
                    // Send to Final Submission Counter
                    if (finalSubmissionCounter != null) {
                        AtomicFinalSubmissionRequestMessage finalMessage = (AtomicFinalSubmissionRequestMessage)gameOutcome.message;
                        finalSubmissionCounter.SetAtomicObjectRequest(finalMessage);
                    } else {
                        Debug.LogError("Unable to Set Final Submission Request to Final Submission Counter. Counter did not load.");
                    }
                }
                    
                UnlockMessage message = (UnlockMessage)gameOutcome.message;
                GameEventBus.AssignGenericUnlockSubscription(message, UpdateGameState);
            }

            foreach(var dialogueData in dialogueDatas) {

                AssignTriggeringChecks(dialogueData);
            }

            UIManager.Instance.SetMainMenuLoadingAction(() => SceneLoader.LoadScene(Utilities.MAIN_MENU_SCENE));

            GameEventBus.Publish(new LevelStartMessage());

            if (!levelEndsOnDialogue) {
                GameEventBus.Subscribe<GameOverMessage>(LevelFinished);
            }

        }

        private void LoadCounter(CounterData counter) {
            DataHandler.TryLoadSO(counter.counterSOpath, out CounterSO counterSO);
            if (counterSO == null) {
                Debug.LogWarning($"Missing CounterSO at path: {counter.counterSOpath}");
                return;
            }

            Vector3 position = counter.position.ToVector3();
            Counter counterObject;
            if (counter.isActive) {
                counterObject = Instantiate(AssetDatabase.LoadAssetAtPath<Counter>(counterSO.counterPrefabPath));
                counterObject.SetCounterSO(counterSO);
            } else {
                counterObject = Instantiate(AssetDatabase.LoadAssetAtPath<LockedCounter>(counterSO.lockedCounterPrefabPath));
                LockedCounter temp = (LockedCounter)counterObject;
                temp.SetLockedCounter(counterSO, counter.purchasePrice);
            }
            counterObject.transform.position = position;

            // If the counter is the Final Submission Counter, keep track of it
            // Make sure there isn't more than one
            if(counterObject is FinalSubmissionCounter) {
                if(finalSubmissionCounter == null) {
                    finalSubmissionCounter = (FinalSubmissionCounter)counterObject;
                } else {
                    Debug.LogError("There is more than one Final Submission Counter in the level.");
                }
            }
        }

        private void UpdateGameState() {
            // Check if all the GameOutcomeMessages are unlocked, if so, set gameover
            bool allOutcomesComplete = gameOutcomeData.gameOutcomes.All(t => !((UnlockMessage)t.message).IsLocked);
            if (allOutcomesComplete) {
                GameEventBus.Publish(new GameOverMessage());
            }
        }

        private void AssignTriggeringChecks(DialogueData dialogueData) {
            for (int i = 0; i < dialogueData.triggeringMessages.Length; i++) {
                int staticIndex = i;

                // If the message is a game over message, set the dialogue close to the end game sequence
                if (dialogueData.triggeringMessages[staticIndex] is GameOverMessage) {
                    var dialogueDataEntry = dialogueData.dialogueEntries[dialogueData.dialogueEntries.Length - 1];
                    if(dialogueDataEntry.onDialogueFinished == null) {
                        dialogueDataEntry.onDialogueFinished = new UnityEvent();
                    }
                    dialogueDataEntry.onDialogueFinished.AddListener(() => LevelFinished(new GameOverMessage()));
                    dialogueData.dialogueEntries[dialogueData.dialogueEntries.Length - 1] = dialogueDataEntry;

                    levelEndsOnDialogue = true;
                }

                UnlockMessage message = (UnlockMessage)dialogueData.triggeringMessages[staticIndex];
                GameEventBus.AssignGenericUnlockSubscription(message, () => {
                    dialogueData.messagesHaveTriggered[staticIndex] = true;
                    if(dialogueData.messagesHaveTriggered.All(t => t)) {
                        UIManager.Instance.StartDialogue(dialogueData);
                    }
                });
            }
        }

        private void LevelFinished(GameOverMessage payload) {
            UIManager.Instance.LevelFinished();
            AnyKeyInput.Reset();
        }
    }
}
