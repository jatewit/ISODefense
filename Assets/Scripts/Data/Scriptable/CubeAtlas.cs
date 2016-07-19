using UnityEngine;
using Jatewit;
using System.Collections.Generic;

public class CubeAtlas : ScriptableObject {
	public Texture2D AtlasTexture;
	public string filePath;
	public Material defaultMat, defaultClickMat;

	Dictionary<string,UVAtlas> _data;
	UVAtlas[] _uvsAtlas;

	void OnEnable () {
		SetUVData();
	}

	public void SetUVData () {
		if (filePath == null)
			return;

		AtlasData file = Utilities.LoadJsonData<AtlasData>(filePath);
		_data = new Dictionary<string,UVAtlas>();
		for (int i = 0; i < file.uvData.Length; ++i) {
			_data.Add(file.uvData[i].name,file.uvData[i]);
		}
	}

	public UVPosition GetUVByTextureName ( QuadData quadData ) {
		if (_data == null) {
			Debug.Log("_data = " + _data);
			SetUVData();
		}
		string nm = quadData.textureName;
		return GetUVPosition (quadData.degree, nm);
	}

	UVPosition GetUVPosition (QuadData.Degree rotationType, string nm)
	{
		if (_data.ContainsKey (nm)) {
			float xMin = _data[nm].xMin;
			float xMax = _data[nm].xMax;
			float yMin = _data[nm].yMin;
			float yMax = _data[nm].yMax;
			Vector2 uv1 = Vector2.zero;
			Vector2 uv2 = Vector2.zero;
			Vector2 uv3 = Vector2.zero;
			Vector2 uv4 = Vector2.zero;
			switch (rotationType) {
			case QuadData.Degree.D0:
				uv1.x = xMin;
				uv1.y = yMax;
				uv2.x = xMax;
				uv2.y = yMax;
				uv3.x = xMin;
				uv3.y = yMin;
				uv4.x = xMax;
				uv4.y = yMin;
				break;
			case QuadData.Degree.D90:
				uv1.x = xMin;
				uv1.y = yMin;
				uv2.x = xMin;
				uv2.y = yMax;
				uv3.x = xMax;
				uv3.y = yMin;
				uv4.x = xMax;
				uv4.y = yMax;
				break;
			case QuadData.Degree.D180:
				uv1.x = xMax;
				uv1.y = yMin;
				uv2.x = xMin;
				uv2.y = yMin;
				uv3.x = xMax;
				uv3.y = yMax;
				uv4.x = xMin;
				uv4.y = yMax;
				break;
			case QuadData.Degree.D270:
				uv1.x = xMax;
				uv1.y = yMax;
				uv2.x = xMax;
				uv2.y = yMin;
				uv3.x = xMin;
				uv3.y = yMax;
				uv4.x = xMin;
				uv4.y = yMin;
				break;
			}
			return new UVPosition (uv1, uv2, uv3, uv4);
		}
		else {
			Debug.LogWarning ("Can't find texture atlas name: " + nm);
			return new UVPosition (Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
		}
	}
}

