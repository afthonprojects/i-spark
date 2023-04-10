using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour {
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Transform boxController;
    [SerializeField]
    private BoxManager boxManager;
    private GameObject box;

    public void OnIdle() {
        animator.SetTrigger("idle");
    }

    public void OnMove(GameObject g) {
        box = g;
        box.transform.SetParent(boxController);
        animator.SetTrigger("move");
    }

    public Transform GetBoxController() {
        return boxController;
    }

    public void BoxReachEnd() {
        if(box != null) {
            boxManager.HideBox(box);
            OnIdle();
        }
    }
}
