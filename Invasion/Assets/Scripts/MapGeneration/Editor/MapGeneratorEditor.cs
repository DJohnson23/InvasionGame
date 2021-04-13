using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
	MapGenerator mapGen;

	public override void OnInspectorGUI()
	{
		mapGen = (MapGenerator)target;

		if(DrawDefaultInspector())
		{
			mapGen.RefreshMap();
		}

		GUIStyle style = GUI.skin.button;
		style.alignment = TextAnchor.MiddleCenter;

		if(GUILayout.Button("Generate Map"))
		{
			mapGen.RefreshMap();
		}

		if(GUILayout.Button("Save Meshes"))
		{
			SaveMeshes();
		}
	}

	void SaveMeshes()
	{
		List<GameObject> floorList = mapGen.floorList;
		List<GameObject> wallList = mapGen.wallList;
		List<GameObject> ceilingList = mapGen.ceilingList;

		Debug.Log(Application.dataPath);

		for(int i = 0; i < floorList.Count; i++)
		{
			string path = Application.dataPath + "/Exports/Floor_" + i + ".obj";
			GameObject floor = floorList[i];

			ObjExporter.MeshToFile(floor.GetComponent<MeshFilter>(), path);
		}
	}
}
