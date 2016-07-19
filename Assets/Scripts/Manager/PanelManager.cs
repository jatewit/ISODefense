using UnityEngine;
using Jatewit;

public class PanelManager : MonoBehaviourSingleton<PanelManager> {
	[SerializeField] HomePanel homePanel;
	[SerializeField] InGamePanel inGamePanel;
	[SerializeField] PausePanel pausePanel;
	[SerializeField] GameoverPanel gameoverPanel;
	
	Overlay _overlay;
	PanelWindow _openNext;
	
	PanelWindow CurrentShowingPanel {
		get {
			if (homePanel.isShowing) return homePanel;
			if (inGamePanel.isShowing) return inGamePanel;
			if (pausePanel.isShowing) return pausePanel;
			if (gameoverPanel.isShowing) return gameoverPanel;
			return null;
		}
	}

 	protected override bool Awake() {
	    if(!base.Awake()) return false;
		if (homePanel == null) {
			homePanel = FindObjectOfType<HomePanel>();
		}
		if (inGamePanel == null) {
			inGamePanel = FindObjectOfType<InGamePanel>();
		}
		if (pausePanel == null) {
			pausePanel = FindObjectOfType<PausePanel>();
		}
		if (gameoverPanel == null) {
			gameoverPanel = FindObjectOfType<GameoverPanel>();
		}
		_overlay = GetComponentInChildren<Overlay>(true);
		return true;
 	}
	
	public void Open<T>() where T : PanelWindow {
		var nextObj = GetObject<T>();
		nextObj.Open();
		if (nextObj.shouldActivateOverlay && !_overlay.isShowing) _overlay.Show();
	}
	
	public void Close<T>() where T : PanelWindow {
		var nextObj = GetObject<T>();
		nextObj.Close();
		if (nextObj.shouldActivateOverlay && _overlay.isShowing) _overlay.Close();
	}
	
	public void SwitchTo<T>() where T : PanelWindow {
		if (_openNext == null) {
			if (PanelWindow.CurrentOpenPanel != null) {
				_openNext = GetObject<T>();
				if (CanClose<HomePanel>(PanelWindow.CurrentOpenPanel)) return;
				if (CanClose<InGamePanel>(PanelWindow.CurrentOpenPanel)) return;
				if (CanClose<PausePanel>(PanelWindow.CurrentOpenPanel)) return;
				if (CanClose<HomePanel>(PanelWindow.CurrentOpenPanel)) return;
			} else {
				if (CurrentShowingPanel != null) {
					_openNext = GetObject<T>();
					if (CanClose<HomePanel>(CurrentShowingPanel)) return;
					if (CanClose<InGamePanel>(CurrentShowingPanel)) return;
					if (CanClose<PausePanel>(CurrentShowingPanel)) return;
					if (CanClose<HomePanel>(CurrentShowingPanel)) return;
				}
			}
		}
	}
	
	public void Home() {
		if (inGamePanel.isShowing) {
			inGamePanel.Close();
		}
		GameManager.Instance.Reset();
		if (_overlay.isShowing) {
			_overlay.Close();
		}
		
		PanelManager.Instance.Open<HomePanel>();
	}
	
	public void Restart() {
		if (_overlay.isShowing) {
			_overlay.Close();
		}
		if (!inGamePanel.isShowing) {
			inGamePanel.Open();
		} else {
			GameManager.Instance.RestartGameWithNewGrid();
		}
	}
	
	void OpenNextPanel () {
		_openNext.Open();
		if (_openNext.shouldActivateOverlay && !_overlay.isShowing) {
			_overlay.Show();
		}
		RemoveAction();
		_openNext = null;
	}
	
	void RemoveAction () {
		homePanel.onWindowClosed -= OpenNextPanel;
		inGamePanel.onWindowClosed -= OpenNextPanel;
		pausePanel.onWindowClosed -= OpenNextPanel;
		gameoverPanel.onWindowClosed -= OpenNextPanel;
	}
	
	bool CanClose<T> (PanelWindow obj) where T : PanelWindow {
		if (EqualType<T>(obj)) {
			var closeObj = GetObject<T>();
			if (closeObj.isShowing) {
				closeObj.Close();
				if (_openNext != null) {
					if (!_openNext.shouldActivateOverlay && _overlay.isShowing) {
						_overlay.Close();
					}
					closeObj.onWindowClosed += OpenNextPanel;
				}
			}
			return true;
		}
		return false;
	}

	T GetObject<T> () where T : PanelWindow {
		if (EqualType<T>(homePanel)) return homePanel as T;
		if (EqualType<T>(inGamePanel)) return inGamePanel as T;
		if (EqualType<T>(pausePanel)) return pausePanel as T;
		if (EqualType<T>(gameoverPanel)) return gameoverPanel as T;
		return null;
	}
	
	public static bool EqualType<T> (PanelWindow obj) where T : PanelWindow {
		var myObj = obj as T;
		if (myObj != null) {
			return true;
		}
		return false;
	}
}
