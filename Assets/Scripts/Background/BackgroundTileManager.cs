using UnityEngine;
using System.Collections.Generic;

namespace Background
{
    public class BackgroundTileManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private float margin = 0.5f;
        private readonly Vector2 tileDimensions = new (1920, 1080);
        private readonly Dictionary<Vector2Int, GameObject> activeTiles = new ();

        private void Start()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            InitializeTiles();
        }

        private void Update()
        {
            UpdateTiles();
        }

        private void InitializeTiles()
        {
            var cameraPos = mainCamera.transform.position;
            var initialTilePos = ScreenToTilePosition(cameraPos);

            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    Vector2Int tilePos = initialTilePos + new Vector2Int(x, y);
                    SpawnTileAt(tilePos);
                }
            }
        }

        private void UpdateTiles()
        {
            Vector3 cameraPosition = mainCamera.transform.position;

            // Calculate bounds with margin in world units based on the proportion of tile size
            float marginInWorldUnitsX = margin * tileDimensions.x;
            float marginInWorldUnitsY = margin * tileDimensions.y;

            // Update left, right, bottom, and top bounds based on current camera position
            float screenLeft = cameraPosition.x - (Screen.width / 2f) - marginInWorldUnitsX;
            float screenRight = cameraPosition.x + (Screen.width / 2f) + marginInWorldUnitsX;
            float screenBottom = cameraPosition.y - (Screen.height / 2f) - marginInWorldUnitsY;
            float screenTop = cameraPosition.y + (Screen.height / 2f) + marginInWorldUnitsY;

            // Convert screen bounds to tile positions to determine the grid area to cover
            Vector2Int minTilePos = ScreenToTilePosition(new Vector2(screenLeft, screenBottom));
            Vector2Int maxTilePos = ScreenToTilePosition(new Vector2(screenRight, screenTop));

            Debug.Log($"Camera Position: {cameraPosition}, minTilePos: {minTilePos}, maxTilePos: {maxTilePos}");

            // Iterate through the tile range and spawn missing tiles as needed
            for (int x = minTilePos.x; x <= maxTilePos.x; x++)
            {
                for (int y = minTilePos.y; y <= maxTilePos.y; y++)
                {
                    Vector2Int tilePos = new Vector2Int(x, y);
                    if (!activeTiles.ContainsKey(tilePos))
                    {
                        SpawnTileAt(tilePos);
                        Debug.Log($"Spawning tile at: {tilePos}");
                    }
                }
            }
        }


        private void SpawnTileAt(Vector2Int tilePos)
        {
            var screenPos = TileToScreenPosition(tilePos);
            var newTile = Instantiate(tilePrefab, transform);
            var rectTransform = newTile.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = screenPos;
            
            var tileBehaviour = newTile.GetComponent<BackgroundTileBehaviour>();
            tileBehaviour.Initialize(this, tilePos, margin);

            activeTiles[tilePos] = newTile;
        }

        private Vector2Int ScreenToTilePosition(Vector2 screenPos)
        {
            int x = Mathf.FloorToInt(screenPos.x / tileDimensions.x);
            int y = Mathf.FloorToInt(screenPos.y / tileDimensions.y);
            return new Vector2Int(x, y);
        }

        private Vector2 TileToScreenPosition(Vector2Int tilePos)
        {
            return new Vector2(tilePos.x * tileDimensions.x, tilePos.y * tileDimensions.y);
        }
        
        public void RemoveTile(Vector2Int tilePos)
        {
            if (!activeTiles.ContainsKey(tilePos)) return;
            
            activeTiles.Remove(tilePos);
        }
    }
}
