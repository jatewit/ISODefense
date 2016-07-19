using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingPanel : PanelWindow {
	[SerializeField] private Slider slider;
    
    Coroutine _loadingRoutine;
    void OnEnable () {
        this.onWindowShown += StartLoading;
    }
    void OnDisable () {
        this.onWindowShown -= StartLoading;
        if (_loadingRoutine != null) {
            _loadingRoutine = null;
        }
    }
    
	protected override void Start () {
        slider.value = 0;
		Open();
	}
	
    void StartLoading () {
        _loadingRoutine = StartCoroutine(WaitForLoading());
    }
    
    IEnumerator WaitForLoading () {
        AsyncOperation async = SceneManager.LoadSceneAsync("Game");
        while ((!async.isDone) && (slider.value < 1)) {
            slider.value = async.progress;
            yield return null;
        }
       	slider.value = 1;
        _loadingRoutine = null;
    }
}