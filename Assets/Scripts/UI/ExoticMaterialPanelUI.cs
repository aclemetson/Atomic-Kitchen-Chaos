using System.Collections.Generic;
using UnityEngine;

namespace AtomicKitchenChaos.UI
{
    internal class ExoticMaterialPanelUI : MonoBehaviour {
        
        [SerializeField] private ExoticMaterialUI exoticMaterialContainer;
        [SerializeField] private Transform emVerticalContainer;

        internal void AddExoticMaterialUI(string material, int count) {
            var go = Instantiate(exoticMaterialContainer, emVerticalContainer);
            go.SetLabels(material, count);
        }
    }
}
