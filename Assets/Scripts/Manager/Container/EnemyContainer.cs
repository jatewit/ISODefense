using UnityEngine;
using Jatewit;
using System.Collections.Generic;

public class EnemyContainer : MonoBehaviour {
    const int kMaxEnemies = 40;
    public int enemyCount = 20;
    [Range(0,10)]
    [SerializeField] float enemySpawnTime = 1f;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] IsoGrid grid;
    [SerializeField] PoolObjectsManager<EnemyBase> _enemies;
    int _enemyCount = 20;

    bool _startCount = false;
    float _time = 0;
    
    public List<EnemyBase> currEnemies { get { return (_enemies != null) ? _enemies.currentObjects : null; }}

	void Awake() {
        Init();
 	}

    void Update () {
        if (GameManager.Instance.isPause) return;

        if ((currEnemies.Count == 0) && (_enemyCount == 0)) {
            _startCount = false;
            GameManager.Instance.Gameover();
            return;
        } else {
            if (_startCount) {
                _time += Time.deltaTime;
                if (_time >= enemySpawnTime) {
                    _time = 0;
                    CreateEnemy();
                }
            }
        } 
    }

    void Init () {
        if (grid == null) grid = GetComponentInChildren<IsoGrid>();
        if (_enemies == null) _enemies = new PoolObjectsManager<EnemyBase>(enemyPrefab,transform,kMaxEnemies); 
        _startCount = false;
        _enemyCount = enemyCount;
        _time = 0;
    }

    public void ResetGame () {  
        Init();
        _enemies.FreeAll();
    }

    public void StartGame () {
        Init();
        _time = 0;
        GameManager.Instance.isPause = false;
        _startCount = true;
    }

    void CreateEnemy () {
        if (_enemyCount > 0) {
            EnemyBase enemy = _enemies.GetObject();
            if (enemy != null) {
                enemy.SetMovePath(grid.pathGen.points);
                _enemyCount--;
            }
        }
    }
}
