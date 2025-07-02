using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.GeneratedObjects.AtomicObjects;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.Counters.Misc
{
    public class Storage : Counter {

        [SerializeField] private int maxContainerSize;
        [SerializeField] private int maxStackSize;
        [SerializeField] protected AtomicObject atomPrefab;

        private List<StorageData> items;

        private void Awake() {
            items = new List<StorageData>();
        }

        protected override void Interact() {
            if(playerManager.HasAtomicObject() && !IsFull()) {
                StorePlayerItem();
            }
        }

        protected override void SettingsInteraction() {
            UIManager.Instance.PopulateStorageMenu(items.ToArray(), GiveItemToPlayer);
        }

        private void StorePlayerItem() {

            string assetPath = AssetDatabase.GetAssetPath(playerManager.AtomicObject.AtomicObjectSO);
            string displayName = playerManager.AtomicObject.AtomicObjectSO.displayName;

            var match = items
                .Select((t, index) => new { Item = t, Index = index })
                .FirstOrDefault(x => x.Item.AtomicObjectSOPath == assetPath && x.Item.quantity < maxStackSize);

            // If no matching items exist in the list, make a new stack
            if(match == null) {
                StorageData item = new StorageData() {
                    AtomicObjectSOPath = assetPath,
                    DisplayName = displayName,
                    quantity = 1,
                };
                items.Add(item);
            } else {
                var item = match.Item;
                var index = match.Index;
                item.quantity++;
                items[index] = item;
            }

            playerManager.RemoveAtomicObject();
        }

        private void GiveItemToPlayer(int index) {
            if (!playerManager.HasAtomicObject()) {
                StorageData data = items[index];
                DataHandler.TryLoadSO(data.AtomicObjectSOPath, out AtomicObjectSO atomicObjectSO);
                var atomicObject = Instantiate(atomPrefab);
                atomicObject.SetAtomicObjectSO(atomicObjectSO);
                playerManager.SetAtomicObject(atomicObject);
                data.quantity--;
                items[index] = data;

                // Remove item from storage all together if quantity reaches zero
                if (items[index].quantity == 0) {
                    items.RemoveAt(index);
                }
            }
        }

        private bool IsFull() {
            if(items.Count == maxContainerSize) {
                string atomicObjectSOPath = AssetDatabase.GetAssetPath(playerManager.AtomicObject.AtomicObjectSO);
                StorageData[] likeItems = items.FindAll(t => t.AtomicObjectSOPath == atomicObjectSOPath).ToArray();
                bool isNotFull = likeItems.Any(t => t.quantity < maxStackSize);
                if(!isNotFull) {
                    Debug.Log($"Storage unit is full");
                    return true;
                }
            }
            return false;
        }
    }
}
