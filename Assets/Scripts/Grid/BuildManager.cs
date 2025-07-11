using AtomicKitchenChaos.Counters;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.InputActions;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.UI;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using BuildData = AtomicKitchenChaos.Messages.BuildDataChangeMessage.BuildData;

namespace AtomicKitchenChaos.Grid
{
    public class BuildManager : MonoBehaviour
    {
        public static BuildManager Instance;

        [SerializeField] private Camera mainCamera;
        [SerializeField] private CounterSO[] counterPrefabs;
        [SerializeField] private LayerMask placementLayer;

        private Counter selectedPrefab;
        private Counter previewInstance;
        private Tile[,] tileGrid;
        private Vector2Int gridSize;

        private PlayerInputActions inputActions;
        private InputAction placeAction;
        private Tile previousHoverTile = null;
        private BuildData[] buildData;
        private float tileSpacing;
        private System.Action<InputAction.CallbackContext> cancelCallback;

        // Counter Delete variables
        private bool deleteCounterState = false;
        private Counter hoveredCounter = null;

        public PlayerInputActions InputActions => inputActions;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }

            inputActions = new PlayerInputActions();
            GenerateBuildData();
        }

        private void Start() {
            UIManager.Instance.SetDeleteButtonAction(() => EnterDeleteMode());
        }

        private void GenerateBuildData() {
            List<BuildData> buildDataList = new List<BuildData>();
            for (int i = 0; i < counterPrefabs.Length; i++) {
                int staticIndex = i;
                buildDataList.Add(new BuildData {
                    name = counterPrefabs[staticIndex].displayName,
                    price = counterPrefabs[staticIndex].price,
                    selectAction = () => {
                        SetSelectedPrefab(AssetDatabase.LoadAssetAtPath<Counter>(counterPrefabs[staticIndex].counterPrefabPath), counterPrefabs[staticIndex]);
                    }
                });
            }
            buildData = buildDataList.ToArray();
            GameEventBus.Publish(new BuildDataChangeMessage() { buildData = buildData });
        }

        public void Initialize(Tile[,] grid, float tileSpacing) {
            this.tileSpacing = tileSpacing;
            tileGrid = grid;
            gridSize = new Vector2Int(grid.GetLength(0), grid.GetLength(1));
        }


        private void OnEnable() {
            inputActions.Enable();

            placeAction = inputActions.Builder.PlaceObject;
            placeAction.performed += OnPlaceObject;
        }

        private void OnDisable() {
            placeAction.performed -= OnPlaceObject;
            inputActions.Disable();
        }
        private void Update() {
            if(deleteCounterState) {
                HandleDeleteMode();
                return;
            }

            if (selectedPrefab == null || previewInstance == null) return;

            Vector3 mouseWorld = GetMouseWorldPosition();
            Vector3 snapped = SnapToGrid(mouseWorld);
            previewInstance.transform.position = snapped;

            var gridIndex = WorldToGridIndex(snapped);
            bool valid = CanPlace(gridIndex, GetObjectSize(selectedPrefab));

            if (valid && IsWithinGrid(gridIndex)) {
                Tile hoverTile = tileGrid[gridIndex.x, gridIndex.y];
                if (hoverTile != previousHoverTile) {
                    previousHoverTile?.Highlight(false);
                    hoverTile?.Highlight(true);
                    previousHoverTile = hoverTile;
                }
            }

            SetPreviewColor(valid ? Color.green : Color.red);
        }

        public void SetSelectedPrefab(Counter prefab, CounterSO counterSO) {
            selectedPrefab = prefab;
            selectedPrefab.SetCounterSO(counterSO);
            if (previewInstance != null) Destroy(previewInstance);
            previewInstance = Instantiate(prefab);
            previewInstance.SetCounterSO(counterSO);
            previewInstance.GetComponent<Collider>().enabled = false;
            cancelCallback = _ => CancelSelection();
            inputActions.Builder.CancelAction.performed += cancelCallback;
        }

        private void CancelSelection() {
            selectedPrefab = null;
            if (previewInstance != null) Destroy(previewInstance.gameObject);

            // Make sure all Tiles are unhighlighted
            for (int x = 0; x < gridSize.x; x++) {
                for (int y = 0; y < gridSize.y; y++) {
                    tileGrid[x, y].TurnOff();
                }
            }

            if (cancelCallback != null) {
                inputActions.Builder.CancelAction.performed -= cancelCallback;
                cancelCallback = null;
            }
        }

        private void OnPlaceObject(InputAction.CallbackContext ctx) {
            if(selectedPrefab == null || previewInstance == null) return;

            Vector3 mouseWorld = GetMouseWorldPosition();
            Vector3 snapped = SnapToGrid(mouseWorld);
            Vector2Int gridIndex = WorldToGridIndex(snapped);
            Vector2Int size = GetObjectSize(selectedPrefab);

            if (CanPlace(gridIndex, size)) {
                var obj = Instantiate(selectedPrefab, snapped, Quaternion.identity);
                obj.SetCounterSO(selectedPrefab.CounterSO);
                MarkGridOccupied(gridIndex, size);
            }
        }

        private Vector3 GetMouseWorldPosition() {
            Vector2 screenPos = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementLayer)) {
                return hit.point;
            }
            return Vector3.zero;
        }

        private Vector3 SnapToGrid(Vector3 worldPos) {
            int x = Mathf.FloorToInt(worldPos.x / tileSpacing);
            int z = Mathf.FloorToInt(worldPos.z / tileSpacing);
            return new Vector3(x * tileSpacing, 1, z * tileSpacing);
        }

        private Vector2Int WorldToGridIndex(Vector3 worldPos) {
            int x = Mathf.FloorToInt(worldPos.x / tileSpacing);
            int y = Mathf.FloorToInt(worldPos.z / tileSpacing);
            return new Vector2Int(x, y);
        }

        private bool CanPlace(Vector2Int start, Vector2Int size) {
            for (int x = 0; x < size.x; x++) {
                for (int y = 0; y < size.y; y++) {
                    int checkX = start.x + x;
                    int checkY = start.y + y;

                    if (checkX < 0 || checkY < 0 || checkX >= gridSize.x || checkY >= gridSize.y) return false;
                    if (tileGrid[checkX, checkY].IsOccupied) return false;
                }
            }

            return true;
        }

        private void MarkGridOccupied(Vector2Int start, Vector2Int size) {
            for (int x = 0; x < size.x; x++) {
                for (int y = 0; y < size.y; y++) {
                    tileGrid[start.x + x, start.y + y].SetOccupied(true);
                }
            }
        }

        private Vector2Int GetObjectSize(Counter prefab) {
            var placeable = prefab.GetComponent<PlaceableObject>();
            return placeable != null ? placeable.Size : Vector2Int.one;
        }

        private void SetPreviewColor(Color color) {
            if (previewInstance == null) return;
            previewInstance.SetColor(color);
        }

        private bool IsWithinGrid(Vector2Int index) {
            return index.x >= 0 && index.y >= 0 && index.x < gridSize.x && index.y < gridSize.y;
        }

        #region DeleteMode

        private void EnterDeleteMode() {
            CancelSelection();
            cancelCallback = _ => ExitDeleteMode();
            inputActions.Builder.CancelAction.performed += cancelCallback;
            deleteCounterState = true;
        }

        private void ExitDeleteMode() {
            inputActions.Builder.CancelAction.performed -= cancelCallback;
            deleteCounterState = false;
            ClearHoveredCounterHighlight();
        }

        private void HandleDeleteMode() {
            Vector3 mouseWorld = GetMouseWorldPosition();
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 100f)) {
                var counter = hit.collider.GetComponent<Counter>();
                if (counter != null) {
                    if (hoveredCounter != counter) {
                        ClearHoveredCounterHighlight();

                        hoveredCounter = counter;
                        hoveredCounter.SetColor(Color.red);
                    }

                    if (Mouse.current.leftButton.wasPressedThisFrame) {
                        Destroy(hoveredCounter.gameObject);
                        hoveredCounter = null;
                    }
                } else {
                    ClearHoveredCounterHighlight();
                }
            } else {
                ClearHoveredCounterHighlight();
            }
        }

        private void ClearHoveredCounterHighlight() {
            if (hoveredCounter != null) {
                hoveredCounter.ResetVisual();
                hoveredCounter = null;
            }
        }

        #endregion
    }
}
