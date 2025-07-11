using AtomicKitchenChaos.InputActions;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.SceneManagement;
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
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }

            SceneLoader.Init();
            AnyKeyInput.Init();

            GameEventBus.Subscribe<AddQuarks>(AddQuarks);
            GameEventBus.Subscribe<TryUnlockMessage>(TryUnlock);
        }

        private void TryUnlock(TryUnlockMessage payload) {
            if(quarkCount >= payload.unlockPrice) {
                quarkCount -= payload.unlockPrice;
                GameEventBus.Publish(new QuarkCountMessage() { quarkCount = quarkCount });
                UIManager.Instance.SetQuarkCount(quarkCount);
                payload.callback.Invoke();
            }
        }

        private void AddQuarks(AddQuarks payload) {
            quarkCount += payload.changeInQuarks;
            netWorth += payload.changeInQuarks;
            Debug.Log($"Net Worth: {netWorth}");
            UIManager.Instance.SetQuarkCount(quarkCount);
            GameEventBus.Publish(new QuarkCountMessage() { quarkCount = quarkCount });
            GameEventBus.Publish(new NetWorthMessage() { value = netWorth });
        }
    }
}
