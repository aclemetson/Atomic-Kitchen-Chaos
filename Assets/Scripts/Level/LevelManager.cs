using AtomicKitchenChaos.Counters;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.Level
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private LockedCounter lockedCounterPrefab;
        [SerializeField] private string levelName;


        private void Start() {
            LevelData levelData = LevelLoader.LoadLevel(levelName);

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

            Vector3 position = counter.position.ToVector3();
            Counter counterObject;
            if (counter.isActive) {
                counterObject = Instantiate(AssetDatabase.LoadAssetAtPath<Counter>(counterSO.counterPrefabPath));
                counterObject.SetCounterSO(counterSO);
            } else {
                counterObject = Instantiate(lockedCounterPrefab);
            }
            counterObject.transform.position = position;
        }
    }
}
