using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxManager : MonoBehaviour {
    [SerializeField]
    private ObjectPool objectPool;

    public GameObject SpawnBox(Transform parent) {
        GameObject g = objectPool.GetObjectFromPool();
        g.transform.SetParent(parent);
        g.transform.localRotation = Quaternion.identity;
        g.transform.localPosition = Vector3.zero;
        return g;
    }

    public void HideBox(GameObject g) {
        objectPool.AddObjectToPool(g);
    }
}
