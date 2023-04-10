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
    private Button buttonSpawnBox;
    [SerializeField]
    private GameObject box;

    public void OnIdle() {
        animator.SetTrigger("idle");
        buttonSpawnBox.interactable = true;
    }

    public void OnSpawn() {
        animator.SetTrigger("spawn");
        buttonSpawnBox.interactable = false;
    }

    public void SpawnBox() {
        box = boxManager.SpawnBox(boxController);
    }

    public GameObject OnTakingBox() {
        if(box != null) {
            GameObject g = box;
            box = null;
            return g;
        } else {
            return null;
        }
    }
}
