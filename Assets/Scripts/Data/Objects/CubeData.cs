using UnityEngine;

[System.Serializable]
public struct CubeData {
	public const float kCubeSize = 1.0f;
    public enum Type {Regular,Start,End}
	public Type type;
	public QuadData front, back, left, right, top;
    Mesh _mesh;
	Cube _cube;

	public void Init ( Cube cube, Mesh mesh ) {
		if (_cube == null) {
			_cube = cube;
			_mesh = mesh;
		}

		front.textureType = QuadData.TextureType.Side;
		back.textureType = QuadData.TextureType.Side;
		left.textureType = QuadData.TextureType.Side;
		right.textureType = QuadData.TextureType.Side;
		top.textureType = QuadData.TextureType.Floor;
	}
    
    public void UpdateTexture () {
		if ((_cube.textureAtlas != null) && (_cube.textureAtlas.AtlasTexture != null)) {
			_mesh = CreateCube(_mesh,kCubeSize);
		}
#if UNITY_EDITOR // For creating second uv channel -> lightmap 
			UnityEditor.UnwrapParam param = new UnityEditor.UnwrapParam();
			UnityEditor.UnwrapParam.SetDefaults( out param );
			UnityEditor.Unwrapping.GenerateSecondaryUVSet( _mesh, param );
#endif
	}

	Mesh CreateCube(Mesh editMesh, float dimension)
	{
		if (editMesh.vertices.Length > 0) {
			editMesh.Clear();
		}
		float fHS = dimension * 0.5f;
		editMesh.vertices = GetVertices(_cube.quadPosition,fHS);
		int[] triangles = new int[editMesh.vertices.Length / 4 * 2 * 3];
		int iPos = 0;
		for (int i = 0; i < editMesh.vertices.Length; i = i + 4) {
			triangles[iPos++] = i;
			triangles[iPos++] = i+1;
			triangles[iPos++] = i+2;
			triangles[iPos++] = i;
			triangles[iPos++] = i+2;
			triangles[iPos++] = i+3;
		}
		
		editMesh.triangles = triangles;
		editMesh.RecalculateNormals();
		editMesh.uv = GetUV(_cube.quadPosition,editMesh.vertices.Length);
		return editMesh;
	}

