using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AtomicKitchenChaos.GeneratedObjects {
    [CreateAssetMenu()]
    public class RecipeSO : ScriptableObject, ISettingsObject {

        public string displayName;
        public Material[] materials;
        public AtomicObjectSO result;

        public string DisplayName => displayName;

        [Serializable]
        public struct Material {
            public AtomicObjectSO atomicObject;
            public int quantity;
        }

        public bool ContainsAtomicObject(AtomicObject target) {
            return materials.Any(m => m.atomicObject == target);
        }

        public Dictionary<string, int> GetRecipeDictionary() {
            var results = new Dictionary<string, int>();
            foreach (var item in materials) {
                results.Add(item.atomicObject.displayName, item.quantity);
            }
            return results;
        }
    }
}