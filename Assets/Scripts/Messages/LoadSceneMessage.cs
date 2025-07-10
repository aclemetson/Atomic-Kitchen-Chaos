using UnityEngine;

namespace AtomicKitchenChaos.Messages
{
    public class LoadSceneMessage : GameEventMessage
    {
        public string sceneName;
        public string levelDataPath;
    }
}
