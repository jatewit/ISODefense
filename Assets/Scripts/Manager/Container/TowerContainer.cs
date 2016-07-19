using UnityEngine;

public class TowerContainer : MonoBehaviour {
    [SerializeField] GameObject towerPrefab;
    [SerializeField] IsoGrid grid;
	void Awake() {
        if (grid == null) grid = GetComponentInChildren<IsoGrid>();
        Cube.OnClickBlock += OnClickBlock;
 	}

    void OnClickBlock (Cube cube) {
        if (cube.tower == null) {
            TowerBase tower = towerPrefab.GetComponent<TowerBase>();
           	if (GameManager.Instance.CanBuy(tower.cost)) {
                Vector3 pos = cube.transform.position;
                pos.y += CubeData.kCubeSize;
                GameObject obj = Instantiate(towerPrefab,pos,Quaternion.identity) as GameObject;
                obj.transform.SetParent(transform);
                cube.tower = obj.GetComponent<TowerBase>();
                cube.tower.SetCube(cube);
                GameManager.Instance.UpdateMoney(-cube.tower.cost);
            } else {
                Debug.LogWarning("Not enough money");
            }
        } else {
            Debug.LogWarning("Sell tower half price");
            GameManager.Instance.UpdateMoney(cube.tower.cost*0.5f);
            Destroy(cube.tower.gameObject);
            cube.tower = null;
        }
    }
}