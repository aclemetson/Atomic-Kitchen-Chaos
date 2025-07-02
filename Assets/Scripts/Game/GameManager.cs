using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.UI;
using System;
using UnityEngine;

namespace AtomicKitchenChaos.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        private long quarkCount = 0;
        private long netWorth = 0;

        public long QuarkCount => quarkCount;
        private void Awake() {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }

            GameEventBus.Subscribe<AddQuarks>(AddQuarks);
            GameEventBus.Subscribe<TryUnlockMessage>(TryUnlock);
        }

        private void TryUnlock(TryUnlockMessage payload) {
            if(quarkCount >= payload.unlockPrice) {
                quarkCount -= payload.unlockPrice;
                UIManager.Instance.SetQuarkCount(quarkCount);
                payload.callback.Invoke();
            }
        }

        private void AddQuarks(AddQuarks payload) {
            quarkCount += payload.changeInQuarks;
            netWorth += payload.changeInQuarks;
            Debug.Log($"Net Worth: {netWorth}");
            UIManager.Instance.SetQuarkCount(quarkCount);
            GameEventBus.Publish(new NetWorthMessage() { value = netWorth });
        }
    }
}
