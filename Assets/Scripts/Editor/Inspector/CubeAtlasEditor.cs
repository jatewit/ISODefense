using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(CubeAtlas))]
public class CubeAtlasEditor : Editor {
	CubeAtlas _cubeAtlas;
	
	void OnEnable ()
	{
		_cubeAtlas = ((CubeAtlas)serializedObject.targetObject);
	}
	
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();
		base.DrawDefaultInspector();

		if (_cubeAtlas == null) {
			_cubeAtlas = ((CubeAtlas)serializedObject.targetObject);
		}

		if ((_cubeAtlas.filePath == null) || (_cubeAtlas.AtlasTexture == null)) {
			if (GUILayout.Button("Create UV Data")) {
				_cubeAtlas.SetUVData();
				SceneView.RepaintAll();
			}
		}
		
		serializedObject.ApplyModifiedProperties();
	}
}
