using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrackManager : MonoBehaviour {
    [Serializable]
    public class Start {
        public TrackPoint startPoint;
    }


    [Serializable]
    public class End {
        [SerializeField]
        public List<TrackPoint> intersections;
        [SerializeField]
        public TrackPoint endPoint;
    }

    [SerializeField]
    private List<Start> startPoints;
    [SerializeField]
    private List<End> endPoints;
    [SerializeField]
    private TMP_InputField inputStart;
    [SerializeField]
    private TMP_InputField inputEnd;
    public static event Action<Start, End> OnPlay;
    public static event Action OnRestart;

    public void OnClickedPlay() {
        int startId = 0;
        if (int.TryParse(inputStart.text, out startId) == false) {
            Debug.Log("TryParse failed.");
            return;
        }
        int endId = 0;
        if (int.TryParse(inputEnd.text, out endId) == false) {
            Debug.Log("TryParse failed.");
            return;
        }
        if (!startPoints.Exists(result => result.startPoint.id == startId)) {
            Debug.Log("StartId null.");
            return;
        }
        if (!endPoints.Exists(result => result.endPoint.id == endId)) {
            Debug.Log("EndId null.");
            return;
        }
        OnPlay.Invoke(startPoints.Find(result => result.startPoint.id == startId), endPoints.Find(result => result.endPoint.id == endId));
    }

    public void OnClickedRestart() {
        OnRestart.Invoke();
    }
}
