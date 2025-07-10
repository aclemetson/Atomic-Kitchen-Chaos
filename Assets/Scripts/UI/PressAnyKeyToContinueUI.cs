using AtomicKitchenChaos.InputActions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace AtomicKitchenChaos.UI
{
    internal class PressAnyKeyToContinueUI : MonoBehaviour
    {
        [SerializeField] private float showSuccessTimer;
        [SerializeField] private TextMeshProUGUI pressAnyKeyLabel;

        private float timer;
        private bool canAdvance, addedAction = false;
        private UnityAction advanceScreenAction;
        private Animator animator;
        private readonly string TRIGGER_TEXT = "ShowText";

        private void Start() {
            animator = GetComponent<Animator>();
        }

        private void Update() {
            if (!canAdvance) {
                timer += Time.deltaTime;
                if (timer > showSuccessTimer) {
                    canAdvance = true;
                    animator.SetTrigger(TRIGGER_TEXT);
                }
            }

            if (canAdvance && !addedAction) {
                Debug.Log("Added On Any Key Pressed");
                AnyKeyInput.AddOnAnyKeyPressed(advanceScreenAction);
                addedAction = true;
            }
        }

        internal void SetAdvanceScreen(UnityAction action) {
            advanceScreenAction = action;
            timer = 0f;
        }
    }
}
