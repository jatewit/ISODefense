using UnityEngine;
using Jatewit;
using System.Collections.Generic;

public class GameManager : MonoBehaviourSingleton<GameManager> {
    public delegate void UpdateDelegate (float value);
    public static event UpdateDelegate onScoreUpdate;
    public static event UpdateDelegate onMoneyUpdate;
    public static event UpdateDelegate onHPUpdate;
    [Range(0,100),SerializeField] float maxHP = 100;
    [Range(0,100),SerializeField] float startMoney = 100;
    public bool isPause;

    IsoGrid _grid;
    EnemyContainer _enemies;
    TowerContainer _towers;

    float _hitPoint = 0;
    float _score = 0;
    float _money = 100;

    public string score { get { return _score.ToString("N0");}}
    public string money { get { return _money.ToString("C0");}}

    public List<EnemyBase> currentEnemies { get { return _enemies.currEnemies;}}

 	protected override bool Awake() {
	    if(!base.Awake()) return false;
		Reset();
		return true;
 	}

    public bool CanBuy ( float cost ) {
        return ((_money-cost) >= 0);
    }

    public void UpdateMoney ( float money ) {
        _money = Mathf.Max(_money+money,0);
        if (onMoneyUpdate != null) onMoneyUpdate(_money);
    }

    public void UpdateScore ( float score ) {
        _score = Mathf.Max(_score+score,0);
        if (onScoreUpdate != null) onScoreUpdate(_score);
    }

    public void UpdateHp ( float hp ) {
        _hitPoint = Mathf.Clamp(_hitPoint+hp,0,maxHP);
        if (onHPUpdate != null) onHPUpdate(_hitPoint/maxHP);
    }

    public void Reset () {
        isPause = true;
        if (_grid == null) _grid = GetComponentInChildren<IsoGrid>();
        if (_enemies == null) _enemies = GetComponentInChildren<EnemyContainer>();
        if (_towers == null) _towers = GetComponentInChildren<TowerContainer>();
        _grid.ResetCubes();
        _enemies.ResetGame();
        _money = startMoney;
        _hitPoint = maxHP;
        _score = 0;
        if (onMoneyUpdate != null) onMoneyUpdate(_money);
        if (onScoreUpdate != null) onScoreUpdate(_score);
        if (onHPUpdate != null) onHPUpdate(_hitPoint/maxHP);
    }

    public void RestartGame () {
        Reset();
        _enemies.StartGame();
    }

    public void RestartGameWithNewGrid () {
        RestartGame();
        _grid.CreateNewPathOnly();
    }

    public void Gameover () {
        isPause = true;
        PanelManager.Instance.SwitchTo<GameoverPanel>();
    }
}
