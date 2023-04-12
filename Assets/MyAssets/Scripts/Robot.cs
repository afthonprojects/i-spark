using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour {
    [SerializeField]
    private List<TrackPoint> tracks;
    [SerializeField]
    private int trackIndex;
    [SerializeField]
    private float moveSpeed = 2f;
    [SerializeField]
    private float rotationSpeed = 4f;
    [SerializeField]
    private Transform carryPoint;
    [SerializeField]
    private BoxManager boxManager;
    private TrackPoint currentDestination;
    private bool isReachEnd;
    private Vector3 startPos;
    private Quaternion startRot;
    private GameObject currentBox;
    private APIManager apiManager;

    private void Start() {
        apiManager = FindObjectOfType<APIManager>();
        startPos = this.transform.position;
        startRot = this.transform.rotation;
    }

    private void OnEnable() {

        TrackManager.OnPlay += OnMove;
        TrackManager.OnRestart += OnRestart;
    }

    private void OnDisable() {
        TrackManager.OnPlay -= OnMove;
        TrackManager.OnRestart -= OnRestart;
    }

    public void OnMove(TrackManager.Start start, TrackManager.End end) {
        trackIndex = 0;
        isReachEnd = false;
        tracks.Clear();
        tracks.Add(start.startPoint);
        for (int i = 0; i < end.intersections.Count; i++) {
            tracks.Add(end.intersections[i]);
        }
        tracks.Add(end.endPoint);
        StartCoroutine(OnMoveCoroutine());
    }

    public void OnRestart() {
        StopAllCoroutines();
        if (currentBox != null) {
            boxManager.HideBox(currentBox);
            currentBox = null;
        }
        trackIndex = 0;
        isReachEnd = false;
        tracks.Clear();
        this.transform.position = startPos;
        this.transform.rotation = startRot;
    }

    private IEnumerator OnMoveCoroutine() {
        if (tracks.Count == 0)
            yield break;
        while (!isReachEnd && tracks.Count > 0) {
            currentDestination = tracks[trackIndex];
            if (currentDestination.type == TrackPoint.Type.startIntersection) {
                trackIndex += 1;
            } else {
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
                Vector3 lookPos = currentDestination.point.position - transform.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
                if (IsDestinationReached(this.transform.position, currentDestination.point.position)) {
                    switch (currentDestination.type) {
                        case TrackPoint.Type.start:
                        yield return StartCoroutine(OnStartPointCoroutine());
                        trackIndex += 1;
                        break;
                        case TrackPoint.Type.end:
                        yield return StartCoroutine(OnEndPointCoroutine());
                        trackIndex += 1;
                        isReachEnd = true;
                        apiManager.FinishTask(currentDestination.id);
                        break;
                        case TrackPoint.Type.intersection:
                        trackIndex += 1;
                        break;
                        default:
                        break;
                    }
                }
            }
            yield return null;
        }
        trackIndex = tracks.Count - 2;
        while (trackIndex >= 0 && tracks.Count > 0) {
            currentDestination = tracks[trackIndex];
            if (currentDestination.type == TrackPoint.Type.start) {
                trackIndex -= 1;
            } else {
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
                Vector3 lookPos = currentDestination.point.position - transform.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
                if (IsDestinationReached(this.transform.position, currentDestination.point.position)) {
                    switch (currentDestination.type) {
                        case TrackPoint.Type.startIntersection:
                        trackIndex -= 1;
                        break;
                        case TrackPoint.Type.end:
                        trackIndex -= 1;
                        break;
                        case TrackPoint.Type.intersection:
                        trackIndex -= 1;
                        break;
                        default:
                        break;
                    }
                }
            }
            yield return null;
        }
        Debug.Log(IsDestinationReached(this.transform.position, startPos));
        while (!IsDestinationReached(this.transform.position, startPos) && tracks.Count > 0) {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
            Vector3 lookPos = startPos - transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        while (Mathf.Abs(transform.eulerAngles.y - startRot.eulerAngles.y) > 1f && tracks.Count > 0) {
            transform.rotation = Quaternion.Slerp(transform.rotation, startRot, Time.deltaTime * rotationSpeed);
            yield return null;
        }
    }

    private IEnumerator OnStartPointCoroutine() {
        Transform target = currentDestination.point.transform;
        if (target.parent.GetComponent<StartPoint>() == null) {
            yield break;
        }
        StartPoint startPoint = target.parent.GetComponent<StartPoint>();
        while (Mathf.Abs(transform.eulerAngles.y - target.eulerAngles.y) > 1f) {
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        GameObject box = startPoint.OnTakingBox();
        while (box == null) {
            box = startPoint.OnTakingBox();
            yield return null;
        }
        while (!IsDestinationReached(box.transform.position, carryPoint.position)) {
            Vector3 targetPos = new Vector3(carryPoint.position.x, box.transform.position.y, carryPoint.position.z);
            box.transform.position = Vector3.MoveTowards(box.transform.position, targetPos, moveSpeed / 2 * Time.deltaTime);
            yield return null;
        }
        box.transform.SetParent(carryPoint);
        currentBox = box;
        startPoint.OnIdle();
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator OnEndPointCoroutine() {
        Transform target = currentDestination.point.transform;
        if (target.parent.GetComponent<EndPoint>() == null) {
            yield break;
        }
        EndPoint endPoint = target.parent.GetComponent<EndPoint>();
        while (Mathf.Abs(transform.eulerAngles.y - target.eulerAngles.y) > 1f) {
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        if (currentBox == null) {
            yield break;
        }
        Transform boxController = endPoint.GetBoxController();
        while (!IsDestinationReached(currentBox.transform.position, boxController.position)) {
            Vector3 targetPos = new Vector3(boxController.position.x, currentBox.transform.position.y, boxController.position.z);
            currentBox.transform.position = Vector3.MoveTowards(currentBox.transform.position, targetPos, moveSpeed / 2 * Time.deltaTime);
            yield return null;
        }
        endPoint.OnMove(currentBox);
        currentBox = null;
    }

    private bool IsDestinationReached(Vector3 currentPos, Vector3 targetPos) {
        Vector2 now = new Vector2(currentPos.x, currentPos.z);
        Vector2 dest = new Vector2(targetPos.x, targetPos.z);
        if (Vector2.Distance(dest, now) < 0.1f) {
            return true;
        }
        return false;
    }
}
