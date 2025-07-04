using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.UI;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;

namespace AtomicKitchenChaos.Counters
{
    public class FinalSubmissionCounter : Counter {

        private List<AtomicObjectSO> atomicObjectSOs;
        private List<int> quantities;

        private void Awake() {
            atomicObjectSOs = new List<AtomicObjectSO>();
            quantities = new List<int>();
        }

        public void SetAtomicObjectRequest(AtomicFinalSubmissionRequestMessage request) {
            DataHandler.TryLoadSO(request.atomicObjectSOPath, out AtomicObjectSO atomicObjectSO);
            atomicObjectSOs.Add(atomicObjectSO);
            quantities.Add(request.quantity);
            int index = atomicObjectSOs.Count - 1;
            GameEventBus.AssignGenericUnlockSubscription(request, () => { CheckFinalSubmission(); });
        }

        protected override void Interact() {
            if(playerManager.HasAtomicObject() && atomicObjectSOs.Contains(playerManager.AtomicObject.AtomicObjectSO)) {
                var targetSO = playerManager.AtomicObject.AtomicObjectSO;
                int index = atomicObjectSOs.Select((item, i) => new { item, i }).First(x => x.item == targetSO).i;
                if (quantities[index] > 0) {
                    quantities[index]--;
                    playerManager.RemoveAtomicObject();
                    string targetPath = AssetDatabase.GetAssetPath(targetSO);
                    GameEventBus.Publish(new AtomicFinalSubmissionRequestMessage() { atomicObjectSOPath = targetPath, quantity = quantities[index] });
                }
            }
        }

        protected override void SettingsInteraction() {
            // This will open up a menu that will show all the requirements
            UnityEngine.Debug.Log("Final Submission Counter Menu");
            UIManager.Instance.PopulateFinalSubmissionPanel(atomicObjectSOs.ToArray(), quantities.ToArray());
        }

        private void CheckFinalSubmission() {
            bool hasSubmittedAll = quantities.All(t => t <= 0);
            if (hasSubmittedAll) {
                UnityEngine.Debug.Log("Has Submitted All");
            }
        }
    }
}
