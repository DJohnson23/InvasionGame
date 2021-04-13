using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapPalette))]
public class MapPaletteEditor : Editor
{
	MapPalette palette;
	Texture2D checkBox;
	Texture2D redX;

	bool showFloorTiles;
	bool[] floorTilesShown;

	bool showWallTiles;

	bool showCeilingTiles;

	private void OnEnable()
	{
		checkBox = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Scripts/MapGeneration/Icons/Check.png", typeof(Texture2D));
		redX = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Scripts/MapGeneration/Icons/RedX.png", typeof(Texture2D));
		palette = (MapPalette)target;

		floorTilesShown = new bool[palette.floorTiles.Length];
		showFloorTiles = false;

		showWallTiles = false;
	}

	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();

		DrawFloorTileList();
		DrawWallTiles();
		DrawCeilingTiles();

		if(EditorGUI.EndChangeCheck())
		{
			MapGenerator mapGen = FindObjectOfType<MapGenerator>();

			if(mapGen.mapPalette == palette)
			{
				mapGen.RefreshMap();
			}
		}
	}

	void DrawCeilingTiles()
	{
		showCeilingTiles = EditorGUILayout.Foldout(showCeilingTiles, "Ceiling Tiles");

		if (!showCeilingTiles)
		{
			return;
		}

		EditorGUI.indentLevel++;

		palette.ceilingTile = (GameObject)EditorGUILayout.ObjectField("Ceiling Tile", palette.ceilingTile, typeof(GameObject), false);

		EditorGUI.indentLevel--;
	}

	void DrawWallTiles()
	{
		showWallTiles = EditorGUILayout.Foldout(showWallTiles, "Wall Tiles");

		if(!showWallTiles)
		{
			return;
		}

		EditorGUI.indentLevel++;

		palette.wallTileSize = EditorGUILayout.Vector3Field("Tile Size", palette.wallTileSize);
		palette.wallTrim = (GameObject)EditorGUILayout.ObjectField("Wall Bottom Trim", palette.wallTrim, typeof(GameObject), false);
		palette.wallTile = (GameObject)EditorGUILayout.ObjectField("Wall Tile", palette.wallTile, typeof(GameObject), false);

		EditorGUI.indentLevel--;
	}

	void DrawFloorTileList()
	{
		showFloorTiles = EditorGUILayout.Foldout(showFloorTiles, "Floor Tiles");

		if(!showFloorTiles)
		{
			return;
		}

		EditorGUI.indentLevel++;


		palette.floorTileSize = EditorGUILayout.Vector3Field("Tile Size", palette.floorTileSize);

		int newLength = Mathf.Max(EditorGUILayout.IntField("List Size", palette.floorTiles.Length), 0);

		if (newLength != palette.floorTiles.Length)
		{
			MapTile[] newTiles = new MapTile[newLength];
			bool[] newShown = new bool[newLength];

			for (int i = 0; i < newLength && i < palette.floorTiles.Length; i++)
			{
				newTiles[i] = palette.floorTiles[i];
				newShown[i] = floorTilesShown[i];
			}

			palette.floorTiles = newTiles;
			floorTilesShown = newShown;
		}

		EditorGUI.indentLevel++;

		for (int i = 0; i < palette.floorTiles.Length; i++)
		{
			MapTile tile = palette.floorTiles[i];
			floorTilesShown[i] = EditorGUILayout.Foldout(floorTilesShown[i], i.ToString());
			//EditorGUILayout.LabelField(i + ":");

			if(floorTilesShown[i])
			{
				EditorGUI.indentLevel++;
				DrawFloorTile(tile);
				EditorGUI.indentLevel--;
			}
		}

		EditorGUI.indentLevel -= 2;
	}

	void DrawFloorTile(MapTile tile)
	{
		int padding = 2;
		int cellSize = 25;
		int gapSize = 5;

		tile.selectionType = (MapTileSelectionType)EditorGUILayout.EnumPopup("Selection Type", tile.selectionType);

		if(tile.selectionType == MapTileSelectionType.Constant)
		{
			tile.constantPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", tile.constantPrefab, typeof(GameObject), false);
		}
		else if(tile.selectionType == MapTileSelectionType.RandomFromList)
		{
			DrawRandomList(tile);
		}

		tile.extraRotation = EditorGUILayout.FloatField("Extra Rotation", tile.extraRotation);

		GUIStyle style = GUI.skin.button;

		style.padding = new RectOffset(padding, padding, padding, padding);
		style.alignment = TextAnchor.MiddleCenter;

		int gridSize = cellSize * 3 + gapSize * 2;
		int gridMargin = 10;

		Rect r = EditorGUILayout.BeginHorizontal(GUILayout.Height(gridSize + gridMargin * 2));
		r.y += gridMargin;
		r.x += r.width - gridSize - gridMargin;

		EditorGUILayout.Separator();

		GUI.skin.button.alignment = TextAnchor.MiddleRight;

		Rect rect = new Rect(r);
		rect.width = cellSize;
		rect.height = cellSize;

		DrawGridButton(rect, style, MapTileRuleArea.TopLeft, tile);

		rect.x += cellSize + gapSize;
		DrawGridButton(rect, style, MapTileRuleArea.TopMiddle, tile);

		rect.x += cellSize + gapSize;
		DrawGridButton(rect, style, MapTileRuleArea.TopRight, tile);

		rect.x = r.x;
		rect.y += cellSize + gapSize;
		DrawGridButton(rect, style, MapTileRuleArea.LeftMiddle, tile);

		rect.x += (cellSize + gapSize) * 2;
		DrawGridButton(rect, style, MapTileRuleArea.RightMiddle, tile);

		rect.x = r.x;
		rect.y += cellSize + gapSize;
		DrawGridButton(rect, style, MapTileRuleArea.BottomLeft, tile);

		rect.x += cellSize + gapSize;
		DrawGridButton(rect, style, MapTileRuleArea.BottomMiddle, tile);

		rect.x += cellSize + gapSize;
		DrawGridButton(rect, style, MapTileRuleArea.BottomRight, tile);

		EditorGUILayout.EndHorizontal();
	}

	void DrawRandomList(MapTile tile)
	{
		int size = Mathf.Max(EditorGUILayout.IntField("Size", tile.randomList.Length), 1);

		if(size != tile.randomList.Length || size != tile.randomWeight.Length)
		{
			GameObject[] newList = new GameObject[size];
			float[] newWeights = new float[size];

			for(int i = 0; i < tile.randomList.Length && i < size && i < tile.randomWeight.Length; i++)
			{
				newList[i] = tile.randomList[i];
				newWeights[i] = tile.randomWeight[i];
			}

			tile.randomList = newList;
			tile.randomWeight = newWeights;
		}

		EditorGUI.indentLevel++;

		for(int i = 0; i < tile.randomList.Length; i++)
		{
			EditorGUILayout.LabelField("Tile " + i);

			EditorGUI.indentLevel++;

			tile.randomList[i] = (GameObject)EditorGUILayout.ObjectField("Prefab", tile.randomList[i], typeof(GameObject), false);
			tile.randomWeight[i] = Mathf.Max(0, EditorGUILayout.FloatField("Weight", tile.randomWeight[i]));

			EditorGUI.indentLevel--;
		}

		EditorGUI.indentLevel--;
	}

	void DrawGridButton(Rect rect, GUIStyle style, MapTileRuleArea area, MapTile tile)
	{
		Texture2D tex;
		MapTileRule rule = tile.CheckRule(area);

		if (rule == MapTileRule.Empty)
		{
			tex = redX;
		}
		else if (rule == MapTileRule.Full)
		{
			tex = checkBox;
		}
		else
		{
			tex = new Texture2D(0, 0);
		}

		if (GUI.Button(rect, tex, style))
		{
			int newRule = ((int)rule + 1) % 3;
			tile.SetRule(area, (MapTileRule)newRule);
		}
	}
}
