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

            Vector2Int originOffet = new Vector2Int(width / 2, height / 2);

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {

                    int worldX = x - originOffet.x;
                    int worldZ = y - originOffet.y;
                    Vector3 pos = new Vector3(worldX * tileSize, 0.1f, worldZ * tileSize);

                    GameObject tileObj = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                    Tile tile = tileObj.GetComponent<Tile>();
                    tile.Initialize(new Vector2Int(worldX, worldZ));
                    Tiles[x, y] = tile;
                }
            }

            BuildManager.Instance.Initialize(Tiles);
        }
    }
}
