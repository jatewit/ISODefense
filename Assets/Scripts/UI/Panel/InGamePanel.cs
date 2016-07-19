using UnityEngine;
using UnityEngine.UI;

public class InGamePanel : PanelWindow {
	[SerializeField] Slider hpSlider;
	[SerializeField] Text scoreText;
	[SerializeField] Text moneyText;
	
	void OnEnable () {
		GameManager.onScoreUpdate += OnScoreUpdate;
		GameManager.onMoneyUpdate += OnMoneyUpdate;
		GameManager.onHPUpdate += OnHPUpdate;
	}
	void OnDisable () {
		GameManager.onScoreUpdate -= OnScoreUpdate;
		GameManager.onMoneyUpdate -= OnMoneyUpdate;
		GameManager.onHPUpdate -= OnHPUpdate;
	}
	
	void OnScoreUpdate ( float score ) {
		scoreText.text = "SCORE: " + score.ToString("N0");
	}
	void OnMoneyUpdate ( float money ) {
		moneyText.text = "MONEY: " + money.ToString("C0");
	}
	void OnHPUpdate ( float value ) {
		hpSlider.value = value;
		if (value == 0) {
			GameManager.Instance.Gameover();
		}
	}

	protected override void Awake() {
		base.Awake();
		this.onWindowShown += OnWindowShown;
		this.onWindowClosed += OnWindowClosed;
	}
	
	void OnWindowShown () {
		GameManager.Instance.RestartGame();
	}
	
	void OnWindowClosed () {
		scoreText.text = "SCORE: 0";
		moneyText.text = "MONEY: $0";
	}
	
	public override void Open () {
		base.Open();
		transform.SetAsFirstSibling();
	}
	
	public void Pause () {
		GameManager.Instance.isPause = true;
		PanelManager.Instance.Open<PausePanel>();
	}
}
