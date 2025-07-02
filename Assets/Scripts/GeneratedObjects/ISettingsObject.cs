namespace AtomicKitchenChaos.GeneratedObjects {
    public interface ISettingsObject {
        public string DisplayName { get; }
        public long UnlockCost { get; }

        public bool IsLocked { get; }

        public void UnlockObject();
    }
}
