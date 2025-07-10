using UnityEngine;
using UnityEngine.Events;

namespace AtomicKitchenChaos.UI
{
    internal class GameOverPanelUI : MonoBehaviour {

        private PressAnyKeyToContinueUI pressAnyKeyToContinueUI;

        public void SetMainMenuLoadingAction(UnityAction mainMenuLoadingAction) {
            pressAnyKeyToContinueUI = GetComponent<PressAnyKeyToContinueUI>();
            pressAnyKeyToContinueUI.SetAdvanceScreen(mainMenuLoadingAction);
        }

        public void LevelFinished() {
            gameObject.SetActive(true);
        }
    }
}
