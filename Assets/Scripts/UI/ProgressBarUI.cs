using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AtomicKitchenChaos.UI {
    public class ProgressBarUI : MonoBehaviour {

        [SerializeField] private GameObject fillObject;

        private float fillTime;
        private float clock = 0;
        private bool start = false;

        private Image fillImage;
        private UnityEvent onTimeElapsed;

        public UnityEvent OnTimeElapsed => onTimeElapsed;

        private void Awake() {
            fillImage = fillObject.GetComponent<Image>();
            onTimeElapsed = new UnityEvent();
            gameObject.SetActive(false);
        }

        public void SetFillTime(float fillTime) {
            this.fillTime = fillTime;
            gameObject.SetActive(true);
            start = true;
        }

        private void Update() {
            if (start && clock <= fillTime) {
                clock += Time.deltaTime;

                fillImage.fillAmount = Mathf.Clamp01(clock / fillTime);
            }

            if (clock > fillTime) {
                start = false;
                clock = 0;
                onTimeElapsed?.Invoke();
                gameObject.SetActive(false);
            }
        }
    }
}
