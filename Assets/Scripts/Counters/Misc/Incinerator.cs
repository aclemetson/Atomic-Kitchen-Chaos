using AtomicKitchenChaos.Players;
using UnityEngine;

namespace AtomicKitchenChaos.Counters
{
    public class Incinerator : Counter {

        protected override void Interact() {
            if(playerManager.HasAtomicObject()) {
                playerManager.RemoveAtomicObject();
            }
        }

        protected override void SettingsInteraction() {
            
        }

    }
}
