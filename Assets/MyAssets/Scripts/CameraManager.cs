using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    [SerializeField]
    private Transform camTransform;
    [SerializeField]
    private Transform topViewCenter;
    [SerializeField]
    private Transform eagleViewCenter;
    [SerializeField]
    private FreeFlyCamera freeFlyCamera;
    [SerializeField]
    private TopViewCamera topViewCamera;
    [SerializeField]
    private float transitionTime;
    private bool isTopView;

    private void Start() {
        isTopView = false;
        freeFlyCamera.enabled = true;
        topViewCamera.enabled = false;
    }

    public void ToggleView() {
        StopAllCoroutines();
        isTopView = !isTopView;
        if (isTopView) {
            freeFlyCamera.enabled = false;
            StartCoroutine(TransitionCameraSmooth(topViewCenter, delegate {
                topViewCamera.enabled = true;
            }));
        } else {
            topViewCamera.enabled = false;
            StartCoroutine(TransitionCameraSmooth(eagleViewCenter, delegate {
                freeFlyCamera.enabled = true;
            }));
        }
    }

    private IEnumerator TransitionCameraSmooth(Transform target, Action DoneEvent) {
        float elapsedTime = 0;
        while (elapsedTime < transitionTime) {
            camTransform.position = Vector3.Lerp(camTransform.transform.position, target.position, elapsedTime / transitionTime);
            camTransform.rotation = Quaternion.Slerp(camTransform.transform.rotation, target.rotation, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        camTransform.position = target.position;
        camTransform.rotation = target.rotation;
        DoneEvent.Invoke();
    }
}
