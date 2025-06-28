
namespace AtomicKitchenChaos.Counters.Misc
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
