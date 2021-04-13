using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class MapGenerator : MonoBehaviour
{
	public bool autoUpdate;
	public Texture2D mapTex;
	public MapPalette mapPalette;
	public int wallHeight = 5;
	public int randomSeed = 1234;

	public Material mainMat;

	// Start is called before the first frame update
	void Start()
	{
		RefreshMap();
	}

	public void RefreshMap()
	{
		while (transform.childCount > 0)
		{
			DestroyImmediate(transform.GetChild(0).gameObject);
		}

		CreateMap();
	}

	void CreateMap()
	{
		Random.InitState(randomSeed);

		CreateFloorAndCeiling();
		CreateWalls();

	}

	void CreateWalls()
	{
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		List<Vector2> uvs = new List<Vector2>();

		Vector2 wallTileSize = mapPalette.wallTileSize;
		Vector2 floorTileSize = mapPalette.floorTileSize;

		for (int x = 0; x < mapTex.width; x++)
		{
			for (int y = 0; y < mapTex.height; y++)
			{
				float xPos = x * floorTileSize.x;
				float zPos = y * floorTileSize.y;

				if (mapTex.GetPixel(x, y) != Color.black)
				{
					continue;
				}

				if (x == 0 || mapTex.GetPixel(x - 1, y) != Color.black)
				{
					Vector3[] trimPoints = new Vector3[4];


					trimPoints[0] = new Vector3(xPos, 0, zPos);
					trimPoints[1] = new Vector3(xPos, wallTileSize.y, zPos);
					trimPoints[2] = new Vector3(xPos, wallTileSize.y, zPos + floorTileSize.y);
					trimPoints[3] = new Vector3(xPos, 0, zPos + floorTileSize.y);

					CreateWallColumn(vertices, triangles, uvs, trimPoints);
				}

				if (x == mapTex.width - 1 || mapTex.GetPixel(x + 1, y) != Color.black)
				{
					Vector3[] trimPoints = new Vector3[4];


					trimPoints[0] = new Vector3(xPos + floorTileSize.x, 0, zPos + floorTileSize.y);
					trimPoints[1] = new Vector3(xPos + floorTileSize.x, wallTileSize.y, zPos + floorTileSize.y);
					trimPoints[2] = new Vector3(xPos + floorTileSize.x, wallTileSize.y, zPos);
					trimPoints[3] = new Vector3(xPos + floorTileSize.x, 0, zPos);

					CreateWallColumn(vertices, triangles, uvs, trimPoints);
				}

				if (y == mapTex.height - 1 || mapTex.GetPixel(x, y + 1) != Color.black)
				{
					Vector3[] trimPoints = new Vector3[4];


					trimPoints[0] = new Vector3(xPos, 0, zPos + floorTileSize.y);
					trimPoints[1] = new Vector3(xPos, wallTileSize.y, zPos + floorTileSize.y);
					trimPoints[2] = new Vector3(xPos + floorTileSize.x, wallTileSize.y, zPos + floorTileSize.y);
					trimPoints[3] = new Vector3(xPos + floorTileSize.x, 0, zPos + floorTileSize.y);

					CreateWallColumn(vertices, triangles, uvs, trimPoints);
				}

				if (y == 0 || mapTex.GetPixel(x, y - 1) != Color.black)
				{
					Vector3[] trimPoints = new Vector3[4];


					trimPoints[0] = new Vector3(xPos + floorTileSize.x, 0, zPos);
					trimPoints[1] = new Vector3(xPos + floorTileSize.x, wallTileSize.y, zPos);
					trimPoints[2] = new Vector3(xPos, wallTileSize.y, zPos);
					trimPoints[3] = new Vector3(xPos, 0, zPos);

					CreateWallColumn(vertices, triangles, uvs, trimPoints);
				}
			}
		}

		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();

		GameObject wall = CreateMeshObject("Map_Wall", mesh);
		wall.layer = LayerMask.NameToLayer("Wall");
	}

	void AddQuad(List<Vector3> vertices, List<int> triangles, Vector3[] points)
	{
		vertices.Add(points[0]);
		vertices.Add(points[1]);
		vertices.Add(points[2]);
		vertices.Add(points[3]);

		triangles.Add(vertices.Count - 4);
		triangles.Add(vertices.Count - 2);
		triangles.Add(vertices.Count - 1);

		triangles.Add(vertices.Count - 4);
		triangles.Add(vertices.Count - 3);
		triangles.Add(vertices.Count - 2);
	}

	void CreateWallColumn(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, Vector3[] trimPoints)
	{
		AddQuad(vertices, triangles, trimPoints);

		MapTile trimTile = mapPalette.wallTrim;

		if (trimTile != null)
		{
			uvs.AddRange(GetUVSquare(trimTile));
		}

		MapTile wallTile = mapPalette.wallTile;

		if (wallTile == null)
		{
			return;
		}

		for (int i = 1; i < wallHeight; i++)
		{
			Vector3[] newPoints = new Vector3[4];
			Vector3 offset = new Vector3(0, mapPalette.wallTileSize.y * i, 0);

			newPoints[0] = trimPoints[0] + offset;
			newPoints[1] = trimPoints[1] + offset;
			newPoints[2] = trimPoints[2] + offset;
			newPoints[3] = trimPoints[3] + offset;

			AddQuad(vertices, triangles, newPoints);

			uvs.AddRange(GetUVSquare(wallTile));
		}
	}

	void CreateFloorAndCeiling()
	{
		List<Vector3> floorVerts = new List<Vector3>();
		List<int> floorTriangles = new List<int>();
		List<Vector2> floorUvs = new List<Vector2>();

		List<Vector3> ceilingVerts = new List<Vector3>();
		List<int> ceilingTriangles = new List<int>();
		List<Vector2> ceilingUvs = new List<Vector2>();

		for (int x = 0; x < mapTex.width; x++)
		{
			for (int y = 0; y < mapTex.height; y++)
			{
				if (mapTex.GetPixel(x, y) != Color.black)
				{
					continue;
				}

				Vector3 tileSize = new Vector3(mapPalette.floorTileSize.x, 0, mapPalette.floorTileSize.y);

				Vector3 minPosition = new Vector3(x * tileSize.x, 0, y * tileSize.z);
				Vector3 maxposition = minPosition + tileSize;

				Vector3[] floorPoints = new Vector3[4];
				floorPoints[0] = minPosition;
				floorPoints[1] = new Vector3(minPosition.x, 0, maxposition.z);
				floorPoints[2] = maxposition;
				floorPoints[3] = new Vector3(maxposition.x, 0, minPosition.z);

				AddQuad(floorVerts, floorTriangles, floorPoints);

				MapTile floorTile = GetFloorTile(x, y);

				if (floorTile != null)
				{
					floorUvs.AddRange(GetUVSquare(floorTile));
				}

				Vector3 ceilingOffset = new Vector3(0, mapPalette.wallTileSize.y * wallHeight, 0);

				Vector3[] ceilingPoints = new Vector3[4];

				for (int i = 0; i < 4; i++)
				{
					ceilingPoints[3 - i] = floorPoints[i] + ceilingOffset;
				}

				AddQuad(ceilingVerts, ceilingTriangles, ceilingPoints);

				MapTile ceilingTile = mapPalette.ceilingTile;

				if (ceilingTile != null)
				{
					ceilingUvs.AddRange(GetUVSquare(ceilingTile));
				}
			}
		}

		Mesh floorMesh = new Mesh();
		floorMesh.vertices = floorVerts.ToArray();
		floorMesh.triangles = floorTriangles.ToArray();
		floorMesh.uv = floorUvs.ToArray();

		GameObject floor = CreateMeshObject("Map_Floor", floorMesh);
		floor.layer = LayerMask.NameToLayer("Floor");

		Mesh ceilingMesh = new Mesh();
		ceilingMesh.vertices = ceilingVerts.ToArray();
		ceilingMesh.triangles = ceilingTriangles.ToArray();
		ceilingMesh.uv = ceilingUvs.ToArray();

		GameObject ceiling = CreateMeshObject("Map_Ceiling", ceilingMesh);
		ceiling.layer = LayerMask.NameToLayer("Wall");
	}

	GameObject CreateMeshObject(string name, Mesh mesh)
	{
		mesh.RecalculateNormals();
		GameObject newObj = new GameObject(name);
		MeshRenderer renderer = newObj.AddComponent<MeshRenderer>();
		MeshFilter filter = newObj.AddComponent<MeshFilter>();
		MeshCollider collider = newObj.AddComponent<MeshCollider>();

		filter.sharedMesh = mesh;
		collider.sharedMesh = mesh;
		collider.isTrigger = false;
		collider.convex = false;
		renderer.material = mainMat;

		newObj.transform.SetParent(transform);

		return newObj;
	}

	Vector2[] GetUVSquare(MapTile tile)
	{
		MapTileTexture tex;
		if (tile.selectionType == MapTileSelectionType.Constant)
		{
			tex = mapPalette.tileSet.tiles[tile.constTextureIndex];
		}
		else if (tile.selectionType == MapTileSelectionType.RandomFromList)
		{
			tex = mapPalette.tileSet.tiles[tile.GetRandomTileIndex()];
		}
		else
		{
			return new Vector2[4];
		}

		return tex.GetSquareCoords(tile.orientation);
	}

	MapTile GetFloorTile(int x, int y)
	{
		MapTile[] tiles = mapPalette.floorTiles;

		if (tiles.Length == 0)
		{
			return null;
		}

		int highest = tiles[0].GetPriority(mapTex, x, y);
		int highIndex = 0;

		for (int i = 1; i < tiles.Length; i++)
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
}