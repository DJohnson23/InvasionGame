using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MapGenerationV2
{
	[CustomEditor(typeof(MapGenerator))]
	public class MapGeneratorEditor : Editor
	{
		MapGenerator mapGen;

		public override void OnInspectorGUI()
		{
			mapGen = (MapGenerator)target;

			if (DrawDefaultInspector() && mapGen.autoUpdate)
			{
				mapGen.RefreshMap();
			}

			GUIStyle style = GUI.skin.button;
			style.alignment = TextAnchor.MiddleCenter;

			if (GUILayout.Button("Generate Map"))
			{
				mapGen.RefreshMap();
			}
		}
	}
}