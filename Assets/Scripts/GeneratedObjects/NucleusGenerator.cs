using System;
using System.Collections.Generic;
using UnityEngine;

namespace AtomicKitchenChaos.GeneratedObjects
{
    public class NucleusGenerator : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject protonPrefab;
        [SerializeField] private GameObject neutronPrefab;
        [SerializeField] private Transform nucleusContainer;
        [SerializeField] private Transform visualContainer;

        [Header("Configuration")]
        [SerializeField] private int protonCount;
        [SerializeField] private int neutronCount;
        [SerializeField] private float particleSpacing = .25f;
        [SerializeField] private float nucleusRadius = 0.4f;

        [Header("Scaling Factors")]
        [SerializeField] private float baseSize = 0.2f;

        private List<Transform> spawnedPartciles = new List<Transform>();

        private void Start() {
            if (protonCount + neutronCount != 0) {
                GenerateNucleus(protonCount, neutronCount);
            }
        }

        public void GenerateNucleus(int protons, int neutrons) {
            ClearNucleus();
            float scaleFactor = CalculateScaleFactor(protons + neutrons);

            List<Vector3> positions = GenerateClusterPositions(protons + neutrons);

            int i = 0;
            for (; i < protons; i++) {
                SpawnParticle(protonPrefab, positions[i]);
            }

            for (; i < protons + neutrons; i++) {
                SpawnParticle(neutronPrefab, positions[i]);
            }

            visualContainer.localScale = Vector3.one * scaleFactor;
        }

        private void SpawnParticle(GameObject prefab, Vector3 localPosition) {
            GameObject particle = Instantiate(prefab, nucleusContainer);
            particle.transform.localPosition = localPosition;
            spawnedPartciles.Add(particle.transform);
        }

        private List<Vector3> GenerateClusterPositions(int totalParticles) {
            List<Vector3> positions = new();
            int attempts = 0;

            while (positions.Count < totalParticles && attempts < 1000) {
                Vector3 candidate = UnityEngine.Random.insideUnitSphere * nucleusRadius;

                bool overlaps = positions.Exists(pos =>
                    Vector3.Distance(pos, candidate) < particleSpacing);

                if (!overlaps)
                    positions.Add(candidate);

                attempts++;
            }

            Vector3 center = Vector3.zero;
            foreach (var pos in positions)
                center += pos;
            center /= positions.Count;

            for (int i = 0; i < positions.Count; i++)
                positions[i] -= center;

            return positions;
        }

        public void ClearNucleus() {
            foreach(Transform t in spawnedPartciles) {
                if(t) Destroy(t.gameObject);
            }
            spawnedPartciles.Clear();
        }

        private float CalculateScaleFactor(int particleCount) {
            float shrinkFactor = Mathf.Clamp01(1f - (particleCount - 1) * 0.03f);
            return baseSize * shrinkFactor;
        }
    }
}
