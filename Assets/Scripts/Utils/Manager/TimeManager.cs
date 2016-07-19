using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// Easy time manager coroutine 
namespace Jatewit {
	public class TimeManager : MonoBehaviourSingleton<TimeManager> {

        List<Coroutine> _currentRoutines = new List<Coroutine>();

		protected override void OnDestroy() {
            StopAllRoutines();
			_currentRoutines = null;
            base.OnDestroy();
		}
        
        public void StartAndWaitForTime (float second, bool ignoreTimeScale = false, Action callback = null) {
            _currentRoutines.Add(StartCoroutine(WaitForTime(second,ignoreTimeScale,callback)));
        }
        
        IEnumerator WaitForTime(float second, bool ignoreTimeScale, Action callback)
		{
            if (ignoreTimeScale) {
                var targetTime = Time.realtimeSinceStartup+second;
                while (Time.realtimeSinceStartup < targetTime) {
                    yield return null;
                }
            } else {
                yield return new WaitForSeconds(second);
            }
            
            if (callback != null) {
                callback();
                callback = null;
            }
        }
        
        public void StopAllRoutines () {
            if (_currentRoutines != null) {
                for (int i = 0; i < _currentRoutines.Count; ++i) {
                    _currentRoutines[i] = null;
                }
                _currentRoutines.Clear();
            }
        }
        
        public void StartAnimation (Animator anim, Action callback = null, string animationName = "", bool waitForLastFrame = true)
		{
			_currentRoutines.Add(StartCoroutine(WaitForAnimation(anim,animationName,callback,waitForLastFrame)));
		}

		IEnumerator WaitForAnimation(Animator anim, string animationName, Action callback, bool waitForLastFrame = true)
		{
			bool closedStateReached = false;
			bool isLastFrame = true;
			if (waitForLastFrame) {
				isLastFrame = false;
			}
			while (!closedStateReached && !isLastFrame && (anim != null))
			{
				if (anim != null) {
					AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
                    if (string.IsNullOrEmpty(animationName)) {
                        closedStateReached = true;
                    } else {
                        if (!anim.IsInTransition(0)) {
                            closedStateReached = state.IsName(animationName);
                        }
                    }
					if (closedStateReached && (state.normalizedTime >= 0.9f)) {
						isLastFrame = true;
					}
				}
				yield return new WaitForEndOfFrame();
			}

			if (callback != null) {
				callback();
                callback = null;
			}
		}
	}
}