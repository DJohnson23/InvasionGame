using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewMapPallette", menuName = "MapGeneration/MapPallette")]
public class MapPalette : ScriptableObject
{
	public MapTile[] floorTiles = new MapTile[0];
	public Vector3 floorTileSize = new Vector3(1, 0.1f, 1);

	public Vector3 wallTileSize = new Vector3(1, 1, 0.1f);
	public GameObject wallTrim;
	public GameObject wallTile;

	public GameObject ceilingTile;
}
