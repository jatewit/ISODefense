using UnityEngine;
using System.Collections.Generic;

namespace Jatewit {
	public class PoolObjectsManager<T> where T : PoolObject {
		List<T> _poolObjects = new List<T>();
		GameObject _prefab = null;
		Transform _parent = null;
		int _maxObjects = 20;

		public List<T> currentObjects { 
			get { 
				List<T> unFreeObjects = new List<T>();
				for (int i = 0 ; i < _poolObjects.Count; ++i) {
					if ((_poolObjects[i] != null) && (!_poolObjects[i].isFree)) {
						unFreeObjects.Add(_poolObjects[i]);
					}
				}
				return unFreeObjects; 
			} 
		}

		public PoolObjectsManager (GameObject prefab, Transform parent, int maxObjects = 20) {
			_prefab = prefab;
			_parent = parent;
			_maxObjects = maxObjects;
		}

		public T GetObject () {
			if (_poolObjects == null) _poolObjects = new List<T>();
			for (int i = 0 ; i < _poolObjects.Count; ++i) {
				if ((_poolObjects[i] != null) && (_poolObjects[i].isFree)) {
					_poolObjects[i].UnFree();
					return _poolObjects[i];
				}
			}
			if (_poolObjects.Count < _maxObjects) {
				GameObject obj;
				if (_prefab == null) {
					obj = new GameObject("Pool Object");
				} else {
					obj = GameObject.Instantiate(_prefab);
				}
				obj.transform.SetParent(_parent);
				T newObj = obj.GetComponent<T>();
				if (newObj == null) {
					newObj = obj.AddComponent<T>();
				}
				_poolObjects.Add(newObj);
				return newObj;
			}
			return null;
		}

		public void FreeAll () { 
			if (_poolObjects != null) {
				for (int i = 0 ; i < _poolObjects.Count; ++i) {
					_poolObjects[i].Free();
				}
			}
		}
	}
}