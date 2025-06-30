using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.UI;
using System.Collections.Generic;
using UnityEngine;

namespace AtomicKitchenChaos.Game {
    public class ExoticMaterialManager : MonoBehaviour{

        public static ExoticMaterialManager Instance;

        private int positronCount = 0;
        private int neutrinoCount = 0;
        private int gammaRayCount = 0;

        public int PositronCount => positronCount;
        public int NeutrinoCount => neutrinoCount;
        public int GammaRayCount => gammaRayCount;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }

        public void HandleExoticMatter(Dictionary<ExoticMaterialSO.ExoticMaterial, int> exoticMaterialCounts) {
            foreach (var material in exoticMaterialCounts) {
                switch (material.Key) {
                    case ExoticMaterialSO.ExoticMaterial.Positron:
                        AddPositron(material.Value); break;

                    case ExoticMaterialSO.ExoticMaterial.Neutrino:
                        AddNeutrino(material.Value); break;

                    case ExoticMaterialSO.ExoticMaterial.GammaRay:
                        AddGammaRay(material.Value); break;
                }
            }
        }

        public void AddPositron(int count) {
            positronCount += count;
            UIManager.Instance.AddExoticMaterialUI("Positrons", positronCount);
        }

        public void AddNeutrino(int count) {
            neutrinoCount += count;
            UIManager.Instance.AddExoticMaterialUI("Neutrinos", neutrinoCount);
        }

        public void AddGammaRay(int count) {
            gammaRayCount += count;
            UIManager.Instance.AddExoticMaterialUI("Gamma Rays", gammaRayCount);
        }
    }
}
