using AtomicKitchenChaos.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.GeneratedObjects.ScriptableObjects {
    [CreateAssetMenu(menuName = "Recipes/RecipeSO")]
    public class RecipeSO : ScriptableObject, ISettingsObject {

        public string displayName;
        public MaterialCount[] materials;
        public AtomicObjectSO[] results;
        public ExoticMaterialCount[] exoticMaterialCounts;
        public float cookTime;
        public long unlockCost;
        private bool isLocked;

        public string DisplayName => displayName;
        public long UnlockCost => unlockCost;

        public bool IsLocked => isLocked;

        private void OnEnable() {
            isLocked = true;
        }

        [Serializable]
        public struct MaterialCount {
            public AtomicObjectSO atomicObject;
            public int quantity;
        }

        [Serializable]
        public struct ExoticMaterialCount {
            public ExoticMaterialSO exoticMaterial;
            public int quantity;
        }
        
        public Dictionary<ExoticMaterialSO.ExoticMaterial, int> GetExoticMaterialCounts() {
            var results = new Dictionary<ExoticMaterialSO.ExoticMaterial, int>();
            foreach (var item in exoticMaterialCounts) {
                results.Add(item.exoticMaterial.materialName, item.quantity);
            }
            return results;
        }

        public Dictionary<AtomicObjectSO, int> GetRecipeDictionary() {
            var results = new Dictionary<AtomicObjectSO, int>();
            foreach (var item in materials) {
                results.Add(item.atomicObject, item.quantity);
            }
            return results;
        }

        public void UnlockObject() {
            isLocked = false;
            GameEventBus.Publish(new RecipeUnlockMessage() { recipeSOPath = AssetDatabase.GetAssetPath(this) });
        }
    }
}