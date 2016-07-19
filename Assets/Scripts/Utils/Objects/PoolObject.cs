using UnityEngine;


namespace Jatewit {
	public abstract class PoolObject : MonoBehaviour {
        public bool isFree { get; private set; }
        
        public virtual void Free () {
            // move object out of the scene
		    transform.position = new Vector3(-10000,-10000,-10000);
            gameObject.SetActive(false);
            isFree = true;
        }

        public virtual void UnFree () {
            // move object out of the scene
		    transform.position = Vector3.zero;
            gameObject.SetActive(true);
            isFree = false;
        }
    }
}