using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.Editor.MessageMappers;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Messages;
using UnityEditor;

namespace AtomicKitchenChaos.Editor
{
    public class RecipeUnlockMessageEditor : MessageEditor
    {
        private RecipeSO unlockRecipeSO;

        public RecipeUnlockMessageEditor(GameEventMessage message) : base(message) {
        }

        public RecipeSO UnlockRecipeSO => unlockRecipeSO;

        public override void EditorDrawingFunction() {
            RecipeUnlockMessage message = (RecipeUnlockMessage)_message;

            if (!string.IsNullOrEmpty(message.recipeSOPath) && unlockRecipeSO == null) {
                DataHandler.TryLoadSO(message.recipeSOPath, out unlockRecipeSO);
            }

            unlockRecipeSO = (RecipeSO)EditorGUILayout.ObjectField("Counter SO", unlockRecipeSO, typeof(RecipeSO), false);

            message.recipeSOPath = AssetDatabase.GetAssetPath(unlockRecipeSO);
            SetMessage(message);
        }
    }
}
