using UnityEngine;
namespace Jatewit {
	public abstract class PersistentMonoBehaviourSingleton<T> : MonoBehaviourSingleton<T> where T : Component {
		protected override bool Awake () {
			if (!base.Awake()) return false;
			DontDestroyOnLoad (this.gameObject);
			return true;
		}
		
		protected override void OnDestroy() {
			// Empty. Prevents the base class from nulling the instance when destroying extra copies of the object
		}
	}
}
