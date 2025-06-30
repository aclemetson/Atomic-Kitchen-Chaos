using System.Collections.Generic;
using UnityEngine;

namespace AtomicKitchenChaos.GeneratedObjects.ScriptableObjects {
    [CreateAssetMenu(menuName = "Recipes/RecipeListSO")]
    public class RecipeListSO : ScriptableObject {
        public string displayName;
        public List<RecipeSO> recipeList;
    }
}
