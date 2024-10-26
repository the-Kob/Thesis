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
            var cameraPosition = mainCamera.transform.position;
            
            var screenLeft = cameraPosition.x - Screen.width / 2f - margin * tileDimensions.x;
            var screenRight = cameraPosition.x + Screen.width / 2f + margin * tileDimensions.x;
            var screenBottom = cameraPosition.y - Screen.height / 2f - margin * tileDimensions.y;
            var screenTop = cameraPosition.y + Screen.height / 2f + margin * tileDimensions.y;


            var minTilePos = ScreenToTilePosition(new Vector2(screenLeft, screenBottom));
            var maxTilePos = ScreenToTilePosition(new Vector2(screenRight, screenTop));

            for (var x = minTilePos.x; x <= maxTilePos.x; x++)
            {
                for (var y = minTilePos.y; y <= maxTilePos.y; y++)
                {
                    var tilePos = new Vector2Int(x, y);

                    if (!activeTiles.ContainsKey(tilePos))
                    {
                        SpawnTileAt(tilePos);
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
