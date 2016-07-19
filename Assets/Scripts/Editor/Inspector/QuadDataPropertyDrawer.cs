// QuadDataEditor.cs
//
// Author: Jate Wittayabundit <jate@ennface.com>
// Copyright (c) 2015 Ennface
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(QuadData))]
public class QuadDataPropertyDrawer : PropertyDrawer
{
	const float HEIGHT = 16f;

	SerializedProperty _textureType;
	SerializedProperty _tile;
	SerializedProperty _rotation;

	bool _isTile = false;

	// Here you must define the height of your property drawer. Called by Unity.
	public override float GetPropertyHeight (SerializedProperty prop, GUIContent label) {
		if (!_isTile)
			return base.GetPropertyHeight (prop, label);
		else
			return base.GetPropertyHeight (prop, label) + HEIGHT;
	}

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        
		_textureType = property.FindPropertyRelative("textureType");
		_tile = property.FindPropertyRelative("face");
		_rotation = property.FindPropertyRelative("degree");

		if (_tile.enumValueIndex == (int)QuadData.Face.None) {
			_isTile = false;
		} else {
			_isTile = true;
		}

		Rect pos = EditorGUI.PrefixLabel(position, label);
		pos.width *= 0.5f;
		pos.height = HEIGHT;
		EditorGUI.indentLevel = 0;
		EditorGUIUtility.labelWidth = 60f;

		_textureType.enumValueIndex = (int)((QuadData.TextureType)EditorGUI.EnumPopup(pos,"Texture:",(QuadData.TextureType)_textureType.enumValueIndex));
		if (_textureType.enumValueIndex == (int)QuadData.TextureType.Floor) {
			pos.x += pos.width;
			EditorGUIUtility.labelWidth = 60f;
			_tile.enumValueIndex = (int)((QuadData.Face)EditorGUI.EnumPopup(pos,"Type:",(QuadData.Face)_tile.enumValueIndex));
			
			if (_tile.enumValueIndex == (int)QuadData.Face.None) {
				_isTile = false;
			} else {
				_isTile = true;
			}
			
			if (!_isTile) {
				_rotation.enumValueIndex = (int)QuadData.Degree.D0;
			} else {
				Rect newPos = pos;
				newPos.y += HEIGHT;
				EditorGUIUtility.labelWidth = 60f;
				_rotation.enumValueIndex = (int)((QuadData.Degree)EditorGUI.EnumPopup(newPos,"Degree:",(QuadData.Degree)_rotation.enumValueIndex));
			}
		}
		EditorGUI.EndProperty();
	}
}

