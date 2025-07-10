using UnityEngine;

namespace AtomicKitchenChaos.Grid
{
    public class Tile : MonoBehaviour
    {
        public Vector2Int GridPosition { get; private set; }
        public bool IsOccupied { get; private set; }

        [SerializeField] private Renderer tileRenderer;
        [SerializeField] private Color defaultColor = Color.gray;
        [SerializeField] private Color hoverColor = Color.yellow;
        [SerializeField] private Color occupiedColor = Color.red;

        public void Initialize(Vector2Int gridPosition) {
            GridPosition = gridPosition;
            SetColor(defaultColor);
        }

        public void SetOccupied(bool value) {
            IsOccupied = value;
            SetColor(IsOccupied ? occupiedColor : defaultColor);
        }

        public void Highlight(bool hovering) {
            if (!IsOccupied) {
                SetColor(hovering ? hoverColor : defaultColor);
            }
        }

        private void SetColor(Color color) {
            if (tileRenderer != null) {
                tileRenderer.material.color = color;
            }
        }
    }
}
