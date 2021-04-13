using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MapGenerationV2
{
	[CustomEditor(typeof(MapPalette))]
	public class MapPaletteEditor : Editor
	{
		MapPalette palette;
		Texture2D checkBox;
		Texture2D redX;

		bool showFloorTiles;
		bool[] floorTilesShown;

		bool showWallList;

		bool showWallTrim;
		bool showWallTile;

		bool showCeilingList;
		bool showCeilingTile;

		private void OnEnable()
		{
			checkBox = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Scripts/MapGeneration/Icons/Check.png", typeof(Texture2D));
			redX = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Scripts/MapGeneration/Icons/RedX.png", typeof(Texture2D));
			palette = (MapPalette)target;

			floorTilesShown = new bool[palette.floorTiles.Length];
			showFloorTiles = false;

			showWallList = false;
		}

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			palette.autoUpdate = EditorGUILayout.Toggle("Auto Update", palette.autoUpdate);
			palette.tileSet = (MapTileSet)EditorGUILayout.ObjectField("Tile Set", palette.tileSet, typeof(MapTileSet), false);

			if(palette.tileSet == null)
			{
				EditorGUI.EndChangeCheck();
				return;
			}

			palette.tileSet.Initalize();

			DrawFloorTileList();
			DrawWallTiles();
			DrawCeilingTiles();

			if (EditorGUI.EndChangeCheck() && palette.autoUpdate)
			{
				MapGenerator mapGen = FindObjectOfType<MapGenerator>();

				if (mapGen.mapPalette == palette)
				{
					mapGen.RefreshMap();
				}
			}
		}

		void DrawCeilingTiles()
		{
			showCeilingList = EditorGUILayout.Foldout(showCeilingList, "Ceiling Tiles");

			if (!showCeilingList)
			{
				return;
			}

			EditorGUI.indentLevel++;

			EditorGUILayout.LabelField("Ceiling Tile");
			DrawMapTile(palette.ceilingTile, false);

			EditorGUI.indentLevel--;
		}

		void DrawWallTiles()
		{
			showWallList = EditorGUILayout.Foldout(showWallList, "Wall Tiles");

			if (!showWallList)
			{
				return;
			}

			EditorGUI.indentLevel++;

			palette.wallTileSize = EditorGUILayout.Vector2Field("Tile Size", palette.wallTileSize);

			showWallTrim = EditorGUILayout.Foldout(showWallTrim, "Wall Bottom Trim");
			if(showWallTrim)
			{
				DrawMapTile(palette.wallTrim, false);
			}

			showWallTile = EditorGUILayout.Foldout(showWallTile, "Wall Tile");

			if(showWallTile)
			{
				DrawMapTile(palette.wallTile, false);
			}
			
			EditorGUI.indentLevel--;
		}

		void DrawFloorTileList()
		{
			showFloorTiles = EditorGUILayout.Foldout(showFloorTiles, "Floor Tiles");

			if (!showFloorTiles)
			{
				return;
			}

			EditorGUI.indentLevel++;


			palette.floorTileSize = EditorGUILayout.Vector2Field("Tile Size", palette.floorTileSize);

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

				if (floorTilesShown[i])
				{
					EditorGUI.indentLevel++;
					DrawMapTile(tile, true);
					EditorGUI.indentLevel--;
				}
			}

			EditorGUI.indentLevel -= 2;
		}

		void DrawMapTile(MapTile tile, bool hasRules)
		{
			int padding = 2;
			int cellSize = 25;
			int gapSize = 5;

			tile.selectionType = (MapTileSelectionType)EditorGUILayout.EnumPopup("Selection Type", tile.selectionType);

			DrawTileTextureSelection(tile);

			tile.orientation = EditorGUILayout.IntSlider("Orientation", tile.orientation, 0, 3);

			if(hasRules)
			{
				GUIStyle style = GUI.skin.button;

				style.padding = new RectOffset(padding, padding, padding, padding);
				style.alignment = TextAnchor.MiddleCenter;

				int gridSize = cellSize * 3 + gapSize * 2;
				int gridMargin = 10;

				Rect r = EditorGUILayout.BeginHorizontal(GUILayout.Height(gridSize + gridMargin * 2));
				r.y += gridMargin;
				r.x += r.width - gridSize - gridMargin;

				EditorGUILayout.Separator();

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
		}

		const int texPreviewSize = 50;
		const int texPreviewMargin = 10;

		void DrawTileTextureSelection(MapTile tile)
		{
			if (tile.selectionType == MapTileSelectionType.Constant)
			{
				tile.constTextureIndex = DrawTexturePicker(tile.constTextureIndex, tile.orientation);
			}
			else if (tile.selectionType == MapTileSelectionType.RandomFromList)
			{
				DrawRandomList(tile);
			}
		}

		int DrawTexturePicker(int curIndex, int orientation)
		{
			int newIndex = EditorGUILayout.IntSlider(curIndex, 0, palette.tileSet.tiles.Count - 1);
			Rect r = EditorGUILayout.BeginVertical();
			GUILayout.Space(texPreviewSize + texPreviewMargin * 2);
			r.x += r.width - texPreviewSize - texPreviewMargin;
			r.y += texPreviewMargin;
			r.width = texPreviewSize;
			r.height = texPreviewSize;

			GUI.DrawTexture(r, palette.tileSet.tiles[newIndex].CropTex(palette.tileSet.texture, texPreviewSize, texPreviewSize, orientation));
			EditorGUILayout.EndVertical();
			return newIndex;
		}

		
		void DrawRandomList(MapTile tile)
		{
			int size = Mathf.Max(EditorGUILayout.IntField("Size", tile.randomList.Length), 1);

			if (size != tile.randomList.Length || size != tile.randomWeight.Length)
			{
				int[] newList = new int[size];
				float[] newWeights = new float[size];

				for (int i = 0; i < tile.randomList.Length && i < size && i < tile.randomWeight.Length; i++)
				{
					newList[i] = tile.randomList[i];
					newWeights[i] = tile.randomWeight[i];
				}

				tile.randomList = newList;
				tile.randomWeight = newWeights;
			}

			EditorGUI.indentLevel++;

			for (int i = 0; i < tile.randomList.Length; i++)
			{
				EditorGUILayout.LabelField("Tile " + i);

				EditorGUI.indentLevel++;

				tile.randomList[i] = DrawTexturePicker(tile.randomList[i], tile.orientation);
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
}