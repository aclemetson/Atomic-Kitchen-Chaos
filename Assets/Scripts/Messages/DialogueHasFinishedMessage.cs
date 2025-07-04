namespace AtomicKitchenChaos.Messages
{
    public class DialogueHasFinishedMessage : UnlockMessage
    {
        public string dialogueName;

        public override void SubscriptionCheck(GameEventMessage payload) {
            DialogueHasFinishedMessage message = (DialogueHasFinishedMessage)payload;
            if (message.dialogueName == dialogueName) {
                Trigger();
            }
        }
    }
}
