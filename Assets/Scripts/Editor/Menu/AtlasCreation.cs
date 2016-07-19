// AtlasCreation.cs
//
// Author: Jate Wittayabundit

using UnityEngine;
using UnityEditor;
using Jatewit;
using System.Collections.Generic;

public class AtlasCreation : EditorWindow
{
	const string ASSET_PATH = "Assets/Textures/Atlas/";
	#region Custom Params
	public int MaxTextureSize = 2048;
	public int Padding = 2;
	public float OffsetPixel = 0.5f;
	public string FileName = "Atlas";

	public Texture2D AlphaITile;
	public Texture2D AlphaLTile;
	public Texture2D AlphaUTile;
	public Texture2D FloorTexture;
	public Texture2D SideTexture;

	public TextureFormat CompressFormat = TextureFormat.ARGB32;
	public TextureWrapMode TextureWarpMode = TextureWrapMode.Clamp;
	public FilterMode Filter = FilterMode.Bilinear;
	#endregion

	SerializedObject _object;
	SerializedProperty _alphaITile;
	SerializedProperty _alphaLTile;
	SerializedProperty _alphaUTile;
	SerializedProperty _floorTexture;
	SerializedProperty _sideTexture;

	Vector2 _scrollPosition;
	string _msg;
	string _folderPath;
	bool _showTextures = true;

	List<Texture2D> _allTextures = new List<Texture2D>();

	// Creation of window
	[MenuItem("Editor/Create Texture Atlas")]
	public static void Init() {
		GetWindow(typeof(AtlasCreation), false, "Atlas Creation");
	}
	
	Object[] GetSelectedTexture2D()
	{
		return Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
	}
	
	void OnEnable () {
		_object = new SerializedObject (this);
		_alphaITile = _object.FindProperty("AlphaITile");
		_alphaLTile = _object.FindProperty("AlphaLTile");
		_alphaUTile = _object.FindProperty("AlphaUTile");
		_floorTexture = _object.FindProperty("FloorTexture");
		_sideTexture = _object.FindProperty("SideTexture");

		_scrollPosition = Vector2.zero;
		_folderPath = Application.dataPath+"/Textures/Atlas";

		_allTextures = new List<Texture2D>();
	}
	
