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
        }

        public void AddToQuarkCount(long quarkCount) {
            this.quarkCount += quarkCount;
            UIManager.Instance.SetQuarkCount(this.quarkCount);
        }

        public void SubtractFromQuarkCount(long quarkCount) {
            this.quarkCount -= quarkCount;
            UIManager.Instance.SetQuarkCount(this.quarkCount);
        }
    }
}
