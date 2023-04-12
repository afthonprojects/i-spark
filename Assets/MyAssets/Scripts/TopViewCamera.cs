using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopViewCamera : MonoBehaviour {
    private void Update() {
        Vector3 deltaPosition = Vector3.zero;
        float currentSpeed = 10;

        if (Input.GetKey(KeyCode.W))
            deltaPosition += transform.up;

        if (Input.GetKey(KeyCode.S))
            deltaPosition -= transform.up;

        if (Input.GetKey(KeyCode.A))
            deltaPosition -= transform.right;

        if (Input.GetKey(KeyCode.D))
            deltaPosition += transform.right;

        transform.position += deltaPosition * currentSpeed * Time.deltaTime;
    }
}
