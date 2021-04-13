using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewMapTileTexture", menuName = "MapGeneration/MapTileSet")]
[System.Serializable]
public class MapTileSet : ScriptableObject
{
    [SerializeField]
    public bool clipTransparent = true;
    [SerializeField]
    public Texture2D texture;
    [SerializeField]
    public Vector2Int numTiles = new Vector2Int();
    public Vector2Int edgePadding = new Vector2Int(2, 2);

    [SerializeField]
    [HideInInspector]
    public List<MapTileTexture> tiles;

    Vector2Int tileSize = new Vector2Int();

    public void Initalize()
    {
        if (texture == null)
        {
            return;
        }

        tileSize.x = texture.width / Mathf.Max(numTiles.x, 1);
        tileSize.y = texture.height / Mathf.Max(numTiles.y, 1);

        tiles = new List<MapTileTexture>();

        for (int y = numTiles.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < numTiles.x; x++)
            {
                CreateTile(x, y);
            }
        }
    }

    void CreateTile(int x, int y)
    {
        Vector2Int min = new Vector2Int(x * tileSize.x, y * tileSize.y);
        Vector2Int max = min + tileSize;

        for (int i = min.x; i < max.x; i++)
        {
            for (int j = min.y; j < max.y; j++)
            {
                if (!clipTransparent || texture.GetPixel(i, j).a > 0)
                {
                    tiles.Add(new MapTileTexture(min, max, texture, edgePadding));
                    return;
                }
            }
        }
    }
}