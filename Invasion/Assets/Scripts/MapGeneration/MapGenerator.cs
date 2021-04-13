using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MapGenerator : MonoBehaviour
{
	public Texture2D mapTex;
	public MapPalette mapPalette;
	public int wallHeight = 5;
	public int randomSeed = 1234;

	public Material mainMat;

	public List<GameObject> floorList;
	public List<GameObject> wallList;
	public List<GameObject> ceilingList;

    // Start is called before the first frame update
    void Start()
    {
		RefreshMap();
    }

	public void RefreshMap()
	{
		while(transform.childCount > 0)
		{
			DestroyImmediate(transform.GetChild(0).gameObject);
		}

		CreateMap();
	}

	MapSectionData CreateSection(GameObject obj)
	{
		MapSectionData newSection = new MapSectionData();

		newSection.meshFilter = obj.AddComponent<MeshFilter>();
		newSection.meshRenderer = obj.AddComponent<MeshRenderer>();
		newSection.meshCollider = obj.AddComponent<MeshCollider>();

		return newSection;
	}

	const int maxVertices = 65535;

	int floorVerts = 0;
	int wallVerts = 0;
	int ceilingVerts = 0;

	MapSectionData floorData;
	MapSectionData wallData;
	MapSectionData ceilingData;

	void CreateMap()
	{
		Random.InitState(randomSeed);
		floorVerts = 0;
		wallVerts = 0;
		ceilingVerts = 0;

		floorList = new List<GameObject>();
		wallList = new List<GameObject>();
		ceilingList = new List<GameObject>();

		GameObject newFloor = new GameObject("Map_Floor_0");
		newFloor.layer = LayerMask.NameToLayer("Floor");
		floorData = CreateSection(newFloor);
		floorList.Add(newFloor);

		GameObject newWall = new GameObject("Map_Wall_0");
		newWall.layer = LayerMask.NameToLayer("Wall");
		wallData = CreateSection(newWall);
		wallList.Add(newWall);

		GameObject newCeiling = new GameObject("Map_Ceiling_0");
		newCeiling.layer = LayerMask.NameToLayer("Wall");
		ceilingData = CreateSection(newCeiling);
		ceilingList.Add(newCeiling);

		newFloor.transform.SetParent(transform);
		newWall.transform.SetParent(transform);
		newCeiling.transform.SetParent(transform);


		MapTile[] tiles = mapPalette.floorTiles;
		Vector3 tileSize = mapPalette.floorTileSize;

		if(tiles.Length == 0)
		{
			return;
		}

		int width = mapTex.width;
		int height = mapTex.height;

		List<CombineInstance> combineFloors = new List<CombineInstance>();
		List<CombineInstance> combineCeiling = new List<CombineInstance>();
		List<CombineInstance> combineWalls = new List<CombineInstance>();

		for(int x = 0; x < width; x++)
		{
			for(int z = 0; z < height; z++)
			{
				Color pixel = mapTex.GetPixel(x, z);

				if(pixel == Color.black)
				{
					MapTile tile = GetFloorTile(x, z);

					GameObject tilePrefab = GetPrefab(tile);

					MeshFilter tileMeshFilter;

					Vector3 position = new Vector3(x * tileSize.x, -tileSize.y, z * tileSize.z);

					if (GetMeshFilter(tilePrefab, out tileMeshFilter))
					{
						CombineInstance newCombine = new CombineInstance();

						newCombine.mesh = tileMeshFilter.sharedMesh;
						newCombine.transform = Matrix4x4.Translate(position) * Matrix4x4.Rotate(Quaternion.Euler(0, tile.extraRotation, 0));

						floorVerts += newCombine.mesh.vertexCount;

						MapSectionData newData;

						if (!CheckVertCount("Map_Floor_", floorVerts, floorData, out newData, combineFloors, "Floor", floorList))
						{
							floorData = newData;
							floorVerts = newCombine.mesh.vertexCount;
						}

						combineFloors.Add(newCombine);
					}

					if(GetMeshFilter(mapPalette.ceilingTile, out tileMeshFilter))
					{
						CombineInstance newCombine = new CombineInstance();
						Vector3 ceilingPosition = position + new Vector3(0, mapPalette.wallTileSize.y * wallHeight, 0);

						newCombine.mesh = tileMeshFilter.sharedMesh;
						newCombine.transform = Matrix4x4.Translate(ceilingPosition);

						ceilingVerts += newCombine.mesh.vertexCount;

						MapSectionData newData;

						if (!CheckVertCount("Map_Ceiling_", ceilingVerts, ceilingData, out newData, combineCeiling, "Wall", ceilingList))
						{
							ceilingData = newData;
							ceilingVerts = newCombine.mesh.vertexCount;
						}

						combineCeiling.Add(newCombine);
					}

					MakeWalls(x, z, combineWalls);
				}
			}
		}

		FinalizeData(floorData, combineFloors);
		FinalizeData(wallData, combineWalls);
		FinalizeData(ceilingData, combineCeiling);
	}

	bool CheckVertCount(string baseName, int vertCount, MapSectionData data, out MapSectionData newData,
		List<CombineInstance> combine, string layer, List<GameObject> objectList)
	{

		if (vertCount > maxVertices)
		{
			FinalizeData(data, combine);

			GameObject newObj = new GameObject(baseName + objectList.Count);
			newObj.layer = LayerMask.NameToLayer(layer);
			newObj.transform.SetParent(transform);
			newData = CreateSection(newObj);
			objectList.Add(newObj);

			combine.Clear();

			return false;
		}

		newData = data;
		return true;
	}

	GameObject GetPrefab(MapTile tile)
	{
		if(tile.selectionType == MapTileSelectionType.Constant)
		{
			return tile.constantPrefab;
		}
		else if(tile.selectionType == MapTileSelectionType.RandomFromList)
		{
			return tile.GetRandomTile();
		}

		return null;
	}

	void FinalizeData(MapSectionData data, List<CombineInstance> combine)
	{
		data.meshFilter.sharedMesh = new Mesh();
		data.meshFilter.sharedMesh.CombineMeshes(combine.ToArray());
		data.meshFilter.sharedMesh.Optimize();

		data.meshRenderer.material = mainMat;

		data.meshCollider.sharedMesh = data.meshFilter.sharedMesh;
		data.meshCollider.isTrigger = false;
		data.meshCollider.convex = false;
	}

	void MakeWalls(int x, int y, List<CombineInstance> combineWalls)
	{
		Vector3 floorTileSize = mapPalette.floorTileSize;

		// Right
		if(x == mapTex.width - 1 || mapTex.GetPixel(x + 1, y) != Color.black)
		{
			Vector3 position = new Vector3(x * floorTileSize.x + floorTileSize.x / 2, 0, y * floorTileSize.z);
			Quaternion rotation = Quaternion.Euler(0, 90, 0);
			MakeWallColumn(position, rotation, combineWalls);
		}

		// Left
		if (x == 0 || mapTex.GetPixel(x - 1, y) != Color.black)
		{
			Vector3 position = new Vector3((x - 1) * floorTileSize.x + floorTileSize.x / 2, 0, y * floorTileSize.z);
			Quaternion rotation = Quaternion.Euler(0, -90, 0);
			MakeWallColumn(position, rotation, combineWalls);
		}

		// Top
		if (y == mapTex.height - 1 || mapTex.GetPixel(x, y + 1) != Color.black)
		{
			Vector3 position = new Vector3(x * floorTileSize.x, 0, y * floorTileSize.z + floorTileSize.z / 2);
			Quaternion rotation = Quaternion.identity;
			MakeWallColumn(position, rotation, combineWalls);
		}

		// Bottom
		if (y == 0 || mapTex.GetPixel(x, y - 1) != Color.black)
		{
			Vector3 position = new Vector3(x * floorTileSize.x, 0, (y - 1) * floorTileSize.z + floorTileSize.z / 2);
			Quaternion rotation = Quaternion.Euler(0, 180, 0);
			MakeWallColumn(position, rotation, combineWalls);
		}
	}

	void MakeWallColumn(Vector3 basePos, Quaternion rotation, List<CombineInstance> combineWalls)
	{
		CombineInstance newCombine = new CombineInstance();

		MeshFilter wallMF;

		if(!GetMeshFilter(mapPalette.wallTrim, out wallMF))
		{
			return;
		}

		newCombine.mesh = wallMF.sharedMesh;
		newCombine.transform = Matrix4x4.Translate(basePos) * Matrix4x4.Rotate(rotation);

		wallVerts += newCombine.mesh.vertexCount;
		MapSectionData newData;

		if (!CheckVertCount("Map_Wall_", wallVerts, wallData, out newData, combineWalls, "Wall", wallList))
		{
			wallData = newData;
			wallVerts = newCombine.mesh.vertexCount;
		}

		combineWalls.Add(newCombine);

		//GameObject newWall = Instantiate(mapPalette.wallTrim, basePos, rotation, wallsContainer.transform);

		for(int i = 1; i < wallHeight; i++)
		{
			if(!GetMeshFilter(mapPalette.wallTile, out wallMF))
			{
				continue;
			}

			Vector3 offset = Vector3.up * i * mapPalette.wallTileSize.y;

			newCombine = new CombineInstance();
			newCombine.mesh = wallMF.sharedMesh;
			newCombine.transform = Matrix4x4.Translate(basePos + offset) * Matrix4x4.Rotate(rotation);

			wallVerts += newCombine.mesh.vertexCount;

			if (!CheckVertCount("Map_Wall_", wallVerts, wallData, out newData, combineWalls, "Wall", wallList))
			{
				wallData = newData;
				wallVerts = newCombine.mesh.vertexCount;
			}

			combineWalls.Add(newCombine);

			//newWall = Instantiate(mapPalette.wallTile, basePos + offset, rotation, wallsContainer.transform);
			//newWall.layer = LayerMask.NameToLayer("Wall");
		}
	}

	bool GetMeshFilter(GameObject prefab, out MeshFilter meshFilter)
	{
		if(prefab == null)
		{
			meshFilter = null;
			return false;
		}

		meshFilter = prefab.GetComponent<MeshFilter>();

		if(meshFilter == null)
		{
			meshFilter = prefab.GetComponentInChildren<MeshFilter>();

			if(meshFilter == null)
			{
				return false;
			}
		}

		return true;
	}

	MapTile GetFloorTile(int x, int y)
	{
		MapTile[] tiles = mapPalette.floorTiles;

		int highest = tiles[0].GetPriority(mapTex, x, y);
		int highIndex = 0;

		for(int i = 1; i < tiles.Length; i++)
		{
			int priority = tiles[i].GetPriority(mapTex, x, y);

			if (priority > highest)
			{
				highIndex = i;
				highest = priority;
			}
		}

		return tiles[highIndex];
	}

	struct MapSectionData
	{
		public MeshFilter meshFilter;
		public MeshRenderer meshRenderer;
		public MeshCollider meshCollider;
	}
}
