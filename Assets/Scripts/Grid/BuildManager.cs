using AtomicKitchenChaos.Counters;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.InputActions;
using AtomicKitchenChaos.Messages;
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

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }

            inputActions = new PlayerInputActions();
            GenerateBuildData();
        }

        private void GenerateBuildData() {
            List<BuildData> buildDataList = new List<BuildData>();
            for (int i = 0; i < counterPrefabs.Length; i++) {
                int staticIndex = i;
                buildDataList.Add(new BuildData {
                    name = counterPrefabs[i].displayName,
                    selectAction = () => {
                        selectedPrefab = AssetDatabase.LoadAssetAtPath<Counter>(counterPrefabs[staticIndex].counterPrefabPath);
                    }
                });
            }
            buildData = buildDataList.ToArray();
            GameEventBus.Publish(new BuildDataChangeMessage() { buildData = buildData });
        }

        public void Initialize(Tile[,] grid) {
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
            if (selectedPrefab == null || previewInstance == null) return;

            Vector3 mouseWorld = GetMouseWorldPosition();
            Vector3 snapped = SnapToGrid(mouseWorld);
            previewInstance.transform.position = snapped;

            var gridIndex = WorldToGridIndex(snapped);
            bool valid = CanPlace(gridIndex, GetObjectSize(selectedPrefab));

            Tile hoverTile = tileGrid[gridIndex.x, gridIndex.y];
            if (hoverTile != previousHoverTile) {
                previousHoverTile?.Highlight(false);
                hoverTile?.Highlight(true);
                previousHoverTile = hoverTile;
            }

            SetPreviewColor(valid ? Color.green : Color.red);
        }

        public void SetSelectedPrefab(Counter prefab) {
            selectedPrefab = prefab;
            if (previewInstance != null) Destroy(previewInstance);
            previewInstance = Instantiate(prefab);
            previewInstance.GetComponent<Collider>().enabled = false;
        }

        private void OnPlaceObject(InputAction.CallbackContext ctx) {
            if(selectedPrefab == null || previewInstance == null) return;

            Vector3 mouseWorld = GetMouseWorldPosition();
            Vector3 snapped = SnapToGrid(mouseWorld);
            Vector2Int gridIndex = WorldToGridIndex(snapped);
            Vector2Int size = GetObjectSize(selectedPrefab);

            if (CanPlace(gridIndex, size)) {
                var obj = Instantiate(selectedPrefab, snapped, Quaternion.identity);
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
            int x = Mathf.RoundToInt(worldPos.x);
            int z = Mathf.RoundToInt(worldPos.z);
            return new Vector3(x, 0, z);
        }

        private Vector2Int WorldToGridIndex(Vector3 worldPos) {
            return new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.z));
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
            return placeable != null ? placeable.Size : 2 * Vector2Int.one;
        }

        private void SetPreviewColor(Color color) {
            if (previewInstance == null) return;
            foreach(var r in previewInstance.GetComponentsInChildren<Renderer>()) {
                r.material.color = color;
            }
        }
    }
}
