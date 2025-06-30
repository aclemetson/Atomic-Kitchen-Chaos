
using System.Collections.Generic;
using UnityEngine;

namespace AtomicKitchenChaos.GeneratedObjects.AtomicObjects
{
    public class ElectronGenerator : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private ElectronOrbit electronPrefab;

        [Header("Transforms")]
        [SerializeField] private Transform electronContainer;
        [SerializeField] private Transform nucleusLocation;

        [Header("Configuration")]
        [SerializeField] private int electronCount;
        [SerializeField] private float electronOrbitRadius;

        private List<ElectronOrbit> spawnedElectrons = new List<ElectronOrbit>();

        private void Start() {
            if (electronCount != 0) {
                GenerateElectrons(electronCount);
            }
        }

        public void GenerateElectrons(int count) {
            ClearElectrons();

            for(int i = 0; i < count; i ++) {
                Vector3 direction = Random.onUnitSphere;
                Vector3 position = direction * electronOrbitRadius;

                ElectronOrbit electron = Instantiate(electronPrefab);
                electron.transform.SetParent(electronContainer, false);
                electron.transform.localPosition = position;
                electron.SetNucleusLocation(nucleusLocation);
                electron.SetOrbitRadius(electronOrbitRadius);
                spawnedElectrons.Add(electron);
            }
        }

        public void ClearElectrons() {
            foreach(var electron in spawnedElectrons) {
                if(electron) Destroy(electron.gameObject);
            }
            spawnedElectrons.Clear();
        }
    }
}
