using UnityEngine;

namespace AtomicKitchenChaos.Grid
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private int width = 20;
        [SerializeField] private int height = 20;
        [SerializeField] private float tileSize = 2f;
        [SerializeField] private GameObject tilePrefab;

        public Tile[,] Tiles { get; private set; }

        private void Start() {
            Tiles = new Tile[width, height];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {

                    Vector3 pos = new Vector3(x * tileSize, 0.1f, y * tileSize);

                    GameObject tileObj = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                    Tile tile = tileObj.GetComponent<Tile>();
                    tile.Initialize(new Vector2Int(x, y));
                    Tiles[x, y] = tile;
                }
            }

            BuildManager.Instance.Initialize(Tiles, tileSize);
        }
    }
}
