using UnityEngine;
using System.Collections;
using Jatewit;

[RequireComponent(typeof(Animator))]
public class Overlay : MonoBehaviour {
	[SerializeField] private string openParameter = "Open";
	[SerializeField] private string openAnimationName = "Open";
	[SerializeField] private string closedAnimationName = "Close";
	
	Animator _animator;
	int _count;
	int _openParameterId;
	public bool isShowing { get; private set; }
	
	void Awake() {
		_animator = GetComponent<Animator>();
		_openParameterId = Animator.StringToHash (openParameter);
	}
	
	IEnumerator Start () {
		while (!_animator.isInitialized) {
			yield return null; // Wait For Animation Initialize
		}
		gameObject.SetActive(false);
	}
	
	public void Show() {
		gameObject.SetActive(true);
		ToggleOverlay(true);
	}
	
	public void Close() {
		ToggleOverlay(false);
	}
	
	void ToggleOverlay (bool show) {
		if ((_animator != null) && (_animator.isInitialized)) {
			if (show) {
				if (!_animator.GetBool(_openParameterId) && !CheckingAnimationState(_animator,openAnimationName)) {
					isShowing = true;
					TimeManager.Instance.StartAnimation(_animator,null,openAnimationName);
					_animator.SetBool(_openParameterId,true);
				}
			} else {
				if (_animator.GetBool(_openParameterId) && !CheckingAnimationState(_animator,closedAnimationName)) {
					isShowing = false;
					TimeManager.Instance.StartAnimation(_animator,()=>{ gameObject.SetActive(false); },closedAnimationName);
					_animator.SetBool(_openParameterId,false);
				}
			}
		}
	}

	bool CheckingAnimationState (Animator anim, string animationName) {
		bool isDone = false;
		AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
		isDone = state.IsName(animationName);
		if (isDone && (state.normalizedTime >= 0.9f)) {
			return true;
		}
		return false;
	}
}
