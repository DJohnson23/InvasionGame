using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MapGenerationV2
{
    [CustomEditor(typeof(MapTileSet))]
    public class MapTileSetEditor : Editor
    {
        MapTileSet mtt;

        const int tileSize = 50;
        const int tileGap = 10;
        const int tilesPerRow = 5;

        public override void OnInspectorGUI()
        {
            mtt = (MapTileSet)target;

            if(DrawDefaultInspector())
            {
                mtt.Initalize();
            }

            if(GUILayout.Button("Initialize"))
            {
                mtt.Initalize();
            }

            DrawTiles();
        }

        void DrawTiles()
        {
            Rect r = EditorGUILayout.BeginVertical();

            int space = (tileSize + tileGap) * ((mtt.tiles.Count / tilesPerRow) + 1);
            //Debug.Log(r);
            //Debug.Log(space);

            GUILayout.Space(space);

            for(int i = 0; i < mtt.tiles.Count; i++)
            {
                int x = (i % tilesPerRow) * (tileSize + tileGap);
                int y = (i / tilesPerRow) * (tileSize + tileGap);

                Rect position = new Rect(x + r.x, y + r.y, tileSize, tileSize);
                GUI.DrawTexture(position, mtt.tiles[i].CropTex(mtt.texture, tileSize, tileSize), ScaleMode.ScaleToFit);
            }

            EditorGUILayout.EndVertical();
        }
    }
}
