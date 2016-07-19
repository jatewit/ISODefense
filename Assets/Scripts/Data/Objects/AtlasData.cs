using UnityEngine;

[System.Serializable]
public class AtlasData {
	public UVAtlas[] uvData;
}

[System.Serializable]
public struct UVAtlas {
	public string name;
	public float xMin;
	public float xMax;
	public float yMin;
	public float yMax;

	public UVAtlas (string name = "", float xMin = 0, float xMax = 0, float yMin = 0, float yMax = 0) {
		this.name = name;
		this.xMin = xMin;
		this.xMax = xMax;
		this.yMin = yMin;
		this.yMax = yMax;
	}
}

[System.Serializable]
public struct UVPosition {
	public Vector2 uv1;
	public Vector2 uv2;
	public Vector2 uv3;
	public Vector2 uv4;
	
	public UVPosition (Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4) {
		this.uv1 = uv1;
		this.uv2 = uv2;
		this.uv3 = uv3;
		this.uv4 = uv4;
	}
}