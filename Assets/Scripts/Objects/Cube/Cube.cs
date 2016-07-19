// Cube.cs
//
// Author: Jate Wittayabundit <jate@ennface.com>
// Copyright (c) 2015 Ennface

using UnityEngine;
using Jatewit;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Cube : UniqueMesh {
	public enum Position {Default,FL,L,BL,B,BR,R,FR,F,T}
	// Note: for the position Front = +z, Back = -z, Left = -x, Right = +x, Top = +y, Bottom = -y 

	public delegate void ClickedBlockDelegate (Cube cube);
	public static event ClickedBlockDelegate OnClickBlock;
	public Position quadPosition;
	public CubeAtlas textureAtlas;
	public Vector2Int index;
	public CubeData cubeData;
	public TowerBase tower;

	MeshRenderer _meshRenderer;
	Vector3 _enemyPoint;

	#region Get & Set
	public Vector3 enemyPath {
		get {
			if (_enemyPoint == Vector3.zero) {
				_enemyPoint = transform.position;
				_enemyPoint.y = CubeData.kCubeSize*0.75f; // Offset in y - position
			}
			return _enemyPoint;
		}
	}
	public MeshRenderer meshRenderer {
		get { 
			if (_meshRenderer == null) _meshRenderer = GetComponent<MeshRenderer>();
			return _meshRenderer;
		}
	}
	public string Id {
		get {
			return "cube_"+index.x+"_"+index.y;
		} 
	}
	public bool IsStart {
		get { return (cubeData.type == CubeData.Type.Start); }
	}
	
	public bool IsEnd {
		get { return (cubeData.type == CubeData.Type.End); }
	}
	#endregion

	public void Mouseover () {
		if (GameManager.Instance.isPause) return;
		if (tower != null) tower.ShowRadius();
		if (cubeData.top.face == QuadData.Face.None)
			meshRenderer.sharedMaterial = textureAtlas.defaultClickMat;
	}

	public void MouseExit () {
		if (GameManager.Instance.isPause) return;
		if (tower != null) tower.HideRadius();
		meshRenderer.sharedMaterial = textureAtlas.defaultMat;
	}

	public void ClickAsButton () {
		if (GameManager.Instance.isPause) return;
		if (cubeData.top.face == QuadData.Face.None) {
			if (OnClickBlock != null) OnClickBlock(this);
		}
	}

	void OnMouseOver() {
		Mouseover();
	}

	void OnMouseExit() {
		MouseExit();
	}
	
	void OnMouseUpAsButton() {
		ClickAsButton();
	}

	#region Editor Function

	public void UpdateTexture () {
		cubeData.Init(this,mesh);
        cubeData.UpdateTexture();
		meshRenderer.sharedMaterial = textureAtlas.defaultMat;
	}

	public void SetEntrance (QuadData.Path entrance) {
		cubeData.top.entrance = entrance;
	}

	public QuadData.Path GetNextCubeEntrance () {
		QuadData.Path path = cubeData.top.exit;
		switch (path) {
			case QuadData.Path.Top:		return QuadData.Path.Bottom;
			case QuadData.Path.Bottom:	return QuadData.Path.Top;
			case QuadData.Path.Left:	return QuadData.Path.Right;
			case QuadData.Path.Right:	return QuadData.Path.Left;
		}
		return QuadData.Path.None;
	}
	
	public Vector2Int GetNextCubeIndex () {
		QuadData.Path path = cubeData.top.exit;
		Vector2Int nextIndex = index;
		switch (path) {
			case QuadData.Path.Top:
				nextIndex.y++;
				break;
			case QuadData.Path.Bottom:
				nextIndex.y--;
				break;
			case QuadData.Path.Left:
				nextIndex.x--;
				break;
			case QuadData.Path.Right:
				nextIndex.x++;
				break;
		}
		return nextIndex;
	}
	#endregion
}