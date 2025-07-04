using AtomicKitchenChaos.Counters;
using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.SceneManagement;
using AtomicKitchenChaos.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

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

            requestManager = new RequestManager(levelRequirementData);
        }

        private void Start() { 
            foreach(var c in levelData.Counters) {
                LoadCounter(c);
            }

            foreach (var gameOutcome in gameOutcomeData.gameOutcomes) {
                if(gameOutcome.message.GetType() == typeof(AtomicFinalSubmissionRequestMessage)) {
                    // Send to Final Submission Counter
                    if(finalSubmissionCounter != null) {
                        AtomicFinalSubmissionRequestMessage message = (AtomicFinalSubmissionRequestMessage)gameOutcome.message;
                        finalSubmissionCounter.SetAtomicObjectRequest(message);
                    } else {
                        Debug.LogError("Unable to Set Final Submission Request to Final Submission Counter. Counter did not load.");
                    }
                } else {
                    // Temporary
                    UnlockMessage message = (UnlockMessage)gameOutcome.message;
                    GameEventBus.AssignGenericUnlockSubscription(message, UpdateGameState);
                }
            }

            foreach(var dialogueData in dialogueDatas) {

                AssignTriggeringChecks(dialogueData);
            }

            GameEventBus.Publish(new LevelStartMessage());
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
            Debug.Log("Game State is Successfully Updated");
        }

        private void AssignTriggeringChecks(DialogueData dialogueData) {
            for (int i = 0; i < dialogueData.triggeringMessages.Length; i++) {
                int staticIndex = i;
                UnlockMessage message = (UnlockMessage)dialogueData.triggeringMessages[staticIndex];
                GameEventBus.AssignGenericUnlockSubscription(message, () => {
                    dialogueData.messagesHaveTriggered[staticIndex] = true;
                    if(dialogueData.messagesHaveTriggered.All(t => t)) {
                        UIManager.Instance.StartDialogue(dialogueData);
                    }
                });
            }
        }
    }
}
