using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Cube))]

public class CubeEditor : Editor {

	Object[] _cubes;
	
	void OnEnable ()
	{
		_cubes = serializedObject.targetObjects;
		EditorApplication.hierarchyWindowChanged = UpdateHierarchy;
	}

	void UpdateHierarchy ()
	{
		UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
		EditorUtility.UnloadUnusedAssetsImmediate();
	}
	
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();
		DrawDefaultInspector();

		if (_cubes == null) {
			_cubes = serializedObject.targetObjects;
		}

		if (GUILayout.Button("Set Texture")) {
			if (_cubes != null) {
				for (int i = 0; i < _cubes.Length; ++i) {
					if (((Cube)_cubes[i]).textureAtlas != null) {
						((Cube)_cubes[i]).UpdateTexture();
					}
				}
			}
		}

		if (GUI.changed) {
			UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
			EditorUtility.UnloadUnusedAssetsImmediate();
		}

        serializedObject.ApplyModifiedProperties();
	}
}