	Vector3[] GetVertices (Cube.Position position, float fHS) {
		switch (position)
		{
			case Cube.Position.FL:  // Front - Left 
				return 	new Vector3[] { 
						new Vector3(-fHS, -fHS,  fHS), new Vector3( fHS, -fHS,  fHS), new Vector3( fHS,  fHS,  fHS), new Vector3(-fHS,  fHS,  fHS),  // Front
						new Vector3(-fHS,  fHS,  fHS), new Vector3(-fHS,  fHS, -fHS), new Vector3(-fHS, -fHS, -fHS), new Vector3(-fHS, -fHS,  fHS),  // Left
						new Vector3(-fHS,  fHS,  fHS), new Vector3( fHS,  fHS,  fHS), new Vector3( fHS,  fHS, -fHS), new Vector3(-fHS,  fHS, -fHS)  // Top
					};
			case Cube.Position.L:  	// Left 
				return 	new Vector3[] { 
						new Vector3(-fHS,  fHS,  fHS), new Vector3(-fHS,  fHS, -fHS), new Vector3(-fHS, -fHS, -fHS), new Vector3(-fHS, -fHS,  fHS),  // Left
						new Vector3(-fHS,  fHS,  fHS), new Vector3( fHS,  fHS,  fHS), new Vector3( fHS,  fHS, -fHS), new Vector3(-fHS,  fHS, -fHS)  // Top
					};
			case Cube.Position.BL:  // Back - Left
				return 	new Vector3[] { 
						new Vector3(-fHS,  fHS, -fHS), new Vector3( fHS,  fHS, -fHS), new Vector3( fHS, -fHS, -fHS), new Vector3(-fHS, -fHS, -fHS),  // Back
						new Vector3(-fHS,  fHS,  fHS), new Vector3(-fHS,  fHS, -fHS), new Vector3(-fHS, -fHS, -fHS), new Vector3(-fHS, -fHS,  fHS),  // Left
						new Vector3(-fHS,  fHS,  fHS), new Vector3( fHS,  fHS,  fHS), new Vector3( fHS,  fHS, -fHS), new Vector3(-fHS,  fHS, -fHS)  // Top
					};
			case Cube.Position.B:  	// Back
				return 	new Vector3[] { 
						new Vector3(-fHS,  fHS, -fHS), new Vector3( fHS,  fHS, -fHS), new Vector3( fHS, -fHS, -fHS), new Vector3(-fHS, -fHS, -fHS),  // Back
						new Vector3(-fHS,  fHS,  fHS), new Vector3( fHS,  fHS,  fHS), new Vector3( fHS,  fHS, -fHS), new Vector3(-fHS,  fHS, -fHS)  // Top
					};
			case Cube.Position.BR:	// Back - Right
				return 	new Vector3[] { 
						new Vector3(-fHS,  fHS, -fHS), new Vector3( fHS,  fHS, -fHS), new Vector3( fHS, -fHS, -fHS), new Vector3(-fHS, -fHS, -fHS),  // Back
						new Vector3( fHS,  fHS, -fHS), new Vector3( fHS,  fHS,  fHS), new Vector3( fHS, -fHS,  fHS), new Vector3( fHS, -fHS, -fHS),  // Right
						new Vector3(-fHS,  fHS,  fHS), new Vector3( fHS,  fHS,  fHS), new Vector3( fHS,  fHS, -fHS), new Vector3(-fHS,  fHS, -fHS)  // Top
					};
			case Cube.Position.R:	// Right
				return 	new Vector3[] { 
						new Vector3( fHS,  fHS, -fHS), new Vector3( fHS,  fHS,  fHS), new Vector3( fHS, -fHS,  fHS), new Vector3( fHS, -fHS, -fHS),  // Right
						new Vector3(-fHS,  fHS,  fHS), new Vector3( fHS,  fHS,  fHS), new Vector3( fHS,  fHS, -fHS), new Vector3(-fHS,  fHS, -fHS)  // Top
					};
			case Cube.Position.FR:	// Front - Right	
				return 	new Vector3[] { 
						new Vector3(-fHS, -fHS,  fHS), new Vector3( fHS, -fHS,  fHS), new Vector3( fHS,  fHS,  fHS), new Vector3(-fHS,  fHS,  fHS),  // Front
						new Vector3( fHS,  fHS, -fHS), new Vector3( fHS,  fHS,  fHS), new Vector3( fHS, -fHS,  fHS), new Vector3( fHS, -fHS, -fHS),  // Right
						new Vector3(-fHS,  fHS,  fHS), new Vector3( fHS,  fHS,  fHS), new Vector3( fHS,  fHS, -fHS), new Vector3(-fHS,  fHS, -fHS)  // Top
					};
			case Cube.Position.F:	// Front
				return 	new Vector3[] { 
						new Vector3(-fHS, -fHS,  fHS), new Vector3( fHS, -fHS,  fHS), new Vector3( fHS,  fHS,  fHS), new Vector3(-fHS,  fHS,  fHS),  // Front
						new Vector3(-fHS,  fHS,  fHS), new Vector3( fHS,  fHS,  fHS), new Vector3( fHS,  fHS, -fHS), new Vector3(-fHS,  fHS, -fHS)  // Top
					};
			case Cube.Position.T:	// Top
				return 	new Vector3[] { 
						new Vector3(-fHS,  fHS,  fHS), new Vector3( fHS,  fHS,  fHS), new Vector3( fHS,  fHS, -fHS), new Vector3(-fHS,  fHS, -fHS)  // Top
					};
			default: // Full Cube - no bottom
				return 	new Vector3[] { 
						new Vector3(-fHS, -fHS,  fHS), new Vector3( fHS, -fHS,  fHS), new Vector3( fHS,  fHS,  fHS), new Vector3(-fHS,  fHS,  fHS),  // Front
						new Vector3(-fHS,  fHS, -fHS), new Vector3( fHS,  fHS, -fHS), new Vector3( fHS, -fHS, -fHS), new Vector3(-fHS, -fHS, -fHS),  // Back
						new Vector3(-fHS,  fHS,  fHS), new Vector3(-fHS,  fHS, -fHS), new Vector3(-fHS, -fHS, -fHS), new Vector3(-fHS, -fHS,  fHS),  // Left
						new Vector3( fHS,  fHS, -fHS), new Vector3( fHS,  fHS,  fHS), new Vector3( fHS, -fHS,  fHS), new Vector3( fHS, -fHS, -fHS),  // Right
						new Vector3(-fHS,  fHS,  fHS), new Vector3( fHS,  fHS,  fHS), new Vector3( fHS,  fHS, -fHS), new Vector3(-fHS,  fHS, -fHS)  // Top
					};
		}
	}

