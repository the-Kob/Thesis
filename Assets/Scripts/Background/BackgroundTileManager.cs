using System.Collections.Generic;
using UnityEngine;

namespace Background
{
    public class BackgroundTileManager : MonoBehaviour
    {
        public Camera mainCamera;
        public GameObject backgroundTilePrefab;  // Prefab with the BackgroundTile script
        public Vector2 tileSize = new Vector2(1920, 1080);  // Size of each tile
        public RectTransform canvasRectTransform;  // Reference to the parent canvas's RectTransform

        private Vector2 cameraPreviousPosition;
        private Dictionary<Vector2, GameObject> activeTiles = new Dictionary<Vector2, GameObject>(); // Track active tiles

        void Start()
        {
            cameraPreviousPosition = mainCamera.transform.position;
            // Initialize by spawning the 9 initial tiles around the camera
            InitializeTiles();
        }

        void Update()
        {
            Vector2 cameraMoveDelta = (Vector2)mainCamera.transform.position - cameraPreviousPosition;

            if (cameraMoveDelta.magnitude >= tileSize.x / 2 || cameraMoveDelta.magnitude >= tileSize.y / 2)
            {
                cameraPreviousPosition = mainCamera.transform.position;
                UpdateTiles();  // Check for new tiles and add as needed
            }
        }

        // Spawns the initial 9 tiles around the camera
        void InitializeTiles()
        {
            Vector2 cameraPosition = mainCamera.transform.position;
            int camX = Mathf.FloorToInt(cameraPosition.x / tileSize.x);
            int camY = Mathf.FloorToInt(cameraPosition.y / tileSize.y);

            // Spawning 9 tiles: the central tile and 8 surrounding tiles
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2 tileCoord = new Vector2(camX + x, camY + y);
                    if (!activeTiles.ContainsKey(tileCoord))
                    {
                        CreateTile(tileCoord);
                    }
                }
            }
        }

        // Updates tiles based on camera movement and adds new tiles dynamically
        void UpdateTiles()
        {
            Vector2 cameraPosition = mainCamera.transform.position;
            int camX = Mathf.FloorToInt(cameraPosition.x / tileSize.x);
            int camY = Mathf.FloorToInt(cameraPosition.y / tileSize.y);

            int halfTilesX = Mathf.CeilToInt(mainCamera.orthographicSize * mainCamera.aspect / tileSize.x) + 1;
            int halfTilesY = Mathf.CeilToInt(mainCamera.orthographicSize / tileSize.y) + 1;

            for (int x = -halfTilesX; x <= halfTilesX; x++)
            {
                for (int y = -halfTilesY; y <= halfTilesY; y++)
                {
                    Vector2 tileCoord = new Vector2(camX + x, camY + y);

                    // If this tile doesn't exist yet, create it
                    if (!activeTiles.ContainsKey(tileCoord))
                    {
                        CreateTile(tileCoord);
                    }
                }
            }
        }

        // Function to create and position a tile at the given coordinate
        void CreateTile(Vector2 tileCoord)
        {
            GameObject newTile = Instantiate(backgroundTilePrefab);
            RectTransform tileRectTransform = newTile.GetComponent<RectTransform>();
            tileRectTransform.SetParent(canvasRectTransform, false);

            tileRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            tileRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            tileRectTransform.pivot = new Vector2(0.5f, 0.5f);
            tileRectTransform.sizeDelta = tileSize;

            Vector2 tilePosition = new Vector2(tileCoord.x * tileSize.x, tileCoord.y * tileSize.y);
            tileRectTransform.anchoredPosition = tilePosition;

            activeTiles.Add(tileCoord, newTile);
        }
    }
}
