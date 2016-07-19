using UnityEngine;
using Jatewit;
using System.Collections.Generic;

public class EnemyBase : PoolObject {
	[Range(0,100)]
	public int hitPoint = 20;

	[Range(0,100)]
	public int damage = 10;

	[Range(0,100)]
	public int money = 10;

	[Range(0,1000)]
	public int score = 100;

	[Range(0,100)]
	public float moveSpeed = 2;

	int _currentHitPoint = 0;
	Queue<Vector3> _moveTarget;
	bool _isMove = false;

	void Awake () {
		_currentHitPoint = hitPoint;
	}

	public void HitEnemy (BulletBase bullet) {
		_currentHitPoint = Mathf.Max(0,_currentHitPoint - bullet.hitPower);
		if (_currentHitPoint == 0) {
			// Added Particle if needed 
			bullet.Free();
			Free();
		} else {
			// Added Effect if needed
			bullet.Free();
		}
	}

	void Update () {
		if (GameManager.Instance.isPause) return;
		if (!_isMove) return;
		if ((_moveTarget == null) || (_moveTarget.Count == 0)) {
			Free();
			GameManager.Instance.UpdateHp(-damage);
		} else {
			Vector3 target = _moveTarget.Peek();
			Vector3 direction = (target - transform.position);
			transform.position = Vector3.MoveTowards(transform.position,target,Time.smoothDeltaTime*moveSpeed);
			Vector3 newDirection = Vector3.RotateTowards(transform.forward,direction,Time.smoothDeltaTime*10,0);
			transform.rotation = Quaternion.LookRotation(newDirection);
			if (direction.sqrMagnitude <= 0.001f) {
				_moveTarget.Dequeue();
			}
		}
	}

	public void SetMovePath (List<Vector3> points) {
		if ((_moveTarget == null) || (_moveTarget.Count == 0)) {
			_moveTarget = new Queue<Vector3>();
			for (int i = 0; i < points.Count; ++i) {
				_moveTarget.Enqueue(points[i]);
			}
			Vector3 target = _moveTarget.Peek();
			transform.position = target;
			Vector3 direction = (points[1] - transform.position);
			transform.rotation = Quaternion.LookRotation(direction);
			_isMove = true;
		}
	}

	public override void Free () {
		_isMove = false;
		if (!GameManager.Instance.isPause) {
			GameManager.Instance.UpdateScore(score);
			GameManager.Instance.UpdateMoney(money);
		}
		_currentHitPoint = hitPoint;
		_moveTarget.Clear();
		base.Free();
	}
}
