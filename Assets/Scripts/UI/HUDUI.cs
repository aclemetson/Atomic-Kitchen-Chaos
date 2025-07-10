using AtomicKitchenChaos.Utility;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AtomicKitchenChaos.UI
{
    internal class HUDUI : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI quarkCountGUI;
        [SerializeField] private TextMeshProUGUI timerGUI;
        [SerializeField] private Button menuButton;

        [Header("Drop Down Menus")]
        [SerializeField] private BuildMenuUI buildMenuUI;

        private float clock = 0f;
        private int lastDisplayedSecond = -1;

        private void Awake() {
            menuButton.onClick.AddListener(DisplayMenu);
        }

        private void Update() {
            clock += Time.deltaTime;

            int currentSecond = Mathf.FloorToInt(clock);

            if (currentSecond != lastDisplayedSecond) {
                SetTimer(currentSecond);
                lastDisplayedSecond = currentSecond;
            }
        }

        internal void SetQuarkCount(long quarkCount) {
            quarkCountGUI.text = NumberFormatter.FormatNumber(quarkCount);
        }

        // Time is in seconds
        private void SetTimer(int time) {
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            timerGUI.text = timeSpan.ToString(@"hh\:mm\:ss");
        }

        private void DisplayMenu() {
            // Temporary, will be a dropdown
            buildMenuUI.gameObject.SetActive(true);
        }
    }
}
