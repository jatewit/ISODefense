using UnityEngine;
using System.Collections.Generic;
public class PathGenerator : MonoBehaviour {
	public delegate void QuadDataDelegate (ref QuadData.Face face, ref QuadData.Degree degree);
	public IsoGrid grid;
	public List<Vector3> points;

	void Awake () {
		if (grid == null) {
			grid = GetComponent<IsoGrid>();
		}
	}

	// To Update path only
	public void UpdatePath () {
		points = new List<Vector3>();
		for (int y = 0;y < grid.cubes[0].Length; ++y) {
			if (grid.cubes[0][y].cubeData.type == CubeData.Type.Start) {
				AddNextPath(grid.cubes[0][y]);
				break;
			}
		}
	}

	void AddNextPath (Cube fromCube) {
		points.Add(fromCube.enemyPath);
		if (fromCube.cubeData.type != CubeData.Type.End) {
			QuadData.Path entrance = fromCube.GetNextCubeEntrance();
			Vector2Int nextIndex = fromCube.GetNextCubeIndex();
			if (nextIndex != fromCube.index) {
				grid.cubes[nextIndex.x][nextIndex.y].SetEntrance(entrance);
				AddNextPath(grid.cubes[nextIndex.x][nextIndex.y]);
			}
		}
	}

	// NOTE: This path will only force going forward +x direction
	public void GeneratePath () {
		points = new List<Vector3>();
		int y = Random.Range(0,grid.cubes[0].Length);
		grid.cubes[0][y].cubeData.type = CubeData.Type.Start;
		grid.cubes[0][y].cubeData.top.face = QuadData.Face.U;
		grid.cubes[0][y].cubeData.top.degree = QuadData.Degree.D90;
		grid.cubes[0][y].UpdateTexture();
		points.Add(grid.cubes[0][y].enemyPath);
		FindNeighbour(grid.cubes[0][y]);
	}

	void FindNeighbour (Cube fromCube) {
		Vector2Int nextIndex = fromCube.GetNextCubeIndex();
		QuadData.Path entrance = fromCube.GetNextCubeEntrance();
		if ((nextIndex.x > 0) && (nextIndex.x < (grid.cubes.Length-1))) {
			if (entrance != QuadData.Path.None) {
				if ((nextIndex.y >= 0) && (nextIndex.y < (grid.cubes[fromCube.index.x].Length))) {
					if (entrance == QuadData.Path.Right) {
						CheckPath(nextIndex,entrance,fromCube,RandomRightMin,RandomRightMax,RandomFaceRight);
					} else if (entrance == QuadData.Path.Bottom) {
						CheckPath(nextIndex,entrance,fromCube,null,ForceL90Degree,RandomFaceBottom);
					} else if (entrance == QuadData.Path.Top) {
						CheckPath(nextIndex,entrance,fromCube,ForceL0Degree,null,RandomFaceTop);
					} else {
						if ((fromCube.cubeData.top.face == QuadData.Face.I) && (fromCube.cubeData.top.degree == QuadData.Degree.D90)) {
							CheckPath(nextIndex,entrance,fromCube,RandomLeftMin,RandomLeftMax,ForceRandomLLeft);
						} else {
							CheckPath(nextIndex,entrance,fromCube,RandomLeftMin,RandomLeftMax,RandomFaceLeft);
						}
					}
				}
			}
		} else {
			if (nextIndex.x == (grid.cubes.Length-1)) {
				// End Path
				SetCube(nextIndex,entrance,CubeData.Type.End,QuadData.Face.U,QuadData.Degree.D270);
			} else if (nextIndex.x == 0) {
				if (entrance == QuadData.Path.Right) {
					SetCube(nextIndex,entrance,CubeData.Type.Regular,QuadData.Face.L,QuadData.Degree.D0);
					FindNeighbour(grid.cubes[nextIndex.x][nextIndex.y]);
				} else if (entrance == QuadData.Path.Bottom) {
					CheckPath(nextIndex,entrance,fromCube,null,ForceL90Degree,RandomFaceBottomMin);
				} else if (entrance == QuadData.Path.Top) {
					CheckPath(nextIndex,entrance,fromCube,ForceL0Degree,null,RandomFaceTopMin);
				}
			}
		}
	}

	// ************************************************************************** //
	// X == 0

	// Y == 0 - from top
	void ForceL0Degree (ref QuadData.Face face, ref QuadData.Degree degree) {
		face = QuadData.Face.L;
		degree = QuadData.Degree.D0;
	}

	// Y == Max index - from bottom
	void ForceL90Degree (ref QuadData.Face face, ref QuadData.Degree degree) {
		face = QuadData.Face.L;
		degree = QuadData.Degree.D90;
	}

	void RandomFaceBottomMin(ref QuadData.Face face, ref QuadData.Degree degree) {
		RandomFace(ref face);
		if (face == QuadData.Face.I) {
			degree = QuadData.Degree.D0;
		} else {
			degree = QuadData.Degree.D90;
		}
	}

