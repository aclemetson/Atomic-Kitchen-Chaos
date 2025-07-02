using UnityEngine;

namespace AtomicKitchenChaos.Messages
{
    [SerializeField]
    public class RecipeUnlockMessage : AtomicObjectRequestUnlockMessage
    {
        public string recipeSOPath;

        public override void SubscriptionCheck(GameEventMessage payload) {
            RecipeUnlockMessage temp = (RecipeUnlockMessage)payload;
            if (temp.recipeSOPath == recipeSOPath) {
                Trigger();
            }
        }
    }
}
