// QuadData.cs
//
// Author: Jate Wittayabundit
using UnityEngine;

[System.Serializable]
public struct QuadData {
	public enum TextureType { Floor, Side }
	public enum Face { None, U, I, L }
	public enum Path { None, Top, Left, Right, Bottom }
	public enum Degree { D0, D90, D180, D270 }
	public TextureType textureType;
	public Face face;
	public Degree degree;
	Path _entrance;

	#region Get & Set
	public string textureName {
		get {
			return textureType + "_" + face;
		}
	}
	public Path entrance {
		set { _entrance = value; }
	}	
	public Path exit {
		get {
			switch (face) {
			case Face.U:
				switch (degree) {
					case Degree.D0: 	return Path.Top;
					case Degree.D90: 	return Path.Right;
					case Degree.D180: 	return Path.Bottom;
					case Degree.D270: 	return Path.Left;
				}
				break;
			case Face.I:
				switch (_entrance) {
					case Path.Top: 		return Path.Bottom;
					case Path.Bottom: 	return Path.Top;
					case Path.Left: 	return Path.Right;
					case Path.Right: 	return Path.Left;
				}
				break;
			case Face.L:
				switch (_entrance) {
					case Path.Top:
						switch (degree) {
							case Degree.D270: 	return Path.Left;
						}
						return Path.Right;
					case Path.Bottom:
						switch (degree) {
							case Degree.D180: 	return Path.Left;
						}
						return Path.Right;
					case Path.Left:
						switch (degree) {
							case Degree.D270: 	return Path.Top;
						}
						return Path.Bottom;
					case Path.Right:
						switch (degree) {
							case Degree.D90: 	return Path.Bottom;
						}
						return Path.Top;
				}
				break;
			}
			return Path.None;
		}
	}
	#endregion
}