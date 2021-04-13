using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMapPallette", menuName = "MapGeneration/MapPallette")]
[System.Serializable]
public class MapPalette : ScriptableObject
{
	[SerializeField]
	public bool autoUpdate;
	[SerializeField]
	public MapTileSet tileSet;
	[SerializeField]
	public MapTile[] floorTiles = new MapTile[0];
	[SerializeField]
	public Vector2 floorTileSize = new Vector2(1, 1);

	[SerializeField]
	public Vector2 wallTileSize = new Vector2(1, 1);
	[SerializeField]
	public MapTile wallTrim;
	[SerializeField]
	public MapTile wallTile;

	[SerializeField]
	public MapTile ceilingTile;
}