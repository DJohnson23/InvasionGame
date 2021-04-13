using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MapTileTexture
{
    [SerializeField]
    public Vector2Int min;
    [SerializeField]
    public Vector2Int max;

    [SerializeField]
    public Vector2 relMin;
    [SerializeField]
    public Vector2 relMax;

    public Vector2Int Size
    {
        get
        {
            return max - min;
        }
    }

    public MapTileTexture()
    {
        min = new Vector2Int();
        max = new Vector2Int();
    }

    public MapTileTexture(Vector2Int min, Vector2Int max, Texture2D tex, Vector2Int edgePadding)
    {
        this.min = min + edgePadding;
        this.max = max - edgePadding;

        Vector2 texSize = new Vector2(tex.width, tex.height);

        relMin = new Vector2(this.min.x / texSize.x, this.min.y / texSize.y);
        relMax = new Vector2(this.max.x / texSize.x, this.max.y / texSize.y);
    }

    public Vector2[] GetSquareCoords()
    {
        Vector2[] coords = new Vector2[4];

        coords[0] = relMin;
        coords[1] = new Vector2(relMin.x, relMax.y);
        coords[2] = relMax;
        coords[3] = new Vector2(relMax.x, relMin.y);

        return coords;
    }

    public Vector2[] GetSquareCoords(int orientation)
    {
        orientation %= 4;

        Vector2[] coords = new Vector2[4];

        coords[(0 + orientation) % 4] = relMin;
        coords[(1 + orientation) % 4] = new Vector2(relMin.x, relMax.y);
        coords[(2 + orientation) % 4] = relMax;
        coords[(3 + orientation) % 4] = new Vector2(relMax.x, relMin.y);

        return coords;
    }

    public Texture2D CropTex(Texture2D tex, int width, int height)
    {
        Vector2Int actualSize = max - min;
        Texture2D newTex = new Texture2D(width, height);

        float widthScale = actualSize.x / (float)width;
        float heightScale = actualSize.y / (float)height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int sampleX = min.x + (int)(x * widthScale);
                int sampleY = min.y + (int)(y * heightScale);
                newTex.SetPixel(x, y, tex.GetPixel(sampleX, sampleY));
            }
        }

        newTex.Apply();

        return newTex;
    }

    public Texture2D CropTex(Texture2D tex, int width, int height, int orientation)
    {
        Vector2Int actualSize = max - min;
        Texture2D newTex = new Texture2D(width, height);

        float widthScale = actualSize.x / (float)width;
        float heightScale = actualSize.y / (float)height;

        orientation += 3;
        orientation %= 4;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int newX = x;
                int newY = y;

                for (int i = 0; i < 3 - orientation; i++)
                {
                    int origY = newY;
                    newY = (int)((width - newX) / (float)width * height) - 1;
                    newX = (int)(origY / (float)height * width);
                }

                int sampleX = min.x + (int)(newX * widthScale);
                int sampleY = min.y + (int)(newY * heightScale);

                newTex.SetPixel(x, y, tex.GetPixel(sampleX, sampleY));
            }
        }

        newTex.Apply();

        return newTex;
    }
}