using AtomicKitchenChaos.Game;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.Counters
{
    public class LockedCounter : Counter {

        [SerializeField] private long unlockPrice;
        [SerializeField] private Transform counterSpawnLocation;

        protected override void Interact() {
            if(GameManager.Instance.QuarkCount >= unlockPrice) {
                Counter prefab = AssetDatabase.LoadAssetAtPath<Counter>(counterSO.counterPrefabPath);

                if (prefab != null) {
                    UnlockCounter(prefab);
                } else {
                    Debug.LogError($"Unable to load asset located in {counterSO.counterPrefabPath}");
                }
            }
        }

        protected override void SettingsInteraction() {
            throw new System.NotImplementedException();
        }

        public void SetLockedCounter(CounterSO counterSO, long unlockPrice) {
            this.counterSO = counterSO;
            this.unlockPrice = unlockPrice;
        }

        private void UnlockCounter(Counter prefab) {
            var go = Instantiate(prefab, counterSpawnLocation, false);
            go.transform.SetParent(null);
            Destroy(gameObject);
        }
    }
}
