using UnityEngine;
using Jatewit;

public class BulletBase : PoolObject {
	[Range(1,20)]
	public int hitPower = 20;
	public float moveSpeed = 10;

	float _distanceSqr;
	float _currentDistanceSqr;
	Vector3 _startPos;
	EnemyBase _target;
	bool _move = false;

	public void MoveTo (Vector3 startPos, EnemyBase target, float distance ) {
		_startPos = startPos;
		_target = target;
		_distanceSqr = distance*distance;
		transform.position = _startPos;
		float diffTarget = (_target.transform.position - _startPos).sqrMagnitude;
		if (diffTarget >= _distanceSqr) {
			Free();
		} else {
			_move = true;
		}
	}

	void Update () {
		if (GameManager.Instance.isPause) return;
		if (_move) {
			Vector3 direction = (_target.transform.position - transform.position);
			float diffTargetToStart = (_target.transform.position - _startPos).sqrMagnitude;
			float diffStart = (transform.position - _startPos).sqrMagnitude;
			float diffTarget = (_target.transform.position - transform.position).sqrMagnitude;
			// Remove out of reach
			if ((diffStart >= _distanceSqr) || (diffTargetToStart >= _distanceSqr) || (diffTarget <= 0.01f)) {
				if (diffTarget <= 0.01f) {
					_target.HitEnemy(this);
				}
				Free();
			} else {
				transform.position = Vector3.MoveTowards(transform.position,_target.transform.position,Time.smoothDeltaTime*moveSpeed);
				Vector3 newDirection = Vector3.RotateTowards(transform.forward,direction,Time.smoothDeltaTime*10,0);
				transform.rotation = Quaternion.LookRotation(newDirection);
			}
		}
	}

	public override void Free () {
		_move = false;
		base.Free();
	}
}
