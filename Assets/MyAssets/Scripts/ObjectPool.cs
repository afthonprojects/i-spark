using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
    public GameObject objectPrefab;
    private Queue<GameObject> availableObject = new Queue<GameObject>();

    public GameObject GetObjectFromPool() {
        if (availableObject.Count == 0)
            SpawnObject();
        GameObject instance = availableObject.Dequeue();
        instance.SetActive(true);
        return instance;
    }

    public void AddObjectToPool(GameObject objectInstance) {
        objectInstance.SetActive(false);
        availableObject.Enqueue(objectInstance);
    }

    private void SpawnObject() {
        GameObject objectToAdd = Instantiate(objectPrefab);
        AddObjectToPool(objectToAdd);
    }
}