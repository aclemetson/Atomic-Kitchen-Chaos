using AtomicKitchenChaos.Counters;
using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.SceneManagement;
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
        private RequestManager requestManager;
        private string levelPath;

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

            requestManager = new RequestManager(levelRequirementData);
        }

        private void Start() { 
            foreach(var c in levelData.Counters) {
                LoadCounter(c);
            }

            foreach (var gameOutcome in gameOutcomeData.gameOutcomes) {
                UnlockMessage temp = (UnlockMessage)gameOutcome.message;
                GameEventBus.AssignGenericUnlockSubscription(temp, UpdateGameState);
            }
        }

        private void LoadCounter(CounterData counter) {
            CounterSO counterSO = AssetDatabase.LoadAssetAtPath<CounterSO>(counter.counterSOpath);
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
        }

        private void UpdateGameState() {
            Debug.Log("Game State is Successfully Updated");
        }
    }
}
