using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrackPoint : MonoBehaviour
{
    public enum Type {
        start,
        end,
        intersection,
        startIntersection
    }

    public string id;
    public Type type;
    public Transform point;
    public TMP_Text textNumber;

    private void Start() {
        if(textNumber != null)
            textNumber.text = id.ToString();
    }
}
