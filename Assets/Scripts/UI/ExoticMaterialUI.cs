using System.Collections;
using TMPro;
using UnityEngine;

namespace AtomicKitchenChaos.UI
{
    [RequireComponent(typeof(TextFadeUI))]
    internal class ExoticMaterialUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI emLabel;
        [SerializeField] TextMeshProUGUI emCounter;
        [SerializeField] float holdTime = 5f;

        private bool startCount = false;

        internal void SetLabels(string materialName, int count) {
            gameObject.SetActive(true);
            emLabel.text = materialName;
            emCounter.text = count.ToString();
            startCount = true;
        }

        private void Update() {
            if (startCount) {
                holdTime -= Time.deltaTime;
                if (holdTime <= 0) {
                    GetComponent<TextFadeUI>().StartFade();
                }
            }
        }
    }
}
