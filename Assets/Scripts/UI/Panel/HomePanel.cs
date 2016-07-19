using UnityEngine;

public class HomePanel : PanelWindow {
	protected override void Start () {
		Open();
	}
	public void Play () {
		PanelManager.Instance.SwitchTo<InGamePanel>();
	}
}
