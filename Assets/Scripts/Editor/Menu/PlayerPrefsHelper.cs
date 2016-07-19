using UnityEditor;
using UnityEngine;

public class PlayerPrefsHelper : EditorWindow {

	[MenuItem("Editor/PlayerPrefs/Delete All")]
	public static void CreateGameData() {
		PlayerPrefs.DeleteAll();
		Debug.Log("Delete All PlayerPrefs data");
	}
}
