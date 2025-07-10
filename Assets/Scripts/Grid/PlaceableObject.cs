using UnityEngine;

namespace AtomicKitchenChaos.Grid
{
    public class PlaceableObject : MonoBehaviour
    {
        [SerializeField] private Vector2Int size = new Vector2Int(2, 2);

        public Vector2Int Size => size;
    }
}
