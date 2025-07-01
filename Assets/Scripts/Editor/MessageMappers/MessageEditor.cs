using AtomicKitchenChaos.Messages;

namespace AtomicKitchenChaos.Editor.MessageMappers {
    public abstract class MessageEditor {
        protected GameEventMessage _message;

        public GameEventMessage Message => _message;
        public abstract void EditorDrawingFunction();

        public MessageEditor(GameEventMessage message) {
            _message = message;
        }

        public void SetMessage(GameEventMessage message) {
            _message = message;
        }
    }
}
