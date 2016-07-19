using UnityEngine;
using UnityEngine.UI;

public class GameoverPanel : PanelWindow {
	[SerializeField] Text scoreText;
	[SerializeField] Text moneyText;

	void OnEnable () {
		scoreText.text = GameManager.Instance.score;
		moneyText.text = GameManager.Instance.money;
	}
	
	public void Home () {
		Close();
		this.onWindowClosed += GoToHome;
	}
	
	public void Restart () {
		Close();
		this.onWindowClosed += RestartGame;
	}
	
	void GoToHome () {
		this.onWindowClosed -= GoToHome;
		PanelManager.Instance.Home();
	}
	
	void RestartGame () {
		this.onWindowClosed -= RestartGame;
		PanelManager.Instance.Restart();
	}
}
