using UnityEngine;

namespace AtomicKitchenChaos.UI
{
    [RequireComponent(typeof(Animator))]
    internal class TextFadeUI : MonoBehaviour
    {

        private readonly string FADE_TRIGGER = "StartFade";

        internal void StartFade() {
            Animator animator = GetComponent<Animator>();
            if (animator != null) {
                animator.SetTrigger(FADE_TRIGGER);
            }
        }

        public void FinishFade() {
            Destroy(gameObject);
        }
    }
}