	// Main GUI Function - call subs
	void OnGUI() {
		_object.Update();
		EditorGUILayout.BeginVertical("Box");
		_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
		FileName = EditorGUILayout.TextField("File Name",FileName);
		MaxTextureSize = EditorGUILayout.IntSlider("Max Texture Size", MaxTextureSize,32,4096);
		OffsetPixel = EditorGUILayout.Slider("Offset", OffsetPixel,0,10);
		Padding = EditorGUILayout.IntSlider("Texture Padding", Padding,0,20);
		CompressFormat = (TextureFormat)EditorGUILayout.EnumPopup("Compress Format",CompressFormat);
		TextureWarpMode = (TextureWrapMode)EditorGUILayout.EnumPopup("Texture WarpMode",TextureWarpMode);
		Filter = (FilterMode)EditorGUILayout.EnumPopup("Filter Mode",Filter);

		_showTextures = EditorGUILayout.Foldout(_showTextures, "All Textures");
		if (_showTextures) {
			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.PropertyField(_alphaITile, new GUIContent("Alpha I Tile"));
			EditorGUILayout.PropertyField(_alphaLTile, new GUIContent("Alpha L Tile"));
			EditorGUILayout.PropertyField(_alphaUTile, new GUIContent("Alpha U Tile"));
			EditorGUILayout.PropertyField(_floorTexture, new GUIContent("Floor Texture"));
			EditorGUILayout.PropertyField(_sideTexture, new GUIContent("Side Texture"));
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		if ((_floorTexture != null) && (_sideTexture != null) && (_alphaITile != null) &&
		    (_alphaLTile != null) && (_alphaUTile != null)) {
			if (GUILayout.Button("Create Texture")) {
				CreateNormalTexture();
			}
		} else {
			_msg = "Please select all textures for packing!";
		}

		if (_msg != null) {
			GUILayout.Label(_msg,EditorStyles.miniBoldLabel);
		}
		EditorGUILayout.Space();
		_object.ApplyModifiedProperties();
	}
    
    void CreateNormalTexture () {
        CleanUpTexture();
        _allTextures = new List<Texture2D>();
        bool success = CreateNormalTextureFace();
        
        if (success) {
            SaveTexture();
            _msg = "Successfully create texture atlas!";
        } else {
            _msg = "Error, Please select all textures for packing!";
        }
    }
    
    bool CreateNormalTextureFace () {
		// Create All Textures
		if (CreateTexture(_floorTexture,QuadData.Face.I) && CreateTexture(_floorTexture,QuadData.Face.L) && 
			CreateTexture(_floorTexture,QuadData.Face.U) && CreateTexture(_floorTexture,QuadData.Face.None) &&
			CreateTexture(_sideTexture,QuadData.Face.None)) {
			return true;
		}

		return false;
    }

	bool CreateTexture ( SerializedProperty textureSerialized, QuadData.Face face ) {
		Texture2D tex = (Texture2D)textureSerialized.objectReferenceValue;
		if (tex != null) {
			Vector2 size = new Vector2(tex.width,tex.height);
			AddTexture(tex.name,size,face,tex.GetPixels());
			return true;
		}
		return false;
	}

	void AddTexture (string texName, Vector2 size, QuadData.Face face, Color[] baseColor) {
		string fileName = texName;
		Color[] newColors = GetFinalColors(baseColor, face, ref fileName);
		Texture2D newText = new Texture2D((int)size.x,(int)size.y,TextureFormat.ARGB32,false);
		newText.SetPixels(newColors);
		newText.Apply();
		newText.name = fileName;
		_allTextures.Add(newText);
	}

	Color[] GetFinalColors ( Color[] baseColor, QuadData.Face face, ref string fileName) {
		Color[] alphaColor = GetTileAlphaColors(face);
		string firstCap = face.ToString();
		fileName += "_" + firstCap;
		Color[] newColors = new Color[baseColor.Length];
		for (int i = 0 ; i < baseColor.Length; ++i) {
			float red = baseColor[i].r;
			float green = baseColor[i].g;
			float blue = baseColor[i].b;
			float newAlpha = 1;
			if (alphaColor != null) {
				float alpha = 1-alphaColor[i].a;
				if (alpha < 1) {
					newAlpha = alpha;
					red *= alpha;
					green *= alpha;
					blue *= alpha;
				}
			}
			newColors[i] = new Color(red,green,blue,newAlpha);
		}
		return newColors;
	}

	Color[] GetTileAlphaColors ( QuadData.Face face ) {
		Texture2D alphaTex;
		switch (face) {
		case QuadData.Face.I:
			alphaTex = (Texture2D)_alphaITile.objectReferenceValue;
			break;
		case QuadData.Face.L:
			alphaTex = (Texture2D)_alphaLTile.objectReferenceValue;
			break;
		case QuadData.Face.U:
			alphaTex = (Texture2D)_alphaUTile.objectReferenceValue;
			break;
		default:
			return null;
		}
		return alphaTex.GetPixels();
	}

	void SaveTexture (bool isSelect = false) {
        string name = FileName;
		Texture2D textureAtlas = new Texture2D(MaxTextureSize, MaxTextureSize, CompressFormat,false);
		Rect[] atlasUvs = textureAtlas.PackTextures(_allTextures.ToArray(), Padding, MaxTextureSize);
		textureAtlas.wrapMode = TextureWarpMode;
		textureAtlas.filterMode = Filter;
		textureAtlas.Apply();	

		AtlasData atlasFile = new AtlasData();
		atlasFile.uvData = new UVAtlas[_allTextures.Count];
		for (int i = 0; i < _allTextures.Count; ++i) {
			float wRatio = Mathf.Min(OffsetPixel,_allTextures[i].width-2)/_allTextures[i].width; // Offset 1.5 pixels
			float hRatio = Mathf.Min(OffsetPixel,_allTextures[i].height-2)/_allTextures[i].height; // Offset 1.5 pixels
			float xValue = (atlasUvs[i].xMax - atlasUvs[i].xMin)*wRatio;
			float yValue = (atlasUvs[i].yMax - atlasUvs[i].yMin)*hRatio;
			float xMin = atlasUvs[i].xMin + xValue;
			float xMax = atlasUvs[i].xMax - xValue;
			float yMin = atlasUvs[i].yMin + yValue;
			float yMax = atlasUvs[i].yMax - yValue;
			atlasFile.uvData[i] = new UVAtlas(_allTextures[i].name,xMin,xMax,yMin,yMax);
		}
		
		if (!System.IO.Directory.Exists(_folderPath)) {    
			//if no folder existed, create it
			System.IO.Directory.CreateDirectory(_folderPath);
		}
        Utilities.SaveTexturePNG(textureAtlas,name,_folderPath+"/");
		Utilities.SaveJsonData(atlasFile,_folderPath+"/"+name+".json");
		AssetDatabase.Refresh();
		
		string path = ASSET_PATH+name+".png";
		TextureImporter textureImport = AssetImporter.GetAtPath(path) as TextureImporter;
		textureImport.textureType = TextureImporterType.Image;
		textureImport.maxTextureSize = MaxTextureSize;
		textureImport.textureFormat = TextureImporterFormat.AutomaticTruecolor;
		textureImport.wrapMode = TextureWarpMode;
		textureImport.filterMode = Filter;
		AssetDatabase.ImportAsset(path);
		
		string newAssetPath = ASSET_PATH+name+".asset";
		
		CubeAtlas oldAtlas = AssetDatabase.LoadMainAssetAtPath(newAssetPath) as CubeAtlas;
		
		if (oldAtlas != null) {
			AssetDatabase.StartAssetEditing();
            oldAtlas.filePath = ASSET_PATH+name+".json";
			oldAtlas.AtlasTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(ASSET_PATH+name+".png",typeof(Texture2D));
			oldAtlas.SetUVData();
			AssetDatabase.StopAssetEditing();
			EditorUtility.SetDirty(oldAtlas);
		} else {
			CubeAtlas cubeAtlas = ScriptableObject.CreateInstance<CubeAtlas>();
			AssetDatabase.CreateAsset(cubeAtlas, AssetDatabase.GenerateUniqueAssetPath(newAssetPath));
			AssetDatabase.StartAssetEditing();
            cubeAtlas.filePath = ASSET_PATH+name+".json";
			cubeAtlas.AtlasTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(ASSET_PATH+name+".png",typeof(Texture2D));
			cubeAtlas.SetUVData();
			AssetDatabase.StopAssetEditing();
			EditorUtility.SetDirty(cubeAtlas);
		}
		DestroyImmediate(textureAtlas,true);
		CleanUpTexture();
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		SceneView.RepaintAll();
	}

	void CleanUpTexture () {
		if ((_allTextures != null) && (_allTextures.Count > 0)) {
			while (_allTextures.Count > 0) {
				DestroyImmediate(_allTextures[0],true);
				_allTextures.RemoveAt(0);
			}
			_allTextures.Clear();
		}
	}
	
	void OnInspectorUpdate() {
		Repaint();
	}
}
