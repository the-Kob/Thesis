using UnityEngine;

public class StaticBackground : MonoBehaviour
{
    public GameObject backgroundTilePrefab;
    public RectTransform canvasRectTransform;

    // Number of tiles horizontally and vertically, configurable in the Inspector
    public int tilesHorizontally = 3;
    public int tilesVertically = 3;

    private float tileWidth = 1920f;
    private float tileHeight = 1080f;
    
    void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        // Calculate the starting position to center the grid on the canvas
        float startX = -(tilesHorizontally / 2f) * tileWidth;
        float startY = (tilesVertically / 2f) * tileHeight;

        // Loop through and create the tiles based on inspector-defined grid size
        for (int y = 0; y < tilesVertically; y++)
        {
            for (int x = 0; x < tilesHorizontally; x++)
            {
                Vector3 position = new Vector3(startX + x * tileWidth, startY - y * tileHeight, 0);
                GameObject tile = Instantiate(backgroundTilePrefab, transform);
                tile.GetComponent<RectTransform>().anchoredPosition = position;
            }
        }
    }
}
