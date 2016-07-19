// IsoGrid.cs
//
// Author: Jate Wittayabundit <jate@ennface.com>

using UnityEngine;

[RequireComponent(typeof(PathGenerator))]
public class IsoGrid : MonoBehaviour {
	public CubeAtlas atlas;
	public PathGenerator pathGen;
	public int row;
	public int col;

	Cube[][] _cubes;

	public Cube[][] cubes {
		get {
			if (_cubes == null) {
				_cubes = new Cube[row][];
				for (int i = 0;i < row; ++i) {
					_cubes[i] = new Cube[col];
				}
				Cube[] cubeChildren = GetComponentsInChildren<Cube>();
				if ((cubeChildren != null) && (cubeChildren.Length > 0)) {
					for (int i = 0;i < cubeChildren.Length; ++i) {
						for (int x = 0;x < _cubes.Length; ++x) {
							for (int y = 0;y < _cubes[x].Length; ++y) {
								Vector2Int index = new Vector2Int(x,y);
								if (cubeChildren[i].index == index) {
									_cubes[x][y] = cubeChildren[i];
								}
							}
						}
					}
				}
			}
			return _cubes;
		}
	}
	
	void Awake () {
		if (pathGen == null) {
			pathGen = GetComponent<PathGenerator>();
		}
		if (_cubes == null) {
			_cubes = cubes;
		}
	}

	public void ResetCubesArray () {
		if (_cubes == null) {
			_cubes = new Cube[row][];
			for (int i = 0;i < row; ++i) {
				_cubes[i] = new Cube[col];
			}
		}
	}

	public void ResetCubes () {
		for (int x = 0;x < cubes.Length; ++x) {
			for (int y = 0;y < cubes[x].Length; ++y) {
				if (cubes[x][y].cubeData.top.face == QuadData.Face.None) {
					cubes[x][y].cubeData.type = CubeData.Type.Regular;
				}
				cubes[x][y].meshRenderer.sharedMaterial = cubes[x][y].textureAtlas.defaultMat;
				if (Application.isPlaying) {
					if (cubes[x][y].tower != null) {
						cubes[x][y].tower.FreeAllBullets();
						if (!GameManager.Instance.isPause) {
							 GameManager.Instance.UpdateMoney(cubes[x][y].tower.cost*0.5f);
						}
						Destroy(cubes[x][y].tower.gameObject);
					}
				}
			}
		}
	}

	// Use for create a new grid only if row and col has changed
	public void CreateNewGrid () {	
		ResetCubesArray();
		CreateNewPathOnly();
	}

	public void CreateNewPathOnly () {
		for (int x = 0;x < cubes.Length; ++x) {
			for (int y = 0;y < cubes[x].Length; ++y) {
				cubes[x][y].cubeData.top.face = QuadData.Face.None;
				cubes[x][y].UpdateTexture();
				if (Application.isPlaying) {
					if (cubes[x][y].tower != null) {
						if (!GameManager.Instance.isPause) {
							 GameManager.Instance.UpdateMoney(cubes[x][y].tower.cost*0.5f);
						}
						Destroy(cubes[x][y].tower.gameObject);
					}
				}
			}
		}
		pathGen.GeneratePath();
	}

	public void UpdatePointsOnly () {
		pathGen.UpdatePath();
	}

	Cube CreateCube (Vector2Int index) {
		GameObject cubeObj = new GameObject("Cube");
		cubeObj.transform.SetParent(transform);
		BoxCollider boxCollider = cubeObj.AddComponent<BoxCollider>();
		boxCollider.isTrigger = true;
		float halfRow = (float)(row-1)*0.5f;
		float halfCol = (float)(col-1)*0.5f;
		cubeObj.transform.localPosition = new Vector3((index.x-halfRow)*CubeData.kCubeSize,0,(index.y-halfCol)*CubeData.kCubeSize);
		Cube cube = cubeObj.AddComponent<Cube>();
		cube.textureAtlas = atlas;
		cube.index = index;
		cube.quadPosition = GetQuadPosition(index);
		cube.name = cube.Id;
		return cube;
	}

	Cube.Position GetQuadPosition ( Vector2Int index) {
		if (index.x == 0) {
			if (index.y == 0) {
				return Cube.Position.BL;
			} else if (index.y == (col-1)) {
				return Cube.Position.FL;
			} else {
				return Cube.Position.L;
			}
		} else if (index.x == (row-1)) {
			if (index.y == 0) {
				return Cube.Position.BR;
			} else if (index.y == (col-1)) {
				return Cube.Position.FR;
			} else {
				return Cube.Position.R;
			}
		} else {
			if (index.y == 0) {
				return Cube.Position.B;
			} else if (index.y == (col-1)) {
				return Cube.Position.F;
			} else {
				return Cube.Position.T;
			}
		}
	}

	public void RemoveGrid () {
		Cube[] cubeChildren = GetComponentsInChildren<Cube>();
		if ((cubeChildren != null) && (cubeChildren.Length > 0)) {
			for (int i = 0;i < cubeChildren.Length; ++i) {
				DestroyImmediate(cubeChildren[i].gameObject);
			}
		}

		if (_cubes != null) {
			_cubes = null;
		}
	}
}