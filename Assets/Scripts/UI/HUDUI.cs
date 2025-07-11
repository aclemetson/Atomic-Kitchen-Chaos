using AtomicKitchenChaos.Utility;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AtomicKitchenChaos.UI
{
    internal class HUDUI : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI quarkCountGUI;
        [SerializeField] private TextMeshProUGUI timerGUI;
        [SerializeField] private Button buildButton;
        [SerializeField] private Button deleteButton;

        [Header("Drop Down Menus")]
        [SerializeField] private BuildMenuUI buildMenuUI;

        private float clock = 0f;
        private int lastDisplayedSecond = -1;

        private void Awake() {
            buildButton.onClick.AddListener(DisplayBuildMenu);
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

        internal void SetDeleteButtonAction(UnityAction action) {
            deleteButton.onClick.AddListener(action);
        }

        // Time is in seconds
        private void SetTimer(int time) {
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            timerGUI.text = timeSpan.ToString(@"hh\:mm\:ss");
        }

        private void DisplayBuildMenu() {
            // Temporary, will be a dropdown
            buildMenuUI.gameObject.SetActive(true);
        }
    }
}