	void RandomFaceTopMin(ref QuadData.Face face, ref QuadData.Degree degree) {
		RandomFace(ref face);
		if (face == QuadData.Face.I) {
			degree = QuadData.Degree.D0;
		} else {
			degree = QuadData.Degree.D0;
		}
	}
	// ************************************************************************** //
	// X > 0 && X < _grid.Max-1
	void RandomLeftMin (ref QuadData.Face face, ref QuadData.Degree degree) {
		RandomFace(ref face);
		if (face == QuadData.Face.I) {
			degree = QuadData.Degree.D90;
		} else {
			degree = QuadData.Degree.D270;
		}
	}
	void RandomLeftMax (ref QuadData.Face face, ref QuadData.Degree degree) {
		RandomFace(ref face);
		if (face == QuadData.Face.I) {
			degree = QuadData.Degree.D90;
		} else {
			degree = QuadData.Degree.D180;
		}
	}
	void RandomFaceLeft(ref QuadData.Face face, ref QuadData.Degree degree) {
		RandomFace(ref face);
		if (face == QuadData.Face.I) {
			degree = QuadData.Degree.D90;
		} else {
			if (Mathf.RoundToInt(Random.value) > 0) {
				degree = QuadData.Degree.D270;
			} else {
				degree = QuadData.Degree.D180;
			}
		}
	}

	void ForceRandomLLeft(ref QuadData.Face face, ref QuadData.Degree degree) {
		face = QuadData.Face.L;
		if (Mathf.RoundToInt(Random.value) > 0) {
			degree = QuadData.Degree.D270;
		} else {
			degree = QuadData.Degree.D180;
		}
	}

	void RandomRightMin (ref QuadData.Face face, ref QuadData.Degree degree) {
		RandomFace(ref face);
		if (face == QuadData.Face.I) {
			degree = QuadData.Degree.D90;
		} else {
			degree = QuadData.Degree.D0;
		}
	}
	void RandomRightMax (ref QuadData.Face face, ref QuadData.Degree degree) {
		RandomFace(ref face);
		degree = QuadData.Degree.D90;
	}
	void RandomFaceRight(ref QuadData.Face face, ref QuadData.Degree degree) {
		RandomFace(ref face);
		if (face == QuadData.Face.I) {
			degree = QuadData.Degree.D90;
		} else {
			if (Mathf.RoundToInt(Random.value) > 0) {
				degree = QuadData.Degree.D0;
			} else {
				degree = QuadData.Degree.D90;
			}
		}
	}
	void RandomFaceBottom(ref QuadData.Face face, ref QuadData.Degree degree) {
		RandomFace(ref face);
		if (face == QuadData.Face.I) {
			degree = QuadData.Degree.D0;
		} else {
			// only force right direction
			degree = QuadData.Degree.D90;
		}
	}

	void RandomFaceTop(ref QuadData.Face face, ref QuadData.Degree degree) {
		RandomFace(ref face);
		if (face == QuadData.Face.I) {
			degree = QuadData.Degree.D0;
		} else {
			// only force right direction
			degree = QuadData.Degree.D0;
		}
	}

	// ************************************************************************** //

	void CheckPath (Vector2Int nextIndex, QuadData.Path entrance, Cube fromCube, 
					QuadDataDelegate minFunc = null, QuadDataDelegate maxFunc = null,
					QuadDataDelegate regFunc = null) {
		QuadData.Face face = QuadData.Face.None;
		QuadData.Degree degree = QuadData.Degree.D0;
		if (nextIndex.y == 0) {
			if (minFunc != null) minFunc(ref face,ref degree);
		} else if (nextIndex.y == (grid.cubes[fromCube.index.x].Length-1)) {
			if (maxFunc != null) maxFunc(ref face,ref degree);
		} else {
			if (regFunc != null) regFunc(ref face,ref degree);
		}
		SetCube(nextIndex,entrance,CubeData.Type.Regular,face,degree);
		FindNeighbour(grid.cubes[nextIndex.x][nextIndex.y]);
	}

	void RandomFace (ref QuadData.Face face) {
		if (Mathf.RoundToInt(Random.value) > 0) {
			face = QuadData.Face.L;
		} else {
			face = QuadData.Face.I;
		}
	}

	void SetCube ( 	Vector2Int nextIndex, QuadData.Path entrance, 
					CubeData.Type type, QuadData.Face face,
					QuadData.Degree degree = QuadData.Degree.D0) {
		grid.cubes[nextIndex.x][nextIndex.y].SetEntrance(entrance);
		grid.cubes[nextIndex.x][nextIndex.y].cubeData.type = type;
		grid.cubes[nextIndex.x][nextIndex.y].cubeData.top.face = face;
		grid.cubes[nextIndex.x][nextIndex.y].cubeData.top.degree = degree;
		grid.cubes[nextIndex.x][nextIndex.y].UpdateTexture();
		points.Add(grid.cubes[nextIndex.x][nextIndex.y].enemyPath);
	}

	void OnDrawGizmos () {
		if (points != null) {
			Gizmos.color = Color.white;
			for (int i = 0;i < (points.Count-1); ++i) {
				Gizmos.DrawLine(points[i],points[i+1]);
			}
		}
	}
}
