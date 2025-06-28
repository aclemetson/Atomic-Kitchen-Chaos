using AtomicKitchenChaos.UI;
using UnityEngine;

namespace AtomicKitchenChaos.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        private int quarkCount = 0;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }

        public void AddToQuarkCount(int quarkCount) {
            this.quarkCount += quarkCount;
            UIManager.Instance.SetQuarkCount(this.quarkCount);
        }
    }
}