	Vector2[] GetUV (Cube.Position position, int length) {
		Vector2[] uv = new Vector2[length];
		SetUVsForCube(ref uv, position);
		return uv;
	}

	void SetUVsForCube (ref Vector2[] uvs, Cube.Position position) {
		
		switch (position)
		{
			case Cube.Position.FL:  // Front - Left 
				SetUVs(ref front,ref uvs[2],ref uvs[3],ref uvs[1],ref uvs[0]);
				SetUVs(ref left,ref uvs[4],ref uvs[5],ref uvs[7],ref uvs[6]);
				SetUVs(ref top,ref uvs[8],ref uvs[9],ref uvs[11],ref uvs[10]);
				break;
			case Cube.Position.L:  	// Left 
				SetUVs(ref left,ref uvs[2],ref uvs[3],ref uvs[1],ref uvs[0]);
				SetUVs(ref top,ref uvs[4],ref uvs[5],ref uvs[7],ref uvs[6]);
				break;
			case Cube.Position.BL:  // Back - Left
				SetUVs(ref back,ref uvs[2],ref uvs[3],ref uvs[1],ref uvs[0]);
				SetUVs(ref left,ref uvs[4],ref uvs[5],ref uvs[7],ref uvs[6]);
				SetUVs(ref top,ref uvs[8],ref uvs[9],ref uvs[11],ref uvs[10]);
				break;
			case Cube.Position.B:  	// Back
				SetUVs(ref back,ref uvs[2],ref uvs[3],ref uvs[1],ref uvs[0]);
				SetUVs(ref top,ref uvs[4],ref uvs[5],ref uvs[7],ref uvs[6]);
				break;
			case Cube.Position.BR:	// Back - Right
				SetUVs(ref back,ref uvs[2],ref uvs[3],ref uvs[1],ref uvs[0]);
				SetUVs(ref right,ref uvs[4],ref uvs[5],ref uvs[7],ref uvs[6]);
				SetUVs(ref top,ref uvs[8],ref uvs[9],ref uvs[11],ref uvs[10]);
				break;
			case Cube.Position.R:	// Right
				SetUVs(ref right,ref uvs[2],ref uvs[3],ref uvs[1],ref uvs[0]);
				SetUVs(ref top,ref uvs[4],ref uvs[5],ref uvs[7],ref uvs[6]);
				break;
			case Cube.Position.FR:	// Front - Right	
				SetUVs(ref front,ref uvs[2],ref uvs[3],ref uvs[1],ref uvs[0]);
				SetUVs(ref right,ref uvs[4],ref uvs[5],ref uvs[7],ref uvs[6]);
				SetUVs(ref top,ref uvs[8],ref uvs[9],ref uvs[11],ref uvs[10]);
				break;
			case Cube.Position.F:	// Front
				SetUVs(ref front,ref uvs[2],ref uvs[3],ref uvs[1],ref uvs[0]);
				SetUVs(ref top,ref uvs[4],ref uvs[5],ref uvs[7],ref uvs[6]);
				break;
			case Cube.Position.T:	// Top
				SetUVs(ref top,ref uvs[0],ref uvs[1],ref uvs[3],ref uvs[2]);
				break;
			default: // Full Cube - no bottom
				SetUVs(ref front,ref uvs[2],ref uvs[3],ref uvs[1],ref uvs[0]);
				SetUVs(ref back,ref uvs[4],ref uvs[5],ref uvs[7],ref uvs[6]);
				SetUVs(ref left,ref uvs[8],ref uvs[9],ref uvs[11],ref uvs[10]);
				SetUVs(ref right,ref uvs[12],ref uvs[13],ref uvs[15],ref uvs[14]);
				SetUVs(ref top,ref uvs[16],ref uvs[17],ref uvs[19],ref uvs[18]);
				break;
		}
	}

	void SetUVs (ref QuadData quad, ref Vector2 p1, ref Vector2 p2, 
	             					ref Vector2 p3, ref Vector2 p4) {

		UVPosition uvPos = _cube.textureAtlas.GetUVByTextureName(quad);
		p1 = uvPos.uv1;
		p2 = uvPos.uv2;
		p3 = uvPos.uv3;
		p4 = uvPos.uv4;
	}
}