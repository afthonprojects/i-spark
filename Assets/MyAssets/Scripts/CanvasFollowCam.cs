using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFollowCam : MonoBehaviour
{
    private Transform camTransform;

    private void Start() {
        camTransform = Camera.main.transform;
    }


    void Update() {
        this.transform.rotation = Quaternion.Slerp(Quaternion.Euler(0, this.transform.eulerAngles.y, 0), Quaternion.Euler(0, camTransform.eulerAngles.y, 0), Time.deltaTime * 2);
    }
}
