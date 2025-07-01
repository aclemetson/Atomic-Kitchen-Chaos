using AtomicKitchenChaos.Counters;
using AtomicKitchenChaos.Counters.Misc;
using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Utility;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.Level
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private LockedCounter lockedCounterPrefab;
        [SerializeField] private string levelPath;

        private LevelRequirementData levelRequirementData;
        private void Start() {
            DataHandler.TryLoadFromFile(levelPath, out LevelData levelData);
            DataHandler.TryLoadFromFile(levelData.levelRequirementPath, out levelRequirementData);

            foreach(var c in levelData.Counters) {
                LoadCounter(c);
            }
        }

        private void LoadCounter(CounterData counter) {
            CounterSO counterSO = AssetDatabase.LoadAssetAtPath<CounterSO>(counter.counterSOpath);
            if (counterSO == null) {
                Debug.LogWarning($"Missing CounterSO at path: {counter.counterSOpath}");
                return;
            }

            // For the submission counters, give the level requirement data
            if (counterSO.GetType() == typeof(SubmissionCounterSO)) {
                SubmissionCounterSO temp = (SubmissionCounterSO)counterSO;
                temp.levelRequirementData = levelRequirementData;
                counterSO = temp;
            }

            Vector3 position = counter.position.ToVector3();
            Counter counterObject;
            if (counter.isActive) {
                counterObject = Instantiate(AssetDatabase.LoadAssetAtPath<Counter>(counterSO.counterPrefabPath));
                counterObject.SetCounterSO(counterSO);
            } else {
                counterObject = Instantiate(lockedCounterPrefab);
                LockedCounter temp = (LockedCounter)counterObject;
                temp.SetLockedCounter(counterSO, counter.purchasePrice);
            }
            counterObject.transform.position = position;
        }
    }
}
