using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPoint : MonoBehaviour {

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Transform boxController;
    [SerializeField]
    private BoxManager boxManager;
    [SerializeField]
    private GameObject box;

    private void Start() {
        OnSpawn();
    }

    public void OnIdle() {
        animator.SetTrigger("idle");
    }

    public void OnSpawn() {
        animator.SetTrigger("spawn");
    }

    public void SpawnBox() {
        box = boxManager.SpawnBox(boxController);
    }

    public GameObject OnTakingBox() {
        if(box != null) {
            GameObject g = box;
            box = null;
            OnSpawn();
            return g;
        } else {
            return null;
        }
    }
}
