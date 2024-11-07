using UnityEngine;
using System.Collections.Generic;

namespace Background
{
public class BackgroundTileManager : MonoBehaviour
{
   public GameObject tilePrefab; // Assign your tile prefab here
    public float tileSize = 1920f; // Width of the tile prefab in world units
    public int bufferTiles = 1; // Number of tiles beyond the camera view to spawn

    private HashSet<Vector2Int> tilePositions = new HashSet<Vector2Int>(); // Track positions of active tiles
    private Dictionary<Vector2Int, GameObject> activeTiles = new Dictionary<Vector2Int, GameObject>(); // Store active tile GameObjects
    private Queue<GameObject> tilePool = new Queue<GameObject>();

    private Vector2 lastCameraPosition;

    void Start()
    {
        Vector2 cameraPosition = Camera.main.transform.position;
        lastCameraPosition = cameraPosition;

        // Initial tile spawning around the camera
        SpawnTilesAroundCamera(cameraPosition);
    }

    void Update()
    {
        Vector2 cameraPosition = Camera.main.transform.position;

        // Only spawn tiles if the camera has moved a significant distance (tileSize / 2)
        if (Vector2.Distance(cameraPosition, lastCameraPosition) >= tileSize / 2)
        {
            SpawnTilesAroundCamera(cameraPosition);
            lastCameraPosition = cameraPosition;
        }
    }

    void SpawnTilesAroundCamera(Vector2 cameraPosition)
    {
        // Calculate how many tiles fit in the visible area plus buffer
        float cameraHeight = Camera.main.orthographicSize * 2;
        float cameraWidth = cameraHeight * Camera.main.aspect;

        // Calculate the total width and height including the buffer area
        float spawnAreaWidth = cameraWidth + (bufferTiles * tileSize * 2);
        float spawnAreaHeight = cameraHeight + (bufferTiles * tileSize * 2);

        // Determine the starting and ending positions in the grid
        int startX = Mathf.FloorToInt((cameraPosition.x - spawnAreaWidth / 2) / tileSize);
        int endX = Mathf.CeilToInt((cameraPosition.x + spawnAreaWidth / 2) / tileSize);
        int startY = Mathf.FloorToInt((cameraPosition.y - spawnAreaHeight / 2) / tileSize);
        int endY = Mathf.CeilToInt((cameraPosition.y + spawnAreaHeight / 2) / tileSize);

        // Loop through each grid position within the calculated bounds
        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y);

                // Check if tile exists at this position before creating
                if (!tilePositions.Contains(gridPosition))
                {
                    InstantiateTile(gridPosition);
                }
            }
        }
    }

    void InstantiateTile(Vector2Int gridPosition)
    {
        Vector2 worldPosition = GridToWorldPosition(gridPosition);

        GameObject tile;
        if (tilePool.Count > 0)
        {
            tile = tilePool.Dequeue();
            tile.SetActive(true);
        }
        else
        {
            tile = Instantiate(tilePrefab);
        }

        RectTransform rectTransform = tile.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.position = worldPosition;
            rectTransform.SetParent(transform, false);
        }

        activeTiles[gridPosition] = tile;
        tilePositions.Add(gridPosition);
    }

    Vector2Int WorldToGridPosition(Vector2 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / tileSize);
        int y = Mathf.FloorToInt(worldPosition.y / tileSize);
        return new Vector2Int(x, y);
    }

    Vector2 GridToWorldPosition(Vector2Int gridPosition)
    {
        float x = gridPosition.x * tileSize;
        float y = gridPosition.y * tileSize;
        return new Vector2(x, y);
    }

    void DespawnTile(Vector2Int gridPosition)
    {
        if (activeTiles.TryGetValue(gridPosition, out GameObject tile))
        {
            tile.SetActive(false);
            tilePool.Enqueue(tile);
            activeTiles.Remove(gridPosition);
            tilePositions.Remove(gridPosition);
        }
    }
}

}
