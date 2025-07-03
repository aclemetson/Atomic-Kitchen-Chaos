using AtomicKitchenChaos.Utility;
using TMPro;
using UnityEngine;

namespace AtomicKitchenChaos.UI
{
    public class SubmissionCounterUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI rewardAmountLabel;

        public void SetReward(long amount) {
            rewardAmountLabel.text = NumberFormatter.FormatNumber(amount);
        }
    }
}
