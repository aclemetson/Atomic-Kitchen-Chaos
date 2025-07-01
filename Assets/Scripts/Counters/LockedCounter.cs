using AtomicKitchenChaos.Game;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.UI;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.Counters
{
    public class LockedCounter : Counter {

        [SerializeField] private long unlockPrice;
        [SerializeField] private Transform counterSpawnLocation;
        [SerializeField] private UnlockPanelUI unlockPanelUI;

        protected override void Start() {
            base.Start();
        }

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
            unlockPanelUI.SetUnlockPanel(counterSO.displayName, unlockPrice);
        }

        private void UnlockCounter(Counter prefab) {
            var go = Instantiate(prefab);
            go.SetCounterSO(counterSO);
            go.transform.SetParent(null);
            go.transform.position = counterSpawnLocation.position;
            GameEventBus.Publish(new UpdateQuarkMessage() { changeInQuarks = -unlockPrice });
            GameEventBus.Publish(new CounterUnlockMessage() { counterSOPath = AssetDatabase.GetAssetPath(counterSO) });
            Destroy(gameObject);
        }
    }
}
