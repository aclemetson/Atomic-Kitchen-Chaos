using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.UI;
using UnityEngine;

namespace AtomicKitchenChaos.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        private long quarkCount = 0;

        public long QuarkCount => quarkCount;
        private void Awake() {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }

            GameEventBus.Subscribe<UpdateQuarkMessage>(UpdateQuarks);
        }

        private void UpdateQuarks(UpdateQuarkMessage payload) {
            this.quarkCount += payload.changeInQuarks;
            UIManager.Instance.SetQuarkCount(this.quarkCount);
        }
    }
}
