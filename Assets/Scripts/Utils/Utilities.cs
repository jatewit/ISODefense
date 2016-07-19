//
// Author: Jate Wittayabundit
// Email: jate@ennface.com
// Company: Ennface
// Copyright (c) 2016, Ennface	All rights reserved.

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;

namespace Jatewit
{
	public static class Utilities 
	{
		public static bool RandomBool() {return (UnityEngine.Random.Range(0, 2) == 1);}
		public static Color RandomColor() {return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);}
		public static Color RandomColorIncludingAlpha() {return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);}

		public static Vector3 LerpByDistance(Vector3 A, Vector3 B, float x) {return (x * Vector3.Normalize(B - A) + A);}

		/// <summary>
		/// Shuffles the fast
		/// </summary>
		/// <param name="list">List.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <remarks>by Jate Wittayabundit</remarks>
		public static void ShuffleFast<T> ( IList<T> list ) {
			int n = list.Count;
			while (n > 1) {
				n--;
				int j = UnityEngine.Random.Range(0, n+1);
				T temp = list[j];
				list[j] = list[n];
				list[n] = temp;
			}
		}

		/// <summary>
		/// Shuffles the more randomness
		/// </summary>
		/// <param name="list">List.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <remarks>by Jate Wittayabundit</remarks>
		public static void ShuffleMoreRandom<T> ( IList<T> list ) {
			RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
			int n = list.Count;
			while (n > 1)
			{
				byte[] box = new byte[1];
				do provider.GetBytes(box);
				while (!(box[0] < n * (Byte.MaxValue / n)));
				int j = (box[0] % n);
				n--;
				T temp = list[j];
				list[j] = list[n];
				list[n] = temp;
			}
		}

		/// <summary>
		/// Randoms the dictionary.
		/// </summary>
		/// <returns>The dictionary.</returns>
		/// <param name="dictionary">Dictionary.</param>
		/// <typeparam name="TKey">The 1st type parameter.</typeparam>
		/// <typeparam name="TValue">The 2nd type parameter.</typeparam>
		public static string RandomDictionary<TKey,TValue> ( IDictionary<TKey,TValue> dictionary ) {
			TKey[] keys = new TKey[dictionary.Count];
			dictionary.Keys.CopyTo(keys,0);

			int index = UnityEngine.Random.Range(0,keys.Length);
			return keys[index].ToString();
		}

		public static Texture2D TakeScreenshot (List<Camera> cameras, int width, int height)
		{
			Rect rect = new Rect(0,0,width, height);
			return TakeScreenshot(cameras,rect,TextureFormat.RGB24);
		}

		public static Texture2D TakeScreenshot (List<Camera> cameras, Rect rect, TextureFormat format)
		{
			// BG Camera
			GameObject bgObject = new GameObject("BGCamera");
			Camera bgCamera = bgObject.AddComponent<Camera>();
			bgCamera.clearFlags = CameraClearFlags.SolidColor;
			bgCamera.backgroundColor = Color.clear;
			bgCamera.depth = -10;
			bgCamera.orthographic = true;
			bgCamera.orthographicSize = 1;
			cameras.Insert(0,bgCamera);

			cameras.Sort(CameraComparer);

			RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
			RenderTexture.active = rt;
			for (int i = 0; i < cameras.Count; ++i) {;
				if ((cameras[i] != null) && (cameras[i].enabled)) {
					cameras[i].targetTexture = rt;
					cameras[i].Render();
					cameras[i].targetTexture = null;
				}
			}
			Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, format, false);
			screenShot.ReadPixels(rect, 0, 0,false);
			screenShot.Apply();

			RenderTexture.active = null;
			GameObject.DestroyImmediate(rt);

			cameras.Remove(bgCamera);
			GameObject.DestroyImmediate(bgObject);
			Resources.UnloadUnusedAssets();

			return screenShot;
		}

		public static string SaveTexturePNG (Texture2D texture, string fileName, string absoluteFilePath)
		{
			byte[] bytes = texture.EncodeToPNG();
			string filePath = string.Format("{0}/{1}.png",absoluteFilePath,fileName);
			try {
				System.IO.File.WriteAllBytes(filePath, bytes);
			} catch (Exception e) {
				Debug.Log (e.Message);
			}
			return filePath;
		}

		public static string SaveTextureJPG (Texture2D texture, string fileName, string absoluteFilePath)
		{
			byte[] bytes = texture.EncodeToJPG();
			string filePath = string.Format("{0}/{1}.jpg",absoluteFilePath,fileName);
			try {
				File.WriteAllBytes(filePath, bytes);
			} catch (Exception e) {
				Debug.Log (e.Message);
			}
			return filePath;
		}

		public static string SaveScreenShot (Texture2D screenShot, string fileName, string absoluteFilePath)
		{
			byte[] bytes = screenShot.EncodeToPNG();
			string filePath = string.Format("{0}/{1}_{2}.png",absoluteFilePath,fileName,System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
			try {
				File.WriteAllBytes(filePath, bytes);
			} catch (Exception e) {
				Debug.Log (e.Message);
			}
			return filePath;
		}
		
		public static string SaveScreenShot (Texture2D screenShot, string fileName, bool isSDCard = true)
		{
			byte[] bytes = screenShot.EncodeToPNG();
			string filePath = GetScreenShootSavePath(fileName,isSDCard);
			try {
				File.WriteAllBytes(filePath, bytes);
			} catch (Exception e) {
				Debug.Log (e.Message);
			}
			return filePath;
		}
		
		public static string GetScreenShootSavePath (string fileName, bool isSDCard = true) 
		{
			string filePath = (isSDCard) ? Application.persistentDataPath : Application.dataPath;
			return string.Format("{0}/{1}_{2}.png",filePath,fileName,System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
		}
		
		public static int CameraComparer (Camera camera, Camera other)
		{
			return camera.depth.CompareTo(other.depth);
		}
		
		public static Texture2D DecodeTexture (int width, int height, string decodeTex ) {
			byte[] image = System.Convert.FromBase64String(decodeTex);
			Texture2D tex = new Texture2D(width,height,TextureFormat.ARGB32,false);
			tex.LoadImage(image);
			return tex;
		}

		public static void SaveJsonData (object obj, string filePath) {
			string json = JsonUtility.ToJson(obj);

			// using (StreamWriter outputFile = new StreamWriter(filePath, true)) {
			// 	outputFile.WriteLine(json);
			// }
			File.WriteAllText(filePath, json);
		}

		public static T LoadJsonData<T> (string filePath) {
			if (File.Exists(filePath)) {
				string file = File.ReadAllText(filePath);
				return JsonUtility.FromJson<T>(file);
			}
			return default(T);
		}

		public static void ConvertDictionaryAndSave<T> ( Dictionary<string,T> dict, string f )
		{
			// Convert dictionary to string and save
			string s = ConvertDictionary(dict);
			System.IO.File.WriteAllText(f, s);
		}
		
		public static string ConvertDictionary<T> (Dictionary<string, T> d)
		{
			// Build up each line one-by-one and then trim the end
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			foreach (KeyValuePair<string, T> pair in d)
			{
				builder.Append(pair.Key).Append(":").Append(pair.Value).Append('|');
			}
			string result = builder.ToString();
			// Remove the final delimiter
			result = result.TrimEnd('|');
			return result;
		}
		
		public static Dictionary<string, string> GetDictionaryFromString (string data)
		{
			Dictionary<string, string> d = new Dictionary<string, string>();
			// Divide all pairs (remove empty strings)
			string[] tokens = data.Split(new char[] { ':', '|' }, StringSplitOptions.RemoveEmptyEntries);
			// Walk through each item
			for (int i = 0; i < tokens.Length; i += 2)
			{
				string name = tokens[i];
				string value = tokens[i + 1];
				d.Add(name, value);
			}
			return d;
		}
		
		public static T[] GetObjectAtPath<T> (string path) where T : UnityEngine.Object {
			string filePath = Application.dataPath+"/"+path;
			if (!Directory.Exists(filePath)) {
				return null;
			}
			List<T> al = new List<T>();
			string [] fileEntries = Directory.GetFiles(filePath);
			if ((fileEntries != null) && (fileEntries.Length > 0)) {
				foreach(string fileName in fileEntries)
				{
					int index = fileName.LastIndexOf("/");
					string localPath = "Assets/" + path;
					
					if (index > 0)
						localPath += fileName.Substring(index);
					T t = AssetDatabase.LoadAssetAtPath<T>(localPath);
		
					if(t != null) al.Add(t);
				}
			}
			if (al.Count > 0) {
				return al.ToArray();
			} else {
				return null;
			}
		}
		
		public static string GetAssetsPathFromSelectedObject () {
			string path = "Assets";
			foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
			{
				path = AssetDatabase.GetAssetPath(obj);
				if (File.Exists(path))
				{
					path = Path.GetDirectoryName(path);
				}
				break;
			}
			return path;
		}
	}
}