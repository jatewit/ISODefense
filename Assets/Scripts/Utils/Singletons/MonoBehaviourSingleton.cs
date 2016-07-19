using UnityEngine;
/// <summary>
/// Generic Singleton base class for Unity MonoBehaviours.  Simply extend this class with the child class as the generic type.
/// The Awake() method needs to be overridden if you need to use it and returns a bool value.  It must also check the base class's Awake() method and return false if it failed in order to avoid Instantiating multiples of itself.
/// 
/// Example:
/// 
/// public class MyClass : MonoBehaviourSingleton<MyClass> {
/// 	public override bool Awake() {
/// 		if(!base.Awake()) return false;
/// 
/// 		// Do your initilization stuff
/// 		
/// 		return true;
/// 	}
/// }
/// </summary>
namespace Jatewit {
	public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : Component {
		private static T _instance;
		// Apparently OnDisable/OnDestroy are called almost at the same time per object. It's possible to destroy the manager, then have another object access it in OnDisable. Which is stupid. 
		// Sanity check to make sure we don't create a new manager object that stays alive after quitting the game in the editor
		private static bool _shuttingDown = false;

		public static T Instance {
			get {
				if (_instance == null) {
					_instance = FindObjectOfType<T> ();
					if (_instance == null && !_shuttingDown) {
						GameObject obj = new GameObject (typeof(T).ToString());
						_instance = obj.AddComponent<T> ();
					}
				}
				return _instance;
			}
		}

		// Sanity check to avoid leaking new instances of the Singleton during shutdown
		public virtual void OnApplicationQuit() {
			_shuttingDown = true;
		}

		protected virtual void OnDestroy() {
			if (_instance == this) {
				_instance = null;
				_shuttingDown = true;
			}
		}

		// Preventing multiples instantiate - when using DontDestroyOnLoad
		protected virtual bool Awake() {
			_shuttingDown = false;
			if (_instance == null || _instance == this) {
				_instance = this as T;
				return true;
			} else {
				Destroy(this);
				return false;
			}
		}
	}
}
