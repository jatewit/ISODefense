using UnityEngine;
using System.Collections;

public class PausePanel : PanelWindow {
	public static bool ClickHome {get; private set;}
	
	public override void Open () {
		ClickHome = false;
		base.Open();
	}
	
	public void Resume () {
		GameManager.Instance.isPause = false;
		PanelManager.Instance.Close<PausePanel>();
	}
	
	public void Restart () {
		ClickHome = false;
		PanelManager.Instance.Close<PausePanel>();
		PanelManager.Instance.Restart();
	}
	
	public void Home () {
		ClickHome = true;
		PanelManager.Instance.Close<PausePanel>();
		PanelManager.Instance.Home();
	}
}
