using UnityEngine;
using Jatewit;

public class TowerBase : MonoBehaviour {
	const int kMaxBullets = 20;
	public GameObject bulletPrefab;
	public Transform bulletStartPosition;
	public Transform radius;

	[Range(1,10)]
	public float shotRange = 2;
	public float repeatRate = 1;
	public float cost = 50;
	PoolObjectsManager<BulletBase> _bulletPool;
	MeshRenderer _radiusRenderer;
	Cube _cube;

	EnemyBase _enemy;

	bool _isStartRepeat = false;
    float _time = 0;

	void Awake () {
		_bulletPool = new PoolObjectsManager<BulletBase>(bulletPrefab,transform.parent,kMaxBullets);
		_radiusRenderer = radius.GetComponent<MeshRenderer>();
		radius.localScale = new Vector3(shotRange,shotRange,shotRange);
		_isStartRepeat = false;
		HideRadius();
	}
	
	void Update () {
		if (GameManager.Instance.isPause) return;

		if (radius.localScale.x != shotRange) {
			radius.localScale = new Vector3(shotRange,shotRange,shotRange);
		}

		CheckMouseInput();

		if (GameManager.Instance.currentEnemies != null) {
			bool needShot = false;
			for (int i = 0; i < GameManager.Instance.currentEnemies.Count; ++i) {
				Vector3 dist = (GameManager.Instance.currentEnemies[i].transform.position - transform.position);
				float squrDist = dist.sqrMagnitude;
				if (squrDist <= (shotRange*shotRange)) {
					_enemy = GameManager.Instance.currentEnemies[i];
					needShot = true;
					break;
				}
			}
			if (needShot) {
				if(!_isStartRepeat) {
					_isStartRepeat = true;
					ShotInterval();
					_time = 0;
				}
			}
		}

		if (_isStartRepeat) {
			_time += Time.deltaTime;
			if (_time >= repeatRate) {
				_time = 0;
				ShotInterval();
			}
		}
	}

	public void FreeAllBullets () {
		_bulletPool.FreeAll();
	}

	void ShotInterval() {
		BulletBase bullet = _bulletPool.GetObject();
		if (bullet != null) {
			bullet.MoveTo(bulletStartPosition.position,_enemy,shotRange);
		}
	}

	void CheckMouseInput () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray,out hit)) {
			if ((hit.collider.transform.parent == transform) || 
				((_cube != null) && (hit.collider.transform == _cube.transform))) {
				if (_cube != null) _cube.Mouseover();
				if (Input.GetMouseButtonDown(0)) {
					if (_cube != null) _cube.ClickAsButton();
				}
			} else {
				if (_cube != null) _cube.MouseExit();
			}
		}
	}

	public void SetCube (Cube cube) {
		_cube = cube;
	}

	public void ShowRadius () {
		_radiusRenderer.enabled = true;
	}
	public void HideRadius () {
		_radiusRenderer.enabled = false;
	}
}
