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
    public static event Action<Start, End> OnPlay;
    public static event Action OnRestart;

    public void OnTrackPlay(string id_mc, string id_ma) {
        if (!startPoints.Exists(result => result.startPoint.id == id_mc)) {
            Debug.Log("StartId null.");
            return;
        }
        if (!endPoints.Exists(result => result.endPoint.id == id_ma)) {
            Debug.Log("EndId null.");
            return;
        }
        OnPlay.Invoke(startPoints.Find(result => result.startPoint.id == id_mc), endPoints.Find(result => result.endPoint.id == id_ma));
    }

    public void OnClickedRestart() {
        OnRestart.Invoke();
    }
}
