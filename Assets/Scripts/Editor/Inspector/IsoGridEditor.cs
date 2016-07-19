// IsoGridEditor.cs
//
// Author: Jate Wittayabundit <jate@ennface.com>
// Copyright (c) 2016 Ennface

using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(IsoGrid))]
public class IsoGridEditor : Editor {
	const int kMaxGrid = 10;
	const int kMinGrid = 5;

	SerializedProperty _atlas;
	SerializedProperty _row;
	SerializedProperty _col;
	SerializedProperty _pathGen;

	IsoGrid _grid;
	
	void OnEnable () {
		_grid = (IsoGrid)serializedObject.targetObject;
		_atlas = serializedObject.FindProperty("atlas");
		_pathGen = serializedObject.FindProperty("pathGen");
		_row = serializedObject.FindProperty("row");
		_col = serializedObject.FindProperty("col");

		if (_pathGen.objectReferenceValue == null) {
			serializedObject.Update();
			_pathGen.objectReferenceValue = _grid.GetComponent<PathGenerator>();
			serializedObject.ApplyModifiedProperties();
		}
	}
	public override void OnInspectorGUI() {
		serializedObject.Update();
		
		_atlas.objectReferenceValue = EditorGUILayout.ObjectField("Atlas", _atlas.objectReferenceValue,typeof(CubeAtlas),false);
		_pathGen.objectReferenceValue = EditorGUILayout.ObjectField("Path Generator", _pathGen.objectReferenceValue,typeof(PathGenerator),true);
		_row.intValue = EditorGUILayout.IntSlider("Row",_row.intValue,kMinGrid,kMaxGrid);
		_col.intValue = EditorGUILayout.IntSlider("Column",_col.intValue,kMinGrid,kMaxGrid);
	
		if (_atlas.objectReferenceValue != null) {
			GUILayout.BeginVertical("box");
			if (_grid.transform.childCount > 0) {
				if (GUI.changed) {
					if (GUILayout.Button("Recreate Grid")) {
						_grid.RemoveGrid();
						_grid.CreateNewGrid();
					}
				}
				if (GUILayout.Button("Update Points")) {
					_grid.UpdatePointsOnly();
				}
				if (GUILayout.Button("Create New Path")) {
					_grid.CreateNewPathOnly();
				}
				if (GUILayout.Button("Remove Grid")) {
					_grid.RemoveGrid();
				}
			} else {
				if (GUILayout.Button("Create Grid")) {
					_grid.CreateNewGrid();
				}
			}
			GUILayout.EndVertical();
		}
		
		if (GUI.changed) {
			UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
		}

		serializedObject.ApplyModifiedProperties();
	}
}
